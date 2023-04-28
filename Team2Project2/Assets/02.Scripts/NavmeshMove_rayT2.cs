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
    private NavMeshAgent nav;
    private GameObject target;

    public Transform attackRoot;    //NPC 접촉 포인트
    public Transform eyeTransform;  //NPC 인식 기준 포인트

    private float fov = 100f;    //NPC 시야각
    private float viewDistance = 5f;   //NPC 시야범위
    private float patrolSpeed = 1.5f; //추적 안할 시 이동 속도
    private float runSpeed = 2f;    //추적 시 이동 속도

    //private float damage;
    private float attackRange = 2f; //플레이어 공격 반경
    private float attackDistance = 0f;

    private RaycastHit[] hits = new RaycastHit[10]; //Ray 범위

    private Vector3 targetPoint = Vector3.zero; //랜덤 타겟 초기화

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected(){    //NPC 공격 범위 표시용 테스트 함수
        if(attackRoot != null){
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(attackRoot.position, attackRange);
            Gizmos.DrawSphere(targetPoint, 0.5f);
        }
        Debug.DrawRay(transform.position, transform.forward * 8, Color.red);

        var leftRayRotation = Quaternion.AngleAxis(-fov * 0.5f, Vector3.up);
        var leftRayDirection = leftRayRotation * transform.forward;
        Handles.color = new Color(1f, 1f, 1f, 0.2f);
        Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, fov, viewDistance);


    }
    #endif

    void Awake(){
        nav = GetComponent<NavMeshAgent>(); //NPC 컴포넌트 로드
        //attackDistance = Vector3.Distance(transform.position, new Vector3(attackRoot.position.x , transform.position.y, attackRoot.position.z)) + attackRange;
        //attackDistance += nav.radius;
        target = GameObject.FindWithTag("Player");
        nav.stoppingDistance = attackDistance;  //플레이어 접근 후 공격을 위한 정지
        nav.speed = patrolSpeed;    //로딩 후 초기 이동속도/
    }
    void Start()
    {
        StartCoroutine(UpdatePath());
    }
    void Update()
    {
        if(state == State.Tracking && Vector3.Distance(target.transform.position, transform.position) <= attackDistance){
            BeginAttack();
        }
        Debug.Log(state);
        //Debug.Log("To target: " + nav.remainingDistance);
    }
    private void BeginAttack(){
        state = State.AttackBegin;
        nav.isStopped = true;
    }
    private void EndAttack(){
        if(hasTarget){
            state = State.Tracking;
        }
        else{
            state = State.Patrol;
        }
        nav.isStopped = false;
    }

    private bool hasTarget{ //읽기 전용
        get{
            if(target != null){   //&& !target.dead
                return true;
            }
            return false;
        }
    }
    private IEnumerator UpdatePath()
    {
        while (true)
        {
            var patrolPosition = GetRandomPointOnNavMesh(transform.position, 10f, NavMesh.AllAreas);
            Collider[] colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, targetLayer);
            if((nav.destination != target.transform.position) && nav.remainingDistance <= 2f){ //감지된 콜라이더 없고 목적지까지 거리가 2이하
              //target = null;
              patrolPosition = GetRandomPointOnNavMesh(transform.position, 10f, NavMesh.AllAreas);
              if(state != State.Patrol){
                state = State.Patrol;
                nav.speed = patrolSpeed;
              }
              Debug.Log("random point update!");
              targetPoint = patrolPosition;
              nav.SetDestination(patrolPosition);
              //break;
            }
            else if(colliders.Length != 0){
              foreach(var collider in colliders){
                if(!IsTargetOnSight(collider.transform.position)){
                  Debug.Log("cannot see!");
                  continue;
                }
                if(collider.tag == "Player"){
                  Debug.Log("targeting!");
                  target = collider.gameObject;

                  if(state == State.Patrol){
                    state = State.Tracking;
                    nav.speed = runSpeed;
                  }
                  nav.SetDestination(target.transform.position);
                  break;
                }
              }
            }
            // 0.05 초 시간 간격을 두면서 살아 있는 동안 무한 루프 반복 처리
            yield return new WaitForSeconds(0.05f);

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
                Debug.Log(hit.transform.name);
                return true;
            }
        }

        return false;
    }
}
