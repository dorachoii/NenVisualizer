Shader "Unlit/Shader_DefaultRipple"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _ColorStart ("Color Start", Range(0,1)) = 0
        _ColorEnd ("Color End", Range(0,1)) = 1
        _WaveAmp ("Wave Amplitude", Range(0,0.2)) = 0.1
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
            float _ColorStart;
            float _ColorEnd;
            float _WaveAmp;

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

            float GetWave(float2 uv)
            {
                float2 uvsCentered = uv;
                float radialDistance = length(uvsCentered);

                // i.uv.y: 높이 대신, distance from center를 넣어보자!
                float wave = cos((radialDistance - _Time.y * 0.1) * TAU * 2.5) * 0.5 + 0.5;
                wave *= 1 - radialDistance;
                return wave;
            }

            Interpolators vert (MeshData v)
            {
                Interpolators o;

                v.vertex.y = GetWave(v.uv0) * _WaveAmp;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normals);
                o.uv = v.uv0;
                return o;
            }

            float InverseLerp(float a, float b, float value)
            {
                return (value - a) / (b - a);
            }  

            float4 frag (Interpolators i) : SV_Target
            {
                float wave = GetWave(i.uv);
                
                // 원형 마스크 계산
                float2 uvsCentered = i.uv;
                float radialDistance = length(uvsCentered);
                
                // 원 모양으로 자르기 (반지름 1 기준)
                float circleMask = 1 - step(1, radialDistance);
                
                // 알파값 계산 (거리에 따라 투명도 조절)
                float alpha = circleMask * (1 - radialDistance);
                
                return float4(wave, wave, wave, alpha);
            }
            
            ENDCG
        } 
    }
}

