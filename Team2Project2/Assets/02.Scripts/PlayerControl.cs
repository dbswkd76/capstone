using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;

public class PlayerControl : MonoBehaviour
{
    private float _playerMovingSpeed = 0.0f; // Player가 움직이는 속도
    public float ApplySpeed {
        get { return _playerMovingSpeed; }
        set {
            if(_actionController.IsMovingWall){ // 벽을 옮기는 중일 때 Player의 속도를 느리게 한다
                _playerMovingSpeed = 0.5f;
                _animator.SetFloat("speed", 0.125f);
            }
            else{
                _playerMovingSpeed = value;
            }
        }
    }

    // Speed
    [SerializeField]
    private float _walkSpeed;
    [SerializeField]
    private float _runSpeed;
    [SerializeField]
    private float _crouchSpeed;

    // JumpForce
    [SerializeField]
    private float _jumpForece;

    // Player State
    private bool _isGround = true;
    private bool _isWalk = false;
    private bool _isRun = false;
    private bool _isCrouch = false;
    public bool IsCrouchToNav; // 플레이어가 앉아있는지 NPC에게 알려주는 변수

    // Collider
    private CapsuleCollider _capsuleCollider; // 플레이어 외부 충돌 감지

    // PosY for Camera
    [SerializeField]
    private float _crouchPosY;
    private float _originPosY;
    private float _applyCrouchPosY; // Now Camera PosY

    // _cameraRotationSensitivity for Camera
    [SerializeField]
    private float _cameraRotationSensitivity; 

    // rotationLimit for Camera
    [SerializeField]
    private float _cameraRotationLimit;
    private float _currentCameraRotationX = 0;

    [SerializeField]
    private Camera _camera; // 플레이어에 부착된 카메라
    [SerializeField]
    private ActionController _actionController;

    private SoundManager _soundManager;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private GameObject _playerAvata;

    //for NPC Zoom in/out effect
    private GameObject _npc; // 공격받았을 때 카메라가 zoom in/out할 NPC
    private GameObject _npcEye; // 공격받았을 때 카메라가 zoom in/out할 NPC의 눈
    private float _turnSmoothVelocity;
    [Range(0.01f, 2f)] public float TurnSmoothTime;

    //NPC nearby effect
    public AnalogGlitch GlitchEffect; // NPC가 가까이 있을 때 화면에 노이즈 효과를 준다
    private readonly float _glitchIntensity = 0.5f; // 노이즈의 강도

    void Start()
    {
        _soundManager = SoundManager.Instance;
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerAvata = transform.Find("LowPolyHuman").gameObject;
        _animator = _playerAvata.GetComponent<Animator>();
        _animator.SetFloat("speed", 1f);
        ApplySpeed = _walkSpeed;
        _originPosY = _camera.transform.localPosition.y;
        _applyCrouchPosY = _originPosY;
    }

    // 플레이어의 이동이 프레임마다 불안정하게 움직이는 것을 방지하기 위해 FixedUpdate()에서 이동을 처리한다
    void FixedUpdate()
    {
        if(!GameManager.isAttacked){
            Move();
        }
    }

    // Called once per frame
    void Update()
    {   
        _camera.fieldOfView = 60f;
        if (GameManager.canPlayerMove)
        {
            IsGround();
            TryJump();
            TryRun();
            TryCrouch();
            if (!Inventory.inventoryActivated) // 인벤토리가 활성화 되어있을 때 카메라가 움직이지 않도록 한다
            {
                CameraRotation();
                CharacterRotation();
            }
        }
        else{   //isAttacked
            IsAttackedFov();
        }
    }
   
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch() // 앉기
    {
        _isCrouch = !_isCrouch;

        if (_isCrouch)
        {
            _animator.SetBool("isCrouch", true);
            _soundManager.PlayerFootstepPlayer.mute = true;
            ApplySpeed = _crouchSpeed;
            _applyCrouchPosY = _crouchPosY;
        }
        else
        {
            _animator.SetBool("isCrouch", false);
            _soundManager.PlayerFootstepPlayer.mute = false;
            ApplySpeed = _walkSpeed;
            _animator.SetFloat("speed", 1f);
            _applyCrouchPosY = _originPosY;
        }
        IsCrouchToNav = _isCrouch;   //NPC 처리용
        StartCoroutine(CrouchCoroutine()); // 부드럽게 앉고 일어나는 동작 처리
    }

    IEnumerator CrouchCoroutine() // 부드럽게 앉고 일어나는 동작 처리
    {
        float _posY = _camera.transform.localPosition.y;
        int count = 0;

        while (_posY != _applyCrouchPosY) // 카메라의 위치를 부드럽게 변경한다.
        {
            count++;
            _posY = Mathf.Lerp(_posY, _applyCrouchPosY, 0.3f);
            _camera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;
        }
        _camera.transform.localPosition = new Vector3(0, _applyCrouchPosY, 0f);
    }

    private void IsGround()
    {
        _isGround = Physics.Raycast(transform.position, Vector3.down, _capsuleCollider.bounds.extents.y + 0.1f);
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (_isCrouch)
            Crouch();
        _animator.SetTrigger("jump");
        _soundManager.PlaySound(_soundManager.SfxBasicPlayers, _soundManager.SfxBasics, "PlayerJump");
        _rigidbody.velocity = transform.up * _jumpForece;
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();

        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        if (_isCrouch)
            Crouch();

        _isRun = true;
        ApplySpeed = _runSpeed;
        _animator.SetFloat("speed", _runSpeed / _walkSpeed);
    }

    private void RunningCancel()
    {
        _isRun = false;
        ApplySpeed = _walkSpeed;
        _animator.SetFloat("speed", 1f);
    }

    private void Move()
    {

        float _moveDirX = Input.GetAxis("Horizontal");
        float _moveDirZ = Input.GetAxis("Vertical");

        _isWalk = (_moveDirZ != 0 || _moveDirX != 0) ? true : false; // 이동하고 있음을 판별

        _animator.SetBool("isWalk", _isWalk);
        _animator.SetFloat("horizontal", _moveDirX);
        _animator.SetFloat("vertical", _moveDirZ);

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * ApplySpeed;

        _rigidbody.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * _cameraRotationSensitivity;
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(_characterRotationY));
    }

    private void CameraRotation()

    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * _cameraRotationSensitivity;
        _currentCameraRotationX -= _cameraRotationX;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -_cameraRotationLimit, _cameraRotationLimit);

        _camera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
    }
    
    public void IsAttackedFov(){
        Debug.Log("force to see NPC");
        //GameManager.canPlayerMove = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        foreach(var colider in colliders){
            if(colider.tag == "NPC"){
                _npc = colider.gameObject;
                break;
            }
        }
        Debug.Log("npc name: " + _npc.name);

        _npcEye = _npc.transform.GetChild(0).gameObject;
        _camera.fieldOfView = 30f;
        var lookRotation = Quaternion.LookRotation(_npc.transform.position - transform.position);
        var targetAngleX = lookRotation.eulerAngles.x;
        var targetAngleY = lookRotation.eulerAngles.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref _turnSmoothVelocity, TurnSmoothTime);
        _camera.transform.LookAt(_npcEye.transform);
    }
    private void OnTriggerStay(Collider collider){
        if(collider.tag == "NPC"){
            Vector3 distance = transform.position - collider.transform.position;
            GlitchEffect.scanLineJitter = _glitchIntensity / distance.magnitude;
        }
    }
    private void OnTriggerExit(Collider collider){
        if(collider.tag == "NPC"){
            GlitchEffect.scanLineJitter = 0f;
        }
    }
}