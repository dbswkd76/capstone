using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionController : MonoBehaviour
{
    private float previousSpeed;
    private RaycastHit hitInfo;

    private GameObject selectedWall;
    private GameObject selectedWallParent;

    private GameObject presentObject;
    private GameObject previousObject;

    private bool isPresentRayCastHit;
    private bool isPreviousRayCastHit;
    
    private bool isMovingWall;
    private bool isMoveWallActivated;

    [SerializeField]
    private float rayDistance;

    [SerializeField]
    private Text actionText;

    [SerializeField]
    private Inventory theInventory;

    [SerializeField]
    private PlayerControl thePlayerControl;


    private void Start()
    {
        rayDistance = 3.0f;
        isPresentRayCastHit = false;
        isPreviousRayCastHit = false;
        isMovingWall = false;
        isMoveWallActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTag();
        CheckKeyInput();
        InfoController();
    }

    // CheckTag by RayCast, set isPresentRayCastHit and hitInfo (LayerMask deleted)
    private void CheckTag()
    {
        isPresentRayCastHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, rayDistance);
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
        // RayCast에 어떠한 물체가 감지된 경우
        if (isPresentRayCastHit) 
        {
            presentObject = hitInfo.transform.gameObject;

            // RayCast가 닿은 게임 오브젝트가 달라졌을 때만 실행
            if (presentObject != previousObject && previousObject != null) 
            {
                Debug.Log(previousObject.name + " -> " + presentObject.name);
                switch (presentObject.tag)
                {
                    case "Item":
                        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "  획득" + "<color=yellow>" + " (E)" + "</color>";
                        InfoAppear();
                        OutlineAppear(presentObject, Color.yellow, 10.0f);
                        break;

                    case "MovingWall":
                        if (isMoveWallActivated)
                        {
                            actionText.text = "옮기기" + "<color=yellow>" + " (R)" + "</color>";
                            InfoAppear();
                            OutlineAppear(presentObject, Color.green, 10.0f);
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
                            OutlineAppear(presentObject, Color.red, 10.0f);
                        }
                        else
                        {
                            InfoDisappear();
                        }
                        break;
                    case "Player":
                        OutlineAppear(presentObject, Color.green, 10.0f);
                        break;

                    default:
                        InfoDisappear();
                        break;
                }

                OutlineDisappear(previousObject);
            }
            previousObject = presentObject;
            isPreviousRayCastHit = isPresentRayCastHit;
        }

        if (isPreviousRayCastHit ^ isPresentRayCastHit)
        {
            // isPresentRayCastHit true -> false
            InfoDisappear();
            OutlineDisappear(previousObject);
            isPreviousRayCastHit = false;
            Debug.Log("previousObjet is Null");
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

            // 이동속도 제약
            Debug.Log("applySpeed = " + thePlayerControl.applySpeed);
            previousSpeed = thePlayerControl.applySpeed;
            thePlayerControl.applySpeed = 0.5f;

            // RayCast가 충돌한 게임 오브젝트 가져오기
            selectedWall = hitInfo.transform.gameObject;

            // 오브젝트 경로를 부모->스크립트 부착 오브젝트로 이동.
            selectedWallParent = selectedWall.transform.parent.gameObject;
            selectedWall.transform.SetParent(this.transform); // this = Player
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

            // 속도 정상화
            thePlayerControl.applySpeed = previousSpeed;

            isMovingWall = false;
        }
    }

    private void OutlineAppear(GameObject outlineObject, Color color, float width)
    {
        outlineObject.AddComponent<Outline>();
        outlineObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
        outlineObject.GetComponent<Outline>().OutlineColor = color;
        outlineObject.GetComponent<Outline>().OutlineWidth = width;
    }

    private void OutlineDisappear(GameObject outlineObject)
    {
        Destroy(outlineObject.GetComponent<Outline>());
    }
}
