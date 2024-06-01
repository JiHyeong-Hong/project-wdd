Shader "Custom/SpriteFill"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 텍스처 속성 선언
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.0 // 채우기 양 속성 선언
        _HoleRadius ("Hole Radius", Range(0, 0.5)) = 0.1 // 구멍 반경 속성 선언
        // _CornerRadius ("Corner Radius", Range(0, 0.5)) = 0.1 // 꼭지점 반경 속성 선언
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" } // 셰이더가 렌더링되는 순서를 투명 큐로 설정
        Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 설정
        Cull Off // 백페이스 컬링 비활성화
        Lighting Off // 조명 비활성화
        ZWrite Off // 깊이 쓰기 비활성화

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION; // 정점 위치
                float2 texcoord : TEXCOORD0; // 텍스처 좌표
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0; // 텍스처 좌표
                float4 vertex : SV_POSITION; // 변환된 정점 위치
            };

            sampler2D _MainTex; // 텍스처 샘플러
            float4 _MainTex_ST; // 텍스처 변환 행렬
            float _FillAmount; // 채우기 양
            float _HoleRadius; // 구멍 반경
            float _CornerRadius; // 코너 반경

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // 객체 공간을 클립 공간으로 변환
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex); // 텍스처 좌표 변환
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 texCoord = i.texcoord;
    
                // 각도 계산 (0 ~ 1 범위로 변환)
                float angle = atan2(0.5 - texCoord.x , 0.5 - texCoord.y) / 3.14159265359 * 0.5 + 0.5;

                // 중심으로부터의 거리 계산
                float distanceFromCenter = length(texCoord - center);

                // 구멍 반경 이내, 각도가 _FillAmount보다 크거나, 꼭지점 반경 이내에 있으면 픽셀을 그리지 않음
                if (distanceFromCenter < _HoleRadius || angle > _FillAmount)
                    discard;

                // 텍스처 색상을 샘플링
                fixed4 col = tex2D(_MainTex, texCoord);
                return col;
            }
            ENDCG
        }
    }
}