﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

 Shader "RenderFX/Skybox With Fog" 
 {
	 Properties 
	 {
	     _Fog ("Fog Intensity", Range(0.33,5.0)) = 1.0
	     _FrontTex ("Front (+Z)", 2D) = "white" {}
	     _BackTex ("Back (-Z)", 2D) = "white" {}
	     _LeftTex ("Left (+X)", 2D) = "white" {}
	     _RightTex ("Right (-X)", 2D) = "white" {}
	     _UpTex ("Up (+Y)", 2D) = "white" {}
	     _DownTex ("Down (-Y)", 2D) = "white" {}
	 }
	 
	 SubShader 
	 {
	     Tags { "Queue"="Background" "RenderType"="Background" }
	     Cull Off ZWrite Off Fog { Mode Off }
	     
	     CGINCLUDE
	     #include "UnityCG.cginc"
	 
	     //half4 unity_FogColor; // fog color defined by RenderSettings
	     fixed _Fog;
	     
	     struct appdata_t 
	     {
	         float4 vertex : POSITION;
	         float2 texcoord : TEXCOORD0;
	     };
	     
	     struct v2f 
	     {
	         float4 vertex : POSITION;
	         float2 texcoord : TEXCOORD0;
	         float4 dir: TEXCOORD1;
	     };
	     
	     v2f vert (appdata_t v)
	     {
	         v2f o;
	         o.vertex = UnityObjectToClipPos(v.vertex);
	         o.texcoord = v.texcoord;
	         o.dir = mul(unity_ObjectToWorld, v.vertex);
	         return o;
	     }
	     
	     fixed4 skybox_frag (v2f i, sampler2D sky)
	     {
	         fixed4 tex = tex2D(sky, i.texcoord); // get skybox texel
	         half fog = saturate(1 - normalize(i.dir).y / _Fog); // fog vanishes upwards
	         tex = lerp(tex, unity_FogColor, fog); // blend skybox with fog
	         return tex;
	     }
	     ENDCG
	     
	     Pass 
	     {
	         CGPROGRAM
	         #pragma vertex vert
	         #pragma fragment frag
	         #pragma fragmentoption ARB_precision_hint_fastest
	         sampler2D _FrontTex;
	         fixed4 frag (v2f i) : COLOR { return skybox_frag(i, _FrontTex); }
	         ENDCG 
	     }
	     Pass
	     {
	         CGPROGRAM
	         #pragma vertex vert
	         #pragma fragment frag
	         #pragma fragmentoption ARB_precision_hint_fastest
	         sampler2D _BackTex;
	         fixed4 frag (v2f i) : COLOR { return skybox_frag(i, _BackTex); }
	         ENDCG 
	     }
	     Pass
	     {
	         CGPROGRAM
	         #pragma vertex vert
	         #pragma fragment frag
	         #pragma fragmentoption ARB_precision_hint_fastest
	         sampler2D _LeftTex;
	         fixed4 frag (v2f i) : COLOR { return skybox_frag(i, _LeftTex); }
	         ENDCG
	     }
	     Pass
	     {
	         CGPROGRAM
	         #pragma vertex vert
	         #pragma fragment frag
	         #pragma fragmentoption ARB_precision_hint_fastest
	         sampler2D _RightTex;
	         fixed4 frag (v2f i) : COLOR { return skybox_frag(i, _RightTex); }
	         ENDCG
	     }    
	     Pass
	     {
	         CGPROGRAM
	         #pragma vertex vert
	         #pragma fragment frag
	         #pragma fragmentoption ARB_precision_hint_fastest
	         sampler2D _UpTex;
	         fixed4 frag (v2f i) : COLOR { return skybox_frag(i, _UpTex); }
	         ENDCG
	     }    
	     Pass
	     {
	         CGPROGRAM
	         #pragma vertex vert
	         #pragma fragment frag
	         #pragma fragmentoption ARB_precision_hint_fastest
	         sampler2D _DownTex;
	         fixed4 frag (v2f i) : COLOR { return skybox_frag(i, _DownTex); }
	         ENDCG
	     }
	 }     
 }
