using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenBat : MonoBehaviour
{
    private bool leftHand;  // ���� ����̸� ����ִ� ���� �޼��̸� true, �������̸� false
    public Transform activeObjectPool;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform activeObjectPoolLeftHand;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform activeObjectPoolRightHand;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ

    public Vector3 handPos;

    private void OnEnable()
    {
        // ���� ����̸� ����ִ� ���� ��� ������ �Ǵ� �� ��ȣ�ۿ�� �� ��ü�� �θ� ������Ʈ ����
        if (transform.parent.gameObject.transform.parent.name.Contains("Left"))
        {
            leftHand = true;
            handPos = ARAVRInput.LHandPosition;
        }
        else
        {
            leftHand = false;
            handPos = ARAVRInput.RHandPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����̿� ��� ��ü�� �ݱ��� �� �� �ִ� ��ü���� Ȯ���ϰ� �ݱ��� ����
        if (other.gameObject.GetComponent<ChangeGoldObject>())
        {
            other.transform.parent = activeObjectPool.transform;
            other.gameObject.GetComponent<ChangeGoldObject>().ChangeGold();
        }
    }

    // SpecialBatManager���� SendMessage�� ȣ��
    private void ReturnNomal()
    {
        // acticeObjectPool�� �ִ� ��� ������Ʈ ���� ���·� ����
        int objectCount = activeObjectPool.transform.childCount;
        GameObject[] handPoolObjects = new GameObject[objectCount];
        for (int i = 0; i < objectCount; i++)
        {
            handPoolObjects[i] = activeObjectPool.transform.GetChild(i).gameObject;
        }

        foreach (GameObject target in handPoolObjects)
        {
            // ��ȣ�ۿ��� ������Ʈ�鿡�� ���� ��ġ���� ���������� ���ư���� ����
            if (target.GetComponent<ObjectPosition>())
                target.GetComponent<ObjectPosition>().ReturnToNomal("changeGoldObject");
        }

        // �տ� ����ִ� ObjectPool�� �ִ� ��� ������Ʈ ���� ���·� ����
        if (leftHand)
        {
            int objectCountRight = activeObjectPoolRightHand.transform.childCount;
            if (objectCountRight > 0)
            {
                // �޼տ� ����̸� ����������� ������ �տ� ��ü�� ���� �� ����
                GameObject handPoolObject = activeObjectPoolRightHand.transform.GetChild(0).gameObject;
                if (handPoolObject.GetComponent<ObjectPosition>())
                    if (handPoolObject.GetComponent<ObjectPosition>().changeGoldObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("changeGoldObject");
            }
        }
        else
        {
            int objectCountLeft = activeObjectPoolLeftHand.transform.childCount;
            if (objectCountLeft > 0)
            {
                // �����տ� ����̸� ����������� �޼տ� ��ü�� ���� �� ����
                GameObject handPoolObject = activeObjectPoolLeftHand.transform.GetChild(0).gameObject;
                if (handPoolObject.GetComponent<ObjectPosition>())
                    if (handPoolObject.GetComponent<ObjectPosition>().changeGoldObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("changeGoldObject");
            }
        }
    }

}
