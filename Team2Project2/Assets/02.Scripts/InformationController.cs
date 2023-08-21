using UnityEngine.UI;
using UnityEngine;

public class InformationController : MonoBehaviour
{
    private bool _isInfoControl; // 정보 및 외곽선 표시를 위하여 광선이 새로 충돌하거나 충돌 오브젝트가 변경되었음을 나타내는 변수
    
    [SerializeField]
    private GameObject _hammer;
    [SerializeField]
    private Text _actionText;
    [SerializeField]
    private RaycastInfo _raycastInfo;
    [SerializeField]
    private ActionController _actionController;

    void Start()
    {
        _isInfoControl = false;
    }

    void Update()
    {
        // 광선의 충돌/미충돌 여부가 변경되었을 때
        if (_raycastInfo.IsRaycastHitChanged)
        {
            // 광선이 (충돌X -> 충돌O)로 변경되었을 때
            if (_raycastInfo.IsPresentRaycastHit)
            {
                _isInfoControl = true;
                //Debug.Log("광선 충돌X -> 광선 충돌O");
                //Debug.Log("광선 충돌 물체가 변경됨. Previous = " + _raycastInfo.PreviousObject + ", Present = " + _raycastInfo.PresentObject);
                InfoControl();
            }
            // 광선이 (충돌O -> 충돌X) 변경되었을 때
            else
            {
                _isInfoControl = false;
                InfoDisappear(); // 기존의 정보 UI 비활성화 및 이전 오브젝트 외곽선 제거
                if (_raycastInfo.PreviousObject.GetComponent<Outline>() != null)
                {
                    OutlineDisappear(_raycastInfo.PreviousObject);
                }
                _raycastInfo.PreviousObject = null;

                //Debug.Log("광선 충돌O -> 광선 충돌X");
                //Debug.Log("정보 UI, 외곽선, PreviousObject 초기화");
            }
        }

        // 광선이 새로 충돌하거나, 충돌중인 상태에서 물체가 변경되었을 때 호출
        if (_isInfoControl && _raycastInfo.IsRaycastHitObjectChanged)
        {
            //Debug.Log("광선 충돌 물체가 변경됨. Previous = " + _raycastInfo.PreviousObject + ", Present = " + _raycastInfo.PresentObject);
            InfoControl();
        }
    }


    // 정보 UI 및 외곽선 제어
    private void InfoControl()
    {
        // 정보 UI 비활성화 및 이전 오브젝트 외곽선 제거
        InfoDisappear();
        if (_raycastInfo.PreviousObject != null && _raycastInfo.PreviousObject.GetComponent<Outline>() != null)
        {
            OutlineDisappear(_raycastInfo.PreviousObject);
        }

        // 태그에 따라서 정보 UI 및 외곽선 제어
        switch (_raycastInfo.PresentObject.tag)
        {
            case "Item":
                InfoAppear(_raycastInfo.PresentObject.GetComponent<ItemPickUp>().item.itemName + "  획득" + "<color=yellow>" + " (E)" + "</color>");
                OutlineAppear(_raycastInfo.PresentObject, Color.yellow, 10.0f);
                break;

            case "MovingWall":
                if (_actionController.IsMoveWallActivated)
                {
                    InfoAppear("옮기기" + "<color=yellow>" + " (R)" + "</color>" +"\n" + "남은 횟수 " + _hammer.GetComponent<Hammer>().UseCount + "회");
                    OutlineAppear(_raycastInfo.PresentObject, Color.green, 10.0f);
                }
                break;

            case "FixedWall":
                if (_actionController.IsMoveWallActivated)
                {
                    InfoAppear("<color=red>" + "이동 불가" + "</color>");
                    OutlineAppear(_raycastInfo.PresentObject, Color.red, 10.0f);
                }
                break;

            case "MovingTemp":
                if (_actionController.IsMovingWall)
                {
                    InfoAppear("내려놓기");
                    OutlineAppear(_raycastInfo.PresentObject, Color.red, 10.0f);
                }
                break;

            case "Hammer": // 바닥에 놓여진 망치 오브젝트 상호작용
                InfoAppear("줍기" + "<color=yellow>" + " (E)" + "</color>");
                OutlineAppear(_raycastInfo.PresentObject, Color.yellow, 10.0f);
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
        _actionText.text = txt;
        _actionText.gameObject.SetActive(true);
    }

    // 정보 UI 비활성화
    private void InfoDisappear()
    {
        _actionText.text = "";
        _actionText.gameObject.SetActive(false);
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