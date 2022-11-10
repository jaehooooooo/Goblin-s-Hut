using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeButterflyObject : MonoBehaviour
{
    // ���� ����Ʈ
    public GameObject changeButterflyParticle;
    // ���� ����
    public GameObject model;
    //
    private Rigidbody rig;
    private ObjectPosition objectPosition;

    private bool isChange;
    private float changeEnoughTime = 3f; // ���� ����Ʈ�� ����� �ð�

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        objectPosition = GetComponent<ObjectPosition>();
    }

    // ���� ����Ʈ�� ������ �Լ�
    public void ChangeButterfly()
    {
        // ���� ����Ʈ�� ��������� ���ٸ�
        if (!objectPosition.changeButterflyObject)
        {
            // ���� ��ȣ�ۿ� Ȱ��ȭ �˸�
            objectPosition.changeButterflyObject = true;

            // ���� �پ��ִ� ���¶�� �Ҳ��� ���� ���·� �����϶�� ����
            if (objectPosition.inflammableObject)
                objectPosition.ReturnToNomal("inflammableObject");

            isChange = true;

            // ���� ����Ʈ Ȱ��ȭ
            changeButterflyParticle.transform.position = transform.position + new Vector3(0, 1, 0);
            changeButterflyParticle.SetActive(true);
            // ������ٵ� ����
            rig.isKinematic = true;
            // ���� ���� ������
            model.SetActive(false);
            // ���� �ð� �� ȣ��
            Invoke("ChangeEnough", changeEnoughTime);
        }

    }
    // ���� ����Ʈ�� ������ ����� �Լ�
    private void ChangeEnough()
    {
        // ����Ʈ ��Ȱ��ȭ
        changeButterflyParticle.SetActive(false);
    }

    // �ٽ� �� ������ �Լ�
    public void ReturnNomal()
    {
        // ���� ���� Ȱ��ȭ
        model.SetActive(true);
        // ������ٵ� �ѱ�
        rig.isKinematic = false;
        // ����Ʈ�� Ȱ��ȭ�� ���¶��
        if (changeButterflyParticle.activeSelf)
            ChangeEnough();
    }
}
