using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // 마스터 볼륨
    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;

    // 사운드 목록
    public Sound[] bgm = null;
    public Sound[] sfx = null; // 일반 SFX 종합 목록. 아래의 3개 목록 제외.
    public Sound[] sfxZombieScream = null; // 좀비가 걸어다니면서 내는 그로울링 소리 모음
    public Sound[] sfxZombieMove = null; // 좀비가 걸어다니면서 내는 발소리 모음
    public Sound[] sfxPlayerMove = null; // 플레이어가 걸어다니면서 내는 발소리 모음
    
    // 재생기 목록
    public AudioSource playerFootstepPlayer = null;
    public AudioSource zombieFootstepPlayer = null;
    public AudioSource[] bgmPlayer = null;
    public AudioSource[] sfxPlayer = null;
    public AudioSource[] zombieSfxPlayer = null;


    //Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        instance.PlaySound(bgmPlayer, bgm, "LightBlink");
        bgmPlayer[0].loop = true;
    }

    // 좀비와 사람 발소리 재생 - 발소리 종류가 다양해서 별도 분리
    public void PlayFootstep(string name)
    {
        if(name == "Player")
        {
            playerFootstepPlayer.clip = sfxPlayerMove[Random.Range(0, sfxPlayerMove.Length)].clip;
            playerFootstepPlayer.Play();
        }
        else if(name == "Zombie")
        {
            zombieFootstepPlayer.clip = sfxZombieMove[Random.Range(0, sfxZombieMove.Length)].clip;
            zombieFootstepPlayer.Play();
        }
        else { Debug.Log("존재하지 않는 발소리"); }
    }

    // 좀비가 걸어다니면서 내는 그로울링 재생
    public void PlayZombieScream()
    {
        for (int i = 0; i < zombieSfxPlayer.Length; i++)
        {
            if (!zombieSfxPlayer[i].isPlaying)
            {
                zombieSfxPlayer[i].clip = sfxZombieScream[Random.Range(0, sfxZombieScream.Length)].clip;
                zombieSfxPlayer[i].Play();
                return;
            }
        }
        Debug.Log("모든 오디오 플레이어가 재생중입니다.");
    }

    // 종합 효과음 재생기
    public void PlaySound(AudioSource[] _audio, Sound[] _sound, string _name)
    {
        // 입력받은 소리 목록에 해당 소리가 있는지 검사
        for (int i = 0; i < _sound.Length; i++)
        {
            if (_name == _sound[i].name)
            {
                // 비어있는 오디오 소스가 있다면 음원 재생
                for (int j = 0; j < _audio.Length; j++)
                {
                    if (!_audio[j].isPlaying)
                    {
                        _audio[j].clip = _sound[i].clip;
                        _audio[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 오디오 플레이어가 재생중입니다.");
                return;
            }
        }
        Debug.Log(_name + " 이름의 효과음이 없습니다.");
        return;
    }
}
