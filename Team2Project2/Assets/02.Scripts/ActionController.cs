using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionController : MonoBehaviour
{
    private RaycastHit hitInfo;
    private GameObject selectedWall;
    private GameObject selectedWallParent;

    private bool isRayCastHit = false;
    private bool isMovingWall = false;
    private bool isMoveWallActivated = true;
    
    [SerializeField]
    private float rayDistance;

    [SerializeField]
    private Text actionText;

    [SerializeField]
    private Inventory theInventory;


    // Update is called once per frame
    void Update()
    {
        CheckLayer();
        CheckKeyInput();
        InfoController();
    }

    // CheckLayer by RayCast, set isRayCastHit and hitInfo (LayerMask deleted)
    private void CheckLayer()
    {
        isRayCastHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, rayDistance);
    }


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


    private void InfoController()
    {
        if (isRayCastHit)
        {
            switch (hitInfo.transform.tag)
            {
                case "Item":
                    actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "  획득" + "<color=yellow>" + " (E)" + "</color>";
                    InfoAppear();
                    break;
                case "MovingWall":
                    if (isMoveWallActivated)
                    {
                        actionText.text = "옮기기" + "<color=yellow>" + " (R)" + "</color>";
                        InfoAppear();
                    }
                    else
                    {
                        InfoDisappear();
                    }
                    break;
                case "FixedWall":
                    if (isMoveWallActivated)
                    {
                        actionText.text = "<color=red>" + "이동 불가" + "</color>";
                        InfoAppear();
                    }
                    else
                    {
                        InfoDisappear();
                    }
                    break;
                default:
                    InfoDisappear();
                    break;
            }
        }
    }

    private void InfoAppear()
    {
        actionText.gameObject.SetActive(true);
    }


    private void InfoDisappear()
    {
        actionText.text = "";
        actionText.gameObject.SetActive(false);
    }


    // ItemPickUp
    private void ItemPickUp()
    {
        if (hitInfo.transform.tag == "Item")
        {
            Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "획득했습니다");
            theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
            Destroy(hitInfo.transform.gameObject);
        }
    }
    
    
    // T 버튼을 사용하여 벽 옮기기 기능 활성화/비활성화
    private void MoveWallActive()
    {
        isMoveWallActivated = (isMoveWallActivated) ? false : true;
        Debug.Log("isMoveWallActivated : " + isMoveWallActivated);
    }

    
    private void PutUpWall()
    {
        Debug.Log("문 들어올림");
        
        if (isMoveWallActivated && hitInfo.transform.tag == "MovingWall")
        {
            isMovingWall = true;

            // Ray충돌 오브젝트를 가져온다.
            selectedWall = hitInfo.collider.gameObject;
            Debug.Log("선택한 벽 이름 : " + selectedWall.transform.name);

            // 오브젝트를 부모에서 여기 아래로 가져오기(마우스 회전때문에)
            selectedWallParent = selectedWall.transform.parent.gameObject;
            selectedWall.transform.SetParent(this.transform);
            Debug.Log("부모 오브젝트 이름 : " + selectedWallParent.transform.name);
        }
    }

    private void PutDownWall()
    {
        Debug.Log("문 내림");
        
        if (isMovingWall)
        {
            // 다시 이전 부모 오브젝트의 하위 오브젝트로 배치
            selectedWall.transform.SetParent(selectedWallParent.transform);
            isMovingWall = false;   
        }
    }
}
