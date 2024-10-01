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
    public class CClientView
    {
        private CLocal.SLightStyle[] r_lightstyles;

        public CClientView()
        {
            r_lightstyles = new CLocal.SLightStyle[CLocal.MAX_LIGHTSTYLES];

            for (int i = 0; i < CLocal.MAX_LIGHTSTYLES; i++)
            {
                r_lightstyles[i].white = 0;
                r_lightstyles[i].rgb = new float[3];
            }
        }

        public void V_AddLightStyle(int style, float r, float g, float b)
        {
            if (style < 0 || style > CShared.MAX_LIGHTSTYLES)
                CMain.Error(CMain.EErrorParm.ERR_WARNING, "Bad light style " + style.ToString());

            r_lightstyles[style].white = r + g + b;
            r_lightstyles[style].rgb[0] = r;
            r_lightstyles[style].rgb[1] = g;
            r_lightstyles[style].rgb[2] = b;
        }

        public void V_RenderView()
        {
            CProgram.gQ2Game.gCCommon.gCClientMain.gCClientEntity.CL_AddEntities();

            CClient.cl.RefDef.lightstyles = r_lightstyles;
        }

    }
}
