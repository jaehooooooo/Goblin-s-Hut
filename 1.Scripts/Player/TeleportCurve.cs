using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    // 텔레포트 지점들을 기억할 리스트
    List<Vector3> points = new List<Vector3>();

    // 텔레포트를 표시할 UI
    public Transform teleportCircleUI;
    // 텔레포트 지점에 표시될 UI
    public Transform[] teleportPointTrianglesUI;

    // 선을 그릴 라인 랜더러
    public LineRenderer lr;
    // 최초 텔레포트 UI 크기
    Vector3 originScale = Vector3.one * 0.01f;
    // 커브의 부드러운 정도
    public int lineSmooth = 40;
    // 커브의 길이
    public float curveLength = 50;
    // 커브의 중력
    public float gravity = -60;
    // 곡선 시뮬레이션의 간격 및 시간
    public float simulateTime = 0.01f;
    // 곡선을 이루는 점들을 기억할 리스트
    List<Vector3> lines = new List<Vector3>();
    // 텔레포트 가능 여부 bool값
    private bool teleportable;
    // 현재 텔레포트 중인지 여부 
    private bool isTeleport;
    // 한 번에 텔레포트 할 거리
    public float oneTimeDistance;
    // 라인 렌더러 색 변경
    public Renderer lineMesh;
    private MaterialPropertyBlock block;

    // Start is called before the first frame update
    void Start()
    {
        // 시작할때 비활성화
        teleportCircleUI.gameObject.SetActive(false);
        // 라인렌더러 컴포넌트 얻기
        //lr = GetComponent<LineRenderer>();
        // 라인 렌더러의 선 너비 지정
        lr.startWidth = 0.0f;
        lr.endWidth = 0.1f;
        // 라인 렌더러의 색 변경
        lineMesh = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어가 움직이면 안되는 상황일때
        if (!GameManager.instance.playerMove)
            return;

        // 왼쪽 컨트롤러의 One 버튼을 누르면
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch) && !isTeleport)
        {
            // 라인렌더러 컴포넌트 활성화
            lr.enabled = true;
        }
        // 왼쪽 컨트롤러의 One 버튼에서 손을 떼면
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 추가 텔레포트 불가
            isTeleport = true;
            // 라인렌더러 비활성화
            lr.enabled = false;
            // 텔레포트 UI가 활성화 되어있는 상태라면 + 텔레포트 할 수 있는 위치가 맞다면
            if (teleportCircleUI.gameObject.activeSelf && teleportable)
            {
                // 텔레포트 경로 그려주고 그 위치 모양대로 텔레포트
                DrawTeleportCourse();
            }
            // 텔레포트 UI 비활성화
            teleportCircleUI.gameObject.SetActive(false);
            isTeleport = false;
        }
        // 왼쪽 컨트롤러의 One 버튼을 누르고 있을때
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 주어진 길이 크기의 커브를 만든다
            MakeLines();
        }
    }

    // 라인 렌더러를 이용해 점을 만들고 선을 그린다
    private void MakeLines()
    {
        // 리스트에 담긴 위치 정보들을 비워준다
        lines.RemoveRange(0, lines.Count);
        // 선이 진행될 방향을 정한다
        Vector3 dir = ARAVRInput.LHandDirection * curveLength;
        // 선이 그려질 위치의 초깃값을 설정한다
        Vector3 pos = ARAVRInput.LHandPosition;
        // 최초 위치를 리스트에 담는다
        lines.Add(pos);

        // lineSmooth 개수만큼 반복한다
        for (int i = 0; i < lineSmooth; i++)
        {
            // 현재 위치 기억
            Vector3 lastPos = pos;
            // 중력을 적용한 속도 계산
            // v = v0 + at
            dir.y += gravity * simulateTime;
            // 등속 운동으로 다음 위치 계산
            // P = P0 + vt
            pos += dir * simulateTime;
            // Ray 충돌 체크가 일어났으면
            if (CheckHitRay(lastPos, ref pos))
            {
                // 충돌 지점을 등록하고 종료
                lines.Add(pos);
                break;
            }
            else
            {
                // 텔레포트 UI 비활성화
                teleportCircleUI.gameObject.SetActive(false);
            }

            // 구한 위치를 등록
            lines.Add(pos);
        }
        // 라인 렌더러가 표현할 점의 개수를 등록된 개수의 크기로 할당
        lr.positionCount = lines.Count;
        // 라인 렌더러에 구해진 점의 정보를 지정
        lr.SetPositions(lines.ToArray());
    }

    private bool CheckHitRay(Vector3 lastPos, ref Vector3 pos)
    {
        // 앞 점 lastPos에서 다음 점 pos로 향하는 벡터 계산
        Vector3 rayDir = pos - lastPos;
        Ray ray = new Ray(lastPos, rayDir);
        RaycastHit hitInfo;

        // Raycast할 때 레이의 크기를 앞 점과 다음 점 사이의 거리로 한정한다
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            // 다음 점의 위치를 충돌할 지점으로 설정
            pos = hitInfo.point;

            int layer = LayerMask.NameToLayer("Terrain");
            // Terrain레이어와 충돌했을 경우에만 텔레포트 UI가 표시되도록 설정
            if (hitInfo.transform.gameObject.layer == layer)
            {
                // 텔레포트 UI 활성화
                teleportCircleUI.gameObject.SetActive(true);
                // 텔레포트 UI의 위치 지정
                teleportCircleUI.position = pos;
                // 텔레포트 UI의 방향 설정
                teleportCircleUI.forward = hitInfo.normal;
                float distance = (pos - ARAVRInput.LHandPosition).magnitude;
                // 텔레포트 UI가 보일 크기 지정
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);

                // 텔레포트 UI 색 변경
                CheckObstruction(pos);
            }
            return true;
        }
        return false;
    }

    // 텔레포트 하려는 위치와 내 위치 사이에 장애물이 있나 확인
    private void CheckObstruction(Vector3 pos)
    {
        // 텔레포트 UI와 플레이어를 연결하는 Ray 발사

        // 앞 점 lastPos에서 다음 점 pos로 향하는 벡터 계산
        Vector3 rayDir = pos - ARAVRInput.LHandPosition;
        Ray ray = new Ray(ARAVRInput.LHandPosition, rayDir);
        RaycastHit hitInfo;

        // Raycast할 때 레이의 크기를 앞 점과 다음 점 사이의 거리로 한정한다
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            int layer = LayerMask.NameToLayer("Terrain");

            // 만약 레이에 물체가 인식된다면
            if (hitInfo.transform.gameObject.layer != layer)
            {
                // 텔레포트 UI를 빨간색으로 변경
                lineMesh.SetPropertyBlock(block);
                int id = Shader.PropertyToID("_Color");
                block.SetColor(id, Color.red);
                lineMesh.SetPropertyBlock(block);
                // 텔레포트 불가능 표시
                teleportable = false;
            }
            else
            {
                // 텔레포트 UI를 파란색으로 변경
                lineMesh.SetPropertyBlock(block);
                int id = Shader.PropertyToID("_Color");
                block.SetColor(id, Color.blue);
                lineMesh.SetPropertyBlock(block);

                teleportable = true;
            }
        }
    }

    private void DrawTeleportCourse()
    {

        // 텔레포트가 가능한 위치의 경우 텔레포트 경로 그려주기

        // 현재 플레이어 위치에서 텔레포트 UI위치까지 바닥에 경로 그리기

        // 플레이어 위치에서 수직으로 아래로 향하는 Ray 발사 후 바닥과 맞닺는 점 찾기
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;

        Vector3 pos = new Vector3();
        int layer = LayerMask.NameToLayer("Terrian");
        if(Physics.Raycast(ray,out hitInfo,3f,layer))
        {
            pos = hitInfo.point;
        }

        // pos부터 텔레포트 UI까지 거리의 중간 지점마다 표시하기
        Vector3 dir = (teleportCircleUI.position - pos).normalized; // 방향벡터 구하기
        float distance = (pos - teleportCircleUI.position).magnitude;   // 벡터의 크기(길이) 구하기

        // 이동해야될 횟수 (몫)
        int quotient = (int)(distance / oneTimeDistance);
        // 리스트에 담긴 위치 정보들을 비워준다
        points.RemoveRange(0, points.Count);
        print(quotient + "이동해야될 횟수");

        // 이동해야 할 위치를 List에 저장
        for (int i = 0; i < quotient; i++)
        {
            // 방향벡터에 이동 거리 곱하기
            Vector3 checkPoint = dir * i * oneTimeDistance + pos;
            // 체크포인트 위치를 리스트에 담는다
            points.Add(checkPoint);
            print(checkPoint + "체크포인트" + i);
        }

        // 최종 텔레포트 포지션 위치를 리스트에 담는다
        points.Add(teleportCircleUI.position);
        print(teleportCircleUI.position + " 최종위치");

        foreach (Vector3 checkPos in points)
        {
            print(checkPos);
        }
        MoveTeleportPoint();
    }

    private void MoveTeleportPoint()
    {
        GetComponent<PlayerMove>().isMove = false;
        GetComponent<CharacterController>().enabled = false;
        //// 텔레포트 UI 위치로 순간이동
        //transform.position = teleportCircleUI.position + Vector3.up;
        StartCoroutine(IDrawPoint());
    }

    IEnumerator IDrawPoint()
    {
        yield return null;
        // 텔레포트 할 지점 표시해주기
        for(int i = 1; i<points.Count; i++)
        {
            // 텔레포트 UI 활성화
            teleportPointTrianglesUI[i].gameObject.SetActive(true);
            // 텔레포트 UI의 위치 지정
            teleportPointTrianglesUI[i].position = points[i];
            // 텔레포트 UI의 방향 설정
            teleportPointTrianglesUI[i].forward = Vector3.up;
            float distance = (points[i] - ARAVRInput.LHandPosition).magnitude;
            // 텔레포트 UI가 보일 크기 지정
            teleportPointTrianglesUI[i].localScale = originScale * Mathf.Max(1, distance);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.1f);

        // 텔레포트 실행하기
        for (int i = 1; i < points.Count; i++)
        {
            print(teleportPointTrianglesUI[i].position + "UI 위치" + i);
            Vector3 vec = teleportPointTrianglesUI[i].position;
            transform.position = vec + Vector3.up;
            // 텔레포트 UI 비활성화
            teleportPointTrianglesUI[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }

        GetComponent<CharacterController>().enabled = true;
        GetComponent<PlayerMove>().isMove = true;
    }
}
