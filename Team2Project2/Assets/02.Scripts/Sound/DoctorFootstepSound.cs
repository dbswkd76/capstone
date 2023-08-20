using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorFootstepSound : MonoBehaviour
{
    private SoundManager theSoundManager;

    // Start is called before the first frame update
    void Start()
    {
        theSoundManager = SoundManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            theSoundManager.PlayFootstep("Doctor");
        }
    }
}
