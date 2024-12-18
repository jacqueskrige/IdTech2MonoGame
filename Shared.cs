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

namespace IdTech2MonoGame
{
    public class CShared
    {
        public const int MAX_QPATH = 64;
        public static Vector3 vec3_origin;

        public static string Com_ToString(char[] chr)
        {
            string str;

            if (chr == null)
                return null;

            if (chr.Length == 0)
                return "";

            System.Text.StringBuilder SB = new System.Text.StringBuilder();
            SB.Append(chr);

            str = SB.ToString().Trim();
            SB = null;

            if (str.IndexOf('\0') != -1)
                str = str.Substring(0, str.IndexOf('\0'));

            return str;
        }

        public static char[] Com_ToChar(string str, bool nullTerminate)
        {
            int i;
            char[] chr;

            if (str == null)
                return null;

            if (nullTerminate == true)
                chr = new char[str.Length + 1];
            else
                chr = new char[str.Length];

            for (i = 0; i < str.Length; i++)
            {
                chr[i] = str[i];
            }

            if (nullTerminate == true)
                chr[i] = '\0';

            return chr;
        }

        public static void VectorScale(Vector3 vecIn, float scale, ref Vector3 vecOut)
        {
            vecOut.X = vecIn.X * scale;
            vecOut.Y = vecIn.Y * scale;
            vecOut.Z = vecIn.Z * scale;
        }

        public static void AngleVectors(Vector3 angles, ref Vector3 forward, ref Vector3 right, ref Vector3 up)
        {
            float angle;
            float sr, sp, sy, cr, cp, cy;

            angle = (float)(angles.Y * (Math.PI * 2 / 360));
            sy = (float)Math.Sin(angle);
            cy = (float)Math.Cos(angle);

            angle = (float)(angles.X * (Math.PI * 2 / 360));
            sp = (float)Math.Sin(angle);
            cp = (float)Math.Cos(angle);

            angle = (float)(angles.Z * (Math.PI * 2 / 360));
            sr = (float)Math.Sin(angle);
            cr = (float)Math.Cos(angle);


            forward.X = cp * cy;
            forward.Y = cp * sy;
            forward.Z = -sp;

            right.X = (-1 * sr * sp * cy + -1 * cr * -sy);
            right.Y = (-1 * sr * sp * sy + -1 * cr * cy);
            right.Z = -1 * sr * cp;

            up.X = (cr * sp * cy + -sr * -sy);
            up.Y = (cr * sp * sy + -sr * cy);
            up.Z = cr * cp;
        }

        /*void AngleVectors (vec3_t angles, vec3_t forward, vec3_t right, vec3_t up)
        {
	        float		angle;
	        static float		sr, sp, sy, cr, cp, cy;
	        // static to help MS compiler fp bugs

	        angle = angles[YAW] * (M_PI*2 / 360);
	        sy = sin(angle);
	        cy = cos(angle);
	        angle = angles[PITCH] * (M_PI*2 / 360);
	        sp = sin(angle);
	        cp = cos(angle);
	        angle = angles[ROLL] * (M_PI*2 / 360);
	        sr = sin(angle);
	        cr = cos(angle);

	        if (forward)
	        {
		        forward[0] = cp*cy;
		        forward[1] = cp*sy;
		        forward[2] = -sp;
	        }
	        if (right)
	        {
		        right[0] = (-1*sr*sp*cy+-1*cr*-sy);
		        right[1] = (-1*sr*sp*sy+-1*cr*cy);
		        right[2] = -1*sr*cp;
	        }
	        if (up)
	        {
		        up[0] = (cr*sp*cy+-sr*-sy);
		        up[1] = (cr*sp*sy+-sr*cy);
		        up[2] = cr*cp;
	        }
        }*/



        public struct SCPlane
        {
            public Microsoft.Xna.Framework.Vector3 normal;
            public float dist;
            public byte type; // for fast side tests: 0,1,2 = axial, 3 = nonaxial
            public byte signbits; // signx + (signy<<1) + (signz<<2), used as lookup during collision

            // unused - alignment?
            public byte[] pad; // size: 2
        }



        //
        // per-level limits
        //
        public const int MAX_CLIENTS = 256;     // absolute limit
        public const int MAX_EDICTS = 1024;     // must change protocol to increase more
        public const int MAX_LIGHTSTYLES = 256;
        public const int MAX_MODELS = 256;      // these are sent over the net as bytes
        public const int MAX_SOUNDS = 256;      // so they cannot be blindly increased
        public const int MAX_IMAGES = 256;
        public const int MAX_ITEMS = 256;
        public const int MAX_GENERAL = (MAX_CLIENTS * 2);   // general config strings


        //
        // Config strings are a general means of communication from the server to all connected clients.
        // Each config string can be at most MAX_QPATH characters.
        //
        public const int CS_NAME = 0;
        public const int CS_CDTRACK = 1;
        public const int CS_SKY = 2;
        public const int CS_SKYAXIS = 3;    // %f %f %f format
        public const int CS_SKYROTATE = 4;
        public const int CS_STATUSBAR = 5;  // display program string

        public const int CS_AIRACCEL = 29;      // air acceleration control
        public const int CS_MAXCLIENTS = 30;
        public const int CS_MAPCHECKSUM = 31;   // for catching cheater maps

        public const int CS_MODELS = 32;
        public const int CS_SOUNDS = (CS_MODELS + MAX_MODELS);
        public const int CS_IMAGES = (CS_SOUNDS + MAX_SOUNDS);
        public const int CS_LIGHTS = (CS_IMAGES + MAX_IMAGES);
        public const int CS_ITEMS = (CS_LIGHTS + MAX_LIGHTSTYLES);
        public const int CS_PLAYERSKINS = (CS_ITEMS + MAX_ITEMS);
        public const int CS_GENERAL = (CS_PLAYERSKINS + MAX_CLIENTS);
        public const int MAX_CONFIGSTRINGS = (CS_GENERAL + MAX_GENERAL);





    }
}
