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


struct VertexShaderOutput
{
    float4 Position             : POSITION;
    float2 TextureCoords        : TEXCOORD0;
    float2 LightmapCoords       : TEXCOORD1;
    float3 WorldNormal          : TEXCOORD2;
    float3 WorldPosition        : TEXCOORD3;
    float4 ambientLightColor    : COLOR0;
};

struct VertexShaderSkinOutput
{
    float4 Position             : POSITION;
    float2 TextureCoords        : TEXCOORD0;
    float3 WorldNormal          : TEXCOORD2;
    float3 WorldPosition        : TEXCOORD3;
    float4 ambientLightColor    : COLOR0;
};

struct VertexShaderInput_Skybox
{
    float4 inPos        : POSITION;
    float2 inTexCoords  : TEXCOORD0;
};

struct VertexShaderOutput_Skybox
{
    float4 Position         : SV_POSITION;
    float2 TextureCoords    : TEXCOORD0;
    float3 WorldPosition    : TEXCOORD1;
};


// -------- XNA to HLSL variables --------
cbuffer Globals
{
    float4x4 xView;
    float4x4 xProjection;
    float4x4 xWorld;
    float4 xLightModel;
    float4 xLightAmbient;
    
    float xLightPower;
    float xTextureAlpha;
    float xGamma;
    float xRealTime;
    
    bool xPointLights;
    bool xFlow;
};

// 7:  GaussianQuad  - A 4-sample Gaussian filter used as a texture magnification or minification filter. 
// 6:  PyramidalQuad - A 4-sample tent filter used as a texture magnification or minification filter. 
// 3:  Anisotropic   - Anisotropic texture filtering used as a texture magnification or minification filter. This type of filtering compensates for distortion caused by the difference in angle between the texture polygon and the plane of the screen. 
// 2:  Linear        - Bilinear interpolation filtering used as a texture magnification or minification filter. A weighted average of a 2x2 area of texels surrounding the desired pixel is used. The texture filter used between mipmap levels is trilinear mipmap interpolation, in which the rasterizer performs linear interpolation on pixel color, using the texels of the two nearest mipmap textures.  
// 1:  Point         - Point filtering used as a texture magnification or minification filter. The texel with coordinates nearest to the desired pixel value is used. The texture filter used between mipmap levels is based on the nearest point; that is, the rasterizer uses the color from the texel of the nearest mipmap texture. 
// 0:  None          - Mipmapping disabled. The rasterizer uses the magnification filter instead. 


// -------- Texture Samplers --------
Texture2D xTextureDiffuse;
sampler TextureSamplerDiffuse = sampler_state
{
    texture = <xTextureDiffuse>;
    MagFilter = Anisotropic;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D xTextureLightmap;
sampler TextureSamplerLightmap = sampler_state
{
    texture = <xTextureLightmap>;
    MagFilter = Anisotropic;
    MinFilter = Anisotropic;
    MipFilter = LINEAR;
    MaxAnisotropy = 8;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D xTextureSkin;
sampler TextureSamplerSkin = sampler_state
{
    texture = <xTextureSkin>;
    MagFilter = Anisotropic;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MaxAnisotropy = 8;
    AddressU = Clamp;
    AddressV = Clamp;
};

// -----------------------------------------
//  LIGHT
// -----------------------------------------

/*struct Light
{
    // Array of float4: 16 bytes each, aligned correctly
    float4 position;    // 16 bytes per element (aligned)
    float4 color;       // 16 bytes per element (aligned)
    
    // Float array elements: 4 bytes each, need padding to align
    float falloff;      // 4 bytes per element
    float range;        // 4 bytes per element
    
    // Padding to ensure alignment
    float padding7;     // Padding to align falloff array (to 16 bytes)
    float padding8;     // Padding to align range array (to 16 bytes)
};

cbuffer LightBuffer
{
    Light lights[LIGHT_COUNT];
};*/

cbuffer LightBuffer
{
    float4 LightBuffer_Position[LIGHT_COUNT];
    float4 LightBuffer_Color[LIGHT_COUNT];
    
    float LightBuffer_Falloff[LIGHT_COUNT];
    float LightBuffer_Range[LIGHT_COUNT];
};

float4 CalculateSingleLightmap(int lightindex, float3 worldPosition, float3 worldNormal, float4 LightmapColor)
{
    float3 lightVector = LightBuffer_Position[lightindex].xyz - worldPosition;
    float lightDist = length(lightVector);
    float3 directionToLight = normalize(lightVector);

    float baseIntensity = pow(saturate((LightBuffer_Range[lightindex] - lightDist) / LightBuffer_Range[lightindex]), LightBuffer_Falloff[lightindex]);
    float diffuseIntensity = saturate(dot(directionToLight, worldNormal));
    diffuseIntensity *= xLightPower;

    float4 diffuse = diffuseIntensity * LightBuffer_Color[lightindex];
    float blendfactor = min(1, lightDist / LightBuffer_Range[lightindex]);

    float4 blendFactorVec = float4(blendfactor, blendfactor, blendfactor, blendfactor);
    float4 lightmapPixel = (float4(1, 1, 1, 1) - blendFactorVec) * diffuse + blendFactorVec * LightmapColor;
    
    return lightmapPixel * baseIntensity;
}

float4 CalculateSingleLight(int lightindex, float3 worldPosition, float3 worldNormal, float2 TextureCoords, float4 diffuseColor, float4 specularColor)
{
    float3 lightVector = LightBuffer_Position[lightindex].xyz - worldPosition;
    float lightDist = length(lightVector);
    float3 directionToLight = normalize(lightVector);
    
    //calculate the intensity of the light with exponential falloff
    float baseIntensity = pow(saturate((LightBuffer_Range[lightindex] - lightDist) / LightBuffer_Range[lightindex]), LightBuffer_Falloff[lightindex]);
    float diffuseIntensity = saturate(dot(directionToLight, worldNormal));
    diffuseIntensity *= xLightPower;
	
    float4 diffuse = diffuseIntensity * LightBuffer_Color[lightindex] * diffuseColor;
	
    return diffuse * baseIntensity;
}

// -----------------------------------------
//  GAMMA
// -----------------------------------------

float4 CalculateGamma(float4 color)
{
    return log(1 + color) * xGamma;
}

// -----------------------------------------
//  WARP
// -----------------------------------------

float2 CalculateWarp(float2 TexCoords)
{
    float2 TC;
    float TurbScale = (256.0f / (2 * 3.141592));
    float Flowing = 0.0f;
	
    if (xFlow == true)
        Flowing = -64.0f * ((xRealTime * 0.5f) - (int) (xRealTime * 0.5f));
	
	// muliplying the result by 3.0 to make the warping more dramatic
    TC.x = TexCoords.x + sin(radians((TexCoords.y * 0.125f + xRealTime) * TurbScale)) * 3.0f;
    TC.x += Flowing;
    TC.x *= (1.0f / 64.0f);
    TC.y = TexCoords.y + sin(radians((TexCoords.x * 0.125f + xRealTime) * TurbScale)) * 3.0f;
    TC.y *= (1.0f / 64.0f);
	
    return TC;
}
