// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AthmosphericFog"
{
	Properties
	{
		[HDR]_Tint("Fog Tint", Color) = (1,1,1,0)
		_FogDistance("Fog Distance", Float) = 600
		_Scale("Scale", Float) = 3000
		_Offset("Offset", Float) = 2000
		_FogExposure("Fog Exposure", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Cull Off
		ZWrite Off
		ZTest LEqual
		
		Pass
		{
			CGPROGRAM

			

			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED

		
			struct ASEAttributesDefault
			{
				float3 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				
			};

			struct ASEVaryingsDefault
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
			#if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
			#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform float4 _Tint;
			uniform float _FogExposure;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _Offset;
			uniform float _Scale;
			uniform float _FogDistance;


			float2 UnStereo( float2 UV )
			{
				#if UNITY_SINGLE_PASS_STEREO
				float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex ];
				UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
				#endif
				return UV;
			}
			
			float3 InvertDepthDir72_g1( float3 In )
			{
				float3 result = In;
				#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
				result *= float3(1,1,-1);
				#endif
				return result;
			}
			

			float2 TransformTriangleVertexToUV (float2 vertex)
			{
				float2 uv = (vertex + 1.0) * 0.5;
				return uv;
			}

			ASEVaryingsDefault Vert( ASEAttributesDefault v  )
			{
				ASEVaryingsDefault o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = TransformTriangleVertexToUV (v.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
				o.texcoordStereo = TransformStereoScreenSpaceTex (o.texcoord, 1.0);

				v.texcoord = o.texcoordStereo;
				float4 ase_ppsScreenPosVertexNorm = float4(o.texcoordStereo,0,1);

				

				return o;
			}

			float4 Frag (ASEVaryingsDefault i  ) : SV_Target
			{
				float4 ase_ppsScreenPosFragNorm = float4(i.texcoordStereo,0,1);

				float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
				float2 UV22_g3 = ase_ppsScreenPosFragNorm.xy;
				float2 localUnStereo22_g3 = UnStereo( UV22_g3 );
				float2 break64_g1 = localUnStereo22_g3;
				float clampDepth69_g1 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_ppsScreenPosFragNorm.xy );
				#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g1 = ( 1.0 - clampDepth69_g1 );
				#else
				float staticSwitch38_g1 = clampDepth69_g1;
				#endif
				float3 appendResult39_g1 = (float3(break64_g1.x , break64_g1.y , staticSwitch38_g1));
				float4 appendResult42_g1 = (float4((appendResult39_g1*2.0 + -1.0) , 1.0));
				float4 temp_output_43_0_g1 = mul( unity_CameraInvProjection, appendResult42_g1 );
				float3 temp_output_46_0_g1 = ( (temp_output_43_0_g1).xyz / (temp_output_43_0_g1).w );
				float3 In72_g1 = temp_output_46_0_g1;
				float3 localInvertDepthDir72_g1 = InvertDepthDir72_g1( In72_g1 );
				float4 appendResult49_g1 = (float4(localInvertDepthDir72_g1 , 1.0));
				float temp_output_37_0 = ( 1.0 - saturate( ( ( (mul( unity_CameraToWorld, appendResult49_g1 )).y + _Offset ) / _Scale ) ) );
				float eyeDepth33 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_ppsScreenPosFragNorm.xy ));
				float4 lerpResult31 = lerp( tex2DNode1 , ( ( _Tint * _FogExposure ) * temp_output_37_0 ) , ( temp_output_37_0 * saturate( ( eyeDepth33 / _FogDistance ) ) ));
				float4 lerpResult30 = lerp( tex2DNode1 , lerpResult31 , _Tint.a);
				float temp_output_3_0_g6 = ( 800.0 - eyeDepth33 );
				float4 lerpResult46 = lerp( tex2DNode1 , lerpResult30 , saturate( ( temp_output_3_0_g6 / fwidth( temp_output_3_0_g6 ) ) ));
				

				float4 color = lerpResult46;
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18800
367;391;1104;637;-2628.484;-77.67676;1;True;True
Node;AmplifyShaderEditor.FunctionNode;10;1176.916,-516.6085;Inherit;False;Reconstruct World Position From Depth;-1;;1;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;12;1575.881,-439.149;Inherit;False;Property;_Offset;Offset;4;0;Create;True;0;0;0;False;0;False;2000;2000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;11;1508.039,-520.8952;Inherit;False;False;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;1767.608,-474.6945;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;1574.859,-358.7823;Inherit;False;Property;_Scale;Scale;3;0;Create;True;0;0;0;False;0;False;3000;3000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;1919.512,-108.0336;Inherit;False;Property;_FogDistance;Fog Distance;2;0;Create;True;0;0;0;False;0;False;600;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;16;1938.516,-437.6901;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;33;1912.62,-185.2513;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;17;2054.126,-440.0865;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;34;2092.341,-179.1354;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;600;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;2155.269,29.23532;Inherit;False;Property;_FogExposure;Fog Exposure;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;20;2087.191,138.5746;Inherit;False;Property;_Tint;Fog Tint;1;1;[HDR];Create;False;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;2388.124,11.08238;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;35;2200.311,-180.4638;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;37;2182.722,-437.334;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;2521.423,-259.6205;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;2562.548,-78.10471;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;2492.278,407.1551;Inherit;True;Global;_MainTex;_MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;31;3036.943,-143.3421;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;30;3214.816,148.0754;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;47;3173.641,445.7097;Inherit;False;Step Antialiasing;-1;;6;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;800;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;2780.109,45.65852;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;45;2604.259,102.6905;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;43;2602.313,216.7823;Inherit;False;False;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;42;2111.534,367.5131;Inherit;False;Reconstruct World Position From Depth;-1;;7;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;46;3477.512,275.9316;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;3781.492,36.81484;Float;False;True;-1;2;ASEMaterialInspector;0;2;AthmosphericFog;32139be9c1eb75640a847f011acf3bcf;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;True;2;False;-1;True;0;False;-1;False;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;11;0;10;0
WireConnection;13;0;11;0
WireConnection;13;1;12;0
WireConnection;16;0;13;0
WireConnection;16;1;15;0
WireConnection;17;0;16;0
WireConnection;34;0;33;0
WireConnection;34;1;36;0
WireConnection;39;0;20;0
WireConnection;39;1;40;0
WireConnection;35;0;34;0
WireConnection;37;0;17;0
WireConnection;38;0;37;0
WireConnection;38;1;35;0
WireConnection;41;0;39;0
WireConnection;41;1;37;0
WireConnection;31;0;1;0
WireConnection;31;1;41;0
WireConnection;31;2;38;0
WireConnection;30;0;1;0
WireConnection;30;1;31;0
WireConnection;30;2;20;4
WireConnection;47;1;33;0
WireConnection;44;0;20;4
WireConnection;44;1;45;0
WireConnection;45;0;43;0
WireConnection;43;0;42;0
WireConnection;46;0;1;0
WireConnection;46;1;30;0
WireConnection;46;2;47;0
WireConnection;0;0;46;0
ASEEND*/
//CHKSM=1F8B1D2991D2B77A4E2CB04B37B992191F8C9A42