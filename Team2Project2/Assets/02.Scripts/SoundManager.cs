using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    // 사운드 목록
    public Sound[] SfxBasics = null; // 일반 SFX 종합 목록. 아래의 3개 목록 제외.
    public Sound[] SfxZombieScreams = null; // 좀비가 걸어다니면서 내는 그로울링 소리 모음
    public Sound[] SfxPlayerFootsteps = null; // 플레이어가 걸어다니면서 내는 발소리 모음
    public Sound[] SfxZombieFootsteps = null; // 좀비가 걸어다니면서 내는 발소리 모음
    public Sound[] SfxDoctorFootsteps = null; // Doctor가 걸어다니면서 내는 발소리 모음
    public Sound[] SfxMutantFootsteps = null; // Mutant가 걸어다니면서 내는 발소리 모음
    
    // 재생기 목록
    public AudioSource PlayerFootstepPlayer = null;
    public AudioSource ZombieFootstepPlayer = null;
    public AudioSource DoctorFootstepPlayer = null;
    public AudioSource MutantFootstepPlayer = null;
    public AudioSource[] SfxBasicPlayers = null; 
    public AudioSource[] SfxZombiePlayers = null; // 좀비 그로울링 및 기타 효과음 재생기 목록


    //Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // 좀비와 사람 발소리 재생 - 발소리 종류가 다양해서 별도 분리
    public void PlayFootstep(string name)
    {
        switch (name)
        {
            case "Player":
                PlayerFootstepPlayer.clip = SfxPlayerFootsteps[Random.Range(0, SfxPlayerFootsteps.Length)].Clip;
                PlayerFootstepPlayer.Play();
                break;
            case "Zombie":
                ZombieFootstepPlayer.clip = SfxZombieFootsteps[Random.Range(0, SfxZombieFootsteps.Length)].Clip;
                ZombieFootstepPlayer.Play();
                break;
            case "Doctor":
                DoctorFootstepPlayer.clip = SfxDoctorFootsteps[Random.Range(0, SfxDoctorFootsteps.Length)].Clip;
                DoctorFootstepPlayer.Play();
                break;
            case "Mutant":
                MutantFootstepPlayer.clip = SfxMutantFootsteps[Random.Range(0, SfxMutantFootsteps.Length)].Clip;
                MutantFootstepPlayer.Play();
                break;
            default:
                Debug.Log("존재하지 않는 발소리");
                break;
        }
    }

    // 좀비가 걸어다니면서 내는 그로울링 재생
    public void PlayZombieScream()
    {
        for (int i = 0; i < SfxZombiePlayers.Length; i++)
        {
            if (!SfxZombiePlayers[i].isPlaying)
            {
                SfxZombiePlayers[i].clip = SfxZombieScreams[Random.Range(0, SfxZombieScreams.Length)].Clip;
                SfxZombiePlayers[i].Play();
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
            if (_name == _sound[i].Name)
            {
                // 비어있는 오디오 소스가 있다면 음원 재생
                for (int j = 0; j < _audio.Length; j++)
                {
                    if (!_audio[j].isPlaying)
                    {
                        _audio[j].clip = _sound[i].Clip;
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
