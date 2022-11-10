using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeButterflyObject : MonoBehaviour
{
    // 나비 이펙트
    public GameObject changeButterflyParticle;
    // 원래 형태
    public GameObject model;
    //
    private Rigidbody rig;
    private ObjectPosition objectPosition;

    private bool isChange;
    private float changeEnoughTime = 3f; // 나비 이펙트가 사라질 시간

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        objectPosition = GetComponent<ObjectPosition>();
    }

    // 나비 이펙트가 발현될 함수
    public void ChangeButterfly()
    {
        // 나비 이펙트가 실행된적이 없다면
        if (!objectPosition.changeButterflyObject)
        {
            // 나비 상호작용 활성화 알림
            objectPosition.changeButterflyObject = true;

            // 불이 붙어있는 상태라면 불끄고 원래 상태로 복구하라고 전달
            if (objectPosition.inflammableObject)
                objectPosition.ReturnToNomal("inflammableObject");

            isChange = true;

            // 나비 이펙트 활성화
            changeButterflyParticle.transform.position = transform.position + new Vector3(0, 1, 0);
            changeButterflyParticle.SetActive(true);
            // 리지드바디 끄기
            rig.isKinematic = true;
            // 원래 형태 없에기
            model.SetActive(false);
            // 일정 시간 후 호출
            Invoke("ChangeEnough", changeEnoughTime);
        }

    }
    // 나비 이펙트가 끝나고 실행될 함수
    private void ChangeEnough()
    {
        // 이펙트 비활성화
        changeButterflyParticle.SetActive(false);
    }

    // 다시 재 생성될 함수
    public void ReturnNomal()
    {
        // 원래 형태 활성화
        model.SetActive(true);
        // 리지드바디 켜기
        rig.isKinematic = false;
        // 이펙트가 활성화된 상태라면
        if (changeButterflyParticle.activeSelf)
            ChangeEnough();
    }
}
