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
    public class CClientMain
    {
        public CClientParse gCClientParse;
        public CClientEffect gCClientEffect;
        public CClientView gCClientView;
        public CClientEntity gCClientEntity;

        public CClientMain()
        {
            gCClientParse = new CClientParse();
            gCClientEffect = new CClientEffect();
            gCClientView = new CClientView();
            gCClientEntity = new CClientEntity();



            // TEMPORARY! (SHOULD BE IN SP_WORLDSPAWN)
            if (CClient.cl.configstrings == null)
                CClient.cl.configstrings = new string[CShared.MAX_CONFIGSTRINGS];

            // setup light animation tables. 'a' is total darkness, 'z' is doublebright.

            // 0 normal
            CClient.cl.configstrings[CShared.CS_LIGHTS + 0] = "m";

            // 1 FLICKER (first variety)
            CClient.cl.configstrings[CShared.CS_LIGHTS + 1] = "mmnmmommommnonmmonqnmmo";

            // 2 SLOW STRONG PULSE
            CClient.cl.configstrings[CShared.CS_LIGHTS + 2] = "abcdefghijklmnopqrstuvwxyzyxwvutsrqponmlkjihgfedcba";

            // 3 CANDLE (first variety)
            CClient.cl.configstrings[CShared.CS_LIGHTS + 3] = "mmmmmaaaaammmmmaaaaaabcdefgabcdefg";

            // 4 FAST STROBE
            CClient.cl.configstrings[CShared.CS_LIGHTS + 4] = "mamamamamama";

            // 5 GENTLE PULSE 1
            CClient.cl.configstrings[CShared.CS_LIGHTS + 5] = "jklmnopqrstuvwxyzyxwvutsrqponmlkj";

            // 6 FLICKER (second variety)
            CClient.cl.configstrings[CShared.CS_LIGHTS + 6] = "nmonqnmomnmomomno";

            // 7 CANDLE (second variety)
            CClient.cl.configstrings[CShared.CS_LIGHTS + 7] = "mmmaaaabcdefgmmmmaaaammmaamm";

            // 8 CANDLE (third variety)
            CClient.cl.configstrings[CShared.CS_LIGHTS + 8] = "mmmaaammmaaammmabcdefaaaammmmabcdefmmmaaaa";

            // 9 SLOW STROBE (fourth variety)
            CClient.cl.configstrings[CShared.CS_LIGHTS + 9] = "aaaaaaaazzzzzzzz";

            // 10 FLUORESCENT FLICKER
            CClient.cl.configstrings[CShared.CS_LIGHTS + 10] = "mmamammmmammamamaaamammma";

            // 11 SLOW PULSE NOT FADE TO BLACK
            CClient.cl.configstrings[CShared.CS_LIGHTS + 11] = "abcdefghijklmnopqrrqponmlkjihgfedcba";

            // styles 32-62 are assigned by the light program for switchable lights

            // 63 testing
            CClient.cl.configstrings[CShared.CS_LIGHTS + 63] = "a";
        }

        private void CL_ReadPackets()
        {
            gCClientParse.CL_ParseServerMessage();
        }

        public void CL_Frame(int msec)
        {
            CL_ReadPackets();

            gCClientEffect.CL_RunLightStyles();
        }

        public void CL_ClearState()
        {
            gCClientEffect.CL_ClearEffects();
        }

        public void CL_Disconnect()
        {
            CL_ClearState();
        }

    }
}
