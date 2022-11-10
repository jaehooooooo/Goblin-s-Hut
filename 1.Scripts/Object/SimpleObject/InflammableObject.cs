using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflammableObject : MonoBehaviour
{
    private ObjectPosition objectPosition;
    private Rigidbody rig;
    private BoxCollider boxCollider;
    public GameObject activeObjectPool;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀 // 임시로 작동 (나중에 고칠때는 전체 메세지로 불끄라고 알려줘야 할듯?)

    public GameObject model;                    // 원래 형태
    public GameObject ash;                      // 잿더미 형태
    public ParticleSystem[] burnningParitcles;  // 불타는 파티클
    public bool burnEnough = false;             // 완전 연소 여부
    private bool isBurning = false;             // 불타는 중인지 체크
    public float burnEnoughTime;                // 완전 연소될 시간 (물체마다 다른 속도)
    public float catchFireTime;                 // 불이 옆으로 옮겨붙기 시작할 시간
    public Vector3 boxcastSize;                 // 박스캐스트 크기
    public Vector3 nomalColliderSize = new Vector3(0.1f,0.1f,0.1f);             // 완전 연소 후 콜라이더 크기
    public Vector3 ashColliderSize = new Vector3(0.1f,0.03f,0.1f);             // 완전 연소 후 콜라이더 크기

    // 내 주변 오브젝트 체크 후 불탈 수 있는 물체가 또 있다면 그 물체도 불타게 만들기
    // 불타면 행동할 내용

    private void Awake()
    {
        objectPosition = GetComponent<ObjectPosition>();
        rig = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        foreach (ParticleSystem particle in burnningParitcles) 
        {
            particle.Stop();
        }
    }
    public void Burn()
    {
        // 현재 불타고 있지 않고 && 황금 상태가 아닐때 (황금은 불이 안붙음)
        if(!objectPosition.inflammableObject)
        {
            if(objectPosition.changeGoldObject)
            {
                if (GetComponent<ChangeGoldObject>().isChange)
                    return;
            }
            // 불타는 상호작용 활성화 알림
            objectPosition.inflammableObject = true;

            isBurning = true;
            // 불타는 이펙트 활성화
            foreach (ParticleSystem particle in burnningParitcles)
            {
                particle.Play();
            }
            // 옆 오브젝트에 불 옮겨붙음
            StartCoroutine(IFineInflammableObject());

            // 완전 연소되는 물체인 경우 완전 연소
            Invoke("BurnEnough", burnEnoughTime);
        }
    }

    IEnumerator IFineInflammableObject()
    {
        // 옆 오브젝트에 불이 옮겨붙을 시간
        yield return new WaitForSeconds(catchFireTime);

        // 본인기준 RaycastBox를 만들어서 주변에 또 불탈 수 있는 물체 있는지 감지
        Collider[] hitObjects = Physics.OverlapBox(transform.position, boxcastSize);

        if (hitObjects.Length > 0)
        {
            foreach (Collider collider in hitObjects)
            {
                // 만약에 닿은 물체도 불탈 수 있는 물체라면
                if (collider.gameObject.GetComponent<InflammableObject>())
                {
                    collider.transform.parent = activeObjectPool.transform;
                    collider.gameObject.GetComponent<InflammableObject>().Burn();
                }
            }
        }
    }

    // 완전 연소된 상태일때
    private void BurnEnough()
    {
        
        // 리지드바디 비활성화
        rig.isKinematic = true;
        // 원래 형태 비활성화
        model.SetActive(false);
        // 원래 각도로 조정
        transform.localEulerAngles = new Vector3(0, 0, 0);
        // 박스콜라이더 사이즈 변경
        boxCollider.size = ashColliderSize;
        // 갖고있는 재 형태 활성화
        ash.SetActive(true);
        // 리지드바디 활성화
        rig.isKinematic = false;

        // 불타는 이펙트 비활성화
        foreach (ParticleSystem particle in burnningParitcles)
        {
            particle.Stop();
        }
    }

    public void ReturnNomal()
    {
        StopAllCoroutines();
        CancelInvoke();
        // 박스콜라이더 사이즈 변경
        boxCollider.size = nomalColliderSize;
        // 리지드바디 활성화
        rig.isKinematic = false;
        // 원래 형태 비활성화
        model.SetActive(true);
        // 갖고있는 재 형태 활성화
        ash.SetActive(false);
        // 불타는 이펙트 비활성화
        foreach (ParticleSystem particle in burnningParitcles)
        {
            particle.Stop();
        }
    }
}
