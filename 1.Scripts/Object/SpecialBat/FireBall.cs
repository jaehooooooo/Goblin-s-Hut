using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{

    public LayerMask layer;
    Rigidbody rig;

    public float power;         // 공 속도 보조 수치
    public bool hitNow = false;         // 공을 칠 수 있는 상태 여부
    public GameObject activeObjectPool;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    public void ReadyToHit()
    {
        rig.velocity = Vector3.zero;
        rig.useGravity = false;
        // 카메라 기준으로 일정 위치에 공 위치시키기
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0, -0.15f, 0);
    }

    // 공이 방망이에 닿았을때 값을 알려줄 함수
    public void HitAction(Vector3 dir, float batSpeed)
    {
        Vector3 force = dir * batSpeed * power;
        rig.AddForce(force, ForceMode.Force);
        rig.useGravity = true;

        // 자동 비활성화 코드
        CancelInvoke();
        Invoke("SetActive", 5f);
    }

    // 비활성화하는 코드
    private void SetActive()
    {
        hitNow = false;
        this.gameObject.SetActive(false);
    }

    // 어떤 물체와 닿으면 닿은 물체의 상태 파악 후 파악한 물체가 불탈 수 있으면 불타는 이펙트 실행하라고 전달
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<InflammableObject>())
        {
            collision.gameObject.GetComponent<InflammableObject>().Burn();
            collision.transform.parent = activeObjectPool.transform;

        }

        if (!hitNow && collision.gameObject.name == "FireBat")
        {
            hitNow = true;
            GameObject batCollider = collision.transform.GetChild(1).gameObject;
            float speed = batCollider.gameObject.GetComponent<FireBat>().nowSpeed;
            // 방망이에 닿음
            // 방향과 속력을 받아와서 공에 힘을 줌
            HitAction(collision.contacts[0].normal,speed);
        }
    }
}
