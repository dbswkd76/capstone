﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject menu;
    bool pause = false;
    
    void Update()
    {
        if (Input.GetButtonDown("MainMenu"))
            {
                pause=!pause;
            }

            if(pause)
            {
                Debug.Log("메뉴ON");
                menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true; 
                Time.timeScale = 0;
            }

            if(!pause)
            {
                menu.SetActive(false);
                Time.timeScale = 1;
            }

        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("게임종료");
            Application.Quit();
        }
    }
}
