using UnityEngine;

public class MutantFootstepSound : MonoBehaviour
{
    private SoundManager _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = SoundManager.Instance;
    }
    
    // Mutant의 발이 바닥에 닿을 때 발소리를 재생한다
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            _soundManager.PlayFootstep("Mutant");
        }
    }
}
