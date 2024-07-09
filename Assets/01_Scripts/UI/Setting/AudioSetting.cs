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

	private bool notSaved;
	private void Awake()
	{
		if(masterSlider == null || sfxSlider == null || environmentSlider == null)
		{
			string path = "BG/Panel/Detail/";

			masterSlider = transform.Find(path + "Master Volume Slider/Slider").GetComponent<Slider>();
			sfxSlider = transform.Find(path + "SFX Volume Slider/Slider").GetComponent<Slider>();
			environmentSlider = transform.Find(path + "Environment Volume Slider/Slider").GetComponent<Slider>();
			bgmSlider = transform.Find(path + "BGM Volume Slider/Slider").GetComponent<Slider>();
		}

		if(audioMixer == null)
		{
			audioMixer = GameManager.instance.audioPlayer.audioMixer;
		}

		Load();
	}

	private void OnEnable()
	{
		previousSet = set;
	}

	public void Save()
	{
		JsonManager<AudioSet>.SaveJson(set, fileName);
		previousSet = set;

		notSaved = false;
	}

	public void Load()
	{
		if (JsonManager<AudioSet>.LoadJson(fileName, out set))
		{
			MasterVolume = set.MasterVolume;
			SFXVolume = set.SFXVolume;
			BGMVolume = set.BGMVolume;
			EnvironmentVolume = set.EnvironmentVolume;
			CharacterVoiceVolume = set.CharacterVoiceVolume;
		}
	}

	public void Revert()
	{
		MasterVolume = previousSet.MasterVolume;
		SFXVolume = previousSet.SFXVolume;
		EnvironmentVolume = previousSet.EnvironmentVolume;
		CharacterVoiceVolume = previousSet.CharacterVoiceVolume;
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	public void Open()
	{
		gameObject.SetActive(true);
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

			notSaved = true;
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

			notSaved = true;
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

			notSaved = true;
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

			notSaved = true;
		}
	}

	public float CharacterVoiceVolume
	{
		get { return set.CharacterVoiceVolume; }
		set
		{
			set.CharacterVoiceVolume = value;

			notSaved = true;
		}
	}

	public bool NotSaved => notSaved;
}
