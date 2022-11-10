using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{

    public LayerMask layer;
    Rigidbody rig;

    public float power;         // �� �ӵ� ���� ��ġ
    public bool hitNow = false;         // ���� ĥ �� �ִ� ���� ����
    public GameObject activeObjectPool;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    public void ReadyToHit()
    {
        rig.velocity = Vector3.zero;
        rig.useGravity = false;
        // ī�޶� �������� ���� ��ġ�� �� ��ġ��Ű��
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0, -0.15f, 0);
    }

    // ���� ����̿� ������� ���� �˷��� �Լ�
    public void HitAction(Vector3 dir, float batSpeed)
    {
        Vector3 force = dir * batSpeed * power;
        rig.AddForce(force, ForceMode.Force);
        rig.useGravity = true;

        // �ڵ� ��Ȱ��ȭ �ڵ�
        CancelInvoke();
        Invoke("SetActive", 5f);
    }

    // ��Ȱ��ȭ�ϴ� �ڵ�
    private void SetActive()
    {
        hitNow = false;
        this.gameObject.SetActive(false);
    }

    // � ��ü�� ������ ���� ��ü�� ���� �ľ� �� �ľ��� ��ü�� ��Ż �� ������ ��Ÿ�� ����Ʈ �����϶�� ����
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<InflammableObject>())
        {
            collision.gameObject.GetComponent<InflammableObject>().Burn();
            collision.transform.parent = activeObjectPool.transform;

        }

        if (!hitNow && collision.gameObject.name == "FireBat")
        {
            hitNow = true;
            GameObject batCollider = collision.transform.GetChild(1).gameObject;
            float speed = batCollider.gameObject.GetComponent<FireBat>().nowSpeed;
            // ����̿� ����
            // ����� �ӷ��� �޾ƿͼ� ���� ���� ��
            HitAction(collision.contacts[0].normal,speed);
        }
    }
}
