using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public int useCount;
    private float hammerSpeed = 50f;

    private Vector3 upPosition = new Vector3(0.005f, 0, 0);
    private Vector3 downPosition = new Vector3(-0.005f, 0, 0);

    private void Start()
    {
        useCount = 10;
    }

    public void HammerUP()
    {
        StartCoroutine(HammerMove(upPosition));
    }

    public void HammerDown()
    {
        StartCoroutine(HammerMove(downPosition));
    }

    // ���� ���� �� ��ġ�� ���� �ö󰡰�, �������� �� ��ġ�� ���� ��������.
    IEnumerator HammerMove(Vector3 dir)
    {
        int count = 5;
        while(count >= 0){
            count--;
            transform.Translate(hammerSpeed * Time.deltaTime * dir);
            yield return null;
        }
    }
}
