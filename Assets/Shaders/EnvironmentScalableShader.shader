// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/EnvironmentScalableShader"
{
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Scale("Texture Scale", Float) = 1.0
}
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;
float _Scale;

static const float _sinXWall = sin ( -1.57 );
static const float _cosXWall = cos ( -1.57 );
static const float _sinYWall = sin ( -1.57 );
static const float2x2 rotationMatrixWall = float2x2( _cosXWall, -_sinXWall, _sinYWall, _cosXWall);

static const float _sinXTop = sin ( 1.57 );
static const float _cosXTop = cos ( 1.57 );
static const float _sinYTop = sin ( 1.57 );
static const float2x2 rotationMatrixTop = float2x2( _cosXTop, -_sinXTop, _sinYTop, _cosXTop);

struct Input {
    float3 worldNormal;
    float3 worldPos;
};

void surf (Input IN, inout SurfaceOutput o) {
    float2 UV;
    fixed4 c;

    float3 pos = IN.worldPos;
    if (abs(IN.worldNormal.x) > 0.5) {
        // use WALLSIDE texture
        UV = mul ( pos.yz, rotationMatrixWall );        
    }
    else if (abs(IN.worldNormal.z) > 0.5) {
        // use WALL texture
        UV = pos.xy; // front
    }
    else {
        // use FLR texture
        UV = mul ( pos.xz, rotationMatrixTop );        
        // UV = pos.xz; // top
    }
    c = tex2D(_MainTex, UV* _Scale); // use FLR texture

    o.Albedo = c.rgb * _Color;
}

ENDCG
}

Fallback "Legacy Shaders/VertexLit"
}