using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStandManager : MonoBehaviour
{
    // ����
    enum SpecialBats
    {
        GiantBat, WindBat, ButterflyBat, DrawingBat, FireBat, LevitationBat, GravityFieldBat, GoldBarBat
    }

    // ���� ���� ����� 8����
    public Transform[] specialBats;

    // �� ó�� ���ܳ� ������� ��ġ
    public Vector3 bornPos;
    public Vector3 bornQuat;
    // ������ ������� ��ġ
    public Vector3 mixedBoxCenterPos;

    // ���� �ӵ��� �ƴ϶� ������������ ȿ���� ���� �ִϸ��̼� Ŀ�� ����
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    // 
    public float duration;


    // �ʱ�ȭ

    // ���
    public void MadeSpecialBat(int target)
    {
        StartCoroutine(StartEffect(target));
        StartCoroutine(ShowSpecialBat(target));
    }

    IEnumerator StartEffect(int target)
    {
        // �ͼ��ڽ��� ������ ���� ȿ�� ���
        // ����̿� �´� ���� ������ �� ������?
        //GameObject targetBat = specialBats[target].gameObject;

        print("�ͼ��ڽ� ������");
        yield return null;
    }

    IEnumerator ShowSpecialBat(int target)
    {
        GameObject targetBat = specialBats[target].gameObject;
        // �ش� ������� ��ƼŬ ȿ�� ���
        targetBat.GetComponent<SpecialBatManager>().StartMakeParticle();

        // �ش� ����̰� Ȱ��ȭ �Ǿ����� �ʴٸ� = �������� ���� ����̶��
        if (!targetBat.GetComponent<ObjectPosition>().isGrabbed)
        {
            print("����� ����");
            // ���� �� ����̰� Ȱ��ȭ�Ǿ����� �ʴٸ� ����� �ھƿ����� �ڱ� �ڸ� ã�Ƽ� ���ư��� ȿ��
            targetBat.SetActive(true);
            targetBat.GetComponent<Rigidbody>().isKinematic = true;
            targetBat.transform.localPosition = bornPos;
            targetBat.transform.localEulerAngles = bornQuat;

            float beginTime = Time.time;
            while (true)
            {
                var t = (Time.time - beginTime) / duration;
                print(t);

                if (t >= 1f) // ������ ��� �ð��� ������ ��
                    break;
                t = curve.Evaluate(t);

                // �̵�
                targetBat.transform.position = Vector3.Lerp(targetBat.transform.position, mixedBoxCenterPos, t);

                yield return null;
            }
            // ��ġ�� ����
            targetBat.transform.position = mixedBoxCenterPos;

            yield return new WaitForSeconds(0.5f);

            beginTime = Time.time;
            while (true)
            {
                var t = (Time.time - beginTime) / (duration * 2);

                if (t >= 1f) // ������ ��� �ð��� ������ ��
                    break;
                t = curve.Evaluate(t);

                // �̵�
                targetBat.transform.position = Vector3.Lerp(targetBat.transform.position, targetBat.GetComponent<ObjectPosition>().originPos, t);

                // ȸ��
                float currXAngle = Mathf.LerpAngle(targetBat.transform.eulerAngles.x, targetBat.GetComponent<ObjectPosition>().originQuat.x, t);
                float currYAngle = Mathf.LerpAngle(targetBat.transform.eulerAngles.y, targetBat.GetComponent<ObjectPosition>().originQuat.y, t);
                float currZAngle = Mathf.LerpAngle(targetBat.transform.eulerAngles.z, targetBat.GetComponent<ObjectPosition>().originQuat.z, t);
                targetBat.transform.localEulerAngles = new Vector3(currXAngle, currYAngle, currZAngle);
                //targetBat.transform.localEulerAngles = Vector3.(targetBat.transform.localEulerAngles, targetBat.GetComponent<ObjectPosition>().originQuat, t);
                yield return null;
                print(targetBat.transform.localEulerAngles);
            }
            // ��ġ�� ����
            targetBat.transform.position = targetBat.GetComponent<ObjectPosition>().originPos;
            targetBat.transform.localEulerAngles = targetBat.GetComponent<ObjectPosition>().originQuat;
            
            // �ش� ����� �߷� Ȱ��ȭ
            targetBat.GetComponent<Rigidbody>().isKinematic = false;
        }
        yield return new WaitForSeconds(1f);

        // �ش� ������� ��ƼŬ ȿ�� ����
        targetBat.GetComponent<SpecialBatManager>().FinishMakeParticle();

        // ������ ����� ��ġ ã�� �� �ڸ��� ������ ȿ��
        print("�ش� ����� ��ġ ������");

    }
}
