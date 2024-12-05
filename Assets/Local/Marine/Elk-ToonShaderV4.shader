Shader "Custom/Elk/ToonShading"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (0.5,0.5,0.5,1)
        _OpacityMap ("Opacity Map", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 1)) = 1

        _ShadowStep ("ShadowStep", Range(0, 1)) = 0.5
        _ShadowStepSmooth ("ShadowStepSmooth", Range(0, 1)) = 0.04
        _ShadowColor ("ShadowColor", Color) = (0.0, 0.0, 0.0, 1)
        _ShadowOpacity ("ShadowOpacity", Range(0, 1)) = 1

        _SpecularStep ("SpecularStep", Range(0, 1)) = 0.6
        _SpecularStepSmooth ("SpecularStepSmooth", Range(0, 1)) = 0.05
        _SpecularColor ("SpecularColor", Color) = (1,1,1,1)

        _RimStep ("RimStep", Range(0, 1)) = 0.65
        _RimStepSmooth ("RimStepSmooth",Range(0,1)) = 0.4
        _RimColor ("RimColor", Color) = (1,1,1,1)

        _OutlineWidth ("OutlineWidth", Range(0.0, 1.0)) = 0.15
        _OutlineColor ("OutlineColor", Color) = (0.0, 0.0, 0.0, 1)

        _HalftoneMap ("Halftone Map", 2D) = "white" {}
        _HalftoneMaskColor ("Halftone Mask Color", Color) = (0.0, 0.0, 0.0, 1)

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "UniversalForward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ALPHATEST_ON
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_OpacityMap); SAMPLER(sampler_OpacityMap);
            TEXTURE2D(_HalftoneMap); SAMPLER(sampler_HalftoneMap);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float _ShadowStep;
            float _ShadowStepSmooth;
            float4 _ShadowColor;
            float _ShadowOpacity;
            float _SpecularStep;
            float _SpecularStepSmooth;
            float4 _SpecularColor;
            float _RimStepSmooth;
            float _RimStep;
            float4 _RimColor;
            float _Opacity;
            float4 _HalftoneMaskColor;
            float _NormalStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 bitangentWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5;
                float4 fogCoord : TEXCOORD6;
                float3 positionWS : TEXCOORD7;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                float3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
                float3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = input.uv;
                output.normalWS = float4(normalInput.normalWS, viewDirWS.x);
                output.tangentWS = float4(normalInput.tangentWS, viewDirWS.y);
                output.bitangentWS = float4(normalInput.bitangentWS, viewDirWS.z);
                output.viewDirWS = viewDirWS;
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                output.shadowCoord = TransformWorldToShadowCoord(input.positionOS.xyz);
                return output;
            }

            half remap(half x, half t1, half t2, half s1, half s2)
            {
                return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float2 uv = input.uv;
                float3 N = normalize(input.normalWS.xyz);
                float3 T = normalize(input.tangentWS.xyz);
                float3 B = normalize(input.bitangentWS.xyz);
                float3 V = normalize(input.viewDirWS.xyz);
                float3 L = normalize(_MainLightPosition.xyz);
                float3 H = normalize(V+L);

                // Sample normal map
                float3 normalMap = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv).rgb;
                normalMap = 2.0 * normalMap - 1.0;
                normalMap = normalize(normalMap);

                // Interpolate between surface normal and normal map
                float3 normalWS = normalize(lerp(N, T * normalMap.x + B * normalMap.y + N * normalMap.z, _NormalStrength));

                float NV = dot(normalWS, V);
                float NH = dot(normalWS, H);
                float NL = dot(normalWS, L);

                NL = NL * 0.5 + 0.5;

                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                float4 opacityMap = SAMPLE_TEXTURE2D(_OpacityMap, sampler_OpacityMap, uv);
                float opacity = _Opacity * opacityMap.r;

                // return NH;
                float specularNH = smoothstep((1-_SpecularStep * 0.05) - _SpecularStepSmooth * 0.05, (1-_SpecularStep* 0.05) + _SpecularStepSmooth * 0.05, NH) ;
                float shadowNL = smoothstep(_ShadowStep - _ShadowStepSmooth, _ShadowStep + _ShadowStepSmooth, NL);

                //shadow
                float shadow = MainLightRealtimeShadow(input.shadowCoord);

                //rim
                float rim = smoothstep((1-_RimStep) - _RimStepSmooth * 0.5, (1-_RimStep) + _RimStepSmooth * 0.5, 0.5 - NV);

                //diffuse
                float3 diffuse = _MainLightColor.rgb * baseMap * _BaseColor * shadowNL * shadow * opacity;

                //specular
                float3 specular = _SpecularColor * shadow * shadowNL * specularNH * opacity;

                //ambient
                float3 ambient = rim * _RimColor + SampleSH(normalWS) * _BaseColor * baseMap * opacity;

                //shadow color with halftone
                float4 halftone = SAMPLE_TEXTURE2D(_HalftoneMap, sampler_HalftoneMap, uv);
                float shadowDensity = 1 - shadowNL;
                float3 halftoneColor = halftone.rgb * _HalftoneMaskColor.rgb * shadowDensity * _ShadowOpacity * opacity;

                float3 finalColor = diffuse + ambient + specular + halftoneColor;
                finalColor = MixFog(finalColor, input.fogCoord);

                // Alpha test
                clip(opacity);

                return float4(finalColor, opacity);
            }
            ENDHLSL
        }

        //Outline
        Pass
        {
            Name "Outline"
            Cull Front
            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 fogCoord : TEXCOORD0;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.pos = TransformObjectToHClip(float4(v.vertex.xyz + v.normal * _OutlineWidth * 0.1 ,1));
                o.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 finalColor = MixFog(_OutlineColor, i.fogCoord);
                return float4(finalColor,1.0);
            }

            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}
