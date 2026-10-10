Shader "BetterOreMining/Ripple"
{
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

        Pass
        {
            Name "Depth"
            ZWrite On
            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 vert(float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            fixed4 frag() : SV_Target
            {
                return 0;
            }
            ENDCG
        }

        Pass
        {
            Name "Ring"
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            static const float Lead = 0.15;
            static const float FullBurialDepth = 2.0;

            float4 _Center;
            float _Front;
            float _Width;
            float _Opacity;
            float4 _VisibleColor;
            float4 _BlockedColor;
            sampler2D _GroundMap;
            float4 _GroundRect;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 world : TEXCOORD0;
            };

            v2f vert(float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.world = mul(unity_ObjectToWorld, vertex).xyz;
                return o;
            }

            float Brightness(float distance)
            {
                float behind = (_Front - distance) / _Width;
                if (behind <= 0.0 || behind >= 1.0)
                {
                    return 0.0;
                }

                if (behind < Lead)
                {
                    return behind / Lead;
                }

                float trail = 1.0 - (behind - Lead) / (1.0 - Lead);
                return trail * trail;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float brightness = Brightness(distance(i.world, _Center.xyz));
                clip(brightness - 0.001);
                float2 uv = (i.world.xz - _GroundRect.xy) * _GroundRect.zw;
                float ground = tex2D(_GroundMap, uv).r;
                float burial = saturate((ground - i.world.y) / FullBurialDepth);
                float4 color = lerp(_VisibleColor, _BlockedColor, burial);
                return float4(color.rgb, _Opacity * brightness);
            }
            ENDCG
        }
    }
}
