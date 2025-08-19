Shader "Unlit/Shader_Gugenka"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _JuliaX ("Julia X", Range(-2,2)) = -0.345
        _JuliaY ("Julia Y", Range(-2,2)) = 0.654
        _MaxIterations ("Max Iterations", Range(10,500)) = 360
        _EscapeRadius ("Escape Radius", Range(1,5)) = 2.0
        _ColorSpeed ("Color Speed", Range(0,50)) = 15.0
        _MousePos ("Mouse Position", Vector) = (0,0,0,0)
        _MouseInfluence ("Mouse Influence", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
       
            #include "UnityCG.cginc"
            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;
            float _JuliaX;
            float _JuliaY;
            float _MaxIterations;
            float _EscapeRadius;
            float _ColorSpeed;
            float4 _MousePos;
            float _MouseInfluence;

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normals : NORMAL;
                float4 uv0 : TEXCOORD0;
            };

            struct Interpolators 
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            // HSV to RGB 변환 함수
            float3 hsv2rgb(float h, float s, float v)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(float3(h, h, h) + K.xyz) * 6.0 - K.www);
                return v * lerp(K.xxx, saturate(p - K.xxx), s);
            }

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normals);
                o.uv = v.uv0;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                // UV 좌표를 -1 to 1 범위로 변환
                float2 p = (i.uv - 0.5) * 2.0;
                
                // Julia Set 파라미터
                float2 c = float2(_JuliaX, _JuliaY);
                float2 z = p;
                
                int iterations = 0;
                
                // Julia Set 계산
                for(int j = 0; j < _MaxIterations; j++)
                {
                    iterations = j;
                    if(length(z) > _EscapeRadius) break;
                    
                    // z = z^2 + c
                    z = float2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + c;
                }
                
                // 색상 계산 (반복 횟수 기반)
                float h = abs(fmod(_Time.y * _ColorSpeed - iterations, 360.0) / 360.0);
                float3 fractalColor = hsv2rgb(h, 0.6, 0.8);
                
                // 원형 마스크 (가장자리 자르기)
                float radialDistance = length(i.uv);
                float circleMask = 1 - step(1, radialDistance);
                
                return float4(fractalColor, circleMask);
            }
            
            ENDCG
        }
    }
}
