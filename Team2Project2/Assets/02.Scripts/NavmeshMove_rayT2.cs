using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class NavmeshMove_rayT2 : MonoBehaviour
{
    private enum State
    {
        Patrol, //추적x
        Tracking,   //추적
        AttackBegin,    //공격
        Attacking   //임시(향후 정신력게이지와 연관)
    }

    private State state;    //NPC 상태
    public LayerMask targetLayer;
    private NavMeshAgent nav;   //AI
    private StatusController target;    //타겟 플레이어
    private PlayerControl targetControl; //타겟 플레이어 모션 상태
    private Animator animator;  //NPC 애니메이터
    private SoundManager soundManager; // 사운드 매니저

    public Transform attackRoot;    //NPC 접촉 포인트
    public Transform eyeTransform;  //NPC 인식 기준 포인트

    public NpcData npcData;
    //NPC 스펙
    private int attackPower;
    private float runSpeed; // = 2f;    //추적 시 이동 속도
    private float patrolSpeed;  // = 1.5f; //추적 안할 시 이동 속도
    private float navFloor; // = 12f;  //3층 floor : 11.04

    private float attackRange = 2f; //플레이어 공격 반경
    private float attackDistance = 1f;
    private float fov = 100f;    //NPC 시야각
    private float viewDistance = 5f;   //NPC 시야범위

    //NPC 회전딜레이
    private float turnSmoothVelocity;
    [Range(0.01f, 2f)] public float turnSmoothTime = 0.01f;

    private RaycastHit[] hits = new RaycastHit[10]; //Ray 범위
    private List<StatusController> lastTargets = new List<StatusController>();
    private Vector3 targetPoint = Vector3.zero; //랜덤 타겟 초기화

    //플레이어 피격관련 시각효과 스크립트용 변수
    public LowHP.LowHealthEffect lhe;

    //NPC 위치 리셋 타이머
    private float timer;

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected(){    //NPC 공격 범위 표시용 테스트 함수
        if(attackRoot != null){
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(attackRoot.position, attackRange);
            Gizmos.DrawSphere(targetPoint, 0.5f);
        }
        Debug.DrawRay(transform.position, transform.forward * viewDistance, Color.red);

        var leftRayRotation = Quaternion.AngleAxis(-fov * 0.5f, Vector3.up);
        var leftRayDirection = leftRayRotation * transform.forward;
        Handles.color = new Color(1f, 1f, 1f, 0.2f);
        Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, fov, viewDistance);
    }
    #endif

    void OnEnable(){
        attackPower = npcData.attackPower;
        runSpeed = npcData.runSpeed;
        patrolSpeed = npcData.patrolSpeed;
        navFloor = npcData.navFloor;

        animator = GetComponent<Animator>();;   //NPC 애니메이터 로드
        nav = GetComponent<NavMeshAgent>(); //NPC 컴포넌트 로드
        attackDistance += nav.radius;
        nav.speed = patrolSpeed;    //로딩 후 초기 이동속도
        StartCoroutine(UpdatePath());
    }
    /*void Awake(){
        soundManager = SoundManager.instance;
        animator = GetComponent<Animator>();;   //NPC 애니메이터 로드
        nav = GetComponent<NavMeshAgent>(); //NPC 컴포넌트 로드
        attackDistance += nav.radius;
        //nav.speed = patrolSpeed;    //로딩 후 초기 이동속도
    }*/
    private void Start(){
        soundManager = SoundManager.instance;
    }

    void Update()
    {
        if(hasTarget && state == State.Tracking && Vector3.Distance(target.transform.position, transform.position) <= attackDistance){   //타겟이 있을 때만 동작
            soundManager.PlaySound(soundManager.zombieSfxPlayer, soundManager.sfx, "ZombieFindPlayer");
            if(nav.isStopped == false){
                Debug.Log("ready to attack!");
                soundManager.PlaySound(soundManager.zombieSfxPlayer, soundManager.sfx, "ZombieAttack");
                BeginAttack();
                new WaitForSeconds(1f);
                soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "ZombieHitted");
            }
        }
        animator.SetFloat("speed", nav.desiredVelocity.magnitude);

        timer += Time.deltaTime;
        if(timer > 10f && !hasTarget){  //시간제한 경과 및 타겟이 없을 때
            Debug.Log("npc 위치 시간제한 리셋");
            while(true){    //NPC 위치 리셋
                Vector3 teleportPos = GetRandomPointOnNavMesh(transform.position, 20f, navFloor, NavMesh.AllAreas);
                Collider[] colliders = Physics.OverlapSphere(teleportPos, viewDistance, targetLayer);
                if(Vector3.Distance(transform.position, teleportPos) > 8f && colliders.Length == 0){ //주변 일정범위에 플레이어가 없고 기존 위치와의 거리가 5이상이면 실행
                    nav.enabled = false;
                    transform.position = teleportPos;
                    timer = 0f;
                    break;
                }
            }
        }
        //Debug.Log("Not Found Timer: " + timer);
        Debug.Log(state);
        //Debug.Log(hasTarget + ", " + nav.destination);
        //Debug.Log("path " + nav.hasPath + ", To: " + nav.destination);
        /*if(!nav.hasPath){
            StartCoroutine(UpdatePath());
        }*/
        //Debug.Log("To target: " + nav.remainingDistance);

    }
    private void FixedUpdate(){
        if(hasTarget && state == State.AttackBegin || state == State.Attacking){ //상태가 공격 관련 상태일 때
            var lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            var targetAngleY = lookRotation.eulerAngles.y;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
        }
        if(state == State.Attacking){   //상태가 공격중인 상태일 때
            Debug.Log("fixed attacking");
            var direction = transform.forward;
            var deltaDistance = nav.velocity.magnitude * Time.deltaTime;
            var size = Physics.SphereCastNonAlloc(attackRoot.position, attackRange, direction, hits, deltaDistance, targetLayer);
            
            for(int i = 0; i < size; i++){
                var attackTargetObj = hits[i].collider.GetComponent<StatusController>();
                if(attackTargetObj.tag == "Player" && attackTargetObj != null && !lastTargets.Contains(attackTargetObj)){
                    Debug.Log("player attack success!");
                    var message = new DamageMsg();
                    message.amount = attackPower;
                    message.damager = gameObject;

                    if(hits[i].distance <= 0f){
                        message.hitPoint = attackRoot.position;
                    }
                    else{
                        message.hitPoint = hits[i].point;
                    }
                    message.hitNormal = hits[i].normal;
                   
                    attackTargetObj.OnDamage(message);
                    lastTargets.Add(attackTargetObj);
                    break;
                }

            }
        }
    }
    private bool hasTarget{ //읽기 전용
        get{
            if(target != null && !target.dead){ //타겟이 없지 않고 타겟이 죽지 않았으면
                return true;
            }
            return false;
        }
    }
    private IEnumerator UpdatePath()
    {
        while (true)
        {   
            nav.enabled = true;
            Debug.Log("코루틴 동작중 : target? " + hasTarget);
            if(hasTarget && !IsTargetCrouch(target.GetComponent<Collider>())){    //타겟이 앉아있으면
                Debug.Log("target sitted down");
                target = null;
            }
            /*else if(hasTarget && IsTargetCrouch(target.GetComponent<Collider>()) && !IsTargetOnSight(target.transform.position)){   //타겟이 서있고 레이에 없으면
                Debug.Log("target not sitted down, but no ray");
                target = null;
            }*/

            if(hasTarget){  //타겟 존재
                Debug.Log("코루틴 1st");
                if(state != State.Tracking){
                    state = State.Tracking;
                    nav.speed = runSpeed;
                }
                nav.SetDestination(target.transform.position);
            }
            else{   //타겟 없음
                if(state != State.Patrol){
                    state = State.Patrol;
                    nav.speed = patrolSpeed;
                }
                //var patrolPosition = GetRandomPointOnNavMesh(transform.position, 10f, navFloor, NavMesh.AllAreas);
                Collider[] colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, targetLayer);
                Debug.Log("colider length: " + colliders.Length);
                if(colliders.Length == 0){  //인식된 콜라이더 없을 때
                    Debug.Log("코루틴 2nd 콜라이더0");
                    //nav.SetDestination(patrolPosition);
                    if(nav.remainingDistance <= 2f || nav.hasPath == false){    //목표지점까지의 거리, 네비게이션 경로 조건
                        Debug.Log("random point update!");
                        var patrolPosition = GetRandomPointOnNavMesh(transform.position, 10f, navFloor, NavMesh.AllAreas);    //업데이트
                        targetPoint = patrolPosition;
                        nav.SetDestination(patrolPosition); //목표 재설정
                    }
                    //nav.SetDestination(patrolPosition); //목표 재설정
                    //break;
                }
                else{
                    foreach(var colider in colliders){
                        var targetPlayer = colider.GetComponent<StatusController>();
                        if(!IsTargetOnSight(colider.transform.position)){   //콜라이더 인식되지만 레이에 안보일 때 (false)
                            //nav.SetDestination(patrolPosition); //목표지점 설정
                            Debug.Log("코루틴 2nd 콜라이더1");
                            if(Vector3.Distance(nav.transform.position, colider.transform.position) < 2f && IsTargetCrouch(colider)){  //콜라이더와 거리 2이하이고 콜라이더 오브젝트가 서있는 상태일 때 타겟팅
                                if(targetPlayer != null && !targetPlayer.dead){
                                    target = targetPlayer;
                                    Debug.Log("코루틴 3rd, " + Vector3.Distance(nav.transform.position, target.transform.position));
                                    break;
                                }
                            }

                            if(nav.remainingDistance <= 2f || nav.hasPath == false){    //목표지점까지의 거리, 네비게이션 경로 조건
                                Debug.Log("random point update!");
                                var patrolPosition = GetRandomPointOnNavMesh(transform.position, 10f, navFloor, NavMesh.AllAreas);    //업데이트
                                targetPoint = patrolPosition;       
                                nav.SetDestination(patrolPosition); //목표 재설정
                                break;
                            }
                            //continue;   //패스
                            //break;
                        }
                        //콜라이더 인식, ray 보일 때
                        //var targetPlayer = colider.GetComponent<StatusController>();
                        if(targetPlayer != null && !targetPlayer.dead){
                            target = targetPlayer;
                            Debug.Log("코루틴 4th, " + Vector3.Distance(nav.transform.position, target.transform.position));
                            break;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, float floor, int areaMask)  // 인수 => 중심 위치, 반경 거리, 검색할 Area (내부적으로 int)
    {
        //var randomPos = Random.insideUnitSphere * distance + center;  // center를 중점으로 하여 반지름(반경) distance 내에 랜덤한 위치 리턴. *Random.insideUnitSphere*은 반지름 1 짜리의 구 내에서 랜덤한 위치를 리턴해주는 프로퍼티
        //NavMeshHit hit;  // NavMesh 샘플링의 결과를 담을 컨테이너 
        while(true){    //y좌표 기준으로 층 구분
            NavMeshHit hit;
            var randomPos = Random.insideUnitSphere * distance + center;
            NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);
            //3층 해당 좌표: > 11.02
            //2층 해당 좌표: < 11, > 6
            //1층 해당 좌표: < 6
            //navFloor 변수 값 기준으로 sampleposition y값이
            if(floor == 0){
                Debug.Log("All floor");
                return hit.position;
            }
            else if(floor >= 11){    //3층 NPC일 때  11.17
                if(hit.position.y >= 11){    //랜덤값 y좌표가 3층이면
                    Debug.Log("3rd floor");
                    //break;
                    return hit.position;
                }
            }
            else if(floor < 11 && floor >= 6){  //2층 NPC   6.15
                if(hit.position.y < 11 && hit.position.y >= 6){
                    Debug.Log("2nd floor");
                    //break;
                    return hit.position;
                }
            }
            else{
                if(hit.position.y < 6){
                    Debug.Log("1st floor");
                    //break;
                    return hit.position;
                }
            }
        }
        //NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);  // areaMask에 해당하는 NavMesh 중에서 randomPos로부터 distance 반경 내에서 randomPos에 *가장 가까운* 위치를 하나 찾아서 그 결과를 hit에 담음. 
        //return hit.position;  // 샘플링 결과 위치인 hit.position 리턴
    }
    public static float GetRandomNormalDistribution(float mean, float standard)
    {
        var x1 = Random.Range(0f, 1f);
        var x2 = Random.Range(0f, 1f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }

    private bool IsTargetOnSight(Vector3 targetPosition)
    {
        //true면 레이 닿음
        RaycastHit hit;
        var direction = targetPosition - eyeTransform.position;
        direction.y = eyeTransform.forward.y;

        //그 광선이 시야각을 벗어나선 안되며 
        if (Vector3.Angle(direction, eyeTransform.forward) > fov * 0.5f)
        {
            return false;
        }
        direction = targetPosition - eyeTransform.position;
        //시야각 내에 존재 하더라도 광선이 장애물에 부딪치지 않고 목표에 잘 닿아야 함
        if (Physics.Raycast(eyeTransform.position, direction, out hit, viewDistance))
        {
            if (hit.transform.position == targetPosition){
                return true;
            }
        }
        return false;
    }
    private bool IsTargetCrouch(Collider target){
        targetControl = target.GetComponent<PlayerControl>();
        if(targetControl.isCrouchToNav){ //앉아있으면 false
            return false;
        }
        return true;    //서있으면 true
    }

    private void BeginAttack(){
        Debug.Log("call beginattack");
        state = State.AttackBegin;
        nav.isStopped = true;
        GameManager.isAttacked = true;
        animator.SetTrigger("attack");
        //soundManager.PlaySound(soundManager.zombieSfxPlayer, soundManager.sfx, "ZombieAttack");
        new WaitForSeconds(2f); // 애니메이션 재생과 시간 맞추기
        //soundManager.PlaySound(soundManager.sfxPlayer, soundManager.sfx, "ZombieHitted");
    }
    private void Attack(){
        //메소드 호출 시 NPC상태 변경, 타겟리스트 추가
        Debug.Log("call Attack");
        state = State.Attacking;
        //lhe.helathChangeByNPC(target.health);
        lastTargets.Clear();
    }
    private void Attacking(){
        Debug.Log("attacked by NPC!");
        lhe.helathChangeByNPC(target.health);
        lhe.takeDamageByNPC();
        //target.GetComponent<PlayerControl>().isAttackedFov();
    }
    private void EndAttack(){
        Debug.Log("call EndAttack");
        GameManager.isAttacked = false;
        animator.SetTrigger("teleport");
        while(true){    //NPC 위치 리셋
            Vector3 teleportPos = GetRandomPointOnNavMesh(transform.position, 20f, navFloor, NavMesh.AllAreas);
            Collider[] colliders = Physics.OverlapSphere(teleportPos, viewDistance, targetLayer);
            if(Vector3.Distance(transform.position, teleportPos) > 5f && colliders.Length == 0){ //주변 일정범위에 플레이어가 없고 기존 위치와의 거리가 5이상이면 실행
                transform.position = teleportPos;
                timer = 0f;
                break;
            }
        
        }
        target = null; 
        //timer = 0f;
        if(hasTarget){
            state = State.Tracking;
        }
        else{
            state = State.Patrol;
        }
        nav.isStopped = false;
        nav.enabled = false;
        StartCoroutine(UpdatePath());
        Debug.Log("endattack state:" + state);
    }

}
