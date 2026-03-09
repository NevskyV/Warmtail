Shader "Hidden/DepthCopy2D"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            TEXTURE2D_X_FLOAT(_SourceDepth);
            //SAMPLER(sampler_PointClamp);

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            Varyings Vert(uint vertexID : SV_VertexID)
            {
                Varyings o;
                // Fullscreen triangle (3 вершины покрывают весь экран)
                float2 uv = float2((vertexID << 1) & 2, vertexID & 2);
                o.position = float4(uv * 2.0 - 1.0, 0.0, 1.0);
                #if UNITY_UV_STARTS_AT_TOP
                    o.texcoord = float2(uv.x, 1.0 - uv.y);
                #else
                    o.texcoord = uv;
                #endif
                return o;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float depth = SAMPLE_TEXTURE2D_X(
                    _SourceDepth,
                    sampler_PointClamp,
                    input.texcoord
                ).r;
                return float4(depth, depth, depth, 1.0);
            }
            ENDHLSL
        }
    }
}