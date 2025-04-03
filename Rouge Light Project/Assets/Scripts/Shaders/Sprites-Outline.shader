Shader "Sprites/CircularOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.5)) = 0.05
        _Falloff ("Outline Falloff", Range(0.01, 0.5)) = 0.05
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _Falloff;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Получаем центр текстуры (0.5, 0.5)
                float2 center = IN.texcoord - float2(0.5, 0.5);
                float distanceFromCenter = length(center) * 2.0; // Нормализуем до [0,1]
                
                // Основной цвет текстуры
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Если пиксель прозрачный или близок к границе
                if (distanceFromCenter > (1.0 - _OutlineWidth) || c.a < 0.1)
                {
                    // Плавное затухание окантовки
                    float outlineFactor = smoothstep(1.0 - _OutlineWidth - _Falloff, 
                                                    1.0 - _OutlineWidth, 
                                                    distanceFromCenter);
                    
                    // Смешиваем цвет окантовки с основным цветом
                    c.rgb = lerp(c.rgb, _OutlineColor.rgb, outlineFactor);
                    c.a = max(c.a, outlineFactor * _OutlineColor.a);
                }
                
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}