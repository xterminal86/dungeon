Shader "Custom/CropCircle" 
{
  Properties 
  {
    _MainTex("Texture", 2D) = "white" {}
    _WhiteTex("POT White Texture", 2D) = "white" {}
    _Radius("Circle Radius", float) = 1.0
    _ScreenWidth("Screen Width", int) = 640
    _ScreenHeight("Screen Height", int) = 480  
  }

  SubShader 
  {
    Pass 
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag fullforwardshadows
      #include "UnityCG.cginc"

      uniform float4 _MainTex_ST;
      uniform float4 _WhiteTex_ST;

      uniform sampler2D _MainTex;     
      uniform sampler2D _WhiteTex;     

      uniform float _Radius;

      uniform int _ScreenWidth;
      uniform int _ScreenHeight;

      struct app2vert
      {
        float4 position: POSITION;
        float2 mtc: TEXCOORD0;
        float2 wtc: TEXCOORD1;
      };

      struct vert2frag
      {
        float4 position: POSITION;
        float2 mtc: TEXCOORD0;
        float2 wtc: TEXCOORD1;
      };
      
      vert2frag vert(app2vert input)
      {
        vert2frag output;
        output.position = UnityObjectToClipPos(input.position);
        output.mtc = TRANSFORM_TEX(input.mtc, _MainTex);
        output.wtc = TRANSFORM_TEX(input.wtc, _WhiteTex);
      
        return output;
      }

      fixed4 frag(vert2frag input) : COLOR
      {
        fixed4 main_color = tex2D(_MainTex, input.mtc);
        fixed4 add_color = tex2D(_WhiteTex, input.wtc);

        // Condition evaluation needs to be prioritized.
        //
        // ( (_ScreenHeight == 0) ? _ScreenWidth : (float)_ScreenHeight ) brackets are mandatory or there
        // will be an implicit conversion.
        // (Unity gives error on similar line above without outer brackets about dividing float into bool).

        float aspect = (float)_ScreenWidth / ((_ScreenHeight == 0) ? _ScreenWidth : (float)_ScreenHeight);

        float x = (aspect > 1.0) ? (0.5 - input.mtc.x) * aspect : (0.5 - input.mtc.x);
        float y = (aspect > 1.0) ? (0.5 - input.mtc.y) : (0.5 - input.mtc.y) * aspect;

        float d = sqrt(pow(x, 2) + pow(y, 2));

        return fixed4(main_color.r, main_color.g, main_color.b, (d < _Radius) ? 1.0 : 0.0);
      }
      ENDCG  
    }
  }
}
