﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using System.Drawing;

namespace Graphics
{
    class Renderer
    {
        float r = 1;
        float g = 1;
        float b = 1;

        bool Daylight = true;
        bool NightLight = false;

        int EyePositionID;
        int AmbientLightID;
        int DataID;

        public float Speed = 1;

        Shader sh;
        uint vertexBufferID;
        uint vertexBufferID2;
        uint vertexBufferID3;
        int transID;
        int viewID;
        int projID;
        mat4 scaleMatrix;
        Bitmap image;

        mat4 ProjectionMatrix;
        mat4 ViewMatrix;
        List<float> t_vertices;

        public Camera camera;

        Texture tex;
        Texture tex1;
        Texture tex2;
        Texture tex3;
        Texture tex4;
        Texture tex5;
        Texture tex6;
        Texture tex7;
        Texture tex9;

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");

            tex = new Texture(projectPath + "\\Textures\\crate.jpg", 1);
            tex1 = new Texture(projectPath + "\\Textures\\Ground.jpg", 2);
            tex2 = new Texture(projectPath + "\\Textures\\back.jpg", 3);
            tex3 = new Texture(projectPath + "\\Textures\\bottom.jpg", 4);
            tex4 = new Texture(projectPath + "\\Textures\\front.jpg", 5);
            tex5 = new Texture(projectPath + "\\Textures\\left.jpg", 6);
            tex6 = new Texture(projectPath + "\\Textures\\right.jpg", 7);
            tex7 = new Texture(projectPath + "\\Textures\\top.jpg", 8);
            tex9 = new Texture(projectPath + "\\Textures\\grass.jpg", 9);

            Gl.glClearColor(0, 0, 0.4f, 1);


            float[] verts =
            {
                // front
                -1, -1, 1,      1, 0, 1,  0, 0,   //0
                -1, 1, 1,       1, 0, 1,  0, 1,   //1
                1, -1, 1,       1, 0, 1,  1, 0,   //2

                1, -1, 1,       1, 0, 1,  1, 0,   //3
                1, 1, 1,        1, 0, 1,  1, 1,   //4
                -1, 1, 1,       1, 0, 1,  0, 1,   //5

                 // back
                 -1, 1, -1,     1, 0, 1,   0, 1,   //6
                 -1, -1, -1,    1, 0, 1,   0, 0,   //7
                 1, -1, -1,     1, 0, 1,   1, 0,   //8

                 1, -1, -1,     1, 0, 1,   1, 0,   //9
                 -1, 1, -1,     1, 0, 1,   0, 1,   //10
                 1, 1, -1,      1, 0, 1,   1, 1,   //11

                 // top
                 -1, 1, 1,      1, 0, 1,   0, 0,   //12
                 1, 1, 1,       1, 0, 1,   1, 0,   //13
                 -1, 1, -1,     1, 0, 1,   0, 1,   //14

                 1, 1, 1,       1, 0, 1,   1, 0,   //15
                -1, 1, -1,      1, 0, 1,   0, 1,   //16
                 1, 1, -1,      1, 0, 1,   1, 1,   //17

                 // bottom
                -1, -1, 1,      1, 0, 1,   0, 0,   //18
                 1, -1, 1,      1, 0, 1,   1, 0,   //19
                -1, -1, -1,     1, 0, 1,   0, 1,   //20

                 1, -1, 1,      1, 0, 1,   1, 0,   //21
                -1, -1, -1,     1, 0, 1,   0, 1,   //22
                 1, -1, -1,     1, 0, 1,   1, 1,    //23

                 // right
                 1, -1, 1,      1, 0, 1,   0, 0,    //24
                 1, 1, 1,       1, 0, 1,   0, 1,    //25
                 1, 1, -1,      1, 0, 1,   1, 1,    //26

                 1, 1, -1,      1, 0, 1,   1, 1,    //27
                 1, -1, 1,      1, 0, 1,   0, 0,   //28
                 1, -1, -1,     1, 0, 1,   1, 0,   //29

                 // left
                  -1, -1, 1,    1, 0, 1,   1, 0,  //30
                  -1, -1, -1,   1, 0, 1,   0, 0, //31
                  -1, 1, -1,    1, 0, 1,   0, 1, //32

                  -1, 1, -1,    1, 0, 1,   0, 1, //33
                  -1, -1, 1,    1, 0, 1,   1, 0, //34
                  -1, 1, 1,      1, 0, 1,  1, 1,  //35
            };


