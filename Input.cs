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
using Microsoft.Xna.Framework.Input;

namespace IdTech2MonoGame
{
    public class CInput
    {
        KeyboardState KeyboardStateLast;
        KeyboardState KeyboardStateCurrent;

        MouseState MouseStateOriginal;
        MouseState MouseStateCurrent;

        public CInput()
        {
            Mouse.SetPosition(CProgram.gQ2Game.gGraphicsDevice.Viewport.Width / 2, CProgram.gQ2Game.gGraphicsDevice.Viewport.Height / 2);
            MouseStateOriginal = Mouse.GetState();
        }

        public void ProcessMouse()
        {
            float RotationPitch = 0.0f;
            float RotationYaw = 0.0f;
            Quaternion RotationAdd;

            MouseStateCurrent = Mouse.GetState();

            if (MouseStateCurrent != MouseStateOriginal)
            {
                float xDifference = MouseStateCurrent.X - MouseStateOriginal.X;
                float yDifference = MouseStateCurrent.Y - MouseStateOriginal.Y;

                // the magic number 16 is used because its the number of milliseconds that passes when vsync is turned on
                // this ensures the mouse response remains constant whether vsync is turned on/off
                xDifference *= (16 / CProgram.gQ2Game.TargetElapsedTime.Milliseconds);
                yDifference *= (16 / CProgram.gQ2Game.TargetElapsedTime.Milliseconds);

                RotationYaw += CProgram.gQ2Game.gCMain.SpeedMouse * xDifference;
                RotationPitch -= CProgram.gQ2Game.gCMain.SpeedMouse * yDifference;

                Mouse.SetPosition(CProgram.gQ2Game.gGraphicsDevice.Viewport.Width / 2, CProgram.gQ2Game.gGraphicsDevice.Viewport.Height / 2);
            }


            // update camera pitch & yaw
            CClient.cl.RefDef.ViewAngles.X += Microsoft.Xna.Framework.MathHelper.ToDegrees(RotationPitch);
            CClient.cl.RefDef.ViewAngles.Z += Microsoft.Xna.Framework.MathHelper.ToDegrees(RotationYaw);

            if (CClient.cl.RefDef.ViewAngles.X < 0.0f)
                CClient.cl.RefDef.ViewAngles.X += 360.0f;
            else if (CClient.cl.RefDef.ViewAngles.X > 360.0f)
                CClient.cl.RefDef.ViewAngles.X -= 360.0f;

            if (CClient.cl.RefDef.ViewAngles.Z < 0.0f)
                CClient.cl.RefDef.ViewAngles.Z += 360.0f;
            else if (CClient.cl.RefDef.ViewAngles.Z > 360.0f)
                CClient.cl.RefDef.ViewAngles.Z -= 360.0f;

            // rotation
            RotationAdd = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -RotationPitch) * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), -RotationYaw);
            CClient.cl.RefDef.TargetAngles *= RotationAdd;
        }

        public void ProcessKeyboard()
        {
            float RotationRoll = 0.0f;
            Quaternion RotationAdd;
            Vector3 OriginAdd;

            KeyboardStateLast = KeyboardStateCurrent;
            KeyboardStateCurrent = Keyboard.GetState();


            // update camera roll
            if (KeyboardStateCurrent.IsKeyDown(Keys.Left) == true || KeyboardStateCurrent.IsKeyDown(Keys.Right) == true)
            {
                if (KeyboardStateCurrent.IsKeyDown(Keys.Left) == true)
                    RotationRoll -= CProgram.gQ2Game.gCMain.SpeedRotation;

                if (KeyboardStateCurrent.IsKeyDown(Keys.Right) == true)
                    RotationRoll += CProgram.gQ2Game.gCMain.SpeedRotation;
            }

            CClient.cl.RefDef.ViewAngles.Y += Microsoft.Xna.Framework.MathHelper.ToDegrees(RotationRoll);

            if (CClient.cl.RefDef.ViewAngles.Y < 0.0f)
                CClient.cl.RefDef.ViewAngles.Y += 360.0f;
            else if (CClient.cl.RefDef.ViewAngles.Y > 360.0f)
                CClient.cl.RefDef.ViewAngles.Y -= 360.0f;


            // movement for camera forward/backward
            if (KeyboardStateCurrent.IsKeyDown(Keys.Up) == true || KeyboardStateCurrent.IsKeyDown(Keys.Down) == true)
            {
                if (KeyboardStateCurrent.IsKeyDown(Keys.Up) == true)
                    CProgram.gQ2Game.gCMain.MoveForward = -1;

                if (KeyboardStateCurrent.IsKeyDown(Keys.Down) == true)
                    CProgram.gQ2Game.gCMain.MoveForward = 1;
            }
            else
            {
                CProgram.gQ2Game.gCMain.MoveForward = 0;
            }

            // movement for camera left/right (strafe)
            if (KeyboardStateCurrent.IsKeyDown(Keys.Z) == true || KeyboardStateCurrent.IsKeyDown(Keys.X) == true)
            {
                if (KeyboardStateCurrent.IsKeyDown(Keys.Z) == true)
                    CProgram.gQ2Game.gCMain.MoveSide = 1;

                if (KeyboardStateCurrent.IsKeyDown(Keys.X) == true)
                    CProgram.gQ2Game.gCMain.MoveSide = -1;
            }
            else
            {
                CProgram.gQ2Game.gCMain.MoveSide = 0;
            }

            // movement for camera up/down
            if (KeyboardStateCurrent.IsKeyDown(Keys.Q) == true || KeyboardStateCurrent.IsKeyDown(Keys.A) == true)
            {
                if (KeyboardStateCurrent.IsKeyDown(Keys.Q) == true)
                    CProgram.gQ2Game.gCMain.MoveUp = 1;

                if (KeyboardStateCurrent.IsKeyDown(Keys.A) == true)
                    CProgram.gQ2Game.gCMain.MoveUp = -1;
            }
            else
            {
                CProgram.gQ2Game.gCMain.MoveUp = 0;
            }

            // quit
            if (KeyboardStateCurrent.IsKeyDown(Keys.Escape) == true)
            {
                CProgram.gQ2Game.Exit();
            }

            // switch between wireframe and solid rendering modes
            if (KeyboardStateCurrent.IsKeyDown(Keys.O) == true && KeyboardStateLast.IsKeyUp(Keys.O) == true)
            {
                CProgram.gQ2Game.gCMain.r_wireframe = !CProgram.gQ2Game.gCMain.r_wireframe;
            }

            // switch entity on and off
            if (KeyboardStateCurrent.IsKeyDown(Keys.E) == true && KeyboardStateLast.IsKeyUp(Keys.E) == true)
            {
                CProgram.gQ2Game.gCMain.r_drawentities = !CProgram.gQ2Game.gCMain.r_drawentities;
            }

            // set the gamma intensity
            if (KeyboardStateCurrent.IsKeyDown(Keys.Subtract) == true)
            {
                CProgram.gQ2Game.gCGamma.GammaDarken();
            }
            if (KeyboardStateCurrent.IsKeyDown(Keys.Add) == true)
            {
                CProgram.gQ2Game.gCGamma.GammaLighten();
            }

            // switch hardware pointlight on and off
            if (KeyboardStateCurrent.IsKeyDown(Keys.P) == true && KeyboardStateLast.IsKeyUp(Keys.P) == true)
            {
                CProgram.gQ2Game.gCMain.gSGlobal.HLSL.xPointLights = !CProgram.gQ2Game.gCMain.gSGlobal.HLSL.xPointLights;
            }

            // switch between hardware per-pixel lighting and classic lightmaps
            if (KeyboardStateCurrent.IsKeyDown(Keys.H) == true && KeyboardStateLast.IsKeyUp(Keys.H) == true)
            {
                CProgram.gQ2Game.gCMain.r_hardwarelight = !CProgram.gQ2Game.gCMain.r_hardwarelight;
            }

            // switch pvs lock on and off
            if (KeyboardStateCurrent.IsKeyDown(Keys.L) == true && KeyboardStateLast.IsKeyUp(Keys.L) == true)
            {
                CProgram.gQ2Game.gCMain.r_lockpvs = !CProgram.gQ2Game.gCMain.r_lockpvs;
            }

            if (KeyboardStateCurrent.IsKeyDown(Keys.Space) == true && KeyboardStateLast.IsKeyUp(Keys.Space) == true)
            {
                CProgram.gQ2Game.gCMain.r_controls = !CProgram.gQ2Game.gCMain.r_controls;
            }


            // rotation
            RotationAdd = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -RotationRoll);
            CClient.cl.RefDef.TargetAngles *= RotationAdd;

            // move left/right
            OriginAdd = Vector3.Transform(new Vector3(1, 0, 0), CClient.cl.RefDef.TargetAngles);
            CClient.cl.RefDef.TargetOrigin += OriginAdd * CProgram.gQ2Game.gCMain.SpeedSide;

            // move forward/backward
            OriginAdd = Vector3.Transform(new Vector3(0, 1, 0), CClient.cl.RefDef.TargetAngles);
            CClient.cl.RefDef.TargetOrigin += OriginAdd * CProgram.gQ2Game.gCMain.SpeedForward;

            // move up/down
            OriginAdd = Vector3.Transform(new Vector3(0, 0, 1), CClient.cl.RefDef.TargetAngles);
            CClient.cl.RefDef.TargetOrigin += OriginAdd * CProgram.gQ2Game.gCMain.SpeedUp;

            // update ViewOrigin
            CClient.cl.RefDef.ViewOrigin = CClient.cl.RefDef.TargetOrigin;
        }

    }
}
