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
using Microsoft.Xna.Framework.Graphics;

namespace IdTech2MonoGame
{
    public class CPointLight
    {
        //////////////////////////////////////////////////////////////
        // The only parameter that will be changing for every light //
        // each frame is the postion parameter.  For the sake of    //
        // reducing string look-ups, the position parameter is      //
        // stored as a parameter instance.  The other parameters    //
        // are updated much less frequently, so a tradeoff has been //
        // made for clarity.                                        //
        //////////////////////////////////////////////////////////////

        private int entityIndex = -1;

        private EffectParameter instanceParameter;

        private EffectParameter instanceParameterPosition;
        private EffectParameter instanceParameterColor;
        private EffectParameter instanceParameterFalloff;
        private EffectParameter instanceParameterRange;

        private Vector4 LightPosition;
        private Color LightColor = Color.White;
        private float LightFalloff = 1.1f;
        private float LightRange = 300.0f;

        public CPointLight(int entityIndex, Vector4 initialPosition)
        {
            this.entityIndex = entityIndex;

            gPosition = initialPosition;
        }

        public CPointLight(int entityIndex, Vector4 initialPosition, float initialRange)
        {
            this.entityIndex = entityIndex;

            gPosition = initialPosition;
            gRange = initialRange;
        }

        public CPointLight(int entityIndex, Vector4 initialPosition, float initialRange, float Red, float Green, float Blue)
        {
            this.entityIndex = entityIndex;

            gPosition = initialPosition;
            gRange = initialRange;

            {
                Color lightColor = Color.Black;

                lightColor.R = (byte)(255 * Red);
                lightColor.G = (byte)(255 * Green);
                lightColor.B = (byte)(255 * Blue);
                lightColor.A = (byte)255;
                gColor = lightColor;
            }
        }

        /*public CPointLight(Vector4 initialPosition, EffectParameter lightParameter)
        {
            //////////////////////////////////////////////////////////////
            // An instance of a light is bound to an instance of a      //
            // Light structure defined in the effect.  The              //
            // "StructureMembers" property of a parameter is used to    //
            // access the individual fields of a structure.             //
            //////////////////////////////////////////////////////////////
            instanceParameter = lightParameter;
            positionParameter = instanceParameter.StructureMembers["position"];
            gPosition = initialPosition;
            instanceParameter.StructureMembers["range"].SetValue(LightRange);
            instanceParameter.StructureMembers["falloff"].SetValue(LightFalloff);
            instanceParameter.StructureMembers["color"].SetValue(LightColor.ToVector4());
        }*/

        public void SetParameter(EffectParameter lightParameter)
        {
            if (instanceParameter == null)
                instanceParameter = lightParameter;
        }

        public void SetParameters(EffectParameter lightPosition, EffectParameter lightColor, EffectParameter lightFalloff, EffectParameter lightRange)
        {
            if (instanceParameterPosition == null)
                instanceParameterPosition = lightPosition;

            if (instanceParameterColor == null)
                instanceParameterColor = lightColor;

            if (instanceParameterFalloff == null)
                instanceParameterFalloff = lightFalloff;

            if (instanceParameterRange == null)
                instanceParameterRange = lightRange;
        }

        public void SetLight()
        {
            if (instanceParameter != null)
                return;

            instanceParameter.StructureMembers["position"].SetValue(LightPosition);
            instanceParameter.StructureMembers["color"].SetValue(LightColor.ToVector4());
            instanceParameter.StructureMembers["falloff"].SetValue(LightFalloff);
            instanceParameter.StructureMembers["range"].SetValue(LightRange);
        }

        public void SetLights(int lightIndex)
        {
            if (instanceParameterPosition != null)
            {
                Vector4[] positions = instanceParameterPosition.GetValueVector4Array();

                if (positions != null)
                {
                    if (positions[lightIndex] != LightPosition)
                    {
                        positions[lightIndex] = LightPosition;
                        instanceParameterPosition.SetValue(positions);
                    }
                }
            }

            if (instanceParameterColor != null)
            {
                Vector4[] colors = instanceParameterColor.GetValueVector4Array();

                if (colors != null)
                {
                    Vector4 lightColor = LightColor.ToVector4();

                    if (colors[lightIndex] != lightColor)
                    {
                        colors[lightIndex] = lightColor;
                        instanceParameterColor.SetValue(colors);
                    }
                }
            }

            if (instanceParameterFalloff != null)
            {
                float[] falloffs = instanceParameterFalloff.GetValueSingleArray();

                if (falloffs != null)
                {
                    if (falloffs[lightIndex] != LightFalloff)
                    {
                        falloffs[lightIndex] = LightFalloff;
                        instanceParameterFalloff.SetValue(falloffs);
                    }
                }
            }

            if (instanceParameterRange != null)
            {
                float[] ranges = instanceParameterRange.GetValueSingleArray();

                if (ranges != null)
                {
                    if (ranges[lightIndex] != LightRange)
                    {
                        ranges[lightIndex] = LightRange;
                        instanceParameterRange.SetValue(ranges);
                    }
                }
            }
        }

        #region Light Properties
        public int EntityIndex
        {
            set
            {
                if (entityIndex < 0 && value > 0)
                    entityIndex = value;
            }
            get
            {
                return entityIndex;
            }
        }

        public Vector4 gPosition
        {
            set
            {
                LightPosition = value;

                if (instanceParameter != null)
                    instanceParameter.StructureMembers["position"].SetValue(LightPosition);
            }
            get
            {
                return LightPosition;
            }
        }

        public Color gColor
        {
            set
            {
                LightColor = value;

                if (instanceParameter != null)
                    instanceParameter.StructureMembers["color"].SetValue(LightColor.ToVector4());
            }
            get
            {
                return LightColor;
            }
        }

        public float gFalloff
        {
            set
            {
                LightFalloff = value;

                if (instanceParameter != null)
                    instanceParameter.StructureMembers["falloff"].SetValue(LightFalloff);
            }
            get
            {
                return LightFalloff;
            }
        }

        public float gRange
        {
            set
            {
                LightRange = value;

                if (instanceParameter != null)
                    instanceParameter.StructureMembers["range"].SetValue(LightRange);
            }
            get
            {
                return LightRange;
            }
        }
        #endregion
    }
}
