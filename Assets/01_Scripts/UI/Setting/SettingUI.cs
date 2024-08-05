using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ESettingCategory
{
	Audio,
	Graphics,
	None
}

public class SettingUI : MonoBehaviour
{
	private AudioSetting audioSetting;
	private GraphicSetting graphicSetting;

	private Button AudioBtn;
	private Button GraphicBtn;

	private ISettings currentSettingCategory;

	private Dictionary<ESettingCategory, ISettings> settings = new Dictionary<ESettingCategory, ISettings>();

	private void Awake()
	{
		string path = "Horizontal/Center Section/Frames";
		if (audioSetting == null)
		{
			audioSetting = transform.Find(path+"/Audio Frame").GetComponent<AudioSetting>();

			settings.Add(ESettingCategory.Audio, audioSetting);
		}
		if (graphicSetting == null)
		{
			graphicSetting = transform.Find(path +"/Graphic Frame").GetComponent<GraphicSetting>();

			settings.Add(ESettingCategory.Graphics, graphicSetting);
		}
		if(AudioBtn == null)
		{
			AudioBtn = transform.Find(path + "/Buttons/Audio Button").GetComponent<Button>();

			AudioBtn.onClick.AddListener(()=>ChangeCategory(ESettingCategory.Audio));
		}
		if(GraphicBtn == null)
		{
			GraphicBtn = transform.Find(path + "/Buttons/Graphic Button").GetComponent<Button>();

			GraphicBtn.onClick.AddListener(() => ChangeCategory(ESettingCategory.Graphics));
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

	public void ChangeCategory(ESettingCategory category)
	{
		ISettings target = settings[category];

		if(currentSettingCategory != target)
		{
			CloseCategory();

			currentSettingCategory = target;
			currentSettingCategory.Open();
		}
	}

}
