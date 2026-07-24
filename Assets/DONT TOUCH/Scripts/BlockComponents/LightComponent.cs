using System;
using System.Collections.Generic;
using UnityEngine;

public class LightComponent : SchematicBlock
{
    [Tooltip("Multiplier for Intensity when exporting to JSON. Scale this up if the game is too dark.")]
    public float GameIntensityMultiplier = 100f;

    [Tooltip("Multiplier for Range when exporting to JSON. Scale this up if the game range is too small.")]
    public float GameRangeMultiplier = 100f;

    public override BlockType BlockType => BlockType.Light;

    public override void Compile(SchematicBlockData block)
    {
        TryGetComponent(out Light light);

        block.Properties = new Dictionary<string, object>
        {
            { "LightType", light.type },
            { "Color", GetColorString(light.color) },
            { "Intensity", light.intensity * GameIntensityMultiplier },
            { "Range", light.range * GameRangeMultiplier },
            { "Shape", light.shape },
            { "SpotAngle", light.spotAngle },
            { "InnerSpotAngle", light.innerSpotAngle },
            { "ShadowStrength", light.shadowStrength },
            { "ShadowType", light.shadows },
        };

        base.Compile(block);
    }

    private string GetColorString(Color color)
    {
        if (color.r <= 1f && color.g <= 1f && color.b <= 1f)
            return ColorUtility.ToHtmlStringRGBA(color);

        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}", color.r * 255f, color.g * 255f, color.b * 255f, color.a);
    }

    public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
    {
        LightType lightType = block.Properties.TryGetValue("LightType", out object objLightType) ? (LightType)Convert.ToInt32(objLightType) : LightType.Point;
        Light light = Create<GameObject>($"Assets/Resources/Blocks/Lights/{lightType} Light.prefab").GetComponent<Light>();
        gameObject = light.gameObject;

        light.color = PrimitiveComponent.GetColorFromString(block.Properties["Color"].ToString());
        light.intensity = Convert.ToSingle(block.Properties["Intensity"]) / GameIntensityMultiplier;
        light.range = Convert.ToSingle(block.Properties["Range"]) / GameRangeMultiplier;

		if (block.Properties.TryGetValue("Shadows", out object shadows))
		{
			// Backward compatibility
			light.shadows = Convert.ToBoolean(shadows) ? LightShadows.Soft : LightShadows.None;
		}
		else
		{
			light.shadows = (LightShadows)Convert.ToInt32(block.Properties["ShadowType"]);
			light.shape = (LightShape)Convert.ToInt32(block.Properties["Shape"]);
			light.spotAngle = Convert.ToSingle(block.Properties["SpotAngle"]);
			light.innerSpotAngle = Convert.ToSingle(block.Properties["InnerSpotAngle"]);
			light.shadowStrength = Convert.ToSingle(block.Properties["ShadowStrength"]);
		}

		base.Decompile(ref gameObject, block, parent);
	}
}
