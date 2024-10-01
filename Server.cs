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
    public class CServer
    {
        // max recipients for heartbeat packets
        public const int MAX_MASTERS = 8;

        //server_static_t svs;				// persistant server info
        public static SServer sv; // local server


        public struct SServer
        {
            public EServerState state; // precache commands are only valid during load

            public bool attractloop; // running cinematics and demos for the local system only
            public bool loadgame; // client begins should reuse existing entity

            public uint time; // always sv.framenum * 100 msec
            public int framenum;

            public string name; // size: MAX_QPATH (map name, or cinematic name)
            //struct cmodel_s		*models[MAX_MODELS];

            public string[] configstrings;  // size: [MAX_CONFIGSTRINGS, MAX_QPATH]
            public EClientState[] baselines; // size: MAX_EDICTS

            // the multicast buffer is used to send a message to a set of clients
            // it is only used to marshall data until SV_Multicast is called
            public CCommon.SSizeBuf multicast;
            public byte[] multicast_buf; // size: MAX_MSGLEN

            // demo server information
            //FILE		*demofile;
            public bool timedemo; // don't time sync
        }




        /*typedef struct
{
	server_state_t	state;			// precache commands are only valid during load

	qboolean	attractloop;		// running cinematics and demos for the local system only
	qboolean	loadgame;			// client begins should reuse existing entity

	unsigned	time;				// always sv.framenum * 100 msec
	int			framenum;

	char		name[MAX_QPATH];			// map name, or cinematic name
	struct cmodel_s		*models[MAX_MODELS];

	char		configstrings[MAX_CONFIGSTRINGS][MAX_QPATH];
	entity_state_t	baselines[MAX_EDICTS];

	// the multicast buffer is used to send a message to a set of clients
	// it is only used to marshall data until SV_Multicast is called
	sizebuf_t	multicast;
	byte		multicast_buf[MAX_MSGLEN];

	// demo server information
	FILE		*demofile;
	qboolean	timedemo;		// don't time sync
} server_t;*/





        public enum EServerState
        {
            ss_dead,        // no map loaded
            ss_loading,     // spawning level edicts
            ss_game,        // actively running
            ss_cinematic,
            ss_demo,
            ss_pic
        }
        // some qc commands are only valid before the server has finished
        // initializing (precache commands, static sounds / objects, etc)

        public enum EClientState
        {
            cs_free,        // can be reused for a new connection
            cs_zombie,      // client has been disconnected, but don't reuse connection for a couple seconds
            cs_connected,   // has been assigned to a client_t, but not in game yet
            cs_spawned      // client is fully in game
        }


        

        

    }
}
