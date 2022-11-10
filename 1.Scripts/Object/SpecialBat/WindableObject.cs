using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindableObject : MonoBehaviour
{
    private ObjectPosition objectPosition;
    private Rigidbody rig;
    // ���ư��� �� (���Կ� �ݺ��)
    public float flyPower;
    private bool isFly;
    private float duration = 3f; // �ٽ� ���ư� �� �ִ� ���·� ���ƿ� �ð�

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
        // �ӵ� �ʱ�ȭ
        rig.velocity = Vector3.zero;
    }

}
