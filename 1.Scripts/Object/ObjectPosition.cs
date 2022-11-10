using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPosition : MonoBehaviour
{
    // 모든 오브젝트가 갖고있을 변수 값
    public string objectName;
    public Vector3 originPos;
    public Vector3 handPos;
    public Vector3 originQuat;
    public Vector3 handQuat;
    public GameObject originParentObject;
    public float objectWeight;
    public float objectSize;

    // 방망이와의 상호작용 여부
    public bool inflammableObject = false;
    public bool windableObject = false;
    public bool changeGoldObject = false;
    public bool changeButterflyObject = false;
    public bool gravityFieldObject = false;
    public bool inGravityFieldObject = false;
    public bool levitatableObject = false;
    // 현재 손에 움켜 쥐어있는지 여부
    public bool isGrabbed = false;

    // 본인의 원래 형태로 돌아가는 함수
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
            // 아무런 상호작용도 안하고 있고, 손에 잡고 있는 상태도 아니라면 부모 변경 후 원래 위치로 복귀
            // 부모 변경
            transform.parent = originParentObject.transform;
            // 원래 위치값으로 이동
            transform.position = originPos;
            transform.localEulerAngles = originPos;
        }
    }

    public bool CheckObjectSituation()
    {
        // 모든 상태변화가 해제되었는지 체크
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
