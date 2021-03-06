﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class BasicShaderProgram : ShaderProgram
    {
        public int colorLocation { get; protected set; }

        public uint vertexPositionIndex { get; protected set; }
        public uint vertexTexCoordIndex { get; protected set; }
        public uint vertexNormalIndex { get; protected set; }

        public BasicShaderProgram()
        {
           // initShader();
        }

        public void initShader() {
            Console.WriteLine("BASIC SHADER CONSTRUCTOR");

            loadVertShader("../../shaders/basicVert.vert");
            loadFragShader("../../shaders/basicFrag.frag");
            loadProgram();


            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }

        public new void useProgram() {
            base.useProgram();
            Gl.EnableVertexAttribArray(vertexPositionIndex);
            Gl.EnableVertexAttribArray(vertexTexCoordIndex);
            Gl.EnableVertexAttribArray(vertexNormalIndex);
        }


        public void setTime(int time)
        {
            program["time"].SetValue((float)time);
        }

        public void setLightDirection(Vector3 lightDir)
        {
            program["light_direction"].SetValue(lightDir);

        }

        public void setRotationMatrix(Matrix4 matrix)
        {
            program["rotation_matrix"].SetValue(matrix);

        }

        public void setProjectionMatrix(Matrix4 matrix)
        {
            program["projection_matrix"].SetValue(matrix);
            
        }

        public void setViewMatrix(Matrix4 matrix)
        {
            program["view_matrix"].SetValue(matrix);

        }

        public void setModelMatrix(Matrix4 matrix)
        {
            program["model_matrix"].SetValue(matrix);

        }
    }
}