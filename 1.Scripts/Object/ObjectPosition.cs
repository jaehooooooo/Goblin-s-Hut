using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPosition : MonoBehaviour
{
    // ��� ������Ʈ�� �������� ���� ��
    public string objectName;
    public Vector3 originPos;
    public Vector3 handPos;
    public Vector3 originQuat;
    public Vector3 handQuat;
    public GameObject originParentObject;
    public float objectWeight;
    public float objectSize;

    // ����̿��� ��ȣ�ۿ� ����
    public bool inflammableObject = false;
    public bool windableObject = false;
    public bool changeGoldObject = false;
    public bool changeButterflyObject = false;
    public bool gravityFieldObject = false;
    public bool inGravityFieldObject = false;
    public bool levitatableObject = false;
    // ���� �տ� ���� ����ִ��� ����
    public bool isGrabbed = false;

    // ������ ���� ���·� ���ư��� �Լ�
    public void ReturnToNomal(string clearSkill)
    {
        switch(clearSkill)
        {
            case "inflammableObject":
                {
                    inflammableObject = false;
                    transform.GetComponent<InflammableObject>().ReturnNomal();
                    break;
                }
            case "windableObject":
                {
                    windableObject = false;
                    transform.GetComponent<WindableObject>().ReturnNomal();
                    break;
                }
            case "changeGoldObject":
                {
                    changeGoldObject = false;
                    transform.GetComponent<ChangeGoldObject>().ReturnNomal();
                    break;
                }
            case "changeButterflyObject":
                {
                    changeButterflyObject = false;
                    transform.GetComponent<ChangeButterflyObject>().ReturnNomal();
                    break;
                }
            case "gravityFieldObject":
                {
                    gravityFieldObject = false;
                    inGravityFieldObject = false;
                    transform.GetComponent<LevitatableObject>().ReturnNomal();
                    break;
                }
            case "levitatableObject":
                {
                    levitatableObject = false;

                    transform.GetComponent<LevitatableObject>().ReturnNomal();
                    break;
                }
        }
        if(!inflammableObject && !windableObject && !changeGoldObject && !changeButterflyObject 
            && !gravityFieldObject && !levitatableObject && !isGrabbed)
        {
            // �ƹ��� ��ȣ�ۿ뵵 ���ϰ� �ְ�, �տ� ��� �ִ� ���µ� �ƴ϶�� �θ� ���� �� ���� ��ġ�� ����
            // �θ� ����
            transform.parent = originParentObject.transform;
            // ���� ��ġ������ �̵�
            transform.position = originPos;
            transform.localEulerAngles = originPos;
        }
    }

    public bool CheckObjectSituation()
    {
        // ��� ���º�ȭ�� �����Ǿ����� üũ
        if(inflammableObject || windableObject || changeGoldObject || changeButterflyObject || gravityFieldObject || levitatableObject)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
