Shader "Custom/SpriteFill"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // �ؽ�ó �Ӽ� ����
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.0 // ä��� �� �Ӽ� ����
        _HoleRadius ("Hole Radius", Range(0, 0.5)) = 0.1 // ���� �ݰ� �Ӽ� ����
        // _CornerRadius ("Corner Radius", Range(0, 0.5)) = 0.1 // ������ �ݰ� �Ӽ� ����
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" } // ���̴��� �������Ǵ� ������ ���� ť�� ����
        Blend SrcAlpha OneMinusSrcAlpha // ���� ���� ����
        Cull Off // �����̽� �ø� ��Ȱ��ȭ
        Lighting Off // ���� ��Ȱ��ȭ
        ZWrite Off // ���� ���� ��Ȱ��ȭ

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION; // ���� ��ġ
                float2 texcoord : TEXCOORD0; // �ؽ�ó ��ǥ
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0; // �ؽ�ó ��ǥ
                float4 vertex : SV_POSITION; // ��ȯ�� ���� ��ġ
            };

            sampler2D _MainTex; // �ؽ�ó ���÷�
            float4 _MainTex_ST; // �ؽ�ó ��ȯ ���
            float _FillAmount; // ä��� ��
            float _HoleRadius; // ���� �ݰ�
            float _CornerRadius; // �ڳ� �ݰ�

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // ��ü ������ Ŭ�� �������� ��ȯ
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex); // �ؽ�ó ��ǥ ��ȯ
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 texCoord = i.texcoord;
    
                // ���� ��� (0 ~ 1 ������ ��ȯ)
                float angle = atan2(0.5 - texCoord.x , 0.5 - texCoord.y) / 3.14159265359 * 0.5 + 0.5;

                // �߽����κ����� �Ÿ� ���
                float distanceFromCenter = length(texCoord - center);

                // ���� �ݰ� �̳�, ������ _FillAmount���� ũ�ų�, ������ �ݰ� �̳��� ������ �ȼ��� �׸��� ����
                if (distanceFromCenter < _HoleRadius || angle > _FillAmount)
                    discard;

                // �ؽ�ó ������ ���ø�
                fixed4 col = tex2D(_MainTex, texCoord);
                return col;
            }
            ENDCG
        }
    }
}