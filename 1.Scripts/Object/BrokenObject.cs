using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : MonoBehaviour
{
    // 잘라지거나 부셔진 오브젝트 
    // 랜덤한 방향으로 날아갈 예정
    // 상호작용할 오브젝트들
    public GameObject[] objectsPool;
    // 흩어질 속도
    public float speed;

    public void BrokenStart(float weaponVelocity)
    {
        foreach ( GameObject brokenobject in objectsPool)
        {
            // 랜덤한 방향 설정
            Vector3 direction = Random.insideUnitSphere;
            // 날아가기 
            Rigidbody rb = brokenobject.GetComponent<Rigidbody>();
            rb.velocity = direction * weaponVelocity;
        }
    }

    public void BrokenEnd()
    {
        foreach (GameObject brokenobject in objectsPool)
        {
            brokenobject.transform.localPosition = Vector3.zero;
            print("위치 초기화");
        }
    }



    //private void enable()
    //{
    //    // 랜덤한 방향 설정
    //    Vector3 direction = Random.insideUnitSphere;
    //    // 날아가기 
    //    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
    //    rb.velocity = direction * speed;

    //    StartCoroutine(IResetPosition());
    //}

    //IEnumerator IResetPosition()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    transform.position = Vector3.zero;
    //    print("위치 초기화");
    //}
}
