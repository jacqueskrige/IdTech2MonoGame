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
    public class CCommon
    {
        public CClientMain gCClientMain;

        // IMPORTANT NOTE: #1
        // main.cs will probably be desolved eventually as common.cs will likely be the main entry point
        // either that or main.cs will be used via xna draws while common.cs will be used via xna updates
        // ...something like that

        // IMPORTANT NOTE: #2
        // all the client source code from the common.cs class downwards is not referenced at this time.
        // its a guideline for continued development.

        public CCommon()
        {
            gCClientMain = new CClientMain();
        }

        public void Qcommon_Frame(int msec)
        {
            //if (host_speeds->value)
            //    time_before = Sys_Milliseconds();

            //SV_Frame(msec);

            //if (host_speeds->value)
            //    time_between = Sys_Milliseconds();

            gCClientMain.CL_Frame(msec);

            gCClientMain.gCClientView.V_RenderView();
        }


        public struct SSizeBuf
        {
            public bool allowoverflow; // if false, do a Com_Error
            public bool overflowed; // set to true if the buffer size failed
            public byte[] data;
            public int maxsize;
            public int cursize;
            public int readcount;
        }


        // ==============================================================
        // 
        // PROTOCOL
        // 
        // ==============================================================
        public const int PROTOCOL_VERSION = 34;

        public const int PORT_MASTER = 27900;
        public const int PORT_CLIENT = 27901;
        public const int PORT_SERVER = 27910;

        public const int UPDATE_BACKUP = 16; // copies of entity_state_t to keep buffered, must be power of two
        public const int UPDATE_MASK = UPDATE_BACKUP - 1;

        // the svc_strings[] array in ClientParse.cs should mirror this.

        // server to client
        public enum ESVC_Ops
        {
            svc_bad,
            
            // these ops are known to the game dll
            svc_muzzleflash,
            svc_muzzleflash2,
            svc_temp_entity,
            svc_layout,
            svc_inventory,

            // the rest are private to the client and server
            svc_nop,
            svc_disconnect,
            svc_reconnect,
            svc_sound,                  // <see code>
            svc_print,                  // [byte] id [string] null terminated string
            svc_stufftext,              // [string] stuffed into client's console buffer, should be \n terminated
            svc_serverdata,             // [long] protocol ...
            svc_configstring,           // [short] [string]
            svc_spawnbaseline,
            svc_centerprint,            // [string] to put in center of the screen
            svc_download,               // [short] size [size bytes]
            svc_playerinfo,             // variable
            svc_packetentities,         // [...]
            svc_deltapacketentities,    // [...]
            svc_frame
        }

        // client to server
        public enum ECLC_Ops
        {
            clc_bad,
            clc_nop,
            clc_move,           // [[usercmd_t]
            clc_userinfo,       // [[userinfo string]
            clc_stringcmd       // [string] message
        };

    }
}
