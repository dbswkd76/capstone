using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer_Pull_Z: MonoBehaviour {

	public bool open;
	public Animator pull;
	public Transform Player;

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
					if (open == false) {
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
		yield return new WaitForSeconds (.5f);
	}

	IEnumerator closing(){
		//print ("you are closing the door");
		pull.Play ("closepush");
		open = false;
		yield return new WaitForSeconds (.5f);
	}


}

