using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opencloseDoor5: MonoBehaviour {

	public Animator openandclose1;
	public bool open;
	public Transform Player;

	public KeyControl4 keycon;
	public bool isLocked1 = true;

    private bool previousKeycon;

    [SerializeField]
    private SoundManager soundManager;

    void Start (){
		open = false;
        previousKeycon = true;

        Player = GameObject.FindWithTag("Player").transform;

		if (Player == null)
        {
			Debug.LogError("플레이어 찾지 못함.");
        }
		
	}

	void OnMouseOver (){
		{
			if (Player) {
				float dist = Vector3.Distance (Player.position, transform.position);
				if (dist < 3) {
                    if (open == false && keycon.isLocked == false)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (IsLockedChange()) soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "LockOpen");
                            StartCoroutine(opening());
                            soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "OpenDoor");
                        }
                    }
                    else if (open == false && keycon.isLocked == true)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "LockedDoor");
                        }
                    }
                    else if (open == true)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            StartCoroutine(closing());
                            soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "CloseDoor");
                        }
                    }

                }
			}

		}

	}

    private bool IsLockedChange()
    {
        if (previousKeycon == keycon.isLocked)
        {
            return false;
        }
        else
        {
            previousKeycon = false;
            return true;
        }
    }

    IEnumerator opening(){
		//print ("you are opening the door");
		openandclose1.Play ("Opening 1");
		open = true;
		yield return new WaitForSeconds (.5f);
	}

	IEnumerator closing(){
		//print ("you are closing the door");
		openandclose1.Play ("Closing 1");
		open = false;
		yield return new WaitForSeconds (.5f);
	}

	public void Unlock ()
    {
		isLocked1 = false;
    }
	
}

