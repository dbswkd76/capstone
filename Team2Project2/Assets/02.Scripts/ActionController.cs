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
    private RaycastInfo raycastInfo;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    private PlayerControl thePlayerControl;


    private void Start()
    {
        isMovingWall = false;
        isMoveWallActivated = false;

        selectedWall = null;
        selectedWallParent = null;
        tempWallStorage = GameObject.FindWithTag("MovingTemp");
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
            Debug.Log("E key Pressed");
            ItemPickUp();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T key Pressed");
            MoveWallActive();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key Pressed");
            PutUpWall();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            Debug.Log("R key Released");
            PutDownWall();
        }
    }


    // ItemPickUp
    private void ItemPickUp()
    {
        if (raycastInfo.presentObject.CompareTag("Item"))
        {
            Debug.Log(raycastInfo.presentObject.GetComponent<ItemPickUp>().item.itemName + "획득했습니다");
            theInventory.AcquireItem(raycastInfo.presentObject.GetComponent<ItemPickUp>().item);
            Destroy(raycastInfo.presentObject);
        }
    }


    // T 버튼을 사용하여 벽 옮기기 기능 활성화/비활성화
    private void MoveWallActive()
    {
        isMoveWallActivated = !isMoveWallActivated;
        Debug.Log("isMoveWallActivated : " + isMoveWallActivated);
    }


    private void PutUpWall()
    {
        Debug.Log("문 들어올림 시도");

        if (isMoveWallActivated)
        {
            Debug.Log("present obj=" + raycastInfo.presentObject + ", tag=" + raycastInfo.presentObject.tag);
            // 현재 오브젝트 들어올리기
            if (raycastInfo.presentObject.CompareTag("MovingWall"))
            {
                isMovingWall = true;

                previousSpeed = thePlayerControl.applySpeed;
                thePlayerControl.applySpeed = 0.5f;

                selectedWall = raycastInfo.presentObject;
                selectedWallParent = raycastInfo.presentObjectParent;

                selectedWallChildren = selectedWall.GetComponentsInChildren<Transform>();
                foreach(Transform child in selectedWallChildren)
                {
                    Debug.Log("selectedWall의 자식 : " + child.gameObject.name);
                    if(child.GetComponent<Collider>() == null)
                    {
                        child.gameObject.AddComponent<BoxCollider>();
                    }
                    child.GetComponent<Collider>().isTrigger = true;
                }

                selectedWall.transform.SetParent(tempWallStorage.transform);
                Debug.Log("문 들어올림 성공");
            }
        }
    }

    private void PutDownWall()
    {
        Debug.Log("문 내림 시도");

        if (isMovingWall)
        {
            selectedWall.transform.SetParent(selectedWallParent.transform);
            
            foreach (Transform child in selectedWallChildren)
            {
                child.GetComponent<Collider>().isTrigger = false;
            }

            isMovingWall = false;
            thePlayerControl.applySpeed = previousSpeed;
            Debug.Log("문 내림 성공");
        }
    }
}
