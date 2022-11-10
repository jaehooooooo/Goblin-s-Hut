using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // ���� ����
    public float moveSpeed = 5f;      // �̵� �ӵ�
    public float gravity = -9.8f;     // �߷� 
    private float yVelocity = 0;      // ���� �ӵ�

    public bool isMove = true;
    // ������Ʈ
    CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // �÷��̾ �����̸� �ȵǴ� ��Ȳ�϶�
        if(!GameManager.instance.playerMove)
        {
            if (cc.enabled)
                cc.enabled = false;
            return;
        }
        else
        {
            if (!cc.enabled)
                cc.enabled = true;
        }

        // ��ƽ�̵�
        StickMove();
    }

    private void StickMove()
    {
        if (!isMove || !cc.enabled)
            return;

        // ������� ��ƽ �Է��� �޾Ƽ�
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");
        // ������ ����� (������ǥ)
        Vector3 dir = new Vector3(h, 0, v);
        // ī�޶��� �������� ���� (������ǥ)
        dir = Camera.main.transform.TransformDirection(dir);
        // �߷�
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        // �ٴڿ� ��������� �����׷�
        if (cc.isGrounded)
            yVelocity = 0f;
        // �̵�
        cc.Move(dir * moveSpeed * Time.deltaTime);
    }
}
