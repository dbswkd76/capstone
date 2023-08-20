using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCount : MonoBehaviour
{
    public int deathcount = 0;
    public Clock clock;
    public Transform Player;
    float txt띄우는시간 = 0.0f;

    public void gotohell()
    {
        if(deathcount >0 && deathcount < 3)
        {
            Player = GameObject.FindWithTag("Player").transform;
            Player.transform.position = new Vector3(6.25f, 2.04f, 16.61f);
            if (clock.minutes < 30)
            {
                clock.minutes += 30;
            }
            else
            {
                clock.minutes -= 30;
                clock.hour++;
            }
            
            if (txt띄우는시간 < 3)
            {
                txt띄우는시간 += Time.deltaTime;
                Debug.Log("기절");
            }
            else
            {
                txt띄우는시간 = 0.0f;
            }
        }
        
        else if(deathcount == 3)
        {
            SceneManager.LoadScene("Fail");
        }

    }
}
