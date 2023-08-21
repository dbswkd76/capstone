using UnityEngine;

public class ZombieScreamSound : MonoBehaviour
{
    private SoundManager _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = SoundManager.Instance;
        Invoke("ZombieScream", 4.0f);
    }

    // 좀비가 돌아다니면서 랜덤한 간격으로 소리를 낸다
    public void ZombieScream()
    {
        _soundManager.PlayZombieScream();
        Invoke("ZombieScream", Random.Range(4, 8));
    }
}
