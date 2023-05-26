using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Player Speed - 벽 들고 달리는 현상 방지
    private float playerSpeed = 0.0f;
    public float applySpeed {
        get { return playerSpeed; }
        set {
            playerSpeed = theActionController.isMovingWall ? holdWallSpeed : value;
            anim.SetFloat("speed", holdWallSpeed / walkSpeed);
        }
    }

    // Speed
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;
    [SerializeField]
    private float holdWallSpeed;

    // JumpForce
    [SerializeField]
    private float jumpForece;

    // Player State
    private bool isGround = true;
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    //private bool isInteract 
    public bool isCrouchToNav;

    // Collider
    private CapsuleCollider capsuleCollider;

    // PosY for Camera
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY; // Now Camera PosY

    // lookSensitivity for Camera
    [SerializeField]
    private float lookSensitivity;

    // rotationLimit for Camera
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;

    [SerializeField]
    private Camera theCamera;
    [SerializeField]
    private ActionController theActionController;
    

    private SoundManager theSoundManager;
    private Rigidbody myRigid;
    private Animator anim;
    private GameObject lowPolyHuman;

    
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        lowPolyHuman = transform.Find("LowPolyHuman").gameObject;
        anim = lowPolyHuman.GetComponent<Animator>();
        theSoundManager = SoundManager.instance;
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
        anim.SetFloat("speed", 1);
    }

    // 0.02초마다 한 번씩 실행
    void FixedUpdate()
    {
        Move();
    }

    // Called once per frame
    void Update()
    {
        if (GameManager.canPlayerMove)
        {
            IsGround();
            TryJump();
            TryRun();
            TryCrouch();
            //Move();
            if (!Inventory.inventoryActivated)
            {
                CameraRotation();
                CharacterRotation();
            }
        }
    }
   
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        isCrouch = !isCrouch;

        if (isCrouch)
        {
            anim.SetBool("isCrouch", true);
            theSoundManager.playerFootstepPlayer.mute = true;
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            anim.SetBool("isCrouch", false);
            theSoundManager.playerFootstepPlayer.mute = false;
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        isCrouchToNav = isCrouch;   //NPC 처리용
        StartCoroutine(CrouchCoroutine());
    }

    // For smooth camera movement
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isCrouch)
            Crouch();
        anim.SetTrigger("jump");
        theSoundManager.PlaySound(theSoundManager.sfxPlayer, theSoundManager.sfx, "PlayerJump");
        myRigid.velocity = transform.up * jumpForece;
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
        if (isCrouch)
            Crouch();

        isRun = true;
        applySpeed = runSpeed;
        anim.SetFloat("speed", runSpeed / walkSpeed);
    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
        anim.SetFloat("speed", 1);
    }

    private void Move()
    {

        float _moveDirX = Input.GetAxis("Horizontal");
        float _moveDirZ = Input.GetAxis("Vertical");
        //float _moveDirX = Input.GetAxisRaw("Horizontal");
        //float _moveDirZ = Input.GetAxisRaw("Vertical");

        isWalk = (_moveDirZ != 0 || _moveDirX != 0) ? true : false;

        anim.SetBool("isWalk", isWalk);
        anim.SetFloat("horizontal", _moveDirX);
        anim.SetFloat("vertical", _moveDirZ);

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        //myRigid.velocity = _velocity;
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    private void CameraRotation()

    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}