Shader "UI/PaletteSwapUI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _KeyBase      ("Key Base", Color)      = (0,1,0,1)
        _KeyShadow    ("Key Shadow", Color)    = (0,0,1,1)
        _KeyHighlight ("Key Highlight", Color) = (1,0,1,1)
        _KeySkin      ("Key Skin", Color)      = (1,0.8,0.6,1)

        _BaseColor      ("Base Color", Color)      = (1,1,1,1)
        _ShadowColor    ("Shadow Color", Color)    = (0.5,0.5,0.5,1)
        _HighlightColor ("Highlight Color", Color) = (1,1,0,1)
        _SkinColor      ("Skin Color", Color)      = (1,0.8,0.6,1)

        _Tolerance ("Tolerance", Range(0,1)) = 0.05
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Lighting Off
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _KeyBase, _KeyShadow, _KeyHighlight, _KeySkin;
            float4 _BaseColor, _ShadowColor, _HighlightColor, _SkinColor;
            float _Tolerance;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                if (texColor.a <= 0.001)
                    discard;

                float distBase      = distance(texColor.rgb, _KeyBase.rgb);
                float distShadow    = distance(texColor.rgb, _KeyShadow.rgb);
                float distHighlight = distance(texColor.rgb, _KeyHighlight.rgb);
                float distSkin      = distance(texColor.rgb, _KeySkin.rgb);

                if (distBase < _Tolerance)      
                    return float4(_BaseColor.rgb, texColor.a * _BaseColor.a);

                if (distShadow < _Tolerance)    
                    return float4(_ShadowColor.rgb, texColor.a * _ShadowColor.a);

                if (distHighlight < _Tolerance) 
                    return float4(_HighlightColor.rgb, texColor.a * _HighlightColor.a);

                if (distSkin < _Tolerance)      
                    return float4(_SkinColor.rgb, texColor.a * _SkinColor.a);

                return texColor;
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
