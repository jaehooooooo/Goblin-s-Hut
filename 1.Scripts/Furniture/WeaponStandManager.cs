using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStandManager : MonoBehaviour
{
    // 변수
    enum SpecialBats
    {
        GiantBat, WindBat, ButterflyBat, DrawingBat, FireBat, LevitationBat, GravityFieldBat, GoldBarBat
    }

    // 제작 가능 방망이 8종류
    public Transform[] specialBats;

    // 맨 처음 생겨날 방망이의 위치
    public Vector3 bornPos;
    public Vector3 bornQuat;
    // 떠오를 방망이의 위치
    public Vector3 mixedBoxCenterPos;

    // 같은 속도가 아니라 점점빨라지는 효과를 위해 애니메이션 커브 생성
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    // 
    public float duration;


    // 초기화

    // 기능
    public void MadeSpecialBat(int target)
    {
        StartCoroutine(StartEffect(target));
        StartCoroutine(ShowSpecialBat(target));
    }

    IEnumerator StartEffect(int target)
    {
        // 믹서박스가 빛나고 등의 효과 재생
        // 방망이에 맞는 색이 빛나면 더 좋을듯?
        //GameObject targetBat = specialBats[target].gameObject;

        print("믹서박스 빛나요");
        yield return null;
    }

    IEnumerator ShowSpecialBat(int target)
    {
        GameObject targetBat = specialBats[target].gameObject;
        // 해당 방망이의 파티클 효과 재생
        targetBat.GetComponent<SpecialBatManager>().StartMakeParticle();

        // 해당 방망이가 활성화 되어있지 않다면 = 만든적이 없는 방망이라면
        if (!targetBat.GetComponent<ObjectPosition>().isGrabbed)
        {
            print("방망이 없음");
            // 현재 그 방망이가 활성화되어있지 않다면 방망이 솟아오르고 자기 자리 찾아서 날아가는 효과
            targetBat.SetActive(true);
            targetBat.GetComponent<Rigidbody>().isKinematic = true;
            targetBat.transform.localPosition = bornPos;
            targetBat.transform.localEulerAngles = bornQuat;

            float beginTime = Time.time;
            while (true)
            {
                var t = (Time.time - beginTime) / duration;
                print(t);

                if (t >= 1f) // 지정한 경과 시간이 지나면 끝
                    break;
                t = curve.Evaluate(t);

                // 이동
                targetBat.transform.position = Vector3.Lerp(targetBat.transform.position, mixedBoxCenterPos, t);

                yield return null;
            }
            // 위치값 보정
            targetBat.transform.position = mixedBoxCenterPos;

            yield return new WaitForSeconds(0.5f);

            beginTime = Time.time;
            while (true)
            {
                var t = (Time.time - beginTime) / (duration * 2);

                if (t >= 1f) // 지정한 경과 시간이 지나면 끝
                    break;
                t = curve.Evaluate(t);

                // 이동
                targetBat.transform.position = Vector3.Lerp(targetBat.transform.position, targetBat.GetComponent<ObjectPosition>().originPos, t);

                // 회전
                float currXAngle = Mathf.LerpAngle(targetBat.transform.eulerAngles.x, targetBat.GetComponent<ObjectPosition>().originQuat.x, t);
                float currYAngle = Mathf.LerpAngle(targetBat.transform.eulerAngles.y, targetBat.GetComponent<ObjectPosition>().originQuat.y, t);
                float currZAngle = Mathf.LerpAngle(targetBat.transform.eulerAngles.z, targetBat.GetComponent<ObjectPosition>().originQuat.z, t);
                targetBat.transform.localEulerAngles = new Vector3(currXAngle, currYAngle, currZAngle);
                //targetBat.transform.localEulerAngles = Vector3.(targetBat.transform.localEulerAngles, targetBat.GetComponent<ObjectPosition>().originQuat, t);
                yield return null;
                print(targetBat.transform.localEulerAngles);
            }
            // 위치값 보정
            targetBat.transform.position = targetBat.GetComponent<ObjectPosition>().originPos;
            targetBat.transform.localEulerAngles = targetBat.GetComponent<ObjectPosition>().originQuat;
            
            // 해당 방망이 중력 활성화
            targetBat.GetComponent<Rigidbody>().isKinematic = false;
        }
        yield return new WaitForSeconds(1f);

        // 해당 방망이의 파티클 효과 끄기
        targetBat.GetComponent<SpecialBatManager>().FinishMakeParticle();

        // 마지막 방망이 위치 찾고 그 자리가 빛나는 효과
        print("해당 방망이 위치 빛나요");

    }
}
