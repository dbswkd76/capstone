﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPwOpencloseDoorL: MonoBehaviour {

    public bool open;
    public Animator openandclose;
    public Transform Player;

    private SoundManager soundManager;

    void Start()
    {
        open = false;
        soundManager = SoundManager.instance;
        Player = GameObject.FindWithTag("Player").transform;
        openandclose = GetComponent<Animator>();

        if (Player == null)
        {
            Debug.LogError("플레이어 찾지 못함.");
        }
    }

    void OnMouseOver()
    {
        {
            if (Player)
            {
                float dist = Vector3.Distance(Player.position, transform.position);
                if (dist < 3)
                {
                    if (open == false && Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(opening());
                        PlayOpenSound();

                    }
                    else if (open == true && Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(closing());
                        PlayCloseSound();
                    }
                    else { }
                }
            }
        }
    }

    private void PlayOpenSound()
    {
        switch (this.tag)
        {
            case "Furniture":
                soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "OpenCupboard");
                break;
            case "Door":
                soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "OpenDoor");
                break;
            default:
                break;
        }
    }

    private void PlayCloseSound()
    {
        switch (this.tag)
        {
            case "Furniture":
                soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "CloseCupboard");
                break;
            case "Door":
                soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "CloseDoor");
                break;
            default:
                break;
        }
    }

    IEnumerator opening()
    {
        //print ("you are opening the door");
        openandclose.Play("Opening");
        open = true;
        yield return new WaitForSeconds(.5f);
    }

    IEnumerator closing()
    {
        //print ("you are closing the door");
        openandclose.Play("Closing");
        open = false;
        yield return new WaitForSeconds(.5f);
    }
}

