using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMove_test : MonoBehaviour
{
    public LayerMask targetLayer;   //추적 대상 레이어
    private NavMeshAgent nav;   //경로 계산 AI
    private GameObject target;  //추적 대상
    //public Transform targetPos;

    private bool hasTarget{ //읽기 전용
        get{
            if(target != null /* && !target.dead*/){
                return true;
            }
            return false;
        }
    }
    private void Awake(){
        //컴포넌트 로드
        nav = GetComponent<NavMeshAgent>();
        //target = gameObject.
    }
    private void Start()
    {
        //게임 오브젝트 활성화, AI 루틴 시작
        StartCoroutine(UpdatePath());
        //nav = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        //nav.SetDestination(targetPos.position);
    }

    private IEnumerator UpdatePath(){   //한번 타겟으로 지정되면 범위 벗어나도 계속 추적하는 버그 수정 필요
        while(true){
            if(hasTarget){
                //추적 대상 존재
                nav.isStopped = false; //AI 이동 진행
                nav.SetDestination(target.transform.position);
            }
            else{
                //추적 대상 없음
                nav.isStopped = true;  //AI 이동 중지

                //20유닛의 반지름을 가진 가상의 구와 겹치는 모든 콜라이더 가져옴(플레이어 레이어만 해당)
                Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, targetLayer);

                //모든 콜라이더 순회하며 플레이어 태그 오브젝트 찾기
                for(int i = 0; i < colliders.Length; i++){
                    if(colliders[i].tag == "Player"){
                        Debug.Log("find player!");
                        target = colliders[i].gameObject;
                        break;
                    }
                    else{
                        Debug.Log("no player!");
                        target = null;
                    }
                }
            }
            yield return new WaitForSeconds(0.25f); //0.25초 주기로 반복
        }
    }
}