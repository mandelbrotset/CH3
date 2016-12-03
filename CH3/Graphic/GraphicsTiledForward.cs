using System;
using System.Collections.Generic;
using CH3.Camera;
using CH3.GameObjects.DynamicObjects.Vehicles;
using CH3.Graphic;
using CH3.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using CH3.Shaders;
using CH3.Lights;

namespace CH3
{
    public class GraphicsTiledForward : Graphics
    {
        public World world { get; set; }


        private Window window;

        private PostProcessedImage contourTexture;
        private PostProcessedImage MSAAContourTexture;

        private PostProcessedImage multiSampledFinalImage;
        private PostProcessedImage finalImage;

        private LightGrid m_lightGrid;
        private List<Light> m_lights;

        enum TiledUniformBufferSlots
        {
            TUBS_Globals = 4, //??? use buffers that are not used by obj model.
            TUBS_LightGrid,
            TUBS_LightPositionsRanges,
            TUBS_LightColors,
            TUBS_STRUCT,
            TUBS_Max,
        };


        struct Int16b
        {
            public UInt32 a, b, c, d;

            public Int16b(UInt32 i)
            {
                a = b = c = d = i;
            }
        }

        private GlBufferObject<Int16b> g_gridBuffer;
        private GlBufferObject<int> g_tileLightIndexListsBuffer;
        private GlBufferObject<LightDataStruct> g_lightDataBuffer;

        private int NUM_POSSIBLE_LIGHTS = 10;
        private uint g_tileLightIndexListsTexture = 0;
        private ShaderLoader tiled_forward_shader;
        private PrimitiveShader2 primitive_shader;

        public GraphicsTiledForward(World world, AAMode aaMode, ContourMode contourMode, Window window)
        {
            this.world = world;
            this.aaMode = aaMode;
            this.contourMode = contourMode;
            this.window = window;
            this.debugMode = DebugMode.LightGrid;

            g_gridBuffer = new GlBufferObject<Int16b>();
            g_lightDataBuffer = new GlBufferObject<LightDataStruct>();

            initShaders();
            initCameras();

            m_lightGrid = new LightGrid((uint)Window.WIDTH, (uint)Window.HEIGHT, 32);
            m_lights = new List<Light>();

            m_lights.Add(new PointLight(new Vector3(20, 20, 5), new Vector3(0, 1, 1), 30f));
            m_lights.Add(new PointLight(new Vector3(20, 0, 20), new Vector3(1, 0, 1), 150f));
            m_lights.Add(new PointLight(new Vector3(0, 0, 5), new Vector3(0, 0, 1), 150f));
            m_lights.Add(new SpotLight(new Vector3(0, 0, 1), new Vector3(1, 0, 1), 150f, 1.5f, new Vector3(1, 0, 0)));
            m_lights.Add(new DirectionalLight(new Vector3(1.0f, 0.984f, 0.937f) * 0.25f, new Vector3(1, 0, 1)));


            g_gridBuffer.init(m_lightGrid.LIGHT_GRID_MAX_DIM_X * m_lightGrid.LIGHT_GRID_MAX_DIM_Y, new Int16b[] { new Int16b(0) });
            g_lightDataBuffer.init(NUM_POSSIBLE_LIGHTS, new LightDataStruct[] { new LightDataStruct(0) });


            g_tileLightIndexListsBuffer = new GlBufferObject<int>();
            g_tileLightIndexListsTexture = (uint)GL.GenTexture();
            g_tileLightIndexListsBuffer.init(1, new int[] { 1 });

            GL.BindTexture((TextureTarget)35882, g_tileLightIndexListsTexture);
            GL.TexBuffer(TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32i, g_tileLightIndexListsBuffer.m_id);

            primitive_shader = new PrimitiveShader2();
            tiled_forward_shader = new ShaderLoader();

            int loc = GL.GetUniformBlockIndex(tiled_forward_shader.program_id, "LightGrid");
            GL.UniformBlockBinding((int)tiled_forward_shader.program_id, loc, (int)TiledUniformBufferSlots.TUBS_LightGrid);
            loc = GL.GetUniformBlockIndex(tiled_forward_shader.program_id, "LightData");
            GL.UniformBlockBinding((int)tiled_forward_shader.program_id, loc, (int)TiledUniformBufferSlots.TUBS_STRUCT);

            initFinalImage();
            initMultiSampledFinalImage();
        }

        public override void AddLight(Light l)
        {
            m_lights.Add(l);
        }

        internal override void Render(RenderMode renderMode, AAMode aaMode, ContourMode contourMode)
        {
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Multisample);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            /*
                Stage 1: Rebuild LightGrid
            */

            m_lightGrid.build(m_lights, activeCamera.viewMatrix_opentk, activeCamera.projection_opentk, activeCamera.z_near);

