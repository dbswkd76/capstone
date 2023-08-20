using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu2 : MonoBehaviour
{
    public void ToTitle()
    {
        Debug.Log("타이틀");
        SceneManager.LoadScene(0);
    }

    public void GameExit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
}
