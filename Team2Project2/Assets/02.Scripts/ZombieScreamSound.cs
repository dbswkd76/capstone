using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScreamSound : MonoBehaviour
{
    [SerializeField]
    private SoundManager theSoundManager;

    // Start is called before the first frame update
    void Start()
    {
        theSoundManager = SoundManager.instance;
        Invoke("ZombieScream", 4.0f);
    }

    public void ZombieScream()
    {
        theSoundManager.PlayZombieScream();
        //Debug.Log("비명 지른다!");
        Invoke("ZombieScream", Random.Range(4, 8));
    }
}
