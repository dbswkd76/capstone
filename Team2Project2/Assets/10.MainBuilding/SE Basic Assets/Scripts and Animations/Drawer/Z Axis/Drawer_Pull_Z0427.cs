﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer_Pull_Z0427: MonoBehaviour {

	public Animator pull;
	public bool open;
	public Transform Player;
	public LockControl3211 _lock;
    
    private SoundManager soundManager;

    void Start (){
        soundManager = SoundManager.instance;
        open = false;
		Player = GameObject.FindWithTag("Player").transform;
	}

	void OnMouseOver (){
		{
			if (Player) {
				float dist = Vector3.Distance (Player.position, transform.position);
				if (dist < 3) {
                    print("object name");
					if (open == false && _lock.isOpened == true) {
						if (Input.GetMouseButtonDown (0)) {
							StartCoroutine (opening ());
                            soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "OpenDrawer");
                        }
					} else {
						if (open == true) {
							if (Input.GetMouseButtonDown (0)) {
								StartCoroutine (closing ());
                                soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "CloseDrawer");
                            }
						}

					}

				}
			}

		}

	}

	IEnumerator opening(){
		//print ("you are opening the door");
		pull.Play ("openpull");
		open = true;
		yield return new WaitForSeconds (.25f);
	}

	IEnumerator closing(){
		//print ("you are closing the door");
		pull.Play ("closepush");
		open = false;
		yield return new WaitForSeconds (.25f);
	}


}

