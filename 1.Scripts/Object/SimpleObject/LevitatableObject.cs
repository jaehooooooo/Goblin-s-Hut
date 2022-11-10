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
        // 속도 초기화
        rig.velocity = Vector3.zero;
        rig.isKinematic = false;
        // 중력이 비활성화라면 활성화
        rig.useGravity = true;
    }
}
