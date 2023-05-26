using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opencloseDoor0247 : MonoBehaviour
{
	public Animator openandclose1;
	public bool open;
	public Transform Player;
	public LockControl0247 _lock;

    [SerializeField]
    private SoundManager soundManager;

    void Start()
	{
		open = false;

		Player = GameObject.FindWithTag("Player").transform;

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
					if (open == false && _lock.isOpened == true)
					{
						if (Input.GetMouseButtonDown(0))
						{
							StartCoroutine(opening());
                            soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "OpenDoor");
                        }
						
					}
					else if (open == false && _lock.isOpened == false)
					{
                        if (Input.GetMouseButtonDown(0))
                        {
                            soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "LockedDoor");
                        }
                    }
					else
					{
						if (open == true)
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

	}

	IEnumerator opening()
	{
		//print("you are opening the door");
		openandclose1.Play("Opening 1");
		open = true;
		yield return new WaitForSeconds(.5f);
	}

	IEnumerator closing()
	{
		//print("you are closing the door");
		openandclose1.Play("Closing 1");
		open = false;
		yield return new WaitForSeconds(.5f);
	}


}

