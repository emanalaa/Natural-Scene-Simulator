using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using System.Drawing;
using Assimp;
using System.Windows.Forms;
namespace Graphics
{
    class Renderer
    {
        Shader Shader3D;
        Shader WaterShader;

        public Camera camera;

        //Light 
        float r = 1;
        float g = 1;
        float b = 1;

        bool Daylight = true;
        bool NightLight = false;

        int EyePositionID;
        int AmbientLightID;
        int DataID;

        public float Speed = 1;

        //Transformations
        uint SkyboxBufferID;
        uint TerrainBufferID;
        uint GrassBufferID;

        int transID;
        int viewID;
        int projID;


        mat4 scaleMatrix;
        mat4 ProjectionMatrix;
        mat4 ViewMatrix;
        //Water
        Texture WaterTexture;
        uint WaterBufferID;
        bool reverse;
        float t = 0;
        int WaterTimeID;
        Ground ground;

        //Terrain
        List<float> TerrainVertices;
        public float[,] heights;
        Bitmap TerrainImage;

        //SkyBox
        Texture BackTexture;
        Texture BottomTexture;
        Texture FrontTexture;
        Texture LeftTexture;
        Texture RightTexture;
        Texture TopTexture;

        //Tree and Grass 
        Texture GrassTexture;
        Model3D tree;
        List<float> GrassVertices;
        List<float> TreeVertices;

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            Shader3D = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            WaterShader = new Shader(projectPath + "\\Shaders\\WaterVertexShader.vertexshader", projectPath + "\\Shaders\\WaterFragmentShader.fragmentshader");

            //SkyBox
            BackTexture = new Texture(projectPath + "\\Textures\\back.jpg", 3, false);
            BottomTexture = new Texture(projectPath + "\\Textures\\bottom.jpg", 4, false);
            FrontTexture = new Texture(projectPath + "\\Textures\\front.jpg", 5, false);
            LeftTexture = new Texture(projectPath + "\\Textures\\left.jpg", 6, false);
            RightTexture = new Texture(projectPath + "\\Textures\\right.jpg", 7, false);
            TopTexture = new Texture(projectPath + "\\Textures\\top.jpg", 8, false);

            Gl.glClearColor(0, 0, 0.4f, 1);

            float[] SkyboxVertices =
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

            //Terrain
            TerrainImage = (Bitmap)Bitmap.FromFile("heightMap.jpg");
            heights = new float[TerrainImage.Width, TerrainImage.Height];
            TerrainVertices = new List<float>();
            float h_size = TerrainImage.Height;
            float w_size = TerrainImage.Width;
            Color pixel;
            int x, y;

            for ( x=0; x<TerrainImage.Width; x++)
            {
                for( y=0; y<TerrainImage.Height; y++)
                {
                    pixel = TerrainImage.GetPixel(x, y);              //get pixel di btrg3 el height"color" bta3 el pixel
                    heights[x, y] = pixel.R;                   //bakhod el R component, kan momken akhod g,b bs msh fr2a 3shan hya grey scale
                }
            }

            for (int i = 0; i < TerrainImage.Width-1; i++)                           // blef 3la TerrainImage, btl3 el pixel wl neighbours 
            {                                                               // kda el list di feha vertices(x,y,z) w uv
                for (int j = 0; j < TerrainImage.Height-1; j++)
                {
                    TerrainVertices.Add(i);
                    TerrainVertices.Add(heights[i, j]/4);
                    TerrainVertices.Add(j);
                    TerrainVertices.Add(0);
                    TerrainVertices.Add(1);

                    TerrainVertices.Add(i + 1);
                    TerrainVertices.Add(heights[i+1, j]/4);
                    TerrainVertices.Add(j);
                    TerrainVertices.Add(1);
                    TerrainVertices.Add(1);

                    TerrainVertices.Add(i);
                    TerrainVertices.Add(heights[i, j+1]/4);
                    TerrainVertices.Add(j);
                    TerrainVertices.Add(0);
                    TerrainVertices.Add(0);

                    TerrainVertices.Add(i + 1);
                    TerrainVertices.Add(heights[i+1, j+1]/4);
                    TerrainVertices.Add(j + 1);
                    TerrainVertices.Add(1);
                    TerrainVertices.Add(0);

                   ////////////////////////////repeted SkyboxVertices 3shan arsm square m7taga six vertices msh 4 bs
                 
                   
                    TerrainVertices.Add(i);
                    TerrainVertices.Add(heights[i, j]/4);
                    TerrainVertices.Add(j);
                    TerrainVertices.Add(0);
                    TerrainVertices.Add(0);

                     TerrainVertices.Add(i + 1);
                    TerrainVertices.Add(heights[i, j]/4);
                    TerrainVertices.Add(j);
                    TerrainVertices.Add(1);
                    TerrainVertices.Add(1);
              
                }

            }

