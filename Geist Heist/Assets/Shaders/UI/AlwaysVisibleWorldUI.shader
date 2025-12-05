/*
 * Contributors: Toby
 * Creation Date: 12/3/2025
 * Last Modified: 12/3/2025
 * 
 * Brief Description: Makes worldspace UI visible through walls.
 * Made using code found on the internet. Toby didnt really make this :P
 */ 

Shader "Custom/AlwaysVisibleWorldUI"
{
        Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        
        _TopColor("Top Color", Color) = (1,1,1,1)
        _BottomColor("Bottom Color", Color) = (1,1,1,1)

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness", Range(0,1)) = 0.1

        _FaceSoftness("Softness", Range(0,1)) = 0.02
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            float4 _TopColor;
            float4 _BottomColor;

            float4 _OutlineColor;
            float _OutlineThickness;
            float _FaceSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                // uvs are individual for each letter, apparently, so this will work if theres multiple lines of text.
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample SDF alpha
                float sdf = tex2D(_MainTex, i.uv).a;

                // actually no idea how this works. black magic to me.
                float outline = smoothstep(0.5 - _OutlineThickness - _FaceSoftness,
                                           0.5 - _OutlineThickness,
                                           sdf);

                float face = smoothstep(0.5 - _FaceSoftness,
                                        0.5 + _FaceSoftness,
                                        sdf);

                float4 gradient = lerp(_BottomColor, _TopColor, i.uv.y);

                // Combine gradient with vertex color (TMP tint)
                float4 faceColor = gradient * i.color;

                // Final composite
                float4 finalColor = lerp(_OutlineColor, faceColor, face) * outline;

                finalColor.a = finalColor.a; // keep transparency

                return finalColor;
            }
            ENDCG
        }
    }
}