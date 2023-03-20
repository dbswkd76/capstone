using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu3 : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("타이틀");
            SceneManager.LoadScene(0);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("게임종료");
            Application.Quit();
        }
    }
}

