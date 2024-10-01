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


//-------- Technique: Textured --------
VertexShaderOutput TexturedVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0, float2 inLightCoords : TEXCOORD1)
{
    VertexShaderOutput Output;
    
	// generate the viewprojection and worldviewprojection
    float4x4 preViewProjection = mul(xView, xProjection);
    float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);
	
	// transform the input position to the output
    Output.Position = mul(inPos, preWorldViewProjection);
	
    Output.WorldNormal = normalize(mul(inNormal, (float3x3) xWorld));
    Output.WorldPosition = mul(inPos, xWorld).xyz;
	
	// copy the tex coords to the interpolator
    Output.TextureCoords = inTexCoords;
	
	// copy the lightmap coords to the interpolator
    Output.LightmapCoords = inLightCoords;
	
	// copy the ambient lighting
    Output.ambientLightColor = float4(1.0, 1.0, 1.0, 1.0);
	
    return Output;
}

float4 TexturedPS(VertexShaderOutput Input) : SV_TARGET
{
    float4 diffuseColor = xTextureDiffuse.Sample(TextureSamplerDiffuse, Input.TextureCoords);

	// gamma correction
    diffuseColor = CalculateGamma(diffuseColor);
    diffuseColor.a = 1.0;
	
    return diffuseColor;
}


//-------- Technique: TexturedWarped --------
VertexShaderOutput TexturedWarpedVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0, float2 inLightCoords : TEXCOORD1)
{
    VertexShaderOutput Output;
	
	// generate the viewprojection and worldviewprojection
    float4x4 preViewProjection = mul(xView, xProjection);
    float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);
	
	// transform the input position to the output
    Output.Position = mul(inPos, preWorldViewProjection);
	
    Output.WorldNormal = normalize(mul(inNormal, (float3x3) xWorld));
    Output.WorldPosition = mul(inPos, xWorld).xyz;
	
	// copy the tex coords to the interpolator
    Output.TextureCoords = CalculateWarp(inTexCoords);
	
	// copy the lightmap coords to the interpolator
    Output.LightmapCoords = inLightCoords;
	
	// copy the ambient lighting
    Output.ambientLightColor = float4(1.0, 1.0, 1.0, 1.0);
	
    return Output;
}


//-------- Technique: TexturedLightmap --------
float4 TexturedLightmapPS(VertexShaderOutput Input) : SV_TARGET
{
    float4 Color;
    float4 TextureColor = xTextureDiffuse.Sample(TextureSamplerDiffuse, Input.TextureCoords);
    float4 LightmapColor = xTextureLightmap.Sample(TextureSamplerLightmap, Input.LightmapCoords);
    
	//all color components are summed in the pixel shader
    if (xPointLights == true)
    {
        for (int i = 0; i < LIGHT_COUNT; i++)
        {
            LightmapColor += CalculateSingleLightmap(i, Input.WorldPosition, Input.WorldNormal, LightmapColor);
        }
    }
    
	// gamma correction
    Color = CalculateGamma(TextureColor * LightmapColor);
    Color.a = 1.0;
    
    return Color;
}


//-------- Technique: TexturedTranslucent --------
float4 TexturedTranslucentPS(VertexShaderOutput Input) : SV_TARGET
{
    float4 diffuseColor = xTextureDiffuse.Sample(TextureSamplerDiffuse, Input.TextureCoords);

	// gamma correction
    diffuseColor = CalculateGamma(diffuseColor);
    diffuseColor.a = xTextureAlpha;
	
    return diffuseColor;
}


//-------- Technique: TexturedSkybox --------
VertexShaderOutput_Skybox TexturedSkyboxVS(float4 inPos : POSITION, float2 inTexCoords : TEXCOORD0)
{
    VertexShaderOutput_Skybox Output;
	
	// generate the viewprojection and worldviewprojection
    float4x4 preViewProjection = mul(xView, xProjection);
    float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);
	
	// transform the input position to the output
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.WorldPosition = mul(inPos, xWorld).xyz;
	
	// copy the tex coords to the interpolator
    Output.TextureCoords = inTexCoords;
	
    return Output;
}

float4 TexturedSkyboxPS(VertexShaderOutput_Skybox Input) : SV_TARGET
{
    float4 diffuseColor = xTextureDiffuse.Sample(TextureSamplerDiffuse, Input.TextureCoords);
	
	// gamma correction
    diffuseColor = CalculateGamma(diffuseColor);
    diffuseColor.a = 1.0;
	
    return diffuseColor;
}


//-------- Technique: TexturedLight --------
VertexShaderOutput TexturedLightVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
    VertexShaderOutput Output;
	
	// generate the viewprojection and worldviewprojection
    float4x4 preViewProjection = mul(xView, xProjection);
    float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);
	
	// transform the input position to the output
    Output.Position = mul(inPos, preWorldViewProjection);
	
    Output.WorldNormal = normalize(mul(inNormal, (float3x3) xWorld));
    Output.WorldPosition = mul(inPos, xWorld).xyz;
	
	// copy the tex coords to the interpolator
    Output.TextureCoords = inTexCoords;
    
    // copy the lightmap coords to the interpolator
    Output.LightmapCoords = float2(0, 0);
	
	// copy the ambient lighting
    Output.ambientLightColor = xLightAmbient;
	
    return Output;
}

float4 TexturedLightPS(VertexShaderOutput Input) : SV_TARGET
{
    float4 diffuseColor = xTextureDiffuse.Sample(TextureSamplerDiffuse, Input.TextureCoords);
    float4 specularColor = float4(0.15, 0.15, 0.15, 0.15);
    float4 Color = Input.ambientLightColor * diffuseColor;
	
	//all color components are summed in the pixel shader
    if (xPointLights == true)
    {
        for (int i = 0; i < LIGHT_COUNT; i++)
        {
            Color += CalculateSingleLight(i, Input.WorldPosition, Input.WorldNormal, Input.TextureCoords, diffuseColor, specularColor);
        }
    }
	
	// gamma correction
    Color = CalculateGamma(Color);
    Color.a = 1.0;
	
    return Color;
}
