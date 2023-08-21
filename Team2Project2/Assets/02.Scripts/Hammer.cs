using System.Collections;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public int UseCount; // 해머의 최대 사용 횟수
    private readonly float _hammerMovingSpeed = 50f; // 해머 사용 시 움직이는 동작의 속도

    private Vector3 _upPosition = new(0.005f, 0, 0); // 벽을 들어올릴 때 해머가 움직이는 정도
    private Vector3 _downPosition = new(-0.005f, 0, 0); // 벽을 놓을 때 해머가 움직이는 정도

    private void Start()
    {
        UseCount = 10;
    }

    public void HammerUP()
    {
        StartCoroutine(HammerMove(_upPosition));
    }

    public void HammerDown()
    {
        StartCoroutine(HammerMove(_downPosition));
    }

    // 벽을 옮길 때 해머의 위치가 조금 올라가고, 놓을 때 조금 내려간다
    IEnumerator HammerMove(Vector3 dir)
    {
        int count = 5;
        while(count >= 0){
            count--;
            transform.Translate(_hammerMovingSpeed * Time.deltaTime * dir);
            yield return null;
        }
    }
}
