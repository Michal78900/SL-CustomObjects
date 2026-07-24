using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class TextComponent : SchematicBlock
{
	public override BlockType BlockType => BlockType.Text;

	private TMP_Text _textMesh;
	private MeshRenderer _renderer;

	private void Awake()
	{
		TryGetComponent(out _textMesh);
		TryGetComponent(out _renderer);
	}

	private void Update()
	{
		_textMesh.margin = Vector4.zero;
		_renderer.hideFlags = HideFlags.HideInInspector;
	}

	public override void Compile(SchematicBlockData block)
	{
		string formattedText = _textMesh.text;

		// Wrap in size tag
		formattedText = $"<size={_textMesh.fontSize}>{formattedText}</size>";

		// Wrap in color tag
		formattedText = $"<color=#{ColorUtility.ToHtmlStringRGBA(_textMesh.color)}>{formattedText}</color>";

		// Wrap in alignment tag
		string align = _textMesh.alignment switch
		{
			TextAlignmentOptions.Left or TextAlignmentOptions.TopLeft or TextAlignmentOptions.BottomLeft or TextAlignmentOptions.BaselineLeft or TextAlignmentOptions.MidlineLeft => "left",
			TextAlignmentOptions.Right or TextAlignmentOptions.TopRight or TextAlignmentOptions.BottomRight or TextAlignmentOptions.BaselineRight or TextAlignmentOptions.MidlineRight => "right",
			TextAlignmentOptions.Justified or TextAlignmentOptions.TopJustified or TextAlignmentOptions.BottomJustified or TextAlignmentOptions.BaselineJustified or TextAlignmentOptions.MidlineJustified => "justified",
			_ => "center"
		};
		formattedText = $"<align={align}>{formattedText}</align>";

		block.Properties = new Dictionary<string, object>
		{
			{ "Text", formattedText },
			{ "DisplaySize", (SerializableVector)_textMesh.rectTransform.sizeDelta }
		};

		base.Compile(block);
	}

	public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
	{
		TMP_Text text = Create<GameObject>("Assets/Resources/Blocks/Text.prefab").GetComponent<TMP_Text>();
		gameObject = text.gameObject;

		string rawText = Convert.ToString(block.Properties["Text"]);

		// Parse alignment
		var alignMatch = System.Text.RegularExpressions.Regex.Match(rawText, @"<align=(left|right|center|justified)>");
		if (alignMatch.Success)
		{
			string align = alignMatch.Groups[1].Value;
			text.alignment = align switch
			{
				"left" => TextAlignmentOptions.Left,
				"right" => TextAlignmentOptions.Right,
				"justified" => TextAlignmentOptions.Justified,
				_ => TextAlignmentOptions.Center
			};
			rawText = System.Text.RegularExpressions.Regex.Replace(rawText, @"</?align[^>]*>", "");
		}

		// Parse color
		var colorMatch = System.Text.RegularExpressions.Regex.Match(rawText, @"<color=#([A-Fa-f0-9]{8})>");
		if (colorMatch.Success)
		{
			if (ColorUtility.TryParseHtmlString("#" + colorMatch.Groups[1].Value, out Color color))
				text.color = color;
			rawText = System.Text.RegularExpressions.Regex.Replace(rawText, @"</?color[^>]*>", "");
		}

		// Parse size
		var sizeMatch = System.Text.RegularExpressions.Regex.Match(rawText, @"<size=([0-9\.]+)>");
		if (sizeMatch.Success)
		{
			if (float.TryParse(sizeMatch.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float size))
				text.fontSize = size;
			rawText = System.Text.RegularExpressions.Regex.Replace(rawText, @"</?size[^>]*>", "");
		}

		text.text = rawText;
		text.rectTransform.sizeDelta = JsonConvert.DeserializeObject<Vector2>(block.Properties["DisplaySize"].ToString());

		base.Decompile(ref gameObject, block, parent);
	}
}
