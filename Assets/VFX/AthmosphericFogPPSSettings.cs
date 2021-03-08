// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(AthmosphericFogPPSRenderer), PostProcessEvent.BeforeStack, "Alexis Morin/Athmospheric Fog", true)]
public sealed class AthmosphericFogPPSSettings : PostProcessEffectSettings {
	[Tooltip("Fog Tint")]
	public ColorParameter _Tint = new ColorParameter { value = new Color(0.5322179f, 0.8314942f, 0.9811f, 1f) };
	[Tooltip("Fog Distance")]
	public FloatParameter _FogDistance = new FloatParameter { value = 600f };
	[Tooltip("Scale")]
	public FloatParameter _Scale = new FloatParameter { value = 2300f };
	[Tooltip("Offset")]
	public FloatParameter _Offset = new FloatParameter { value = 575f };
	[Tooltip("Fog Exposure")]
	public FloatParameter _FogExposure = new FloatParameter { value = 3f };
}

public sealed class AthmosphericFogPPSRenderer : PostProcessEffectRenderer<AthmosphericFogPPSSettings> {
	public override void Render(PostProcessRenderContext context) {
		var sheet = context.propertySheets.Get(Shader.Find("AthmosphericFog"));
		sheet.properties.SetColor("_Tint", settings._Tint);
		sheet.properties.SetFloat("_FogDistance", settings._FogDistance);
		sheet.properties.SetFloat("_Scale", settings._Scale);
		sheet.properties.SetFloat("_Offset", settings._Offset);
		sheet.properties.SetFloat("_FogExposure", settings._FogExposure);
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
}
#endif