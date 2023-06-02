using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
public class ActionController : MonoBehaviour
{
    public bool isMovingWall;
    public bool isMoveWallActivated;
 
    private Transform[] selectedWallChildren;

    private float previousSpeed;
    private GameObject selectedWall;
    private GameObject selectedWallParent;
    private GameObject tempWallStorage;

    [SerializeField]
    private GameObject hammer; // 플레이어가 들고있는 해머
    [SerializeField]
    private RaycastInfo raycastInfo;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    private PlayerControl thePlayerControl;

    private SoundManager theSoundManager;
    private Hammer theHammer;

    private void Start()
    {
        isMovingWall = false;
        isMoveWallActivated = false;

        selectedWall = null;
        selectedWallParent = null;
        tempWallStorage = GameObject.FindWithTag("MovingTemp");

        theSoundManager = SoundManager.instance;
        theHammer = hammer.GetComponent<Hammer>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckKeyInput();
    }


    // 사용자 입력 확인
    private void CheckKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("E key Pressed");
            ItemPickUp();
            HammerPickUp();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log("R key Pressed");
            PutUpWall();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            //Debug.Log("R key Released");
            PutDownWall();
        }
    }


    // ItemPickUp
    private void ItemPickUp()
    {
        if (raycastInfo.presentObject.CompareTag("Item"))
        {
            //Debug.Log(raycastInfo.presentObject.GetComponent<ItemPickUp>().item.itemName + "획득했습니다");
            theSoundManager.PlaySound(theSoundManager.sfxPlayer, theSoundManager.sfx, "PickUpItem");
            theInventory.AcquireItem(raycastInfo.presentObject.GetComponent<ItemPickUp>().item);
            Destroy(raycastInfo.presentObject);
        }
    }

    // HammePickUp
    private void HammerPickUp()
    {
        if (raycastInfo.presentObject.CompareTag("Hammer"))
        {
            theSoundManager.PlaySound(theSoundManager.sfxPlayer, theSoundManager.sfx, "PickUpItem");
            hammer.SetActive(true);
            MoveWallActive();
            Destroy(raycastInfo.presentObject);
        }
    }

    // 사용 횟수를 초과한 해머 오브젝트 파괴
    private void CheckAndDestroyHammer()
    {
        --theHammer.useCount;
        if(theHammer.useCount <= 0)
        {
            MoveWallActive();
            theSoundManager.PlaySound(theSoundManager.sfxPlayer, theSoundManager.sfx, "DestroyHammer");
            Destroy(hammer);
        }
    }

    // 벽 옮기기 기능 활성화/비활성화
    private void MoveWallActive()
    {
        isMoveWallActivated = !isMoveWallActivated;
        //Debug.Log("isMoveWallActivated : " + isMoveWallActivated);
    }


    private void PutUpWall()
    {
        //Debug.Log("문 들어올림 시도");

        if (isMoveWallActivated)
        {
            //Debug.Log("present obj=" + raycastInfo.presentObject + ", tag=" + raycastInfo.presentObject.tag);
            // 현재 오브젝트 들어올리기
            if (raycastInfo.presentObject.CompareTag("MovingWall"))
            {
                isMovingWall = true;
                theHammer.HammerUP();

                theSoundManager.PlaySound(theSoundManager.sfxPlayer, theSoundManager.sfx, "PutUpWall");

                previousSpeed = thePlayerControl.applySpeed;
                thePlayerControl.applySpeed = 0.5f;

                selectedWall = raycastInfo.presentObject;
                selectedWallParent = raycastInfo.presentObjectParent;

                selectedWallChildren = selectedWall.GetComponentsInChildren<Transform>();
                foreach (Transform child in selectedWallChildren)
                {
                    //Debug.Log("selectedWall의 자식 : " + child.gameObject.name);
                    if (child.GetComponent<Collider>() == null)
                    {
                        child.gameObject.AddComponent<BoxCollider>();
                    }
                    child.GetComponent<Collider>().isTrigger = true;
                }

                selectedWall.transform.SetParent(tempWallStorage.transform);
                //Debug.Log("문 들어올림 성공");
            }
        }
    }

    private void PutDownWall()
    {
        //Debug.Log("문 내림 시도");

        if (isMovingWall)
        {
            theHammer.HammerDown();

            theSoundManager.PlaySound(theSoundManager.sfxPlayer, theSoundManager.sfx, "PutDownWall");

            selectedWall.transform.SetParent(selectedWallParent.transform);

            foreach (Transform child in selectedWallChildren)
            {
                child.GetComponent<Collider>().isTrigger = false;
            }

            CheckAndDestroyHammer();
            isMovingWall = false;
            thePlayerControl.applySpeed = previousSpeed;
            //Debug.Log("문 내림 성공");
        }
    }
}
