using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindableObject : MonoBehaviour
{
    private ObjectPosition objectPosition;
    private Rigidbody rig;
    // 날아가는 힘 (무게와 반비례)
    public float flyPower;
    private bool isFly;
    private float duration = 3f; // 다시 날아갈 수 있는 상태로 돌아올 시간

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        objectPosition = GetComponent<ObjectPosition>();
    }

    public void FlyHigh(Vector3 forcedir)
    {
        if(!isFly)
        {
            GetComponent<ObjectPosition>().windableObject = true;
            isFly = true;
            rig.AddForce(forcedir * 300 * 1/objectPosition.objectWeight, ForceMode.Force);
            StartCoroutine(IFly());
        }
    }

    IEnumerator IFly()
    {
        yield return new WaitForSeconds(duration);
        isFly = false;
    }
    public void ReturnNomal()
    {
        isFly = false;
        // 속도 초기화
        rig.velocity = Vector3.zero;
    }

}
