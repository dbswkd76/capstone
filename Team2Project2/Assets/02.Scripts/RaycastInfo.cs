using UnityEngine;

public class RaycastInfo : MonoBehaviour
{
    public bool IsPreviousRaycastHit; // 이전 프레임에서 광선이 충돌했는지 여부
    public bool IsPresentRaycastHit; // 현재 프레임에서 광선이 충돌했는지 여부

    public bool IsRaycastHitChanged; // 광선 충돌-비충돌 상태 전환 여부
    public bool IsRaycastHitObjectChanged; // 광선이 새로운 오브젝트와 충돌했는지 여부

    public GameObject PreviousObject; // 이전 프레임에서 광선이 충돌한 오브젝트
    public GameObject PresentObject; // 현재 프레임에서 광선이 충돌한 오브젝트
    public GameObject PresentObjectParent; // 현재 프레임에서 광선이 충돌한 오브젝트의 부모 오브젝트
    
    public RaycastHit HitInfo; // 광선 충돌 정보

    [SerializeField]
    private float _rayDistance; // 광선의 길이

    // Start is called before the first frame update
    void Start()
    {
        IsPresentRaycastHit = false;
        IsPreviousRaycastHit = false;
        IsRaycastHitChanged = false;
        IsRaycastHitObjectChanged = false;

        _rayDistance = 3.0f;

        PreviousObject = null;
        PresentObject = null;
        PresentObjectParent = null;
    }

    // Update is called once per frame
    void Update()
    {
        ShootRay();
        CheckRaycastHitChanged();
        if (IsPresentRaycastHit) // 발사된 광선이 충돌한 경우에만 동작
        {
            RaycastHitObject();
            CheckRaycastHitObjectChanged();
        }
    }

    // Raycast를 진행 
    private void ShootRay()
    {
        IsPreviousRaycastHit = IsPresentRaycastHit;
        IsPresentRaycastHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out HitInfo, _rayDistance);
    }

    // 광선 충돌-비충돌 상태 전환 확인
    private void CheckRaycastHitChanged()
    {
        if (IsPreviousRaycastHit != IsPresentRaycastHit)
        {
            IsRaycastHitChanged = true;   
        }
        else
        {
            IsRaycastHitChanged = false;
        }
    }

    // 광선과 충돌한 오브젝트, 그 오브젝트의 부모 오브젝트 확인
    private void RaycastHitObject()
    {
        PreviousObject = PresentObject;
        PresentObject = HitInfo.transform.gameObject;

        // 부모 오브젝트가 존재하지 않는 경우
        if (PresentObject.transform.parent == null)
        {
            PresentObjectParent = null;
        }
        // 부모 오브젝트는 존재하지만 태그라 지정되지 않은 경우(벽 이동 관련X)
        else if (PresentObject.transform.parent.gameObject.CompareTag("Untagged"))
        {
            PresentObjectParent = PresentObject.transform.parent.gameObject;
        }
        // 부모 오브젝트가 존재하며 태그까지 지정된 경우(벽 이동 관련O)
        else
        {
            PresentObject = PresentObject.transform.parent.gameObject;
            //Debug.Log("Parent->Present, PresentObject tag = " + PresentObject.tag);
            if (PresentObject.transform.parent != null) PresentObjectParent = PresentObject.transform.parent.gameObject;
        }
        //Debug.Log("Present : " + PresentObject + ", Parent : " + PresentObjectParent);
    }

    // 광선이 새로운 오브젝트와 충돌했는지 확인
    private void CheckRaycastHitObjectChanged()
    {
        if (PreviousObject != PresentObject)
        {
            IsRaycastHitObjectChanged = true;
        }
        else
        {
            IsRaycastHitObjectChanged = false;
        }
    }
}
