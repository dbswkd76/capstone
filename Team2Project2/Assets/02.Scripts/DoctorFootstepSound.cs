using UnityEngine;

public class DoctorFootstepSound : MonoBehaviour
{
    private SoundManager _soundManager;

    void Start()
    {
        _soundManager = SoundManager.Instance;
    }

    // Doctor의 발이 바닥에 닿을 때 발소리를 재생한다
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            _soundManager.PlayFootstep("Doctor");
        }
    }
}
