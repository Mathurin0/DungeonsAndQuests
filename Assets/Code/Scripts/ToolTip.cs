using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Text titleField;
	[SerializeField] private Text contentField;
	[SerializeField] private LayoutElement layoutElement;
	[SerializeField] private RectTransform rectTransform;
	[SerializeField] private int maxCharactersPerLine;

	public void SetFieldsContent(string content, string title = "")
	{
		if (title == string.Empty)
		{
			titleField.gameObject.SetActive(false);
		}
		else
		{
			titleField.gameObject.SetActive(true);
			titleField.text = title;
		}

		contentField.text = content;

		layoutElement.enabled = (titleField.text.Length > maxCharactersPerLine || contentField.text.Length > maxCharactersPerLine);
	}

	private void Update()
	{
		Vector2 mousePosition = Input.mousePosition;
		float toolTipPosX;
		float toolTipPosY;

		toolTipPosX = Convert.ToSingle((rectTransform.rect.width / 2));
		toolTipPosY = Convert.ToSingle((rectTransform.rect.height / 2));


		if ((mousePosition.x + toolTipPosX * 2) < Screen.width)
		{
			mousePosition.x += toolTipPosX + 5;
		}
		else
		{
			mousePosition.x -= toolTipPosX;
		}

		if ((mousePosition.y - toolTipPosY * 2) > 0)
		{
			mousePosition.y -= toolTipPosY + 5;
		}
		else
		{
			mousePosition.y += toolTipPosY;
		}

		transform.position = mousePosition;
	}
}
