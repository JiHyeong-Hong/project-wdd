using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager: SingletonMonoBehaviour<SoundManager>
{
	// [SerializeField]
 //    List<AudioSource> _audioSources;
    
    [SerializeField]
    List<AudioClip> BGMList;
    
    [SerializeField]
    List<AudioClip> Skill1EffectList;
    
    [SerializeField]
    List<AudioClip> Skill2EffectList;
    
    [SerializeField]
    List<AudioClip> ItemEffectList;
    
    [SerializeField]
    List<AudioClip> ObjectEffectList;
    
    [SerializeField]
    List<AudioClip> UIEffectList;
    
    [SerializeField]
    List<AudioClip> BossEffectList;
    
    [SerializeField]
    AudioSource BGM;
    [SerializeField]
    List<AudioSource> Skill1Effect;
    [SerializeField]
    List<AudioSource> Skill2Effect;
    [SerializeField]
    List<AudioSource> ItemEffect;
    [SerializeField]
    List<AudioSource> ObjectEffect;
    [SerializeField]
    List<AudioSource> UIEffect;
    [SerializeField]
    List<AudioSource> BossEffect;
    
    // Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    private void Start()
    {
	    BGM = gameObject.AddComponent<AudioSource>();
	    BGM.loop = true;
	    SetAudioSoruce(Skill1EffectList, Skill1Effect);
	    SetAudioSoruce(Skill2EffectList, Skill2Effect);
	    SetAudioSoruce(ItemEffectList, ItemEffect);
	    SetAudioSoruce(ObjectEffectList, ObjectEffect);
	    SetAudioSoruce(UIEffectList, UIEffect);
	    SetAudioSoruce(BossEffectList, BossEffect);
    }

    public override void Init()
    {
       
    }

    public void SetAudioSoruce(List<AudioClip> clipList, List<AudioSource> sourceList)
    {
	    for (int i = 0; i < clipList.Count; i++)
	    {
		    sourceList.Add(gameObject.AddComponent<AudioSource>());
	    }
    }
    public void Play(Define.ESoundMainType main,Define.ESoundType type, float pitch = 1.0f)
	{
		AudioClip audioClip = null;
		switch (main)
		{
			case Define.ESoundMainType.Bgm:
				audioClip = BGMList[(int)type];
				BGM.clip = audioClip;
				BGM.Play();
				break;
			case Define.ESoundMainType.Skill1:
				audioClip = Skill1EffectList[(int)type];
				Skill1Effect[(int)type].clip = audioClip;
				Skill1Effect[(int)type].Play();
				break;
			case Define.ESoundMainType.Skill2:
				audioClip = Skill2EffectList[(int)type];
				Skill2Effect[(int)type].clip = audioClip;
				Skill2Effect[(int)type].Play();
				break;
			case Define.ESoundMainType.Item:
				audioClip = ItemEffectList[(int)type];
				ItemEffect[(int)type].clip = audioClip;
				ItemEffect[(int)type].Play();
				break;
			case Define.ESoundMainType.Object:
				audioClip = ObjectEffectList[(int)type];
				ObjectEffect[(int)type].clip = audioClip;
				ObjectEffect[(int)type].Play();
				break;
			case Define.ESoundMainType.UI:
				audioClip = UIEffectList[(int)type];
				UIEffect[(int)type].clip = audioClip;
				UIEffect[(int)type].Play();
				break;
			case Define.ESoundMainType.Boss:
				audioClip = BossEffectList[(int)type];
				BossEffect[(int)type].clip = audioClip;
				BossEffect[(int)type].Play();
				break;
		}
		
  //       if (main == Define.ESoundMainType.Bgm)
  //       {
	 //        AudioSource audioSource = BGMList[(int)type];
		// 	if (audioSource.isPlaying)
		// 		audioSource.Stop();
  //
		// 	audioSource.pitch = pitch;
		// 	audioSource.Play();
		// }
		// else if (main == Define.ESoundMainType.Effect)
		// {
		// 	AudioSource audioSource = [(int)Define.ESound.Effect];
		// 	audioSource.pitch = pitch;
		// 	audioSource.PlayOneShot(audioClip);
		// }
	}

	// AudioClip GetOrAddAudioClip(string path, Define.ESound type = Define.ESound.Effect)
 //    {
	// 	if (path.Contains("Sounds/") == false)
	// 		path = $"Sounds/{path}";
 //
	// 	AudioClip audioClip = null;
 //
	// 	if (type == Define.ESound.Bgm)
	// 	{
	// 		audioClip = Managers.Resource.Load<AudioClip>(path);
	// 	}
	// 	else
	// 	{
	// 		if (_audioClips.TryGetValue(path, out audioClip) == false)
	// 		{
	// 			audioClip = Managers.Resource.Load<AudioClip>(path);
	// 			_audioClips.Add(path, audioClip);
	// 		}
	// 	}
 //
	// 	if (audioClip == null)
	// 		Debug.Log($"AudioClip Missing ! {path}");
 //
	// 	return audioClip;
 //    }
}