            scaleMatrix = glm.scale(new mat4(1),new vec3(50, 50, 50));

            //grass
            GrassTexture = new Texture(projectPath + "\\Textures\\grass.jpg", 10, false);
            GrassVertices = new List<float>();
            for (int i = 0; i < TerrainImage.Width -1; i++)   // blef 3la TerrainImage, btl3 el pixel wl neighbours 
            {                                                               // kda el list di feha vertices(x,y,z) w uv
                for (int j = 0; j < TerrainImage.Height -1; j++)
                {
                    GrassVertices.Add(i);
                    GrassVertices.Add(heights[i, j] / 4);
                    GrassVertices.Add(j);
                    GrassVertices.Add(0);
                    GrassVertices.Add(1);

                    GrassVertices.Add(i + 1);
                    GrassVertices.Add(heights[i + 1, j] / 4);
                    GrassVertices.Add(j);
                    GrassVertices.Add(1);
                    GrassVertices.Add(1);

                    GrassVertices.Add(i);
                    GrassVertices.Add(heights[i, j + 1] / 4);
                    GrassVertices.Add(j);
                    GrassVertices.Add(0);
                    GrassVertices.Add(0);

                    GrassVertices.Add(i + 1);
                    GrassVertices.Add(heights[i + 1, j + 1] / 4);
                    GrassVertices.Add(j + 1);
                    GrassVertices.Add(1);
                    GrassVertices.Add(0);

                    GrassVertices.Add(i);
                    GrassVertices.Add(heights[i, j] / 4);
                    GrassVertices.Add(j);
                    GrassVertices.Add(0);
                    GrassVertices.Add(0);

                    GrassVertices.Add(i + 1);
                    GrassVertices.Add(heights[i, j] / 4);
                    GrassVertices.Add(j);
                    GrassVertices.Add(1);
                    GrassVertices.Add(1);
                    j += 8;
                }
                i += 8;
            }

            //Tree
            tree = new Model3D();
            tree.LoadFile(projectPath + "\\ModelFiles\\static\\tree", 11, "Tree.obj");

            TreeVertices = new List<float>();
            for (int i = 0; i < TerrainImage.Width / 4; i++)   
            {                                                              
                for (int j = 0; j < TerrainImage.Height / 4; j++)
                {
                    TreeVertices.Add(i);
                    TreeVertices.Add(heights[i, j] / 4);
                    TreeVertices.Add(j);

                    TreeVertices.Add(i + 1);
                    TreeVertices.Add(heights[i + 1, j] / 4);
                    TreeVertices.Add(j);

                    TreeVertices.Add(i);
                    TreeVertices.Add(heights[i, j + 1] / 4);
                    TreeVertices.Add(j);
                }
            }


            //Water
            List<float> WaterVertices = new List<float>();
            for (int i = 0; i < TerrainImage.Width / 4; i++)   // blef 3la TerrainImage, btl3 el pixel wl neighbours 
            {                                                               // kda el list di feha vertices(x,y,z) w uv
                for (int j = 0; j < TerrainImage.Height / 4; j++)
                {
                    WaterVertices.Add(i);
                    WaterVertices.Add(heights[i, j] / 4);
                    WaterVertices.Add(j);
                    WaterVertices.Add(0);
                    WaterVertices.Add(1);

                    WaterVertices.Add(i + 1);
                    WaterVertices.Add(heights[i + 1, j] / 4);
                    WaterVertices.Add(j);
                    WaterVertices.Add(1);
                    WaterVertices.Add(1);

                    WaterVertices.Add(i);
                    WaterVertices.Add(heights[i, j + 1] / 4);
                    WaterVertices.Add(j);
                    WaterVertices.Add(0);
                    WaterVertices.Add(0);

                    WaterVertices.Add(i + 1);
                    WaterVertices.Add(heights[i + 1, j + 1] / 4);
                    WaterVertices.Add(j + 1);
                    WaterVertices.Add(1);
                    WaterVertices.Add(0);

                    WaterVertices.Add(i);
                    WaterVertices.Add(heights[i, j] / 4);
                    WaterVertices.Add(j);
                    WaterVertices.Add(0);
                    WaterVertices.Add(0);

                    WaterVertices.Add(i + 1);
                    WaterVertices.Add(heights[i, j] / 4);
                    WaterVertices.Add(j);
                    WaterVertices.Add(1);
                    WaterVertices.Add(1);
                }
            }

            WaterTexture = new Texture(projectPath + "\\Textures\\water.PNG", 2);
            WaterTimeID = Gl.glGetUniformLocation(Shader3D.ID, "t");

            //ground
            //ground = new Ground(256, 256, 0, 8);

            //Camera
            camera = new Camera();
            camera.Reset(0, 5, 5, 0, 0, 0, 0, 1, 0);
            ProjectionMatrix = camera.GetProjectionMatrix();
            ViewMatrix = camera.GetViewMatrix();
            
