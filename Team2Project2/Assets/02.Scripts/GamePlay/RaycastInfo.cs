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

    // Raycast�� ���� 
    private void ShootRay()
    {
        isPreviousRaycastHit = isPresentRaycastHit;
        isPresentRaycastHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, rayDistance);
    }

    // ���� �浹-���浹 ���� ��ȯ Ȯ��
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

    // ������ �浹�� ������Ʈ, �� ������Ʈ�� �θ� ������Ʈ Ȯ��
    private void RaycastHitObject()
    {
        previousObject = presentObject;
        presentObject = hitInfo.transform.gameObject;

        // �θ� ������Ʈ�� �������� �ʴ� ���
        if (presentObject.transform.parent == null)
        {
            presentObjectParent = null;
        }
        // �θ� ������Ʈ�� ���������� �±׶� �������� ���� ���(�� �̵� ����X)
        else if (presentObject.transform.parent.gameObject.CompareTag("Untagged"))
        {
            presentObjectParent = presentObject.transform.parent.gameObject;
        }
        // �θ� ������Ʈ�� �����ϸ� �±ױ��� ������ ���(�� �̵� ����O)
        else
        {
            presentObject = presentObject.transform.parent.gameObject;
            //Debug.Log("Parent->Present, PresentObject tag = " + presentObject.tag);
            if (presentObject.transform.parent != null) presentObjectParent = presentObject.transform.parent.gameObject;
        }
        //Debug.Log("Present : " + presentObject + ", Parent : " + presentObjectParent);
    }

    // ������ ���ο� ������Ʈ�� �浹�ߴ��� Ȯ��
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
