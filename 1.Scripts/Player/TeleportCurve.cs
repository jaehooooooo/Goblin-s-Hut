using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    // �ڷ���Ʈ �������� ����� ����Ʈ
    List<Vector3> points = new List<Vector3>();

    // �ڷ���Ʈ�� ǥ���� UI
    public Transform teleportCircleUI;
    // �ڷ���Ʈ ������ ǥ�õ� UI
    public Transform[] teleportPointTrianglesUI;

    // ���� �׸� ���� ������
    public LineRenderer lr;
    // ���� �ڷ���Ʈ UI ũ��
    Vector3 originScale = Vector3.one * 0.01f;
    // Ŀ���� �ε巯�� ����
    public int lineSmooth = 40;
    // Ŀ���� ����
    public float curveLength = 50;
    // Ŀ���� �߷�
    public float gravity = -60;
    // � �ùķ��̼��� ���� �� �ð�
    public float simulateTime = 0.01f;
    // ��� �̷�� ������ ����� ����Ʈ
    List<Vector3> lines = new List<Vector3>();
    // �ڷ���Ʈ ���� ���� bool��
    private bool teleportable;
    // ���� �ڷ���Ʈ ������ ���� 
    private bool isTeleport;
    // �� ���� �ڷ���Ʈ �� �Ÿ�
    public float oneTimeDistance;
    // ���� ������ �� ����
    public Renderer lineMesh;
    private MaterialPropertyBlock block;

    // Start is called before the first frame update
    void Start()
    {
        // �����Ҷ� ��Ȱ��ȭ
        teleportCircleUI.gameObject.SetActive(false);
        // ���η����� ������Ʈ ���
        //lr = GetComponent<LineRenderer>();
        // ���� �������� �� �ʺ� ����
        lr.startWidth = 0.0f;
        lr.endWidth = 0.1f;
        // ���� �������� �� ����
        lineMesh = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        // �÷��̾ �����̸� �ȵǴ� ��Ȳ�϶�
        if (!GameManager.instance.playerMove)
            return;

        // ���� ��Ʈ�ѷ��� One ��ư�� ������
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch) && !isTeleport)
        {
            // ���η����� ������Ʈ Ȱ��ȭ
            lr.enabled = true;
        }
        // ���� ��Ʈ�ѷ��� One ��ư���� ���� ����
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // �߰� �ڷ���Ʈ �Ұ�
            isTeleport = true;
            // ���η����� ��Ȱ��ȭ
            lr.enabled = false;
            // �ڷ���Ʈ UI�� Ȱ��ȭ �Ǿ��ִ� ���¶�� + �ڷ���Ʈ �� �� �ִ� ��ġ�� �´ٸ�
            if (teleportCircleUI.gameObject.activeSelf && teleportable)
            {
                // �ڷ���Ʈ ��� �׷��ְ� �� ��ġ ����� �ڷ���Ʈ
                DrawTeleportCourse();
            }
            // �ڷ���Ʈ UI ��Ȱ��ȭ
            teleportCircleUI.gameObject.SetActive(false);
            isTeleport = false;
        }
        // ���� ��Ʈ�ѷ��� One ��ư�� ������ ������
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // �־��� ���� ũ���� Ŀ�긦 �����
            MakeLines();
        }
    }

    // ���� �������� �̿��� ���� ����� ���� �׸���
    private void MakeLines()
    {
        // ����Ʈ�� ��� ��ġ �������� ����ش�
        lines.RemoveRange(0, lines.Count);
        // ���� ����� ������ ���Ѵ�
        Vector3 dir = ARAVRInput.LHandDirection * curveLength;
        // ���� �׷��� ��ġ�� �ʱ갪�� �����Ѵ�
        Vector3 pos = ARAVRInput.LHandPosition;
        // ���� ��ġ�� ����Ʈ�� ��´�
        lines.Add(pos);

        // lineSmooth ������ŭ �ݺ��Ѵ�
        for (int i = 0; i < lineSmooth; i++)
        {
            // ���� ��ġ ���
            Vector3 lastPos = pos;
            // �߷��� ������ �ӵ� ���
            // v = v0 + at
            dir.y += gravity * simulateTime;
            // ��� ����� ���� ��ġ ���
            // P = P0 + vt
            pos += dir * simulateTime;
            // Ray �浹 üũ�� �Ͼ����
            if (CheckHitRay(lastPos, ref pos))
            {
                // �浹 ������ ����ϰ� ����
                lines.Add(pos);
                break;
            }
            else
            {
                // �ڷ���Ʈ UI ��Ȱ��ȭ
                teleportCircleUI.gameObject.SetActive(false);
            }

            // ���� ��ġ�� ���
            lines.Add(pos);
        }
        // ���� �������� ǥ���� ���� ������ ��ϵ� ������ ũ��� �Ҵ�
        lr.positionCount = lines.Count;
        // ���� �������� ������ ���� ������ ����
        lr.SetPositions(lines.ToArray());
    }

    private bool CheckHitRay(Vector3 lastPos, ref Vector3 pos)
    {
        // �� �� lastPos���� ���� �� pos�� ���ϴ� ���� ���
        Vector3 rayDir = pos - lastPos;
        Ray ray = new Ray(lastPos, rayDir);
        RaycastHit hitInfo;

        // Raycast�� �� ������ ũ�⸦ �� ���� ���� �� ������ �Ÿ��� �����Ѵ�
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            // ���� ���� ��ġ�� �浹�� �������� ����
            pos = hitInfo.point;

            int layer = LayerMask.NameToLayer("Terrain");
            // Terrain���̾�� �浹���� ��쿡�� �ڷ���Ʈ UI�� ǥ�õǵ��� ����
            if (hitInfo.transform.gameObject.layer == layer)
            {
                // �ڷ���Ʈ UI Ȱ��ȭ
                teleportCircleUI.gameObject.SetActive(true);
                // �ڷ���Ʈ UI�� ��ġ ����
                teleportCircleUI.position = pos;
                // �ڷ���Ʈ UI�� ���� ����
                teleportCircleUI.forward = hitInfo.normal;
                float distance = (pos - ARAVRInput.LHandPosition).magnitude;
                // �ڷ���Ʈ UI�� ���� ũ�� ����
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);

                // �ڷ���Ʈ UI �� ����
                CheckObstruction(pos);
            }
            return true;
        }
        return false;
    }

    // �ڷ���Ʈ �Ϸ��� ��ġ�� �� ��ġ ���̿� ��ֹ��� �ֳ� Ȯ��
    private void CheckObstruction(Vector3 pos)
    {
        // �ڷ���Ʈ UI�� �÷��̾ �����ϴ� Ray �߻�

        // �� �� lastPos���� ���� �� pos�� ���ϴ� ���� ���
        Vector3 rayDir = pos - ARAVRInput.LHandPosition;
        Ray ray = new Ray(ARAVRInput.LHandPosition, rayDir);
        RaycastHit hitInfo;

        // Raycast�� �� ������ ũ�⸦ �� ���� ���� �� ������ �Ÿ��� �����Ѵ�
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            int layer = LayerMask.NameToLayer("Terrain");

            // ���� ���̿� ��ü�� �νĵȴٸ�
            if (hitInfo.transform.gameObject.layer != layer)
            {
                // �ڷ���Ʈ UI�� ���������� ����
                lineMesh.SetPropertyBlock(block);
                int id = Shader.PropertyToID("_Color");
                block.SetColor(id, Color.red);
                lineMesh.SetPropertyBlock(block);
                // �ڷ���Ʈ �Ұ��� ǥ��
                teleportable = false;
            }
            else
            {
                // �ڷ���Ʈ UI�� �Ķ������� ����
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

        // �ڷ���Ʈ�� ������ ��ġ�� ��� �ڷ���Ʈ ��� �׷��ֱ�

        // ���� �÷��̾� ��ġ���� �ڷ���Ʈ UI��ġ���� �ٴڿ� ��� �׸���

        // �÷��̾� ��ġ���� �������� �Ʒ��� ���ϴ� Ray �߻� �� �ٴڰ� �´�� �� ã��
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;

        Vector3 pos = new Vector3();
        int layer = LayerMask.NameToLayer("Terrian");
        if(Physics.Raycast(ray,out hitInfo,3f,layer))
        {
            pos = hitInfo.point;
        }

        // pos���� �ڷ���Ʈ UI���� �Ÿ��� �߰� �������� ǥ���ϱ�
        Vector3 dir = (teleportCircleUI.position - pos).normalized; // ���⺤�� ���ϱ�
        float distance = (pos - teleportCircleUI.position).magnitude;   // ������ ũ��(����) ���ϱ�

        // �̵��ؾߵ� Ƚ�� (��)
        int quotient = (int)(distance / oneTimeDistance);
        // ����Ʈ�� ��� ��ġ �������� ����ش�
        points.RemoveRange(0, points.Count);
        print(quotient + "�̵��ؾߵ� Ƚ��");

        // �̵��ؾ� �� ��ġ�� List�� ����
        for (int i = 0; i < quotient; i++)
        {
            // ���⺤�Ϳ� �̵� �Ÿ� ���ϱ�
            Vector3 checkPoint = dir * i * oneTimeDistance + pos;
            // üũ����Ʈ ��ġ�� ����Ʈ�� ��´�
            points.Add(checkPoint);
            print(checkPoint + "üũ����Ʈ" + i);
        }

        // ���� �ڷ���Ʈ ������ ��ġ�� ����Ʈ�� ��´�
        points.Add(teleportCircleUI.position);
        print(teleportCircleUI.position + " ������ġ");

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
        //// �ڷ���Ʈ UI ��ġ�� �����̵�
        //transform.position = teleportCircleUI.position + Vector3.up;
        StartCoroutine(IDrawPoint());
    }

    IEnumerator IDrawPoint()
    {
        yield return null;
        // �ڷ���Ʈ �� ���� ǥ�����ֱ�
        for(int i = 1; i<points.Count; i++)
        {
            // �ڷ���Ʈ UI Ȱ��ȭ
            teleportPointTrianglesUI[i].gameObject.SetActive(true);
            // �ڷ���Ʈ UI�� ��ġ ����
            teleportPointTrianglesUI[i].position = points[i];
            // �ڷ���Ʈ UI�� ���� ����
            teleportPointTrianglesUI[i].forward = Vector3.up;
            float distance = (points[i] - ARAVRInput.LHandPosition).magnitude;
            // �ڷ���Ʈ UI�� ���� ũ�� ����
            teleportPointTrianglesUI[i].localScale = originScale * Mathf.Max(1, distance);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.1f);

        // �ڷ���Ʈ �����ϱ�
        for (int i = 1; i < points.Count; i++)
        {
            print(teleportPointTrianglesUI[i].position + "UI ��ġ" + i);
            Vector3 vec = teleportPointTrianglesUI[i].position;
            transform.position = vec + Vector3.up;
            // �ڷ���Ʈ UI ��Ȱ��ȭ
            teleportPointTrianglesUI[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }

        GetComponent<CharacterController>().enabled = true;
        GetComponent<PlayerMove>().isMove = true;
    }
}
