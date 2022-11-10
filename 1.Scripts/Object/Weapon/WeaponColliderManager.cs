using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponColliderManager : MonoBehaviour
{
    // PlayerGrabObject 스크립트에서 일정 속도 이상으로 무기를 휘두르면 사용될 스크립트

    // isTrigger를 활용해서 닿은 물체를 체크
    // 감지된 물체에게 해당하는 상호작용을 실행하라는 코드를 보냄

    // 무기와 닿은 물체를 감지할 Colliders
    public Collider[] colliders;
    private Vector3 prePos;
    private float velocityValue = 2f;

    private void Update()
    {
        // 손에 잡힌 상태가 아니면 return
        if (!transform.parent.name.Contains("Hand"))
            return;
        else
        {
            // 손에 잡힌 상태라면 프레임당 속도 체크
            float velocity = Vector3.Distance(transform.position, prePos) / Time.deltaTime;
            prePos = transform.position;

            // 손의 속도가 1 이상일때 -> 콜라이더에 닿는 물체가 있다면(isTrigger) 잘리든 깨지든 뭔가 하라고 알려주기
            if (velocity > velocityValue)
            {
                // 콜라이더에 닿는 물체가 있는지 파악
                DetectOtherCollider(velocity);
            }
            else if (velocity <= velocityValue)
            {
                DisDetectOtherCollider();
            }
        }
    }

    // 부딪힐 콜라이더 감지
    public void DetectOtherCollider(float velocity)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
            collider.gameObject.GetComponent<WeaponColider>().velocity = velocity;
        }
    }
    // 부딪힐 콜라이더 비활성화
    public void DisDetectOtherCollider()
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
