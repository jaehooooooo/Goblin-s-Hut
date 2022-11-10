using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 변수 선언
    public float moveSpeed = 5f;      // 이동 속도
    public float gravity = -9.8f;     // 중력 
    private float yVelocity = 0;      // 수직 속도

    public bool isMove = true;
    // 컴포넌트
    CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 플레이어가 움직이면 안되는 상황일때
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

        // 스틱이동
        StickMove();
    }

    private void StickMove()
    {
        if (!isMove || !cc.enabled)
            return;

        // 사용자의 스틱 입력을 받아서
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");
        // 방향을 만들고 (월드좌표)
        Vector3 dir = new Vector3(h, 0, v);
        // 카메라쪽 방향으로 변경 (로컬좌표)
        dir = Camera.main.transform.TransformDirection(dir);
        // 중력
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        // 바닥에 닿아있으면 수직항력
        if (cc.isGrounded)
            yVelocity = 0f;
        // 이동
        cc.Move(dir * moveSpeed * Time.deltaTime);
    }
}
