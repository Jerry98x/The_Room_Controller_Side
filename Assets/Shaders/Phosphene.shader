Shader "Shader Graphs/Phosphene"
{
        Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Time ("Time", Float) = 0.0
        _Resolution ("Resolution", Vector) = (512, 512, 1, 1)
        _Alpha ("Alpha", Range(0, 1)) = 1.0 // Add Alpha property
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Cull Front // Ensure rendering from the inside of the sphere
        Blend SrcAlpha OneMinusSrcAlpha // Enable transparency

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            //float _Time;
            float4 _Resolution;
            float _Alpha; // Add Alpha property

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float frac(float x)
            {
                return x - floor(x);
            }

            float noise(float2 pos)
            {
                return frac(sin(dot(pos * 0.001, float2(24.12357, 36.789))) * 12345.123);
            }

            float smooth_noise(float2 pos)
            {
                return (noise(pos + float2(1, 1)) + noise(pos + float2(-1, 1)) + noise(pos + float2(1, -1)) + noise(pos + float2(-1, -1))) / 16.0 +
                       (noise(pos + float2(1, 0)) + noise(pos + float2(-1, 0)) + noise(pos + float2(0, 1)) + noise(pos + float2(0, -1))) / 8.0 +
                       noise(pos) / 4.0;
            }

            float interpolate_noise(float2 pos)
            {
                float a, b, c, d;

                a = smooth_noise(floor(pos));
                b = smooth_noise(floor(pos + float2(1.0, 0.0)));
                c = smooth_noise(floor(pos + float2(0.0, 1.0)));
                d = smooth_noise(floor(pos + float2(1.0, 1.0)));

                a = lerp(a, b, frac(pos.x));
                b = lerp(c, d, frac(pos.x));
                a = lerp(a, b, frac(pos.y));

                return a;
            }

            float perlin_noise(float2 pos)
            {
                float n;

                n = interpolate_noise(pos * 0.0625) * 0.5;
                n += interpolate_noise(pos * 0.125) * 0.25;
                n += interpolate_noise(pos * 0.025) * 0.225;
                n += interpolate_noise(pos * 0.05) * 0.0625;
                n += interpolate_noise(pos) * 0.03125;
                return n;
            }

            float2 rotate(float2 coord, float2 center, float angle)
            {
                float2 result = coord - center;

                float cosa = cos(angle);
                float sina = sin(angle);

                float2x2 rot = float2x2(
                    cosa, -sina,
                    sina, cosa
                );

                return mul(rot, result) + center;
            }

            float4 circle(float2 uv, float _Time, float2 _Resolution, float _Alpha)
            {
                float PERIOD = 6.0;
                float CIRCLE_MIN_RADIUS = 0.0;
                float CIRCLE_MAX_RADIUS = 0.0;

                float3 BG_BASE = float3(0.1, 0.1, 0.1);
                float3 START_COLOR = float3(0.4, 0.0, 1.0);
                float3 END_COLOR = float3(0.5, 0.5, 0.1);

                float tm = fmod(_Time, PERIOD);
                float t = tm / PERIOD;
                t = 1.0 - t;

                float2 center = float2(0.0, 0.0);

                float radius = lerp(CIRCLE_MIN_RADIUS, CIRCLE_MAX_RADIUS, t);
                radius += 0.4 * smooth_noise(tm * uv);
                radius = 0;

                float rotateDir = noise(uv) > 0.5 ? 1.0 : -1.0;

                uv = rotate(uv, float2(0.0, 0.0), t * 6.28 * rotateDir);

                float dist = length(uv - center);
                dist += 0.1 * perlin_noise(uv * _Resolution.xy);

                if (dist < radius)
                {
                    float3 color = lerp(END_COLOR, START_COLOR, t);
                    float dist_t = 1.0 - (dist / radius);
                    return float4(color + color * dist_t, _Alpha); // Apply Alpha here
                }
                else
                {
                    float3 color = BG_BASE;
                    float2 redCoord = rotate(uv, float2(0.0, 0.0), t * 6.28) + 0.1 * smooth_noise(uv * uv);
                    float redVal = smooth_noise(redCoord);
                    float greenVal = smooth_noise(uv * uv * t);
                    float blueVal = smooth_noise(uv * t);

                    color = float3(redVal, greenVal, blueVal);

                    return float4(color * 0.4, _Alpha); // Apply Alpha here
                }
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;

                uv.x *= _Resolution.x / _Resolution.y;
                uv.x *= 0.8;

                return circle(uv, _Time, _Resolution.xy, _Alpha);
            }
            ENDCG
        }
    }
}