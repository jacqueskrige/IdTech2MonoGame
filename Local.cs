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

namespace IdTech2MonoGame
{
    public class CLocal
    {
        public const int MAX_LIGHTSTYLES = 256;
        public const int TEXNUM_LIGHTMAPS = 1024;
        public const int MAX_LBM_HEIGHT = 480;

        public const float gl_modulate = 1.0f;

        public static BoundingBox ClearBounds()
        {
            BoundingBox bounds;

            bounds.Min.X = bounds.Min.Y = bounds.Min.Z = 99999;
            bounds.Max.X = bounds.Max.Y = bounds.Max.Z = -99999;

            return bounds;
        }

        public static void AddPointToBounds(Vector3 v, ref BoundingBox bounds)
        {
            if (v.X < bounds.Min.X)
                bounds.Min.X = v.X;

            if (v.X > bounds.Max.X)
                bounds.Max.X = v.X;


            if (v.Y < bounds.Min.Y)
                bounds.Min.Y = v.Y;

            if (v.Y > bounds.Max.Y)
                bounds.Max.Y = v.Y;


            if (v.Z < bounds.Min.Z)
                bounds.Min.Z = v.Z;

            if (v.Z > bounds.Max.Z)
                bounds.Max.Z = v.Z;
        }



        public struct SLightStyle
        {
            public float[] rgb; // size: 3 (0.0 - 2.0)
            public float white; // highest of rgb
        }

        public struct SRefDef
        {
            public Vector3 ViewOrigin;
            public Vector3 ViewAngles;
            public Vector3 ViewUp;

            public Vector3 TargetOrigin;
            public Quaternion TargetAngles;

            public BoundingFrustum FrustumBounds;

            public int EntityIndex;

            public string MapName;

            public SLightStyle[] lightstyles; // size: MAX_LIGHTSTYLES
        }

        public struct SHLSL
        {
            public Matrix xViewMatrix;
            public Matrix xProjectionMatrix;
            public Matrix xWorld;
            public Color xLightModel;
            public Color xLightAmbient;
            public float xLightPower;
            public float xTextureAlpha;
            public float xGamma;
            public float xRealTime;
            public bool xPointLights;
            public bool xFlow;
        }

        public struct SGlobal
        {
            public SHLSL HLSL;
            public SHLSL OldHLSL;
        }

    }
}
