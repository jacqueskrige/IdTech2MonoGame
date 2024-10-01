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
    public class CClientParse
    {
        public string[] svc_strings =
        {
            "svc_bad",

            "svc_muzzleflash",
            "svc_muzzlflash2",
            "svc_temp_entity",
            "svc_layout",
            "svc_inventory",
            
            "svc_nop",
            "svc_disconnect",
            "svc_reconnect",
            "svc_sound",
            "svc_print",
            "svc_stufftext",
            "svc_serverdata",
            "svc_configstring",
            "svc_spawnbaseline",
            "svc_centerprint",
            "svc_download",
            "svc_playerinfo",
            "svc_packetentities",
            "svc_deltapacketentities",
            "svc_frame"
        };


        public void CL_ParseServerData()
        {
            CProgram.gQ2Game.gCCommon.gCClientMain.CL_ClearState();
        }

        private void CL_ParseConfigString()
        {
            int i;
            string s;

            //i = MSG_ReadShort(&net_message);
            //if (i < 0 || i >= MAX_CONFIGSTRINGS)
            //    Com_Error(ERR_DROP, "configstring > MAX_CONFIGSTRINGS");
            //s = MSG_ReadString(&net_message);

            //strncpy(olds, cl.configstrings[i], sizeof(olds));
            //olds[sizeof(olds) - 1] = 0;


            // jkrige - just temporary setting a fake network message
            i = CShared.CS_LIGHTS;
            s = "gjrbdwed";
            //if (CClient.cl.configstrings == null)
            //    CClient.cl.configstrings = new string[CShared.MAX_CONFIGSTRINGS];
            // jkrige - just temporary setting a fake network message

            CClient.cl.configstrings[i] = s;

            // do something apropriate

            if (i >= CShared.CS_LIGHTS && i < CShared.CS_LIGHTS + CShared.MAX_LIGHTSTYLES)
                CProgram.gQ2Game.gCCommon.gCClientMain.gCClientEffect.CL_SetLightstyle(i - CShared.CS_LIGHTS);
        }

        public void CL_ParseServerMessage()
        {
            CL_ParseServerData();

            CL_ParseConfigString();
        }

    }
}
