using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBatManager : MonoBehaviour
{
    // ����̰� �����ɶ� ���� ��ƼŬ
    public ParticleSystem[] makeBatParitcles;
    // ����̰� �۵��� �� ���� ��ƼŬ
    public ParticleSystem[] useBatParticles;
    // ����� �ֿ� ����� ����ִ� Collider
    public Transform TargetCollider;
    // ���� �ӵ��� �ƴ϶� ������������ ȿ���� ���� �ִϸ��̼� Ŀ�� ����
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private string hand;

    private void Update()
    {
        // �θ��̾ ������ �ٲ���ٸ�
        if (transform.parent.name.Contains("Hand") && !TargetCollider.gameObject.activeSelf)
        {
            // �ش� ����̰� �����ִ� Ư�� �ɷ� ��ũ��Ʈ �� �ݶ��̴��� ����ִ� ������Ʈ Ȱ��ȭ
            TargetCollider.gameObject.SetActive(true);
        }
        else if(!transform.parent.name.Contains("Hand") && TargetCollider.gameObject.activeSelf)
        {
            // ����̿� ��ȣ�ۿ��� ������Ʈ�� ���� ����ġ
            TargetCollider.SendMessage("ReturnNomal");
            // ���� �ش� ����̰� �����ִ� Ư�� �ɷ� ������Ʈ�� Ȱ��ȭ ���¶�� ��Ȱ��ȭ�ϱ�
            TargetCollider.gameObject.SetActive(false);
            // ������� ��ġ�� ���� ��ġ�� ��ġ�� �̵�
            StartCoroutine(IBackToPosition());
        }
    }
    float duration = 1f;    // ���ư��µ� �ɸ� �ð�
    IEnumerator IBackToPosition()
    {
        // ���� �� ����̰� �������ٸ� ���� ��ġ�� ��ġ�� �̵�
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

            if (t >= 1f) // ������ ��� �ð��� ������ ��
                break;
            t = curve.Evaluate(t);

            if (Vector3.Distance(transform.position, bornPos) > 0.03)
                transform.position = Vector3.Lerp(presentPos, bornPos, t);
            else
                transform.position = bornPos;        // ��ġ�� ����

            //// �̵�
            //targetSpecialBat.transform.position = Vector3.Lerp(targetSpecialBat.transform.position, bornPos, t);

            // ȸ��
            float currXAngle = Mathf.LerpAngle(presentRot.x, bornRot.x, t);
            float currYAngle = Mathf.LerpAngle(presentRot.y, bornRot.y, t);
            float currZAngle = Mathf.LerpAngle(presentRot.z, bornRot.z, t);
            targetSpecialBat.transform.localEulerAngles = new Vector3(currXAngle, currYAngle, currZAngle);
            yield return null;
        }
        //��ġ�� ����
        targetSpecialBat.transform.position = bornPos;
        // ��ġ�� ����
        targetSpecialBat.transform.position = bornPos;

        targetSpecialBat.GetComponent<Rigidbody>().isKinematic = false;

    }

    // ����� ���� ��� �÷��̵� �Լ� // WeaponStandMananger ��ũ��Ʈ���� ȣ����
    public void StartMakeParticle()
    {
        print("��ƼŬ ���");
        foreach (ParticleSystem particle in makeBatParitcles)
        {
            print(particle.name);
            particle.Play();
        }
    }
    public void FinishMakeParticle()
    {
        print("��ƼŬ ����");

        foreach (ParticleSystem particle in makeBatParitcles)
        {
            print(particle.name);

            particle.Stop();
        }
    }
}
