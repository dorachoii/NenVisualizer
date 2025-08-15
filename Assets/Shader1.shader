Shader "Unlit/Shader1"
{
    //InputData (lighting, mesh) 등의 정보
    Properties
    {
        _Value ("Value", Float) = 1.0
    }


    SubShader
    {
        Tags { "RenderType"="Opaque" }
        //LOD는 지움

        //SubShader의 Pass가 Actual Shader Code를 포함한다.
        Pass
        {
            CGPROGRAM //HSLS(High-Level Shading Language)코드가 시작되는 시점, 실제 Shader코드를 작성하는 곳곳

            // #pragma는 컴파일러에게 지시를 전달하는 키워드 
            #pragma vertex vert // vert함수를 vertex shader로 사용해라!
            #pragma fragment frag//frag함수를 fragment shader로 사용해라!
       
            #include "UnityCG.cginc" // Unity에서 제공하는 유틸리티 함수들을 포함하는 파일으로 대부분 포함시켜둠!

            float _Value; // Property에서 선언한 변수

            // Automatically filled by Unity
            struct MeshData // per-vertex mesh data
            {
                float4 vertex : POSITION; //vertex position 정점 위치
                float3 normal : NORMAL; //normal vector 법선 벡터
                //float3 tangent : TANGENT;
                //float4 color: COLOR;
                float2 uv0 : TEXCOORD0; // uv coordinates 텍스처 매핑할 때 쓰기도 함.
                //float2 uv1 : TEXCOORD1; 
            };

            //v2f: vertex to fragment로 넘어가는 데이터에 대한 unity의 원래 작명
            //Interpolator: vertex shader에서 계산된 데이터를 fragment shader에 전달하는 구조체로, Rasterizer에 의해 보간된 데이터를 저장하는 구조체
            struct Interpolators 
            {
                //float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION; //clip space of vertex position
            };

            // vertex shader의 return: Interpolators
            // vertex shader의 input: MeshData
            Interpolators vert (MeshData v)
            {
                Interpolators o;// o: output
                o.vertex = UnityObjectToClipPos(v.vertex);// local space to clip space mvp matrix를 계산해서 저장
                return o;
            }

            // float4 = Vector4(32 bit float)
            // half4 = Vector4(16 bit float)
            // fixed4 = Vector4(11 ~12bit float, lower precision) : -1 ~ 1

            // float4x4 = Matrix4x4(32 bit float)
            // half4x4 = Matrix4x4(16 bit float)
            // fixed4x4 = Matrix4x4(11 ~12bit float)

            //원래 fixed4였는데 float4으로 바꿈
            float4 frag (Interpolators i) : SV_Target
            {
                return float4(1,0,0,1);//red
            }
            ENDCG
        }
    }
}