           float[] ground = 
            {
                 -0.5f, -0.5f, -0.5f,
                 1,0,0,
                 0,0,

                 0.5f, -0.5f, -0.5f,
                 1,0,0,
                 1,0,

                 0.0f,  0.5f, -0.5f,
                 1,0,0,
                 0.5f,1
            };
           image = (Bitmap)Bitmap.FromFile("heightMap.jpg");        
           int x, y;
           float[,] heights = new float[image.Width, image.Height];
           Color pixel;
           for( x=0; x<image.Width; x++)
            {
                for( y=0; y<image.Height; y++)
                {
                    pixel = image.GetPixel(x, y);              //get pixel di btrg3 el height"color" bta3 el pixel
                    heights[x, y] = pixel.R;                   //bakhod el R component, kan momken akhod g,b bs msh fr2a 3shan hya grey scale

                }
            }
            t_vertices = new List<float>();
            float h_size = image.Height;
            float w_size = image.Width;
            for (int i = 0; i < image.Width-1; i++)                           // blef 3la image, btl3 el pixel wl neighbours 
            {                                                               // kda el list di feha vertices(x,y,z) w uv
                for (int j = 0; j < image.Height-1; j++)
                {
                    t_vertices.Add(i);
                    t_vertices.Add(heights[i, j]/4);
                    t_vertices.Add(j);
                    t_vertices.Add(0);
                    t_vertices.Add(1);

                    t_vertices.Add(i + 1);
                    t_vertices.Add(heights[i+1, j]/4);
                    t_vertices.Add(j);
                    t_vertices.Add(1);
                    t_vertices.Add(1);

                    t_vertices.Add(i);
                    t_vertices.Add(heights[i, j+1]/4);
                    t_vertices.Add(j);
                    t_vertices.Add(0);
                    t_vertices.Add(0);

                    t_vertices.Add(i + 1);
                    t_vertices.Add(heights[i+1, j+1]/4);
                    t_vertices.Add(j + 1);
                    t_vertices.Add(1);
                    t_vertices.Add(0);

                   ////////////////////////////repeted verts 3shan arsm square m7taga six vertices msh 4 bs
                 
                    t_vertices.Add(i + 1);
                    t_vertices.Add(heights[i, j]/4);
                    t_vertices.Add(j);
                    t_vertices.Add(1);
                    t_vertices.Add(1);
              
                    t_vertices.Add(i);
                    t_vertices.Add(heights[i, j]/4);
                    t_vertices.Add(j);
                    t_vertices.Add(0);
                    t_vertices.Add(0);
                }

            }
        
            vertexBufferID = GPU.GenerateBuffer(verts);
            vertexBufferID2 = GPU.GenerateBuffer(ground);
            vertexBufferID3 = GPU.GenerateBuffer(t_vertices.ToArray());

            scaleMatrix = glm.scale(new mat4(1),new vec3(50, 50, 50));

            camera = new Camera();

            camera.Reset(0, 5, 5, 0, 0, 0, 0, 1, 0);
            ProjectionMatrix = camera.GetProjectionMatrix();
            //ViewMatrix = glm.lookAt(new vec3(0, 0, 5), new vec3(0, 0, 0), new vec3(0, 1, 0));
            ViewMatrix = camera.GetViewMatrix();

            transID = Gl.glGetUniformLocation(sh.ID, "model");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            DataID = Gl.glGetUniformLocation(sh.ID, "data");
            vec2 data = new vec2(100, 50);
            Gl.glUniform2fv(DataID, 1, data.to_array());

            int LightPositionID = Gl.glGetUniformLocation(sh.ID, "LightPosition_worldspace");
            vec3 lightPosition = new vec3(1.0f, 20f, 4.0f);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());
            //setup the ambient light component.
            AmbientLightID = Gl.glGetUniformLocation(sh.ID, "ambientLight");
            vec3 ambientLight = new vec3(0.2f, 0.18f, 0.01f);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
            //setup the eye position.
            EyePositionID = Gl.glGetUniformLocation(sh.ID, "EyePosition_worldspace");
          
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);

        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            sh.UseShader();
            
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, scaleMatrix.to_array());
            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, camera.GetProjectionMatrix().to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, camera.GetViewMatrix().to_array ());


            tex1.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 3);

            GPU.BindBuffer(vertexBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            tex4.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            tex3.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 18, 6);

            tex2.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 6, 6);

            tex5.Bind();    
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 30, 6);

            tex6.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 24, 6);

            tex7.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 12, 6);

            /////////////////////////

            mat4 model = MathHelper.MultiplyMatrices(new List<mat4>() { glm.translate(new mat4(1), new vec3(-30, -65, -100)), glm.scale(new mat4(1), new vec3(0.9f, 0.6f, 0.6f)) }); //b3ml tranformations 3l terrain
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, model.to_array());

            GPU.BindBuffer(vertexBufferID3);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);

            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            tex9.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLE_STRIP, 0, image.Height*image.Width);


            //////////////////////////////


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
        }

        public void SendLightData(float red, float green, float blue, float attenuation, float specularExponent)
        {
            vec3 ambientLight = new vec3(red, green, blue);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
            vec2 data = new vec2(attenuation, specularExponent);
            Gl.glUniform2fv(DataID, 1, data.to_array());
        }

        public void Update(float deltaTime)
        {
            camera.UpdateViewMatrix();
            ProjectionMatrix = camera.GetProjectionMatrix();
            ViewMatrix = camera.GetViewMatrix();

            SendLightData(r, g, b, 100, 20);
            if (Daylight)
            {
                r -= (float)0.0002;
                g -= (float)0.0002;
                b -= (float)0.0002;
                if( r <= 0.1)
                {
                    Daylight = false;
                    NightLight = true;
                }
            }
            else if (NightLight)
            {
                r += (float)0.0002;
                g += (float)0.0002;
                b += (float)0.0002;
                if (r >= 1)
                {
                    Daylight = true;
                    NightLight = false;
                }
            }

        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
