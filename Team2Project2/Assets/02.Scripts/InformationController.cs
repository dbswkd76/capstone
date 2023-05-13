using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine.UI;
using UnityEngine;

public class InformationController : MonoBehaviour
{
    private bool isInfoControl;

    [SerializeField]
    private Text actionText;
    [SerializeField]
    private RaycastInfo raycastInfo;
    [SerializeField]
    private ActionController actionController;

    void Start()
    {
        isInfoControl = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (raycastInfo.isRaycastHitChanged)
        {
            // 광선 충돌X -> 충돌O 변경되었을 때
            if (raycastInfo.isPresentRaycastHit)
            {
                isInfoControl = true;
                Debug.Log("광선 충돌X -> 광선 충돌O");
                Debug.Log("광선 충돌 물체가 변경됨. Previous = " + raycastInfo.previousObject + ", Present = " + raycastInfo.presentObject);
                InfoControl();
            }
            // 광선 충돌O -> 충돌X 변경되었을 때
            else
            {
                isInfoControl = false;
                InfoDisappear();
                if (raycastInfo.previousObject.GetComponent<Outline>() != null)
                {
                    OutlineDisappear(raycastInfo.previousObject);
                }
                raycastInfo.previousObject = null;

                Debug.Log("광선 충돌O -> 광선 충돌X");
                Debug.Log("정보 UI, 외곽선, previousObject 초기화");
            }
        }

        // 광선이 새로 충돌하거나, 충돌중인 상태에서 물체가 변경되었을 때 호출
        if (isInfoControl && raycastInfo.isRaycastHitObjectChanged)
        {
            Debug.Log("광선 충돌 물체가 변경됨. Previous = " + raycastInfo.previousObject + ", Present = " + raycastInfo.presentObject);
            InfoControl();
        }
    }

    // 정보 UI 및 외곽선 제어
    private void InfoControl()
    {
        // 정보 UI 비활성화 및 이전 오브젝트 외곽선 제거
        InfoDisappear();
        if (raycastInfo.previousObject != null && raycastInfo.previousObject.GetComponent<Outline>() != null)
        {
            OutlineDisappear(raycastInfo.previousObject);
        }

        // 태그에 따라서 정보 UI 및 외곽선 제어
        switch (raycastInfo.presentObject.tag)
        {
            case "Item":
                InfoAppear(raycastInfo.presentObject.GetComponent<ItemPickUp>().item.itemName + "  획득" + "<color=yellow>" + " (E)" + "</color>");
                OutlineAppear(raycastInfo.presentObject, Color.yellow, 10.0f);
                break;

            case "MovingWall":
                if (actionController.isMoveWallActivated)
                {
                    InfoAppear("옮기기" + "<color=yellow>" + " (R)" + "</color>");
                    OutlineAppear(raycastInfo.presentObject, Color.green, 10.0f);
                }
                break;

            case "FixedWall":
                if (actionController.isMoveWallActivated)
                {
                    InfoAppear("<color=red>" + "이동 불가" + "</color>");
                    OutlineAppear(raycastInfo.presentObject, Color.red, 10.0f);
                }
                break;

            case "MovingTemp":
                if (actionController.isMovingWall)
                {
                    InfoAppear("내려놓기");
                    OutlineAppear(raycastInfo.presentObject, Color.red, 10.0f);
                }
                break;

            case "Player":
                break;

            default:
                break;
        }
    }

    // 정보 UI 활성화
    private void InfoAppear(string txt)
    {
        actionText.text = txt;
        actionText.gameObject.SetActive(true);
    }

    // 정보 UI 비활성화
    private void InfoDisappear()
    {
        actionText.text = "";
        actionText.gameObject.SetActive(false);
    }

    // 외곽선 활성화 - Outline 컴포넌트 부착
    private void OutlineAppear(GameObject outlineObject, Color color, float width)
    {
        outlineObject.AddComponent<Outline>();
        outlineObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
        outlineObject.GetComponent<Outline>().OutlineColor = color;
        outlineObject.GetComponent<Outline>().OutlineWidth = width;
    }

    // 외곽선 비활성화 - Outline 컴포넌트 파괴
    private void OutlineDisappear(GameObject outlineObject)
    {
        Destroy(outlineObject.GetComponent<Outline>());
    }
}