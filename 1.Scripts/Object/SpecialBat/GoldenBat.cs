using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenBat : MonoBehaviour
{
    private bool leftHand;  // 현재 방망이를 잡고있는 손이 왼손이면 true, 오른손이면 false
    public Transform activeObjectPool;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform activeObjectPoolLeftHand;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform activeObjectPoolRightHand;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀

    public Vector3 handPos;

    private void OnEnable()
    {
        // 현재 방망이를 잡고있는 손이 어느 손인지 판단 후 상호작용시 들어갈 물체의 부모 오브젝트 설정
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
        // 방망이와 닿는 물체가 금괴가 될 수 있는 물체인지 확인하고 금괴로 변경
        if (other.gameObject.GetComponent<ChangeGoldObject>())
        {
            other.transform.parent = activeObjectPool.transform;
            other.gameObject.GetComponent<ChangeGoldObject>().ChangeGold();
        }
    }

    // SpecialBatManager에서 SendMessage로 호출
    private void ReturnNomal()
    {
        // acticeObjectPool에 있는 모든 오브젝트 원래 상태로 복귀
        int objectCount = activeObjectPool.transform.childCount;
        GameObject[] handPoolObjects = new GameObject[objectCount];
        for (int i = 0; i < objectCount; i++)
        {
            handPoolObjects[i] = activeObjectPool.transform.GetChild(i).gameObject;
        }

        foreach (GameObject target in handPoolObjects)
        {
            // 상호작용한 오브젝트들에게 원래 위치값과 설정값으로 돌아가라고 전달
            if (target.GetComponent<ObjectPosition>())
                target.GetComponent<ObjectPosition>().ReturnToNomal("changeGoldObject");
        }

        // 손에 잡고있는 ObjectPool에 있는 모든 오브젝트 원래 상태로 복귀
        if (leftHand)
        {
            int objectCountRight = activeObjectPoolRightHand.transform.childCount;
            if (objectCountRight > 0)
            {
                // 왼손에 방망이를 잡고있음으로 오른쪽 손에 물체를 잡을 수 있음
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
                // 오른손에 방망이를 잡고있음으로 왼손에 물체를 잡을 수 있음
                GameObject handPoolObject = activeObjectPoolLeftHand.transform.GetChild(0).gameObject;
                if (handPoolObject.GetComponent<ObjectPosition>())
                    if (handPoolObject.GetComponent<ObjectPosition>().changeGoldObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("changeGoldObject");
            }
        }
    }

}
