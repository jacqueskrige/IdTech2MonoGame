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

#pragma target 4.0
#define LIGHT_COUNT 8

#include "generic.fx"
#include "skins.fx"
#include "textures.fx"

// SKINS.FX
technique TexturedSkin
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 TexturedSkinVS();
        PixelShader = compile ps_4_0 TexturedSkinPS();
    }
}


// TEXTURES.FX
technique Textured
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 TexturedVS();
        PixelShader = compile ps_4_0 TexturedPS();
    }
}

technique TexturedLightmap
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 TexturedVS();
        PixelShader = compile ps_4_0 TexturedLightmapPS();
    }
}

technique TexturedWarped
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 TexturedWarpedVS();
        PixelShader = compile ps_4_0 TexturedPS();
    }
}

technique TexturedTranslucent
{
    pass Pass0
    {
        AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
        VertexShader = compile vs_4_0 TexturedVS();
        PixelShader = compile ps_4_0 TexturedTranslucentPS();
    }
}

technique TexturedWarpedTranslucent
{
    pass Pass0
    {
        AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
        VertexShader = compile vs_4_0 TexturedWarpedVS();
        PixelShader = compile ps_4_0 TexturedTranslucentPS();
    }
}

technique TexturedSkybox
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 TexturedSkyboxVS();
        PixelShader = compile ps_4_0 TexturedSkyboxPS();
    }
}

technique TexturedLight
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 TexturedLightVS();
        PixelShader = compile ps_4_0 TexturedLightPS();
    }
}