            /*
                Render scene using LightGrid
            */
            bindLightGridConstants();

            GL.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);
            GL.ClearColor(0.1f, 0.2f, 0.4f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            /*
            debugMode = DebugMode.LightGrid;
            drawDebug(world.visibleObjects);
            
            debugMode = DebugMode.PhysicsAABB;
            drawDebug(world.visibleObjects);
            */

            if (aaMode != this.aaMode)
            {
                this.aaMode = aaMode;
                if (aaMode != AAMode.OFF)
                {
                    initMultiSampledFinalImage();
                }
            }

            uint fbo = finalImage.FBO;
            if (aaMode != AAMode.OFF)
            {
                fbo = multiSampledFinalImage.FBO;
                GL.Enable(EnableCap.Multisample);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            renderOpaqueObjects(world.visibleObjects);
            renderTransparentObjects(world.visibleObjects);

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, fbo);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BlitFramebuffer(0, 0, Window.WIDTH, Window.HEIGHT, 0, 0, Window.WIDTH, Window.HEIGHT, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.UseProgram(0);
            window.SwapBuffers();
        }

        private void bindLightGridConstants()
        {

            Int16b[] tmp = new Int16b[m_lightGrid.LIGHT_GRID_MAX_DIM_X * m_lightGrid.LIGHT_GRID_MAX_DIM_Y];
            for (int i = 0; i < m_lightGrid.LIGHT_GRID_MAX_DIM_X * m_lightGrid.LIGHT_GRID_MAX_DIM_Y; ++i) {
                tmp[i].a = m_lightGrid.m_gridCounts[i];
                tmp[i].b = m_lightGrid.m_gridOffsets[i];
            }

            g_gridBuffer.copyFromHost(tmp, m_lightGrid.LIGHT_GRID_MAX_DIM_X * m_lightGrid.LIGHT_GRID_MAX_DIM_Y);

            if (m_lightGrid.m_tileLightIndexLists.Count > 0)
            {
                g_tileLightIndexListsBuffer.copyFromHost(m_lightGrid.m_tileLightIndexLists.ToArray(), m_lightGrid.m_tileLightIndexLists.Count);
                GL.BindTexture((TextureTarget)35882, g_tileLightIndexListsTexture);
                GL.TexBuffer(TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32i, g_tileLightIndexListsBuffer.m_id);
            }

            int NUM_POSSIBLE_LIGHTS = 10;

            LightDataStruct[] light_data = new LightDataStruct[NUM_POSSIBLE_LIGHTS];

            int length = NUM_POSSIBLE_LIGHTS < m_lightGrid.m_viewSpaceLights.Count ? NUM_POSSIBLE_LIGHTS : m_lightGrid.m_viewSpaceLights.Count;
            for (int i = 0; i < length; ++i)
            {
                light_data[i] = new LightDataStruct(m_lightGrid.m_viewSpaceLights[i]);
            }

            g_lightDataBuffer.copyFromHost(light_data, NUM_POSSIBLE_LIGHTS);

            g_gridBuffer.bindSlot(BufferTarget.UniformBuffer,      (uint)TiledUniformBufferSlots.TUBS_LightGrid);
            g_lightDataBuffer.bindSlot(BufferTarget.UniformBuffer, (uint)TiledUniformBufferSlots.TUBS_STRUCT);
        }

        private void renderHUD(int time)
        {

        }

        private void renderTransparentObjects(List<GameObject> objects) { }
        private void renderOpaqueObjects(List<GameObject> objects)
        {
            tiled_forward_shader.UseProgram();
            
            Matrix4 viewMatrix_otk = activeCamera.viewMatrix_opentk;
            GL.UniformMatrix4(GL.GetUniformLocation(tiled_forward_shader.program_id, "viewMatrix"), false, ref viewMatrix_otk);

            foreach (GameObject d in objects)
            {
                if (d != null)
                {
                    if (d is Car)
                    {
                        Car car = (Car)d;
                        car.Render(tiled_forward_shader, activeCamera.viewProjection, activeCamera.viewMatrix, d);
                        car.wheel.Render(tiled_forward_shader, activeCamera.viewProjection_opentk, car.vehicle.GetWheelTransformWS(0));
                        car.wheel.Render(tiled_forward_shader, activeCamera.viewProjection_opentk, car.vehicle.GetWheelTransformWS(1));
                        car.wheel.Render(tiled_forward_shader, activeCamera.viewProjection_opentk, car.vehicle.GetWheelTransformWS(2));
                        car.wheel.Render(tiled_forward_shader, activeCamera.viewProjection_opentk, car.vehicle.GetWheelTransformWS(3));
                    }
                    else {
                        d.Render(tiled_forward_shader, activeCamera.viewProjection, activeCamera.viewMatrix, d);
                    }
                }
            }
        }

        private void drawDebug(List<GameObject> objects)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);

