using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opencloseDoor5395 : MonoBehaviour
{

	public Animator openandclose1;
	public bool open;
	public Transform Player;
	public LockControl5395 _lock;

    private SoundManager soundManager;

    void Start()
	{
		open = false;
        soundManager = SoundManager.Instance;
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
                            soundManager.PlaySound(soundManager.SfxBasicPlayers, soundManager.SfxBasics, "OpenDoor");
                        }
						else Debug.Log("문잠김 ");
					}
					else
					{
						if (open == true)
						{
							if (Input.GetMouseButtonDown(0))
							{
								StartCoroutine(closing());
                                soundManager.PlaySound(soundManager.SfxBasicPlayers, soundManager.SfxBasics, "CloseDoor");
                            }
						}

					}

				}
			}

		}

	}

	IEnumerator opening()
	{
		print("you are opening the door");
		openandclose1.Play("Opening 1");
		open = true;
		yield return new WaitForSeconds(.5f);
	}

	IEnumerator closing()
	{
		print("you are closing the door");
		openandclose1.Play("Closing 1");
		open = false;
		yield return new WaitForSeconds(.5f);
	}


}

