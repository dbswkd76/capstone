using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyControl : MonoBehaviour
{
    public bool isLocked = true;

    private void OnDisable()
    {
        isLocked = false;
    }

}
