using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitatableObject : MonoBehaviour
{
    public GameObject model;
    public Renderer mesh;
    private Rigidbody rig;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        mesh = model.gameObject.GetComponent<Renderer>();
    }

    public void ReturnNomal()
    {
        // �ӵ� �ʱ�ȭ
        rig.velocity = Vector3.zero;
        rig.isKinematic = false;
        // �߷��� ��Ȱ��ȭ��� Ȱ��ȭ
        rig.useGravity = true;
    }
}
