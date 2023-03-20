using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 병실trigger : MonoBehaviour
{
    public DeathCount deathcount;
    public Clock clock;
    public int threeclock = 0;
    public void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.name == "Player")
        {
            if (clock.hour == 3 && clock.minutes == 0)
            {
                threeclock++;
                Debug.Log("세이프");
            }
        }   
    }
    // Update is called once per frame
    void Update()
    {
        deaththree();
    }

    private void deaththree()
    {
        if (clock.hour == 3 && clock.minutes == 1)
        {
            if (threeclock == 0)
            {
                deathcount.deathcount++;
                deathcount.gotohell();
                threeclock++;
            }
        }
    }
        
}
