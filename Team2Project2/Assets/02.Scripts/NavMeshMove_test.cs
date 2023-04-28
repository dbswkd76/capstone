using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMove_test : MonoBehaviour
{
    public LayerMask targetLayer;   //추적 대상 레이어
    private NavMeshAgent nav;   //경로 계산 AI
    private GameObject target;  //추적 대상

    public Transform r_target; //추적 안할 시 랜덤 이동 포인트
    private float range = 5;
    Vector3 point;

    //무작위 좌표 설정(타겟 없을 시 NPC 랜덤 이동)
    private bool randomPoint(Vector3 center, float range, out Vector3 result){
        for(int i = 0; i < 30; i++){
            Vector3 r_point = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(r_point, out hit, 1.0f, NavMesh.AllAreas)){
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    private void Awake(){
        //컴포넌트 로드
        nav = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        //게임 오브젝트 활성화, AI 루틴 시작
        StartCoroutine(UpdatePath());
    }
    private void Update()
    {
        //Debug.Log(nav.remainingDistance);
        //nav.SetDestination(target.position);
    }

    private IEnumerator UpdatePath(){   //한번 타겟으로 지정되면 범위 벗어나도 계속 추적하는 버그 수정 필요
        while(true){
            //20유닛의 반지름을 가진 가상의 구와 겹치는 모든 콜라이더 가져옴(플레이어 레이어만 해당)
            Collider[] colliders = Physics.OverlapSphere(transform.position, 4f, targetLayer);
            Debug.Log(colliders.Length);
            //모든 콜라이더 순회하며 플레이어 태그 오브젝트 찾기

            if(colliders.Length == 1){
                for(int i = 0; i < colliders.Length; i++){
                    if(colliders[i].tag == "Player"){
                        Debug.Log("find player!");
                        target = colliders[i].gameObject;
                        nav.SetDestination(target.transform.position);
                        break;
                    }
                }
            }
            else{   //감지된 플레이어 없으면
                Debug.Log("no player!");
                //Debug.Log(nav.transform.position + ", " + r_target.position);
                Debug.Log(Vector3.Distance(nav.transform.position, r_target.position));

                if(randomPoint(r_target.position, range, out point) && Vector3.Distance(nav.transform.position, r_target.position) <= 1){ //랜덤 목적지 업데이트(NPC, 목적지 간 거리 기준)
                    Debug.Log(nav.transform.position + ", " + r_target.position);
                    r_target.position = point;
                    Debug.Log("point update");
                }
                //Debug.Log("speed: " + nav.velocity.magnitude);
                nav.SetDestination(r_target.position);
            }
            
            //Debug.Log("no player");
            //target = null;
            yield return new WaitForSeconds(0.25f); //0.25초 주기로 반복
        }
    }
}