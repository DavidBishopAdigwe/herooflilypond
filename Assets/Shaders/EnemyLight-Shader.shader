Shader "Custom/EnemyComplexLight"
{
    Properties
    {
        _Color ("Light Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", Range(0, 5)) = 1
        _CircleRadius ("Circle Radius", Range(0, 1)) = 0.2
        _ConeAngle ("Cone Angle", Range(0, 180)) = 80
        _ConeLength ("Cone Length", Range(0, 2)) = 1
        _Falloff ("Falloff Sharpness", Range(1, 10)) = 3
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One  
        ZWrite Off
        Cull Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };
            
            uniform half4 _Color;
            uniform half _Intensity;
            uniform half _CircleRadius;
            uniform half _ConeAngle;
            uniform half _ConeLength;
            uniform half _Falloff;
            
            TEXTURE2D(_ShapeLightTexture3);
            SAMPLER(sampler_ShapeLightTexture3);
            
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv * 2 - 1;  
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }
            
            half4 frag(Varyings i) : SV_Target
            {
                float distance = length(i.uv);
                
                float angle = degrees(atan2(i.uv.y, i.uv.x));
                angle = abs(angle);
                half circle = smoothstep(_CircleRadius, _CircleRadius * 0.9, distance);
                circle = 1 - saturate(pow(distance / _CircleRadius, _Falloff));
                
                half cone = 0;
                if(angle < _ConeAngle * 0.5 && distance > _CircleRadius)
                {
                    half distFactor = 1 - smoothstep(_CircleRadius, _ConeLength, distance);
                    
                    half angleFactor = 1 - pow(angle / (_ConeAngle * 0.5), 2);
                    
                    cone = distFactor * angleFactor;
                }
                
                half lightValue = saturate(circle + cone) * _Intensity;
                
                float2 screenUV = i.positionCS.xy / _ScreenParams.xy;
                #if UNITY_UV_STARTS_AT_TOP
                screenUV.y = 1 - screenUV.y;
                #endif
                
                half visionMask = SAMPLE_TEXTURE2D(_ShapeLightTexture3, sampler_ShapeLightTexture3, screenUV).r;
                
                // Apply vision mask to light
                half4 finalColor = _Color * lightValue * visionMask;
                return finalColor;
            }
            ENDHLSL
        }
    }
}