            //Transformations and Shader Configurations
            transID = Gl.glGetUniformLocation(Shader3D.ID, "model");
            projID = Gl.glGetUniformLocation(Shader3D.ID, "projection");
            viewID = Gl.glGetUniformLocation(Shader3D.ID, "view");
            DataID = Gl.glGetUniformLocation(Shader3D.ID, "data");
            vec2 data = new vec2(100, 50);
            Gl.glUniform2fv(DataID, 1, data.to_array());

            //Light
            int LightPositionID = Gl.glGetUniformLocation(Shader3D.ID, "LightPosition_worldspace");
            vec3 lightPosition = new vec3(1.0f, 20f, 4.0f);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());

            //setup the ambient light component.
            AmbientLightID = Gl.glGetUniformLocation(Shader3D.ID, "ambientLight");
            vec3 ambientLight = new vec3(0.2f, 0.18f, 0.01f);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());

            //setup the eye position.
            EyePositionID = Gl.glGetUniformLocation(Shader3D.ID, "EyePosition_worldspace");

            SkyboxBufferID = GPU.GenerateBuffer(SkyboxVertices);
            TerrainBufferID = GPU.GenerateBuffer(TerrainVertices.ToArray());
            GrassBufferID = GPU.GenerateBuffer(GrassVertices.ToArray());
            WaterBufferID = GPU.GenerateBuffer(WaterVertices.ToArray());

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Shader3D.UseShader();

            /*WaterShader.UseShader();*/

            //water motion
            /*if (reverse)
                t -= 0.0002f;
            else
                t += 0.0002f;
            if (t > 0.2f)
                reverse = true;
            if (t < 0)
                reverse = false;
            Gl.glUniform1f(WaterTimeID, t);*/

            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, camera.GetProjectionMatrix().to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, camera.GetViewMatrix().to_array());
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, scaleMatrix.to_array());

            //SKYBOX START
            GPU.BindBuffer(SkyboxBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            FrontTexture.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            BottomTexture.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 18, 6);

            BackTexture.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 6, 6);

            LeftTexture.Bind();    
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 30, 6);

            RightTexture.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 24, 6);

            TopTexture.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 12, 6);
            //SKYBOX END

            //WATER START
            mat4 WaterModel = MathHelper.MultiplyMatrices(new List<mat4>() {
                glm.translate(new mat4(1), new vec3(-30, -65, -100)),
                glm.scale(new mat4(1), new vec3(2.5f, 0.6f, 0.6f))
            });

            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, WaterModel.to_array());

            GPU.BindBuffer(WaterBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);

            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            WaterTexture.Bind();
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glDrawArrays(Gl.GL_TRIANGLE_STRIP, 0, TerrainImage.Height / 4 * TerrainImage.Width / 4);

            Gl.glDisable(Gl.GL_BLEND);

            /*WaterTexture.Bind();
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            ground.Draw(transID);
            Gl.glDisable(Gl.GL_BLEND);*/

            //WATER END

            //TREE START
            for (int i = 0; i<4500- 3; i+=100)
            {
                //tree.scalematrix = glm.scale(new mat4(1), new vec3(2f, 2.5f, 2f));
                tree.transmatrix = glm.translate(new mat4(1), new vec3(TreeVertices[i], -1 * TreeVertices[i + 1], TreeVertices[i + 2]));
                tree.Draw(transID);
            }
            //TREE END

            //GRASS START
            mat4 GrassModel = MathHelper.MultiplyMatrices(new List<mat4>() {
                glm.translate(new mat4(1), new vec3(-30, -65, -100)),
                glm.scale(new mat4(1),new vec3(1.5f, .3f, .3f))
            });

            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, GrassModel.to_array());

            GPU.BindBuffer(GrassBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);

            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            GrassTexture.Bind();

            for (int i = 0; i <GrassVertices.Count; )
            {
                Gl.glDrawArrays(Gl.GL_TRIANGLES, i, 5);
                i += 5;
            }
            //GRASS END

            //TERRAIN START
            mat4 model = MathHelper.MultiplyMatrices(new List<mat4>() { glm.translate(new mat4(1), new vec3(-30, -65, -100)), glm.scale(new mat4(1), new vec3(0.9f, 0.6f, 0.6f)) }); //b3ml tranformations 3l terrain
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, model.to_array());

            GPU.BindBuffer(TerrainBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);

            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            BottomTexture.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLE_STRIP, 0, TerrainImage.Height * TerrainImage.Width);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SOURCE0_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            //TERRAIN END

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
            /*if (Daylight)
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
            }*/
            if(reverse)
                t -= 0.0002f;
            else
                t += 0.0002f;
            if (t > 0.2f)
                reverse = true;
            if (t < 0)
                reverse = false;
            Gl.glUniform1f(WaterTimeID, t);
        }

        public void CleanUp()
        {
            Shader3D.DestroyShader();
        }
    }
}
