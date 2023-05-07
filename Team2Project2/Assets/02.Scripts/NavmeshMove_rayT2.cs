using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class NavmeshMove_rayT2 : MonoBehaviour//StatusController//MonoBehaviour
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
    private NavMeshAgent nav;
    //private GameObject target;  //타겟
    private StatusController target;
    
    public Transform attackRoot;    //NPC 접촉 포인트
    public Transform eyeTransform;  //NPC 인식 기준 포인트

    private float fov = 100f;    //NPC 시야각
    private float viewDistance = 5f;   //NPC 시야범위
    private float patrolSpeed = 1.5f; //추적 안할 시 이동 속도
    private float runSpeed = 2f;    //추적 시 이동 속도
    //NPC 회전딜레이
    private float turnSmoothVelocity;
    [Range(0.01f, 2f)] public float turnSmoothTime = 0.1f;

    private int attackPower = 10;
    private float attackRange = 2f; //플레이어 공격 반경
    private float attackDistance = 1f;

    private RaycastHit[] hits = new RaycastHit[10]; //Ray 범위
    private List<StatusController> lastTargets = new List<StatusController>();

    private Vector3 targetPoint = Vector3.zero; //랜덤 타겟 초기화

    //EndAttack 메소드 테스트용 딜레이 변수
    private int delayTime;
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

    void Awake(){
        nav = GetComponent<NavMeshAgent>(); //NPC 컴포넌트 로드
        attackDistance += nav.radius;
        nav.speed = patrolSpeed;    //로딩 후 초기 이동속도
    }
    public void Setup(int attackPower, float runSpeed, float patrolSpeed){    //NPC 스펙 셋업
        this.runSpeed = runSpeed;
        this.patrolSpeed = patrolSpeed;
        this.attackPower = attackPower;
        nav.speed = patrolSpeed;
    }
    /*void Start()
    {
        //target = GameObject.FindWithTag("Player");
        StartCoroutine(UpdatePath());
        
        //공격 기능 실행 지연용
        timer = 0f;
        delayTime = 3;
    }*/
    void OnEnable(){
        StartCoroutine(UpdatePath());
        timer = 0f;
        delayTime = 3;
    }
    void Update()
    {
        /*if(target == null || target.dead){
            Debug.Log(state);
            //return;    //타겟 dead
        }*/
        /*if(!hasTarget || target.dead){
            Debug.Log("no!");
            return;
        }*/
        //Debug.Log(Vector3.Distance(target.transform.position, transform.position) + ", " + timer);
        if(hasTarget && state == State.Tracking && Vector3.Distance(target.transform.position, transform.position) <= attackDistance){   //타겟이 있을 때만 동작
            Debug.Log("ready to attack!");
            BeginAttack();
        }
        Debug.Log(state);
        //Debug.Log(target.health);
        Debug.Log(hasTarget);
        //Debug.Log("target? " + hasTarget);
        //Debug.Log("To target: " + nav.remainingDistance);

    }
    private void FixedUpdate(){
        /*if(target == null || target.dead){  //target null 임시 해결
            Debug.Log("return " + hasTarget);
            state = State.Patrol;
            nav.isStopped = false;
            //return;
        }*/
        /*if(!hasTarget || target.dead){
            Debug.Log("no!");
            return;
        }*/
        if(hasTarget && state == State.AttackBegin || state == State.Attacking){ //상태가 공격 관련 상태일 때
            Debug.Log("fixed attack");
            var lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            var targetAngleY = lookRotation.eulerAngles.y;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
            //상태 구분(NPC 애니메이터 구현 전 임시 동작)
            //Attack();
            //Debug.Log(timer);
            //공격 주기나 공격 정지 후 NPC 타겟팅 해제를 위한 코드 작성 필요(2023.05.01 오후 11:31)
            //Navmesh 내에서 플레이어와 일정 거리 이상 떨어진 랜덤 포인트로 텔레포트 후 NPC 리셋?
            if(timer < delayTime){
                Attack();
                timer += Time.deltaTime;    //공격 직후부터 계산한 시간을 기준으로 타이머를 설정
            }
            else if(timer > delayTime){
                EndAttack();
                //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, -targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
                /*while(true){
                    Vector3 teleportPos = GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                    //Debug.Log("distance: " + Vector3.Distance(transform.position, teleportPos));
                    if(Vector3.Distance(transform.position, teleportPos) > 15f){
                        Debug.Log("distance: " + Vector3.Distance(transform.position, teleportPos) + ", pos: " + teleportPos);
                        transform.position = teleportPos;
                        //target = null;
                        break;
                    }
                }*/
                //target = null;
            }
            //EndAttack 메소드가 실행이 되야 NPC 동작 리셋 가능
            //EndAttack();
        }
        if(state == State.Attacking){   //상태가 공격중인 상태일 때
            var direction = transform.forward;
            var deltaDistance = nav.velocity.magnitude * Time.deltaTime;
            var size = Physics.SphereCastNonAlloc(attackRoot.position, attackRange, direction, hits, deltaDistance, targetLayer);
            
            for(int i = 0; i < size; i++){
                var attackTargetObj = hits[i].collider.GetComponent<StatusController>();
                if(attackTargetObj.tag == "Player" && attackTargetObj != null && !lastTargets.Contains(attackTargetObj)){
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
            Debug.Log("코루틴 동작중 : target? " + hasTarget);
            if(hasTarget){
                Debug.Log("코루틴 1st");
                nav.SetDestination(target.transform.position);
                break;
            }
            var patrolPosition = GetRandomPointOnNavMesh(transform.position, 10f, NavMesh.AllAreas);
            Collider[] colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, targetLayer);
            Debug.Log("colider length: " + colliders.Length);
            if(!hasTarget && colliders.Length == 0){  //범위 내 감지된 콜라이더 없을 때
                Debug.Log("코루틴 2nd");
                //target = null;
                if(state != State.Patrol){
                    state = State.Patrol;
                    nav.speed = patrolSpeed;
                }
                if(nav.remainingDistance <= 2f){    //타겟 포인트까지 거리가 2 이하일때 업데이트
                    patrolPosition = GetRandomPointOnNavMesh(transform.position, 10f, NavMesh.AllAreas);
                    Debug.Log("random point update!");
                    targetPoint = patrolPosition;       
                    nav.SetDestination(patrolPosition);
                }

            }
            else{   //감지된 콜라이더 있을 때
                Debug.Log("코루틴 3rd");
                foreach(var collider in colliders){
                    Debug.Log("name: " + collider.name);
                    Debug.Log("see: " + IsTargetOnSight(collider.transform.position));
                    if(!IsTargetOnSight(collider.transform.position)){  //ray 안보이면 랜덤포인트로 설정
                        Debug.Log("nearby but cannot see player!");
                        /*nav.SetDestination(patrolPosition);
                        if(nav.remainingDistance <= 2f){
                            patrolPosition = GetRandomPointOnNavMesh(transform.position, 10f, NavMesh.AllAreas);
                            targetPoint = patrolPosition;
                            nav.SetDestination(patrolPosition);
                        }
                        */
                        //target = null;
                        if(state != State.Patrol){
                            state = State.Patrol;
                        }
                        continue;
                    }
                    if(collider.tag == "Player"){
                        Debug.Log("targeting!");
                        var targetPlayer = collider.GetComponent<StatusController>();
                        if(state == State.Patrol){
                            state = State.Tracking;
                            nav.speed = runSpeed;
                        }
                        if(targetPlayer != null && !targetPlayer.dead){
                            target = targetPlayer;
                            break;
                        }
                    }
                    else{
                        nav.SetDestination(patrolPosition);
                        break;
                    }
                }
                //nav.SetDestination(target.transform.position);
            }
            yield return new WaitForSeconds(0.05f);
            //yield return new WaitForFixedUpdate ();
        }
    }

    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask)  // 인수 => 중심 위치, 반경 거리, 검색할 Area (내부적으로 int)
    {
        var randomPos = Random.insideUnitSphere * distance + center;  // center를 중점으로 하여 반지름(반경) distance 내에 랜덤한 위치 리턴. *Random.insideUnitSphere*은 반지름 1 짜리의 구 내에서 랜덤한 위치를 리턴해주는 프로퍼티
        
        NavMeshHit hit;  // NavMesh 샘플링의 결과를 담을 컨테이너
        
        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);  // areaMask에 해당하는 NavMesh 중에서 randomPos로부터 distance 반경 내에서 randomPos에 *가장 가까운* 위치를 하나 찾아서 그 결과를 hit에 담음. 
        
        return hit.position;  // 샘플링 결과 위치인 hit.position 리턴
    }
    public static float GetRandomNormalDistribution(float mean, float standard)
    {
        var x1 = Random.Range(0f, 1f);
        var x2 = Random.Range(0f, 1f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }

    private bool IsTargetOnSight(Vector3 target)
    {
        RaycastHit hit;

        var direction = target - eyeTransform.position;

        direction.y = eyeTransform.forward.y;

        //그 광선이 시야각을 벗어나선 안되며 
        if (Vector3.Angle(direction, eyeTransform.forward) > fov * 0.5f)
        {
            return false;
        }

        direction = target - eyeTransform.position;

        //시야각 내에 존재 하더라도 광선이 장애물에 부딪치지 않고 목표에 잘 닿아야 함
        if (Physics.Raycast(eyeTransform.position, direction, out hit, viewDistance))
        {
            if (hit.transform.position == target){
                return true;
            }
        }

        return false;
    }

    private void BeginAttack(){
        state = State.AttackBegin;
        nav.isStopped = true;
    }
    private void Attack(){
        Debug.Log("call Attack");
        state = State.Attacking;
        lastTargets.Clear();
    }
    private void EndAttack(){
        Debug.Log("call EndAttack");
        nav.enabled = false;
        while(true){    //NPC 위치 랜덤 순간이동
            Vector3 teleportPos = GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
            //Debug.Log("distance: " + Vector3.Distance(transform.position, teleportPos));
            if(Vector3.Distance(transform.position, teleportPos) > 15f){
                Debug.Log("distance: " + Vector3.Distance(transform.position, teleportPos) + ", pos: " + teleportPos);
                transform.position = teleportPos;
                nav.enabled = true;
                //target = null;
                break;
            }
        }
        timer = 0f; //타이머 초기화
        target = null; 
        if(hasTarget){
            state = State.Tracking;
            Debug.Log("End target yes");
        }
        else{
            state = State.Patrol;
            Debug.Log("End no target");
        }
        StartCoroutine(UpdatePath());
        Debug.Log("endattack state:" + state);
    }

}
