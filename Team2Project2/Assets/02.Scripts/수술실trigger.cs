using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 수술실trigger : MonoBehaviour
{
    /*
    public Flashlight_PRO flashlight;
    private int flashlightopen;
    private int onedeath = 0;
    private int min;
    public GameObject 어딜나가;
    public GameObject 무서운수술실문;
    public DeathCount DeathCount;
    public Clock clock;

    public void Update()
    {
        if (onedeath == 1 && (clock.minutes - min) >= 2)
        {
            어딜나가.gameObject.SetActive(false);
            무서운수술실문.gameObject.SetActive(false);
            DeathCount.gotohell();
            flashlightopen = 0;
            onedeath = 0;
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if(flashlight.flashlighttrigger==true)
        {
            Debug.Log("이러면 죽는기야");
            flashlightopen ++;
            무서운수술실문 = transform.Find("무서운수술실문").gameObject;
            무서운수술실문.gameObject.SetActive(true);
        }
    }
    public void OnTriggerExit(Collider other)
    { 
        if (flashlightopen > 0 && onedeath == 0)
        {
            어딜나가 = transform.Find("어딜나가").gameObject;
            어딜나가.gameObject.SetActive(true);
            DeathCount.deathcount++;
            min = clock.minutes;
            if (min >= 58)
            {
                min = min - 60;
            }
            onedeath++;

        }
    }
    */
}
