Shader "Unlit/Shader_Kyouka"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _ColorStart ("Color Start", Range(0,1)) = 0
        _ColorEnd ("Color End", Range(0,1)) = 1
        _WaveAmp ("Wave Amplitude", Range(0,1)) = 0.1
        _MaxRadius ("Max Radius", Range(0,1)) = 0.8
        _HeightFalloff ("Height Falloff", Range(0,5)) = 2.0
        _MaxHeight ("Max Height", Range(0,5)) = 1.0
        _WaveFrequency ("Wave Frequency", Range(0.1,20)) = 5.0
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
       
            #include "UnityCG.cginc"// unity 기본 함수들 쓰기 위함함
            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
            float _WaveAmp;
            float _MaxRadius;
            float _HeightFalloff;
            float _MaxHeight;
            float _WaveFrequency;

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

                float heightMultiplier = saturate(1 - pow(radialDistance / _MaxRadius, _HeightFalloff));

                // 웨이브 패턴 생성
                float wave = cos((radialDistance - _Time.y * 0.1) * TAU * 3) * 0.5 + 0.5;
                
                // 높이와 웨이브를 결합하고 최대 높이 적용
                return wave * heightMultiplier * _MaxHeight;
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

            float4 frag (Interpolators i) : SV_Target
            {
                // 여기에 웨이브 함수를 넣어보자!   
                float wave = GetWave(i.uv);
                float4 color = lerp(_ColorA, _ColorB, wave);
                
                // 원형 마스크 계산
                float2 uvsCentered = i.uv;
                float radialDistance = length(uvsCentered);
                
                // 원 모양으로 자르기 (반지름 0.7 기준)
                float circleMask = 1 - step(1, radialDistance);
                
                // 알파값 계산 (거리에 따라 투명도 조절)
                float alpha = circleMask * (1 - radialDistance);
                
                return float4(color.rgb, alpha);
            }
            
            
            ENDCG
        } 
    }
}

