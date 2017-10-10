Shader "Hidden/ImageEffectTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Duration("Duration", Float) = 0
		_CenterX ("CenterX", Range(1, 60)) = 0.5
		_CenterY ("CenterY", Range(0, 1)) = 0.5
		// 1/スクリーンサイズ
		_PixelRateX ("PixelRateX", Float) = 1.0
		_PixelRateY ("PixelRateY", Float) = 1.0
		// 歪みの強度
		_Distortion ("distortion", Float) = 12.0
		// 歪み幅
		_DistortionWidth ("DistortionWidth", Float) = 3.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Duration;
			float _CenterX;
			float _CenterY;
			float _PixelRateX;
			float _PixelRateY;
			float _Distortion;
			float _DistortionWidth;

			float4 frag (v2f i) : SV_Target
			{
				float2 position = i.uv;
				float2 center = float2(_CenterX, _CenterY);
				// 中心からの方向
				float2 direct = position - center;
				// 中心からの距離
				float d = length(direct);

				float angle = (_Duration - d) * _DistortionWidth * 360;
				if (abs(angle) > 90) {
					return tex2D(_MainTex, i.uv);
				}
				float s = cos(radians(angle));
			
				// 歪み
				float2 uv = float2(	position.x + s * _PixelRateX * _Distortion * saturate(abs(direct.x * 15.0)), 
									position.y + s * _PixelRateY * _Distortion * saturate(abs(direct.y * -15.0)));
//				return float4(s * _PixelRateX * _Distortion * saturate(direct.x * -15.0), s, direct.x, 1);
				float4 color = tex2D(_MainTex, uv);

				//TODO 環境マップを入れてもよいかも
				
				float y_rate = 0.2 * saturate(0.5 - _Duration);
				// 輝度調整
				float y = color.r *  0.299   + color.g *  0.587   + color.b *  0.114;
				float u = color.r * -0.14713 + color.g * -0.28886 + color.b *  0.436;
				float v = color.r *  0.615   + color.g * -0.51499 + color.b * -0.10001;
				color.r = saturate ((y + s * y_rate) + v * 1.13983);
				color.g = saturate ((y + s * y_rate) - u * 0.39465 - 0.58060 * v);
				color.b = saturate ((y + s * y_rate) + u * 2.03211);
				return color;
				//return float4(s, -s, 0, 1);
			}
			ENDCG
		}
	}
}
