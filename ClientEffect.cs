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
    public class CClientEffect
    {
        // ==============================================================
        // 
        // LIGHT STYLE MANAGEMENT
        // 
        // ==============================================================
        public SCLightStyle[] cl_lightstyle;
        public int lastofs;

        public CClientEffect()
        {
            cl_lightstyle = new SCLightStyle[CLocal.MAX_LIGHTSTYLES];

            for (int i = 0; i < CLocal.MAX_LIGHTSTYLES; i++)
            {
                cl_lightstyle[i].length = 0;
                cl_lightstyle[i].value = new float[3];
                cl_lightstyle[i].map = new float[CShared.MAX_QPATH];
            }
        }

        public void CL_ClearLightStyles()
        {
            for (int i = 0; i < cl_lightstyle.Length; i++)
            {
                cl_lightstyle[i].length = 0;

                for (int j = 0; j < cl_lightstyle[i].value.Length; j++)
                {
                    cl_lightstyle[i].value[j] = 0.0f;
                }

                for (int j = 0; j < cl_lightstyle[i].map.Length; j++)
                {
                    cl_lightstyle[i].map[j] = 0.0f;
                }
            }

            lastofs = -1;
        }

        public void CL_RunLightStyles()
        {
            int ofs;

            ofs = (int)(CProgram.gQ2Game.gCMain.gTimeGame.TotalGameTime.TotalMilliseconds / 1000.0f);
            if (ofs == lastofs)
                return;

            lastofs = ofs;

            for (int i = 0; i < CLocal.MAX_LIGHTSTYLES; i++)
            {
                if (cl_lightstyle[i].length == 0)
                {
                    cl_lightstyle[i].value[0] = cl_lightstyle[i].value[1] = cl_lightstyle[i].value[2] = 1.0f;
                    continue;
                }

                if (cl_lightstyle[i].length == 1)
                    cl_lightstyle[i].value[0] = cl_lightstyle[i].value[1] = cl_lightstyle[i].value[2] = cl_lightstyle[i].map[0];
                else
                    cl_lightstyle[i].value[0] = cl_lightstyle[i].value[1] = cl_lightstyle[i].value[2] = cl_lightstyle[i].map[ofs % cl_lightstyle[i].length];
            }
        }

        public void CL_SetLightstyle(int i)
        {
            int len;
            string s;

            s = CClient.cl.configstrings[CShared.CS_LIGHTS + i];
            len = s.Length;

            if (len >= CShared.MAX_QPATH)
                CMain.Error(CMain.EErrorParm.ERR_WARNING, "svc_lightstyle length=" + len.ToString());

            cl_lightstyle[i].length = len;

            for (int j = 0; j < len; j++)
            {
                cl_lightstyle[i].map[j] = (float)(s[j] - 'a') / (float)('m' - 'a');
            }
        }

        public void CL_AddLightStyles()
        {
            for (int i = 0; i < CLocal.MAX_LIGHTSTYLES; i++)
            {
                CProgram.gQ2Game.gCCommon.gCClientMain.gCClientView.V_AddLightStyle(i, cl_lightstyle[i].value[0], cl_lightstyle[i].value[1], cl_lightstyle[i].value[2]);
            }
        }

        public void CL_ClearEffects()
        {
            CL_ClearLightStyles();
        }

        public struct SCLightStyle
        {
            public int length;
            public float[] value; // size: 3
            public float[] map; // size: MAX_QPATH
        }

    }
}
