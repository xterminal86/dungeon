Shader "Custom/CropCircle" 
{
  Properties 
  {
    _MainTex("Texture", 2D) = "white" {}
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

      uniform sampler2D _MainTex;     
      float _Radius;
      int _ScreenWidth;
      int _ScreenHeight;

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

        float aspect = (float)_ScreenWidth / (_ScreenHeight == 0) ? 1.0 : (float)_ScreenHeight;

        float d = sqrt(pow(0.5 - input.texcoord.x, 2) + pow(0.5 - input.texcoord.y, 2));

        return fixed4(main_color.r, main_color.g, main_color.b, (d < _Radius) ? 1.0 : 0.0);
      }
      ENDCG  
    }
  }
}
