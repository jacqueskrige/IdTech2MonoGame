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


//-------- Technique: TexturedSkin --------
VertexShaderSkinOutput TexturedSkinVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
    VertexShaderSkinOutput Output;
	
	// generate the viewprojection and worldviewprojection
    float4x4 preViewProjection = mul(xView, xProjection);
    float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);
	
	// transform the input position to the output
    Output.Position = mul(inPos, preWorldViewProjection);
	
    Output.WorldNormal = normalize(mul(inNormal, (float3x3) xWorld));
    Output.WorldPosition = mul(inPos, xWorld).xyz;
	
	// copy the tex coords to the interpolator
    Output.TextureCoords = inTexCoords;
	
	// copy the ambient lighting
    Output.ambientLightColor = float4(1.0, 1.0, 1.0, 1.0);
	
    return Output;
}

float4 TexturedSkinPS(VertexShaderSkinOutput Input) : SV_TARGET
{
    float4 diffuseColor = xTextureSkin.Sample(TextureSamplerSkin, Input.TextureCoords);

	// gamma correction
    diffuseColor = CalculateGamma(diffuseColor * xLightModel);
    diffuseColor.a = 1.0;
	
    return diffuseColor;
}
