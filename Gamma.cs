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

namespace IdTech2MonoGame
{
    public class CGamma
    {
        private float GammaValue;

        public CGamma()
        {
            GammaValue = 2.1f;

            CProgram.gQ2Game.gCMain.gSGlobal.HLSL.xGamma = GammaValue;
        }

        public void GammaLighten()
        {
            if (GammaValue > 5.0f)
                GammaValue = 5.0f;

            if (GammaValue == 5.0f)
                return;

            GammaValue += 0.02f;
            GammaValue = (float)Math.Round(GammaValue, 2);

            CProgram.gQ2Game.gCMain.gSGlobal.HLSL.xGamma = GammaValue;
            
            //SetGamma(GammaValue, GammaValue, GammaValue);
        }

        public void GammaDarken()
        {
            if (GammaValue < 1.0f)
                GammaValue = 1.0f;

            if (GammaValue <= 1.0f)
                return;

            GammaValue -= 0.02f;
            GammaValue = (float)Math.Round(GammaValue, 2);

            CProgram.gQ2Game.gCMain.gSGlobal.HLSL.xGamma = GammaValue;

            //SetGamma(GammaValue, GammaValue, GammaValue);
        }

        /*private void SetGamma(float Red, float Green, float Blue)
        {
            // map the float values (0.0 min, 1.0 max) to byte values (0 min, 255 max)
            //byte redOffset = (byte)(Red * 255);
            //byte greenOffset = (byte)(Green * 255);
            //byte blueOffset = (byte)(Blue * 255);
            short redOffset = (short)(Red * 255);
            short greenOffset = (short)(Green * 255);
            short blueOffset = (short)(Blue * 255);

            // get the gamma ramp
            GammaRamp ramp = CProgram.gQ2Game.gGraphicsDevice.GetGammaRamp();
            short[] r = ramp.GetRed();
            short[] g = ramp.GetGreen();
            short[] b = ramp.GetBlue();

            // set the gamma values
            // they are stored as shorts, but are treated as ushorts by the hardware
            // if the value is over short.MaxValue, subtract ushort.MaxValue from it
            for (short i = 0; i < 256; i++)
            {
                r[i] *= redOffset;
                if (r[i] > short.MaxValue)
                {
                    r[i] -= (short)(r[i] - ushort.MaxValue * 2);
                    System.Diagnostics.Debug.WriteLine("over red");
                }
                //if (r[i] > 2550)
                //    r[i] = 2550;

                g[i] *= greenOffset;
                if (g[i] > short.MaxValue)
                {
                    g[i] -= (short)(g[i] - ushort.MaxValue * 2);
                    System.Diagnostics.Debug.WriteLine("over green");
                }
                //if (g[i] > 2550)
                //    g[i] = 2550;

                b[i] *= blueOffset;
                if (b[i] > short.MaxValue)
                {
                    b[i] -= (short)(b[i] - ushort.MaxValue * 2);
                    System.Diagnostics.Debug.WriteLine("over blue");
                }
                //if (b[i] > 2550)
                //    b[i] = 2550;
            }

            // set the gamma values
            ramp.SetRed(r);
            ramp.SetGreen(g);
            ramp.SetBlue(b);
            CProgram.gQ2Game.gGraphicsDevice.SetGammaRamp(true, ramp);
        }*/

        public float Gamma
        {
            get
            {
                return GammaValue;
            }
        }

    }
}
