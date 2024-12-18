﻿/*
==========================================================================================
MIT License

Copyright (c) 2011-2024 Corax Software

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
==========================================================================================
Quake is a trademark of Id Software, Inc., (c) 1996 Id Software, Inc. All rights reserved.
All other trademarks are the property of their respective owners.
------------------------------------------------------------------------------------------
 - Jacques Krige
 - jkrige1978@gmail.com
 - www.jacqueskrige.site
 - www.corax.software
==========================================================================================
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IdTech2MonoGame
{
    public class CSky
    {
        private int[] skytexorder;
        private string[] suf;

        private string skyname; // size: MAX_QPATH
        private float skyrotate;
        private Vector3 skyaxis;

        private float[,] skymins;
        private float[,] skymaxs;

        Quaternion SkyboxRotation;

        private VertexDeclaration SkyboxVertexDeclaration;
        private int[,] st_to_vec;

        public CSky()
        {
            skytexorder = new int[] { 0, 2, 1, 3, 4, 5 };
            suf = new string[] { "rt", "bk", "lf", "ft", "up", "dn" };

            // 1 = s, 2 = t, 3 = 2048
            st_to_vec = new int[,]
            {
                { 3, -1, 2 },
                { -3, 1, 2 },
                { 1, 3, 2 },
                { -1, -3, 2 },
                { -2, -1, 3 }, // 0 degrees yaw, look straight up
                { 2, -1, -3 }  // look straight down
            }; // [6, 3]

            skymins = new float[,]
            {
                { -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f },
                { -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f }
            }; // [2, 6]

            skymaxs = new float[,]
            {
                { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },
                { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }
            }; // [2, 6]

            SkyboxRotation = Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, 0.0f);
        }

        private SkyboxVertexFormat MakeSkyVec(int Width, float s, float t, int axis)
        {
            Vector3 v;
            SkyboxVertexFormat SVF;
            float[] v2 = new float[3];
            float[] b = new float[] { s * Width, t * Width, Width };
            float sky_min = 1.0f / Width;
            float sky_max = (Width - 1.0f) / Width;

            for (int i = 0; i < 3; i++)
            {
                int k = st_to_vec[axis, i];

                if (k < 0)
                    v2[i] = -b[-k - 1];
                else
                    v2[i] = b[k - 1];
            }

            v.X = v2[0];
            v.Y = v2[1];
            v.Z = v2[2];

            // avoid bilerp seam
            s = (s + 1) * 0.5f;
            t = (t + 1) * 0.5f;

            if (s < sky_min)
                s = sky_min;
            else if (s > sky_max)
                s = sky_max;

            if (t < sky_min)
                t = sky_min;
            else if (t > sky_max)
                t = sky_max;

            t = 1.0f - t;

            SVF.Position.X = v.X;
            SVF.Position.Y = v.Y;
            SVF.Position.Z = v.Z;
            SVF.TextureCoordinate.X = s;
            SVF.TextureCoordinate.Y = t;

            return SVF;
        }

        public void DrawSkyBox()
        {
            Matrix wMatrix;
            float RotationSpeed = ((float)CProgram.gQ2Game.gCMain.gTimeGame.ElapsedGameTime.TotalMilliseconds / 1000.0f) * skyrotate;

            // update skybox rotation
            SkyboxRotation *= Quaternion.CreateFromYawPitchRoll
                (
                Microsoft.Xna.Framework.MathHelper.ToRadians(skyaxis.Y * RotationSpeed),
                Microsoft.Xna.Framework.MathHelper.ToRadians(skyaxis.X * RotationSpeed),
                Microsoft.Xna.Framework.MathHelper.ToRadians(skyaxis.Z * RotationSpeed)
                );

            // update HLSL variables
            CProgram.gQ2Game.gCMain.UpdateHLSL(-1);

            // set vertex buffer
            CProgram.gQ2Game.gGraphicsDevice.SetVertexBuffer(CQ2BSP.SWorldData.vbSkybox);

            // create skybox rotation and translation matrix
            wMatrix = Matrix.CreateFromQuaternion(SkyboxRotation);
            wMatrix *= Matrix.CreateTranslation(CClient.cl.RefDef.ViewOrigin);

            // update HLSL variables
            CProgram.gQ2Game.gEffect.Parameters["xWorld"].SetValue(wMatrix);

            // set a rendering technique
            CProgram.gQ2Game.gEffect.CurrentTechnique = CProgram.gQ2Game.gEffect.Techniques["TexturedSkybox"];

            // reset the depthstencil state and disable the depth buffer
            CProgram.gQ2Game.gCMain.gDepthStencilState = new DepthStencilState();
            CProgram.gQ2Game.gCMain.gDepthStencilState.DepthBufferWriteEnable = false;
            CProgram.gQ2Game.gGraphicsDevice.DepthStencilState = CProgram.gQ2Game.gCMain.gDepthStencilState;

            for (int i = 0; i < 6; i++)
            {
                // bind the skybox side texture
                CProgram.gQ2Game.gEffect.Parameters["xTextureDiffuse"].SetValue(CQ2BSP.SWorldData.WorldSkies[skytexorder[i]]);

                // render the skybox side primitives
                foreach (EffectPass pass in CProgram.gQ2Game.gEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    CProgram.gQ2Game.gGraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, i * 6, 2);
                }
            }

            // reset the depthstencil state and enable the depth buffer
            CProgram.gQ2Game.gCMain.gDepthStencilState = new DepthStencilState();
            CProgram.gQ2Game.gCMain.gDepthStencilState.DepthBufferWriteEnable = true;
            CProgram.gQ2Game.gGraphicsDevice.DepthStencilState = CProgram.gQ2Game.gCMain.gDepthStencilState;
        }

        public void BuildSkybox()
        {
            List<SkyboxVertexFormat> vertbuffer = new List<SkyboxVertexFormat>();
            SkyboxVertexDeclaration = new VertexDeclaration(SkyboxVertexFormat.VertexElements);

            // get skybox info from the worldspawn
            // "sky"       : environment map name
            // "skyaxis"   : vector axis for rotating sky
            // "skyrotate" : speed of rotation in degrees/second
            for (int i = 0; i < CQ2BSP.SWorldData.entities.Length; i++)
            {
                if (CProgram.gQ2Game.gCMain.gCModel.ValueForKey(CQ2BSP.SWorldData.entities[i], "classname") == "worldspawn")
                {
                    string _skyrotate;
                    string _skyaxis;

                    skyname = CProgram.gQ2Game.gCMain.gCModel.ValueForKey(CQ2BSP.SWorldData.entities[i], "sky");
                    if (string.IsNullOrEmpty(skyname) == true)
                        skyname = "unit1_";

                    _skyaxis = CProgram.gQ2Game.gCMain.gCModel.ValueForKey(CQ2BSP.SWorldData.entities[i], "skyaxis");
                    if (string.IsNullOrEmpty(_skyaxis) == true)
                    {
                        skyaxis.X = 0.0f;
                        skyaxis.Y = 0.0f;
                        skyaxis.Z = 0.0f;
                    }
                    else
                    {
                        string[] SkyAxisArgs;
                        char[] delim = new char[1];

                        delim[0] = ' ';
                        SkyAxisArgs = _skyaxis.Split(delim, StringSplitOptions.RemoveEmptyEntries);

                        if (SkyAxisArgs.Length == 3)
                        {
                            skyaxis.X = Convert.ToSingle(SkyAxisArgs[0]);
                            skyaxis.Y = Convert.ToSingle(SkyAxisArgs[1]);
                            skyaxis.Z = Convert.ToSingle(SkyAxisArgs[2]);
                        }
                    }

                    _skyrotate = CProgram.gQ2Game.gCMain.gCModel.ValueForKey(CQ2BSP.SWorldData.entities[i], "skyrotate");
                    if (string.IsNullOrEmpty(_skyrotate) == true)
                        skyrotate = 0.0f;
                    else
                        skyrotate = Convert.ToSingle(_skyrotate);
                }
            }


            CProgram.gQ2Game.gCMain.gCImage.StartSky(ref CQ2BSP.SWorldData);

            for (int i = 0; i < 6; i++)
            {
                int Width;
                int Height;

                CProgram.gQ2Game.gCMain.gCImage.FindImage(skyname + suf[i], out Width, out Height, CImage.EImageType.IT_SKY);

                // triangle 1 (TriangleList)
                vertbuffer.Add(MakeSkyVec(Width, skymins[0, i], skymins[1, i], i));
                vertbuffer.Add(MakeSkyVec(Width, skymins[0, i], skymaxs[1, i], i));
                vertbuffer.Add(MakeSkyVec(Width, skymaxs[0, i], skymaxs[1, i], i));

                // triangle 2 (TriangleList)
                vertbuffer.Add(MakeSkyVec(Width, skymins[0, i], skymins[1, i], i));
                vertbuffer.Add(MakeSkyVec(Width, skymaxs[0, i], skymaxs[1, i], i));
                vertbuffer.Add(MakeSkyVec(Width, skymaxs[0, i], skymins[1, i], i));

                // TriangleFan
                //vertbuffer.Add(MakeSkyVec(Width, skymins[0, i], skymins[1, i], i));
                //vertbuffer.Add(MakeSkyVec(Width, skymins[0, i], skymaxs[1, i], i));
                //vertbuffer.Add(MakeSkyVec(Width, skymaxs[0, i], skymaxs[1, i], i));
                //vertbuffer.Add(MakeSkyVec(Width, skymaxs[0, i], skymins[1, i], i));
            }

            CQ2BSP.SWorldData.vbSkybox = new VertexBuffer(CProgram.gQ2Game.gGraphicsDevice, SkyboxVertexDeclaration, vertbuffer.Count, BufferUsage.WriteOnly);
            CQ2BSP.SWorldData.vbSkybox.SetData(vertbuffer.ToArray());

            CProgram.gQ2Game.gCMain.gCImage.FinalizeSky(ref CQ2BSP.SWorldData);
        }


        // ================================================================
        // 
        // SKYBOX VERTEX FORMAT
        // 
        // ================================================================

        public struct SkyboxVertexFormat
        {
            public Vector3 Position;
            public Vector2 TextureCoordinate;

            public static VertexElement[] VertexElements =
            {
                new VertexElement(sizeof(float) * 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            };
        }

    }
}
