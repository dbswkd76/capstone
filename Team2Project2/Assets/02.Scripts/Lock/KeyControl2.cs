using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyControl2 : MonoBehaviour
{
    public bool isLocked = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnDisable()
    {
        isLocked = false;
    }

}
