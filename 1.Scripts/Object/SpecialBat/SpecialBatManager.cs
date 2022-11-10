using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBatManager : MonoBehaviour
{
    // 방망이가 생성될때 사용될 파티클
    public ParticleSystem[] makeBatParitcles;
    // 방망이가 작동할 때 사용될 파티클
    public ParticleSystem[] useBatParticles;
    // 방망이 주요 기능이 들어있는 Collider
    public Transform TargetCollider;
    // 같은 속도가 아니라 점점빨라지는 효과를 위해 애니메이션 커브 생성
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private string hand;

    private void Update()
    {
        // 부모레이어가 손으로 바뀌었다면
        if (transform.parent.name.Contains("Hand") && !TargetCollider.gameObject.activeSelf)
        {
            // 해당 방망이가 갖고있는 특수 능력 스크립트 및 콜라이더가 들어있는 오브젝트 활성화
            TargetCollider.gameObject.SetActive(true);
        }
        else if(!transform.parent.name.Contains("Hand") && TargetCollider.gameObject.activeSelf)
        {
            // 방망이와 상호작용한 오브젝트들 전부 원위치
            TargetCollider.SendMessage("ReturnNomal");
            // 만약 해당 방망이가 갖고있는 특수 능력 오브젝트가 활성화 상태라면 비활성화하기
            TargetCollider.gameObject.SetActive(false);
            // 방망이의 위치를 원래 거치대 위치로 이동
            StartCoroutine(IBackToPosition());
        }
    }
    float duration = 1f;    // 돌아가는데 걸릴 시간
    IEnumerator IBackToPosition()
    {
        // 현재 그 방망이가 놓아졌다면 원래 거치대 위치로 이동
        GameObject targetSpecialBat = TargetCollider.parent.gameObject;
        Vector3 bornPos = this.GetComponent<ObjectPosition>().originPos;
        Vector3 presentPos = transform.position;
        Vector3 bornRot = this.GetComponent<ObjectPosition>().originQuat;
        Vector3 presentRot = transform.rotation.eulerAngles;

        targetSpecialBat.GetComponent<Rigidbody>().isKinematic = true;

        float beginTime = Time.time;
        while (true)
        {
            var t = (Time.time - beginTime) / (duration);

            if (t >= 1f) // 지정한 경과 시간이 지나면 끝
                break;
            t = curve.Evaluate(t);

            if (Vector3.Distance(transform.position, bornPos) > 0.03)
                transform.position = Vector3.Lerp(presentPos, bornPos, t);
            else
                transform.position = bornPos;        // 위치값 보정

            //// 이동
            //targetSpecialBat.transform.position = Vector3.Lerp(targetSpecialBat.transform.position, bornPos, t);

            // 회전
            float currXAngle = Mathf.LerpAngle(presentRot.x, bornRot.x, t);
            float currYAngle = Mathf.LerpAngle(presentRot.y, bornRot.y, t);
            float currZAngle = Mathf.LerpAngle(presentRot.z, bornRot.z, t);
            targetSpecialBat.transform.localEulerAngles = new Vector3(currXAngle, currYAngle, currZAngle);
            yield return null;
        }
        //위치값 보정
        targetSpecialBat.transform.position = bornPos;
        // 위치값 보정
        targetSpecialBat.transform.position = bornPos;

        targetSpecialBat.GetComponent<Rigidbody>().isKinematic = false;

    }

    // 방망이 제작 당시 플레이될 함수 // WeaponStandMananger 스크립트에서 호출함
    public void StartMakeParticle()
    {
        print("파티클 재생");
        foreach (ParticleSystem particle in makeBatParitcles)
        {
            print(particle.name);
            particle.Play();
        }
    }
    public void FinishMakeParticle()
    {
        print("파티클 끄기");

        foreach (ParticleSystem particle in makeBatParitcles)
        {
            print(particle.name);

            particle.Stop();
        }
    }
}
