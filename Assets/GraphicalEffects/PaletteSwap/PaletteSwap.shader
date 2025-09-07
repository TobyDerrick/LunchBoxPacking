Shader "Unlit/PaletteSwap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KeyBase ("Key Base", Color) = (0,1,0,1)
        _KeyShadow ("Key Shadow", Color) = (0,0,1,1)
        _KeyHighlight ("Key Highlight", Color) = (1,0,1,1)
        _KeySkin ("Key Skin", Color) = (1,0.8,0.6,1)

        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        _HighlightColor ("Highlight Color", Color) = (1,1,0,1)
        _SkinColor ("Skin Color", Color) = (1,0.8,0.6,1)

        _Tolerance ("Tolerance", Range(0,1)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _KeyBase;
            float4 _KeyShadow;
            float4 _KeyHighlight;
            float4 _KeySkin;

            float4 _BaseColor;
            float4 _ShadowColor;
            float4 _HighlightColor;
            float4 _SkinColor;

            float _Tolerance;

            float4 _MainTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // applies _MainTex_ST
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                if (texColor.a == 0)
                {
                    clip(-1);
                }
                float distBase = distance(texColor.rgb, _KeyBase.rgb);
                float distShadow = distance(texColor.rgb, _KeyShadow.rgb);
                float distHighlight = distance(texColor.rgb, _KeyHighlight.rgb);
                float distSkin = distance(texColor.rgb, _KeySkin.rgb);

                if (distBase < _Tolerance) return _BaseColor;
                if (distShadow < _Tolerance) return _ShadowColor;
                if (distHighlight < _Tolerance) return _HighlightColor;
                if (distSkin < _Tolerance) return _SkinColor;

                return texColor;
            }
            ENDCG
        }
    }
}
