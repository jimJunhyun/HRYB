using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

public class AudioSetting : MonoBehaviour
{
	private AudioSet Set;

	[SerializeField]
	private AudioMixer audioMixer;
	private readonly string fileName = "AudioSetting";

	private void Awake()
	{
		if (JsonManager<AudioSet>.LoadJson(fileName, out Set))
		{
			MasterVolume = Set.MasterVolume;
			SFXVolume = Set.SFXVolume;
			BGMVolume = Set.BGMVolume;
			EnvironmentVolume = Set.EnvironmentVolume;
			CharacterVoiceVolume = Set.CharacterVoiceVolume;
		}
	}

	public void Save()
	{
		JsonManager<AudioSet>.SaveJson(Set, fileName);
	}

	public AudioMixer GetAudioMixer()
	{
		if(audioMixer == null)
		{
			audioMixer = GameManager.instance.audioPlayer.audioMixer;
		}
		return audioMixer;
	}

	public float MasterVolume
	{
		get { return Set.MasterVolume; }
		set
		{
			Set.MasterVolume = value;
		}
	}

	public float SFXVolume
	{
		get { return Set.SFXVolume; }
		set 
		{ 
			Set.SFXVolume = value; 
		}
	}

	public float BGMVolume
	{
		get { return Set.BGMVolume; }
		set
		{
			Set.BGMVolume = value;
		}
	}

	public float EnvironmentVolume
	{
		get { return Set.EnvironmentVolume; }
		set
		{
			Set.EnvironmentVolume = value;
		}
	}

	public float CharacterVoiceVolume
	{
		get { return Set.CharacterVoiceVolume; }
		set
		{
			Set.CharacterVoiceVolume = value;
		}
	}
}
