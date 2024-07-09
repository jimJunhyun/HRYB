using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUI : MonoBehaviour
{
	private AudioSetting audioSetting;
	private GraphicSetting graphicSetting;

	private ISettings currentSettingCategory;

	private void Awake()
	{
		if (audioSetting == null)
		{
			audioSetting = transform.Find("Horizontal/Center Section/Frames/Audio Frame").GetComponent<AudioSetting>();
		}
		if (graphicSetting == null)
		{
			graphicSetting = transform.Find("Horizontal/Center Section/Frames/Graphic Frame").GetComponent<GraphicSetting>();
		}

		if(currentSettingCategory != null)
		{
			if (currentSettingCategory.NotSaved)
			{
				currentSettingCategory.Revert();
			}
			CloseCategory();
		}

		currentSettingCategory = audioSetting;
		currentSettingCategory.Open();
	}

	public void CloseCategory()
	{
		if (currentSettingCategory.NotSaved)
		{
			currentSettingCategory.Revert();
		}

		currentSettingCategory.Close();
	}

	public void ChangeToAudio()
	{
		if(currentSettingCategory != (ISettings)audioSetting)
		{
			CloseCategory();

			currentSettingCategory = audioSetting;
			currentSettingCategory.Open();
		}
	}

	public void ChangeToGraphic()
	{
		if (currentSettingCategory != (ISettings)graphicSetting)
		{
			CloseCategory();

			currentSettingCategory = graphicSetting;
			currentSettingCategory.Open();
		}
	}
}
