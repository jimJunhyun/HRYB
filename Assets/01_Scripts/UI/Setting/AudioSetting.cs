using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum EAudioType
{
	Master,
	SFX,
	BGM,
	Environment,
	CharacterVoice,
	None
}

public struct AudioSet
{
	public float MasterVolume;
	public float SFXVolume;
	public float BGMVolume;
	public float EnvironmentVolume;
	public float CharacterVoiceVolume;

	public float GetVolumeOfAudioType(EAudioType audioType)
	{
		switch(audioType)
		{
			case EAudioType.Master:
				return MasterVolume;
			case EAudioType.SFX:
				return SFXVolume;
			case EAudioType.BGM:
				return BGMVolume;
			case EAudioType.Environment:
				return EnvironmentVolume;
			case EAudioType.CharacterVoice:
				return CharacterVoiceVolume;
		}
		return 0.0f;
	}
}

public class AudioSetting : MonoBehaviour, ISettings
{
	private AudioSet set;
	private AudioSet previousSet;

	[SerializeField]
	private AudioMixer audioMixer;
	private readonly string fileName = "AudioSetting";

	//UI
	private Slider masterSlider;
	private Slider sfxSlider;
	private Slider environmentSlider;
	private Slider bgmSlider;

	private Button saveBtn;

	private bool notSaved;
	private void Awake()
	{
		if(masterSlider == null || sfxSlider == null || environmentSlider == null)
		{
			string path = "BG/Panel/Detail/";

			masterSlider = transform.Find(path + "Master Volume Slider/Frame/Slider").GetComponent<Slider>();
			sfxSlider = transform.Find(path + "SFX Volume Slider/Frame/Slider").GetComponent<Slider>();
			environmentSlider = transform.Find(path + "Environment Volume Slider/Frame/Slider").GetComponent<Slider>();
			bgmSlider = transform.Find(path + "BGM Volume Slider/Frame/Slider").GetComponent<Slider>();
			saveBtn = transform.Find(path + "Save Button").GetComponent<Button>();
		}

		if(audioMixer == null)
		{
			audioMixer = GameManager.instance.audioPlayer.audioMixer;
		}

		Load();
	}

	private void OnDisable()
	{
		if(NotSaved)
		{
			Revert();
		}
	}

	private void OnEnable()
	{
		previousSet = set;
	}

	public void Save()
	{
		JsonManager<AudioSet>.SaveJson(set, fileName);
		previousSet = set;

		ApplyAllVolume(set);

		NotSaved = false;
	}

	public void Load()
	{
		if (JsonManager<AudioSet>.LoadJson(fileName, out AudioSet loadedSet))
		{
			MasterVolume = loadedSet.MasterVolume;
			SFXVolume = loadedSet.SFXVolume;
			BGMVolume = loadedSet.BGMVolume;
			EnvironmentVolume = loadedSet.EnvironmentVolume;
			CharacterVoiceVolume = loadedSet.CharacterVoiceVolume;
			previousSet = set;
			ApplyAllVolume(set);
		}

		NotSaved = false;
	}

	public void Revert()
	{
		Debug.Log($"AudioSetting | {set.BGMVolume} | {previousSet.BGMVolume}");
		MasterVolume = previousSet.MasterVolume;
		SFXVolume = previousSet.SFXVolume;
		EnvironmentVolume = previousSet.EnvironmentVolume;
		CharacterVoiceVolume = previousSet.CharacterVoiceVolume;

		ApplyAllVolume(previousSet);
		NotSaved = false;

		Debug.Log("AudioSetting Revert");
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	public void Open()
	{
		gameObject.SetActive(true);
	}

	public void ApplyVolume(EAudioType audioType, float value)
	{
		if(audioMixer != null)
		{
			float finalValue = Mathf.Lerp(-40.0f, 0.0f, value);
			if (finalValue == -40.0f) finalValue = -80.0f;
			audioMixer.SetFloat(audioType.ToString(), finalValue);
			Debug.Log($"Apply {audioType} Volume Successed");
		}
		else
		{
			Debug.LogError($"Apply {audioType} Volume Failed");
		}
	}

	public void ApplyAllVolume(AudioSet set)
	{
		for(int i = 0; i<(int)EAudioType.None; i++)
		{
			ApplyVolume((EAudioType)i, set.GetVolumeOfAudioType((EAudioType)i));
		}
	}

	public float MasterVolume
	{
		get { return set.MasterVolume; }
		set
		{
			set.MasterVolume = value;

			if(masterSlider.value != value)
			{
				masterSlider.value = value;
			}


			NotSaved = true;
		}
	}

	public float SFXVolume
	{
		get { return set.SFXVolume; }
		set 
		{ 
			set.SFXVolume = value;

			if(sfxSlider.value != value)
			{
				sfxSlider.value = value;
			}


			NotSaved = true;
		}
	}

	public float BGMVolume
	{
		get { return set.BGMVolume; }
		set
		{
			set.BGMVolume = value;

			if(bgmSlider.value != value)
			{
				bgmSlider.value = value;
			}


			NotSaved = true;
		}
	}

	public float EnvironmentVolume
	{
		get { return set.EnvironmentVolume; }
		set
		{
			set.EnvironmentVolume = value;

			if(environmentSlider.value != value)
			{
				environmentSlider.value = value;
			}


			NotSaved = true;
		}
	}

	public float CharacterVoiceVolume
	{
		get { return set.CharacterVoiceVolume; }
		set
		{
			set.CharacterVoiceVolume = value;

			NotSaved = true;
		}
	}


	public bool NotSaved
	{
		get => notSaved;
		private set
		{
				saveBtn.gameObject.SetActive(value);
				notSaved = value;
		}
	}

}
