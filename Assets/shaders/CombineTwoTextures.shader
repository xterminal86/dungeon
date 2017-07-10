// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CombineTwoTextures"
{
  Properties 
  {
    _MainTex ("Texture 1", 2D) = "white" {}
    _AddTex ("Texture 2", 2D) = "white" {}
  }
 
  SubShader 
  {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    ZWrite Off
    ZTest Off
    Blend SrcAlpha OneMinusSrcAlpha
    Pass 
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "UnityCG.cginc"

      uniform sampler2D _MainTex;
      uniform sampler2D _AddTex;
      uniform float4 _MainTex_ST;
      uniform float4 _AddTex_ST;

      struct app2vert
      {
        float4 position: POSITION;
        float2 texcoord: TEXCOORD0;
      };

      struct vert2frag
      {
        float4 position: POSITION;
        float2 texcoord: TEXCOORD0;
      };

      vert2frag vert(app2vert input)
      {
        vert2frag output;
        output.position = UnityObjectToClipPos(input.position);
        output.texcoord = TRANSFORM_TEX(input.texcoord, _MainTex);
        return output;
      }
       
      fixed4 frag(vert2frag input) : COLOR
      {
        fixed4 main_color = tex2D(_MainTex, input.texcoord);
        fixed4 add_color = tex2D(_AddTex, input.texcoord);

        float cr = add_color.r * add_color.a + main_color.r * (1 - add_color.a);
        float cg = add_color.g * add_color.a + main_color.g * (1 - add_color.a);
        float cb = add_color.b * add_color.a + main_color.b * (1 - add_color.a);

        return fixed4(cr, cg, cb, 1.0);
      }
      ENDCG
    }
  }
}