            primitive_shader.UseProgram();

            Matrix4 viewMatrix_otk = activeCamera.viewMatrix_opentk;
            GL.UniformMatrix4(GL.GetUniformLocation(primitive_shader.program_id, "view_matrix"), false, ref viewMatrix_otk);

            Matrix4 projectionMatrix_otk = activeCamera.projection_opentk;
            GL.UniformMatrix4(GL.GetUniformLocation(primitive_shader.program_id, "projection_matrix"), false, ref projectionMatrix_otk);

            GL.Disable(EnableCap.DepthTest);
            if (debugMode == DebugMode.PhysicsAABB)
            {
                foreach (GameObject d in objects)
                {
                    if (d != null && d.has_physics)
                    {
                        Vector4 color = new Vector4(1, 0, 0, 1);
                        if (d is Car)
                            color = new Vector4(0, 1, 1, 1);
                        else if (d.body.IsActive)
                            color = new Vector4(0, 1, 0, 1);
                        else
                            color = new Vector4(1, 0, 0, 1);
                        GL.Uniform4(GL.GetUniformLocation(primitive_shader.program_id, "color"), ref color);

                        Vector3 min, max;
                        d.body.GetAabb(out min, out max);

                        Utils.DrawBox.Render(simpleEdgeShader, min, max);
                    }
                }
            }
            if (debugMode == DebugMode.LightGrid)
            {
                showGrid();
            }
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        private bool initFinalImage()
        {

            uint modelTex = (uint)GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, modelTex);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, 10497);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, 10497);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 9729);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 9729);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);


            uint modelFBO = (uint)GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, modelFBO);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, modelTex, 0);


            // The depth buffer
            uint depthrenderbuffer = (uint)GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.StencilAttachmentExt, RenderbufferTarget.RenderbufferExt, depthrenderbuffer);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("FAILED");
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.finalImage = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new OpenGL.Vector3(0, 0, 0), this);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return true;

        }

        private bool initMultiSampledFinalImage()
        {

            if (this.multiSampledFinalImage != null)
            {
                uint[] list = { this.multiSampledFinalImage.FBO };
                GL.DeleteFramebuffers(1, list);
            }

            bool multiSampling = false;
            int numberOfSamples = 0;

            if (aaMode == AAMode.MSAA_X2)
            {
                multiSampling = true;
                numberOfSamples = 2;
            }
            else if (aaMode == AAMode.MSAA_X4)
            {
                multiSampling = true;
                numberOfSamples = 4;
            }
            else if (aaMode == AAMode.MSAA_X8)
            {
                multiSampling = true;
                numberOfSamples = 8;
            }

            if (!multiSampling)
                return false;

            // Create multisample texture

            GL.Enable(EnableCap.Multisample);

            uint modelTex = (uint)GL.GenTexture();

            uint modelMultiSampledTex = (uint)GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DMultisample, modelMultiSampledTex);

            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, numberOfSamples, PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, true);

            // Create and bind the FBO
            uint modelFBO = (uint)GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, modelFBO);

            // Create color render buffer
            uint colorBuffer = (uint)GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, colorBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, numberOfSamples, RenderbufferStorage.Rgba8, Window.WIDTH, Window.HEIGHT);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, RenderbufferTarget.RenderbufferExt, colorBuffer);

            uint depthBuffer = (uint)GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, numberOfSamples, RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.StencilAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);


            // Bind Texture assuming we have created a texture
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, modelTex, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {

                Console.WriteLine("FAILED TO CREATE FRAME BUFFER");

                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferUndefined)
                {
                    Console.WriteLine("UNDEFINED");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferUnsupported)
                {
                    Console.WriteLine("UNSUPPORTED");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteReadBuffer)
                {
                    Console.WriteLine("INCOMPLETE READ BUFFER");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteMultisample)
                {
                    Console.WriteLine("INCOMPLETE MULTISAMPLE");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteMissingAttachment)
                {
                    Console.WriteLine("INCOMPLETE MISSING ATTACHMENT");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteAttachment)
                {
                    Console.WriteLine("INCOMPLETE ATTACHMENT");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteDrawBuffer)
                {
                    Console.WriteLine("INCOMPLETE DRAW BUFFER");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteLayerCount)
                {
                    Console.WriteLine("INCOMPLETE LAYER  COUNT");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteLayerTargets)
                {
                    Console.WriteLine("INCOMPLETE LAYER TARGETS");
                }

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.multiSampledFinalImage = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new OpenGL.Vector3(0, 0, 0), this);

            return true;

        }

        private bool initMultiSampledContourTexture()
        {
            if (this.MSAAContourTexture != null)
            {
                uint[] list = { this.MSAAContourTexture.FBO };
                GL.DeleteFramebuffers(1, list);
            }

            bool multiSampling = false;
            int numberOfSamples = 0;

            if (aaMode == AAMode.MSAA_X2)
            {
                multiSampling = true;
                numberOfSamples = 2;
            }
            else if (aaMode == AAMode.MSAA_X4)
            {
                multiSampling = true;
                numberOfSamples = 4;
            }
            else if (aaMode == AAMode.MSAA_X8)
            {
                multiSampling = true;
                numberOfSamples = 8;
            }

            if (!multiSampling)
                return false;

            // Create multisample texture

            GL.Enable(EnableCap.Multisample);

            uint modelTex = (uint)GL.GenTexture();

            uint modelMultiSampledTex = (uint)GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DMultisample, modelMultiSampledTex);

            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, numberOfSamples, PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, true);

            // Create and bind the FBO
            uint modelFBO = (uint)GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, modelFBO);

            // Create color render buffer
            uint colorBuffer = (uint)GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, colorBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, numberOfSamples, RenderbufferStorage.Rgba8, Window.WIDTH, Window.HEIGHT);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, RenderbufferTarget.RenderbufferExt, colorBuffer);



            uint depthBuffer = (uint)GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, numberOfSamples, RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.StencilAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);


            // Bind Texture assuming we have created a texture
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, modelTex, 0);


            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {

                Console.WriteLine("FAILED TO CREATE FRAME BUFFER");

                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferUndefined)
                {
                    Console.WriteLine("UNDEFINED");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferUnsupported)
                {
                    Console.WriteLine("UNSUPPORTED");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteReadBuffer)
                {
                    Console.WriteLine("INCOMPLETE READ BUFFER");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteMultisample)
                {
                    Console.WriteLine("INCOMPLETE MULTISAMPLE");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteMissingAttachment)
                {
                    Console.WriteLine("INCOMPLETE MISSING ATTACHMENT");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteAttachment)
                {
                    Console.WriteLine("INCOMPLETE ATTACHMENT");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteDrawBuffer)
                {
                    Console.WriteLine("INCOMPLETE DRAW BUFFER");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteLayerCount)
                {
                    Console.WriteLine("INCOMPLETE LAYER  COUNT");
                }
                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteLayerTargets)
                {
                    Console.WriteLine("INCOMPLETE LAYER TARGETS");
                }



                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.MSAAContourTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new OpenGL.Vector3(0, 0, 0), this);

            return true;
        }


        private bool initContourTexture()
        {

            uint modelTex = (uint)GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, modelTex);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, 10497);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, 10497);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 9729);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 9729);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            uint modelFBO = (uint)GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, modelFBO);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, modelTex, 0);

            // The depth buffer
            uint depthrenderbuffer = (uint)GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.StencilAttachmentExt, RenderbufferTarget.RenderbufferExt, depthrenderbuffer);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                return false;

            this.contourTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new OpenGL.Vector3(0, 0, 0), this);
            return true;
        }

        private void showGrid()
        {


            Matrix4 projectionMatrix_otk = Matrix4.CreateOrthographicOffCenter(0, Window.WIDTH, 0, Window.HEIGHT, -1.0f, 1.0f);
            GL.UniformMatrix4(GL.GetUniformLocation(primitive_shader.program_id, "projection_matrix"), false, ref projectionMatrix_otk);

            Matrix4 view_matrix = Matrix4.Identity;
            GL.UniformMatrix4(GL.GetUniformLocation(primitive_shader.program_id, "view_matrix"), false, ref view_matrix);


            int tileSize = m_lightGrid.m_tileSize;

            for (int y = 0; y < m_lightGrid.m_gridDim.Y; ++y)
            {
                for (int x = 0; x < m_lightGrid.m_gridDim.X; ++x)
                {
                    uint count = m_lightGrid.tileLightCount(x, y);
                    if (count > 0)
                    {
                        Vector4 color = new Vector4(0, 0, 0, 0);
                        uint offset = m_lightGrid.tileLightIndexListOffset(x, y);
                        for (int i = 0; i < count; ++i)
                        {
                            int light_index = m_lightGrid.m_tileLightIndexLists[(int)offset + i];
                            Light l = m_lightGrid.m_viewSpaceLights[light_index];
                            color += new Vector4(l.color.X, l.color.Y, l.color.Z, 0) / count;
                        }
                        color.W = 0.9f;
                        //primitiveShader.setColor(color);
                        GL.Uniform4(GL.GetUniformLocation(primitive_shader.program_id, "color"), ref color);
                        DrawQuad.Render(primitive_shader,
                            new Vector2(x * tileSize, y * tileSize),
                            new Vector2((x + 1) * tileSize, y * tileSize),
                            new Vector2((x + 1) * tileSize, (y + 1) * tileSize),
                            new Vector2(x * tileSize, (y + 1) * tileSize));
                    }
                }
            }
        }
    }
}