using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CH3.Camera;
using OpenGL;
using CH3.Lights;

namespace CH3
{
    public abstract class Graphics
    {

        public BasicShaderProgram basicShader { get; private set; }
        public CelShader celShader { get; private set; }
        public NormalShader normalShader { get; private set; }
        public DepthShader depthShader { get; private set; }
        public ModelShader modelShader { get; private set; }
        public EdgeShader edgeShader { get; private set; }
        public SimpleEdgeShader simpleEdgeShader { get; private set; }
        public FXAAShader fxAAShader { get; private set; }
        public GBufferShader gbufferShader { get; private set; }
        public FloorShaderProgram floorShader { get; private set; }
        public PrimitiveShader primitiveShader { get; private set; }
        public PointLightShader pointlightShader { get; private set; }

        public CameraMode cameraMode { get; set; }
        protected AAMode aaMode;
        protected ContourMode contourMode;
        protected DebugMode debugMode;

        public enum CameraMode
        {
            FPS, PLAYER
        }

        public enum RenderMode
        {
            BASIC, NORMAL, CEL, DEPTH, EDGE, SIMPLE_EDGE, MODEL, FXAA, GBuffer
        }

        public enum AAMode
        {
            OFF, MSAA_X2, MSAA_X4, MSAA_X8, FXAA
        }

        public enum ContourMode
        {
            OFF, ON, MSAA
        }

        public enum DebugMode
        {
            OFF, LightGrid, PhysicsAABB
        }

        public FPSCamera fpsCamera { get; protected set; }
        public AboveCamera aboveCamera { get; protected set; }
        public Camera.Camera activeCamera;

        protected void initShaders()
        {

            basicShader = new BasicShaderProgram();
            basicShader.initShader();

            celShader = new CelShader();
            celShader.initShader();

            normalShader = new NormalShader();
            normalShader.initShader();

            depthShader = new DepthShader();
            depthShader.initShader();

            modelShader = new ModelShader();
            modelShader.initShader();

            edgeShader = new EdgeShader();
            edgeShader.initShader();

            simpleEdgeShader = new SimpleEdgeShader();
            simpleEdgeShader.initShader();

            fxAAShader = new FXAAShader();
            fxAAShader.initShader();

            gbufferShader = new GBufferShader();
            gbufferShader.initShader();

            floorShader = new FloorShaderProgram();
            floorShader.initShader();

            primitiveShader = new PrimitiveShader();
            primitiveShader.initShader();

            pointlightShader = new PointLightShader();
            pointlightShader.initShader();
        }

        protected void initCameras()
        {
            cameraMode = CameraMode.FPS;
            fpsCamera = new FPSCamera(new Vector3(0, 0, 100), new Vector3(0, 0, 0));
            aboveCamera = new AboveCamera();
            activeCamera = fpsCamera;
            aboveCamera.height = 100;

        }


        public virtual void AddLight(Light l) { }

        internal abstract void Render(RenderMode renderMode, AAMode aaMode, ContourMode contourMode);
    }
}
