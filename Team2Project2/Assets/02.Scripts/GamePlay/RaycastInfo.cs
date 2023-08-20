using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInfo : MonoBehaviour
{
    public bool isPreviousRaycastHit;
    public bool isPresentRaycastHit;

    public bool isRaycastHitChanged;
    public bool isRaycastHitObjectChanged;

    public GameObject previousObject;
    public GameObject presentObject;
    public GameObject presentObjectParent;
    
    public RaycastHit hitInfo;

    [SerializeField]
    private float rayDistance;

    // Start is called before the first frame update
    void Start()
    {
        isPresentRaycastHit = false;
        isPreviousRaycastHit = false;
        isRaycastHitChanged = false;
        isRaycastHitObjectChanged = false;

        rayDistance = 3.0f;

        previousObject = null;
        presentObject = null;
        presentObjectParent = null;
    }

    // Update is called once per frame
    void Update()
    {
        ShootRay();
        CheckRaycastHitChanged();
        if (isPresentRaycastHit)
        {
            RaycastHitObject();
            CheckRaycastHitObjectChanged();
        }
    }

    // Raycast를 진행 
    private void ShootRay()
    {
        isPreviousRaycastHit = isPresentRaycastHit;
        isPresentRaycastHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, rayDistance);
    }

    // 광선 충돌-비충돌 상태 전환 확인
    private void CheckRaycastHitChanged()
    {
        if (isPreviousRaycastHit != isPresentRaycastHit)
        {
            isRaycastHitChanged = true;   
        }
        else
        {
            isRaycastHitChanged = false;
        }
    }

    // 광선과 충돌한 오브젝트, 그 오브젝트의 부모 오브젝트 확인
    private void RaycastHitObject()
    {
        previousObject = presentObject;
        presentObject = hitInfo.transform.gameObject;

        // 부모 오브젝트가 존재하지 않는 경우
        if (presentObject.transform.parent == null)
        {
            presentObjectParent = null;
        }
        // 부모 오브젝트는 존재하지만 태그라 지정되지 않은 경우(벽 이동 관련X)
        else if (presentObject.transform.parent.gameObject.CompareTag("Untagged"))
        {
            presentObjectParent = presentObject.transform.parent.gameObject;
        }
        // 부모 오브젝트가 존재하며 태그까지 지정된 경우(벽 이동 관련O)
        else
        {
            presentObject = presentObject.transform.parent.gameObject;
            //Debug.Log("Parent->Present, PresentObject tag = " + presentObject.tag);
            if (presentObject.transform.parent != null) presentObjectParent = presentObject.transform.parent.gameObject;
        }
        //Debug.Log("Present : " + presentObject + ", Parent : " + presentObjectParent);
    }

    // 광선이 새로운 오브젝트와 충돌했는지 확인
    private void CheckRaycastHitObjectChanged()
    {
        if (previousObject != presentObject)
        {
            isRaycastHitObjectChanged = true;
        }
        else
        {
            isRaycastHitObjectChanged = false;
        }
    }
}
