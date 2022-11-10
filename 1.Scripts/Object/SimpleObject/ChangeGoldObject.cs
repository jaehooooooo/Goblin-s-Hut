using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGoldObject : MonoBehaviour
{
    private ObjectPosition objectPosition;
    // 
    public GameObject model;
    // 황금으로 변할때 터질 파티클
    public ParticleSystem particle;
    // 황금 
    public GameObject gold;
    public float goldWeight;
    public float orginWeight;
    // 상태 변화 체크
    public bool isChange = false;
    // 상태 변화 가능주기
    private float changeDuration = 3f;
    private bool isDuration;
    

    private void Awake()
    {
        objectPosition = GetComponent<ObjectPosition>();
        particle.Stop();
    }

    public void ChangeGold()
    {
        // 금괴 상호작용 활성화 알림
        objectPosition.changeGoldObject = true;

        // 불이 붙어있는 상태라면 불끄고 원래 상태로 복구하라고 전달
        if (objectPosition.inflammableObject)
            objectPosition.ReturnToNomal("inflammableObject");

        // 물체의 무게 변경
        orginWeight = objectPosition.objectWeight;
        objectPosition.objectWeight = goldWeight;

        particle.Play();
        isChange = !isChange;
        // 원래 오브젝트 비활성화
        model.SetActive(!isChange);
        // 황금 오브젝트 활성화
        gold.SetActive(isChange);
        isDuration = true;
        StartCoroutine(IChangeDuration());
    }
    IEnumerator IChangeDuration()
    {
        // 정해진 기간 후에 다시 바뀔 수 있음
        yield return changeDuration;
        isDuration = false;
    }

    public void ReturnNomal()
    {
        // 물체의 무게 변경
        objectPosition.objectWeight = orginWeight;

        // 파티클 멈추기
        particle.Play();
        isDuration = false;
        isChange = false;
        // 골드 비활성화
        gold.SetActive(false);
        // 골드 위치 초기화
        gold.transform.position = new Vector3(0, 0, 0);
        // 원래 형태 활성화
        model.SetActive(true);
    }

}
