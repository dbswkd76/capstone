using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;

public class PlayerControl : MonoBehaviour
{
    // Player Speed - 벽 들고 달리는 현상 방지
    private float playerSpeed = 0.0f;
    public float applySpeed {
        get { return playerSpeed; }
        set {
            playerSpeed = theActionController.isMovingWall ? 0.5f : value;
        }
    }

    // Speed
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;

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

    private Rigidbody myRigid;
    private Animator anim;
    private GameObject lowPolyHuman;

    //for NPC Zoom in/out effect
    private Camera playerCamera;
    private float playerFov;
    private GameObject npc;
    private float turnSmoothVelocity;
    [Range(0.01f, 2f)] public float turnSmoothTime;

    //NPC nearby effect
    public AnalogGlitch glitchEffect;
    private float intensity = 0.5f;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        lowPolyHuman = transform.Find("LowPolyHuman").gameObject;
        anim = lowPolyHuman.GetComponent<Animator>();
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

        playerFov = 0f;
    }

    // 0.02초마다 한 번씩 실행
    void FixedUpdate()
    {
        if(!GameManager.isAttacked){
            Move();
        }
    }

    // Called once per frame
    void Update()
    {   
        theCamera.fieldOfView = 60f;
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
        else{   //isAttacked
            isAttackedFov();
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
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            anim.SetBool("isCrouch", false);
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
    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
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
    
    public void isAttackedFov(){
        Debug.Log("force to see NPC");
        //GameManager.canPlayerMove = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        foreach(var colider in colliders){
            if(colider.tag == "NPC"){
                npc = colider.gameObject;
                break;
            }
        }
        Debug.Log("npc name: " + npc.name);

        theCamera.fieldOfView = 30f;
        var lookRotation = Quaternion.LookRotation(npc.transform.position - transform.position);
        var targetAngleY = lookRotation.eulerAngles.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
        theCamera.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
    }
    private void OnTriggerStay(Collider collider){
        if(collider.tag == "NPC"){
            Vector3 distance = transform.position - collider.transform.position;
            glitchEffect.scanLineJitter = intensity / distance.magnitude;
        }
    }
    private void OnTriggerExit(Collider collider){
        if(collider.tag == "NPC"){
            glitchEffect.scanLineJitter = 0f;
        }
    }
}