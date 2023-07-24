// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" {
    Properties {
        _WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
        _Cutoff ("Cutoff", float) = 0.5
    }

CGINCLUDE
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

        void TerrainBillboardGrassKritoa( inout float4 pos, float2 offset )
        {
            float3 grasspos = pos.xyz - _CameraPosition.xyz;
            if (dot(grasspos, grasspos) > _WaveAndDistance.w)
                offset = 0.0;

// aim at camera instead of orienting to camera plane. Probably looks better to use (0,1,0) instead of newup.

            float3 right = normalize(cross(float3(0, 1, 0), grasspos));
            float3 newup = normalize(cross(grasspos, right));

            pos.xyz += offset.x * right;
            pos.xyz += offset.y * newup;

//            pos.xyz += offset.x * _CameraRight.xyz;    // old method to orient to camera orientation
//            pos.xyz += offset.y * _CameraUp.xyz;
        }


void WavingGrassBillboardVertKritoa (inout appdata_full v)
{
    TerrainBillboardGrassKritoa (v.vertex, v.tangent.xy);
    // wave amount defined by the grass height
    float waveAmount = v.tangent.y;
    v.color = TerrainWaveGrass (v.vertex, waveAmount, v.color);
}

struct v2f {
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;
    float4 uv : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};
v2f BillboardVert (appdata_full v) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    WavingGrassBillboardVert (v);
    o.color = v.color;

    o.color.rgb *= ShadeVertexLights (v.vertex, v.normal);

    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord;
    return o;
}
ENDCG

    SubShader {
        Tags {
            "Queue" = "Geometry+200"
            "IgnoreProjector"="True"
            "RenderType"="GrassBillboard"
            "DisableBatching"="True"
        }
        Cull Off
        LOD 200
        ColorMask RGB

CGPROGRAM
#pragma surface surf Lambert vertex:WavingGrassBillboardVertKritoa addshadow exclude_path:deferred

sampler2D _MainTex;
fixed _Cutoff;

struct Input {
    float2 uv_MainTex;
    fixed4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
    clip (o.Alpha - _Cutoff);
    o.Alpha *= IN.color.a;
}

ENDCG
    }

    Fallback Off
}
