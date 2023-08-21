using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public GameObject manual;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //SoundManager.Instance.PlaySound(SoundManager.Instance.bgmPlayer, SoundManager.Instance.bgm, "Opening");
        

    }

    public void GameStart()
    {
        Debug.Log("게임시작");
        SceneManager.LoadScene(1);
    }

    public void GameManual()
    {
        if (manual.activeSelf)
            manual.SetActive(false);
        else
            manual.SetActive(true);
    }

    public void GameExit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
}
