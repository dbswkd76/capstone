using UnityEngine;
public class ActionController : MonoBehaviour
{
    private float previousSpeed; // 벽을 옮기기 이전의 플레이어 속도
 
    public bool IsMovingWall; // 벽을 들어올리고 있는지 여부
    public bool IsMoveWallActivated; // 벽 옮기기 기능 활성화 여부
    private Transform[] _selectedWallChildren; // 벽을 구성하는 자식 오브젝트들
    private GameObject _selectedWall; // 선택하여 마주보고 있는 벽
    private GameObject _selectedWallParent; // 벽을 놓았을 때 돌아갈 오브젝트
    private GameObject _tempWallStorage; // 벽을 들었을 때 플레이어 자식으로 잠시 옮겨지는 위치

    [SerializeField]
    private GameObject _playerHammer; // 플레이어가 들고있는 해머(기본 비활성화)
    [SerializeField]
    private RaycastInfo _raycastInfo; // 마주한 오브젝트 정보를 가져오기 위한 스크립트
    [SerializeField]
    private Inventory _inventory; // 인벤토리 스크립트
    [SerializeField]
    private PlayerControl thePlayerControl; // 벽을 옮길 때 플레이어의 이동속도를 조절하기 위한 스크립트

    private SoundManager _soundManager; 
    private Hammer _hammer; // _playerHammer의 Hammer 스크립트

    private void Start()
    {
        IsMovingWall = false;
        IsMoveWallActivated = false;

        _selectedWall = null;
        _selectedWallParent = null;
        _tempWallStorage = GameObject.FindWithTag("MovingTemp");

        _soundManager = SoundManager.Instance;
        _hammer = _playerHammer.GetComponent<Hammer>();
    }

    void Update()
    {
        CheckKeyInput();
    }


    // 사용자 입력 확인
    private void CheckKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // E key로 아이템과 상호작용
            ItemPickUp();
            HammerPickUp();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            // R key를 눌렀을 때 벽을 들어올리며
            PutUpWall();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            // R key를 놓았을 때 벽을 놓는다
            PutDownWall();
        }
    }

    private void ItemPickUp()
    {
        if (_raycastInfo.PresentObject.CompareTag("Item"))
        {
            //Debug.Log(_raycastInfo.PresentObject.GetComponent<ItemPickUp>().item.itemName + "획득했습니다");
            _soundManager.PlaySound(_soundManager.SfxBasicPlayers, _soundManager.SfxBasics, "PickUpItem");
            _inventory.AcquireItem(_raycastInfo.PresentObject.GetComponent<ItemPickUp>().item);
            Destroy(_raycastInfo.PresentObject);
        }
    }

    // HammePickUp
    private void HammerPickUp()
    {
        if (_raycastInfo.PresentObject.CompareTag("Hammer"))
        {
            _soundManager.PlaySound(_soundManager.SfxBasicPlayers, _soundManager.SfxBasics, "PickUpItem");
            _playerHammer.SetActive(true); // 플레이어 아바타의 손에 들린 해머 오브젝트 활성화
            MoveWallActive(); // 벽 옮기기 기능 활성화
            Destroy(_raycastInfo.PresentObject); // 바닥에 놓여있는 해머 오브젝트 파괴
        }
    }

    // 사용 횟수를 초과한 해머 오브젝트 파괴
    private void CheckAndDestroyHammer()
    {
        --_hammer.UseCount;
        if(_hammer.UseCount <= 0)
        {
            MoveWallActive(); 
            _soundManager.PlaySound(_soundManager.SfxBasicPlayers, _soundManager.SfxBasics, "DestroyHammer");
            Destroy(_playerHammer);
        }
    }

    // 벽 옮기기 기능 활성화/비활성화
    private void MoveWallActive()
    {
        IsMoveWallActivated = !IsMoveWallActivated;
        //Debug.Log("IsMoveWallActivated : " + IsMoveWallActivated);
    }


    private void PutUpWall()
    {
        //Debug.Log("문 들어올림 시도");

        if (IsMoveWallActivated)
        {
            //Debug.Log("present obj=" + _raycastInfo.PresentObject + ", tag=" + _raycastInfo.PresentObject.tag);
            // 마주하고 있는 오브젝트가 옮길 수 있는 벽일 때 동작
            if (_raycastInfo.PresentObject.CompareTag("MovingWall"))
            {
                IsMovingWall = true;
                _hammer.HammerUP(); // 망치 오브젝트를 들어올리는 동작 실행

                _soundManager.PlaySound(_soundManager.SfxBasicPlayers, _soundManager.SfxBasics, "PutUpWall");

                previousSpeed = thePlayerControl.ApplySpeed; // 플레이어 이동 속도변화
                thePlayerControl.ApplySpeed = 0.5f;

                _selectedWall = _raycastInfo.PresentObject;
                _selectedWallParent = _raycastInfo.PresentObjectParent;

                _selectedWallChildren = _selectedWall.GetComponentsInChildren<Transform>();  // 벽을 옮길 때 주변에 부딪히지 않기 위하여
                foreach (Transform child in _selectedWallChildren) // 선택한 벽의 모든 구성요소에 대하여 물리적 충돌을 없애고 트리거로 설정
                {
                    //Debug.Log("selectedWall의 자식 : " + child.gameObject.Name);
                    if (child.GetComponent<Collider>() == null)
                    {
                        child.gameObject.AddComponent<BoxCollider>();
                    }
                    child.GetComponent<Collider>().isTrigger = true;
                }

                _selectedWall.transform.SetParent(_tempWallStorage.transform); // 플레이어의 자식 오브젝트로 이동
                //Debug.Log("문 들어올림 성공");
            }
        }
    }

    private void PutDownWall()
    {
        //Debug.Log("문 내림 시도");

        if (IsMovingWall)
        {
            _hammer.HammerDown(); // 망치 오브젝트를 내리는 동작 실행

            _soundManager.PlaySound(_soundManager.SfxBasicPlayers, _soundManager.SfxBasics, "PutDownWall");

            _selectedWall.transform.SetParent(_selectedWallParent.transform); // 플레이어의 자식 오브젝트에서 본래의 위치로 이동

            foreach (Transform child in _selectedWallChildren) // 벽의 모든 구성요소에 대하여 물리적 충돌을 다시 설정
            {
                child.GetComponent<Collider>().isTrigger = false;
            }

            CheckAndDestroyHammer();
            IsMovingWall = false;
            thePlayerControl.ApplySpeed = previousSpeed; // 플레이어의 속도를 원래대로 돌려놓음
            //Debug.Log("문 내림 성공");
        }
    }
}
