/*
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CQ2Game : Game
    {
        public GraphicsDeviceManager gGraphicsDeviceManager;
        public GraphicsDevice gGraphicsDevice;
        public Effect gEffect;
        public SpriteBatch spriteBatch;

        public CMain gCMain;
        public CCommon gCCommon;
        public CGamma gCGamma;

        // temporary text drawing
        SpriteFont Font1;
        Vector2 FontPos;

        /// <summary>
        /// CQ2Game
        /// ----------
        /// The game constructor
        /// </summary>
        public CQ2Game()
        {
            gGraphicsDeviceManager = new GraphicsDeviceManager(this);

            // set the GraphicsProfile to HiDef
            gGraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;

            Content.RootDirectory = "Content";
        }

        private void DrawOverlayText(Vector2 Origin, string Text, Color TextColor)
        {
            spriteBatch.Begin();

            Origin.X -= 1;
            Origin.Y -= 1;
            spriteBatch.DrawString(Font1, Text, FontPos, Color.Black, 0, Origin, 1.0f, SpriteEffects.None, 0.5f);

            Origin.X += 1;
            Origin.Y += 1;
            spriteBatch.DrawString(Font1, Text, FontPos, TextColor, 0, Origin, 1.0f, SpriteEffects.None, 0.5f);

            spriteBatch.End();
        }


        // =====================================================================
        //
        // XNA SPECIFIC FUNCTIONS
        // 
        // =====================================================================

        /// <summary>
        /// Initialize
        /// ----------
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = CMain.GameTitle;
            gGraphicsDeviceManager.PreferMultiSampling = true;
            gGraphicsDeviceManager.IsFullScreen = CConfig.GetConfigBOOL("Fullscreen");

            if (gGraphicsDeviceManager.IsFullScreen == true)
            {
                int Width;
                int Height;

                try
                {
                    Width = Convert.ToInt32(CConfig.GetConfigSTRING("Width"));
                    Height = Convert.ToInt32(CConfig.GetConfigSTRING("Height"));
                }
                catch
                {
                    Width = 1024;
                    Height = 768;
                }

                gGraphicsDeviceManager.PreferredBackBufferWidth = Width;
                gGraphicsDeviceManager.PreferredBackBufferHeight = Height;
            }
            else
            {
                gGraphicsDeviceManager.PreferredBackBufferWidth = 1024;
                gGraphicsDeviceManager.PreferredBackBufferHeight = 768;
            }

            if (CConfig.GetConfigBOOL("Vertical Sync") == true)
            {
                gGraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
                gGraphicsDeviceManager.GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Default;
            }
            else
            {
                gGraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
                gGraphicsDeviceManager.GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Immediate;

                TargetElapsedTime = TimeSpan.FromMilliseconds(1);
            }
            IsFixedTimeStep = true;

            gGraphicsDeviceManager.ApplyChanges();
            gGraphicsDevice = gGraphicsDeviceManager.GraphicsDevice;

            gCMain = new CMain();
            gCCommon = new CCommon();
            gCGamma = new CGamma();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent
        /// -----------
        /// Will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //if (CProgram.gQ2Game.gGraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
            {
                gCMain.r_maxLights = 8;
                gEffect = Content.Load<Effect>("effects40");
            }

            Font1 = Content.Load<SpriteFont>("SpriteFont1");
            FontPos = new Vector2(5, 5);

            gCMain.BuildWorldModel(CConfig.GetConfigSTRING("Map Name"));
            gCMain.WorldViewInit();
        }

        /// <summary>
        /// UnloadContent
        /// -------------
        /// Will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Update
        /// ------
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            gCMain.gTimeGame = gameTime;

            gCMain.FrameUpdate();
            gCCommon.Qcommon_Frame(1);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw
        /// ----
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Vector2 FontOrigin;
            gCMain.gTimeGame = gameTime;

            gGraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);


            // reset and set the rasterizer states
            CProgram.gQ2Game.gCMain.gRasterizerState = new RasterizerState();
            CProgram.gQ2Game.gCMain.gRasterizerState.CullMode = CullMode.CullCounterClockwiseFace;

            if (CProgram.gQ2Game.gCMain.r_wireframe == false)
                CProgram.gQ2Game.gCMain.gRasterizerState.FillMode = FillMode.Solid;
            else
                CProgram.gQ2Game.gCMain.gRasterizerState.FillMode = FillMode.WireFrame;

            gGraphicsDevice.RasterizerState = CProgram.gQ2Game.gCMain.gRasterizerState;
            
            
            // reset the blend and depth states
            gGraphicsDevice.BlendState = BlendState.Opaque;


            // reset and set the depthstencil states
            CProgram.gQ2Game.gCMain.gDepthStencilState = new DepthStencilState();
            CProgram.gQ2Game.gCMain.gDepthStencilState.DepthBufferEnable = true;
            gGraphicsDevice.DepthStencilState = CProgram.gQ2Game.gCMain.gDepthStencilState;


            gCMain.FrameRenderView();


            // we are drawing the overlay text here, because we don't want it to be affected by postprocessing

            // reset and set the rasterizer states
            CProgram.gQ2Game.gCMain.gRasterizerState = new RasterizerState();
            CProgram.gQ2Game.gCMain.gRasterizerState.FillMode = FillMode.Solid;
            gGraphicsDevice.RasterizerState = CProgram.gQ2Game.gCMain.gRasterizerState;

            FontOrigin.X = 0;
            FontOrigin.Y = 0;
            DrawOverlayText(FontOrigin, CClient.cl.RefDef.MapName, Color.White);

            FontOrigin.X = 0;
            FontOrigin.Y = -40;
            DrawOverlayText(FontOrigin, "FPS Rate: " + gCMain.FrameRate.FrameRate.ToString(), Color.Silver);

            FontOrigin.X = 0;
            FontOrigin.Y = -60;
            DrawOverlayText(FontOrigin, "PVS Cluster: " + CMain.r_viewcluster.ToString(), Color.Silver);

            FontOrigin.X = 0;
            FontOrigin.Y = -80;
            DrawOverlayText(FontOrigin, "PVS Locked: " + gCMain.r_lockpvs.ToString(), Color.Silver);

            FontOrigin.X = 0;
            FontOrigin.Y = -100;
            DrawOverlayText(FontOrigin, "Primitives: " + CMain.c_brush_polys.ToString(), Color.Silver);

            FontOrigin.X = 0;
            FontOrigin.Y = -120;
            DrawOverlayText(FontOrigin, "Origin: (XYZ) " + Convert.ToInt64(CClient.cl.RefDef.ViewOrigin.X).ToString() + ", " + Convert.ToInt64(CClient.cl.RefDef.ViewOrigin.Y).ToString() + ", " + Convert.ToInt64(CClient.cl.RefDef.ViewOrigin.Z).ToString(), Color.Silver);

            FontOrigin.X = 0;
            FontOrigin.Y = -140;
            DrawOverlayText(FontOrigin, "View: (XYZ) " + Convert.ToInt64(CClient.cl.RefDef.ViewAngles.X).ToString() + ", " + Convert.ToInt64(CClient.cl.RefDef.ViewAngles.Y).ToString() + ", " + Convert.ToInt64(CClient.cl.RefDef.ViewAngles.Z).ToString(), Color.Silver);

            FontOrigin.X = 0;
            FontOrigin.Y = -160;
            DrawOverlayText(FontOrigin, "Gamma: " + gCMain.gSGlobal.HLSL.xGamma.ToString(), Color.Silver);

            FontOrigin.X = 0;
            FontOrigin.Y = -200;
            DrawOverlayText(FontOrigin, "[SPACE] show/hide controls", Color.Gray);

            if (CProgram.gQ2Game.gCMain.r_controls == true)
            {
                FontOrigin.X = 0;
                FontOrigin.Y = -220;
                DrawOverlayText(FontOrigin, "[ARROWS] forward/back, roll", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -240;
                DrawOverlayText(FontOrigin, "[Q]/[A] up/down", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -260;
                DrawOverlayText(FontOrigin, "[Z]/[X] left/right", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -280;
                DrawOverlayText(FontOrigin, "[MOUSE] pitch/yaw", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -300;
                DrawOverlayText(FontOrigin, "[O] fill mode", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -320;
                DrawOverlayText(FontOrigin, "[+]/[-] gamma", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -340;
                DrawOverlayText(FontOrigin, "[P] pointlights on/off", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -360;
                DrawOverlayText(FontOrigin, "[H] lightmaps on/off", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -380;
                DrawOverlayText(FontOrigin, "[L] pvs lock on/off", Color.Gray);

                FontOrigin.X = 0;
                FontOrigin.Y = -400;
                DrawOverlayText(FontOrigin, "[E] entities on/off", Color.Gray);
            }

            base.Draw(gameTime);
        }

    }
}
