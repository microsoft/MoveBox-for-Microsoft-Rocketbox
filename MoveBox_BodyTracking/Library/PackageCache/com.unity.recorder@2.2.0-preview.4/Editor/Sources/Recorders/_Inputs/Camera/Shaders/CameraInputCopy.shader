// Slight rework of Unity's Hidden/BlitCopy shader, with vertical flip and optional alpha overwrite.
Shader "Hidden/Recorder/Inputs/CameraInput/Copy" {
    Properties
    {
        _MainTex ("Texture", any) = "" {}
    }
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile ___ TRANSPARENCY_ON
            #pragma multi_compile ___ VERTICAL_FLIP

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            uniform float4 _MainTex_ST;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float2 get_texcoord(v2f i)
            {
                float2 t = i.texcoord.xy * 0.5 + 0.5;
                return t;
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float2 t = i.texcoord;
                #if defined(VERTICAL_FLIP)
                t.y = 1.0 - t.y;
                #endif
                fixed4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, t);
                #if !defined(TRANSPARENCY_ON)
                c.a = 1.0f;
                #endif
                return c;
            }
            ENDCG
        }
    }
    Fallback Off
}
