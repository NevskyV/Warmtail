#ifndef BLUR_DEPTH_INCLUDED
#define BLUR_DEPTH_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

void GaussianBlurDepth_float(float2 UV, float EdgeWorldWidth, float Power, float ScreenHeight,float OrthoHeight, out float EdgeMask)
{
    float depth = SampleSceneDepth(UV);

    float deriv = length(float2(ddx(depth), ddy(depth)));
    
    float pixelSizeWorld = (OrthoHeight) / ScreenHeight;
    float scaledDeriv = deriv * (EdgeWorldWidth / pixelSizeWorld);

    EdgeMask = saturate(1.0 - scaledDeriv);
    EdgeMask = pow(EdgeMask, Power);    
}

#endif

void SmoothDepthGradient_float(float rawDepth, float edge, float depthRange, float power, float fadeStart, out float gradient)
{
    // 1. Базовая инверсия + нормализация
    float d = 1.0 - rawDepth;                    // 1 у объектов, 0 в глубине
    d = saturate(d + edge * 2.0);                // расширяем зону мелководья через edge
    
    // 2. Плавный переход через cubic ease-in-out + power
    float t = saturate(d * depthRange);
    t = t * t * (3.0 - 2.0 * t);                 // cubic smoothstep
    t = pow(t, power);                           // дополнительная кривизна (1.8–4.0)
    
    // 3. Fade start (позволяет держать shallow цвет дольше)
    t = saturate((t - fadeStart) / (1.0 - fadeStart));
    
    gradient = t;
}


#ifndef WATER_DEPTH_EDGE_INCLUDED
#define WATER_DEPTH_EDGE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

void WaterFoamEdge_float(
    float2 ScreenUV,
    float  EdgeWidth,
    float  EdgeSoftness,
    out float FoamMask,
    out float SceneDepthRaw)
{
    float2 texel = EdgeWidth / _ScreenParams.xy;
    
    float c = SampleSceneDepth(ScreenUV);
    
    float l = SampleSceneDepth(ScreenUV + float2(-texel.x, 0));
    float r = SampleSceneDepth(ScreenUV + float2( texel.x, 0));
    float lu = SampleSceneDepth(ScreenUV + float2(-texel.x, texel.y));
    float ru = SampleSceneDepth(ScreenUV + float2( texel.x, texel.y));
    float ld = SampleSceneDepth(ScreenUV + float2(-texel.x, -texel.y));
    float rd = SampleSceneDepth(ScreenUV + float2( texel.x, -texel.y));
    float u = SampleSceneDepth(ScreenUV + float2(0,  texel.y));
    float d = SampleSceneDepth(ScreenUV + float2(0, -texel.y));
    
    float edge = max(max(max(max(max(max(max(r, l), u),d), lu), ru), ld), rd);
    // Плавная или резкая граница
    FoamMask = edge;
    
    //FoamMask = lerp(lerp(lerp(l, r, EdgeSoftness), u, EdgeSoftness),d,EdgeSoftness);
    //FoamMask = smoothstep(0.0, EdgeSoftness, edge);
    
    SceneDepthRaw = c;
}

void WaterFoamEdge_half(
    half2  ScreenUV,
    half   EdgeWidth,
    half   EdgeSoftness,
    out half FoamMask,
    out half SceneDepthRaw)
{
    float foamF, depthF;
    WaterFoamEdge_float(ScreenUV, EdgeWidth, EdgeSoftness, foamF, depthF);
    FoamMask = (half)foamF;
    SceneDepthRaw = (half)depthF;
}

#endif
