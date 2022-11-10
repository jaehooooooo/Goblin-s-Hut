using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : MonoBehaviour
{
    // �߶����ų� �μ��� ������Ʈ 
    // ������ �������� ���ư� ����
    // ��ȣ�ۿ��� ������Ʈ��
    public GameObject[] objectsPool;
    // ����� �ӵ�
    public float speed;

    public void BrokenStart(float weaponVelocity)
    {
        foreach ( GameObject brokenobject in objectsPool)
        {
            // ������ ���� ����
            Vector3 direction = Random.insideUnitSphere;
            // ���ư��� 
            Rigidbody rb = brokenobject.GetComponent<Rigidbody>();
            rb.velocity = direction * weaponVelocity;
        }
    }

    public void BrokenEnd()
    {
        foreach (GameObject brokenobject in objectsPool)
        {
            brokenobject.transform.localPosition = Vector3.zero;
            print("��ġ �ʱ�ȭ");
        }
    }



    //private void enable()
    //{
    //    // ������ ���� ����
    //    Vector3 direction = Random.insideUnitSphere;
    //    // ���ư��� 
    //    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
    //    rb.velocity = direction * speed;

    //    StartCoroutine(IResetPosition());
    //}

    //IEnumerator IResetPosition()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    transform.position = Vector3.zero;
    //    print("��ġ �ʱ�ȭ");
    //}
}
