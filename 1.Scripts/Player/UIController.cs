using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIController : BaseInputModule
{
    public GraphicRaycaster graphicRaycaster; //Canvas�� �ִ� GraphicRaycaster
    private List<RaycastResult> raycastResults; //Raycast�� �浹�� UI���� ��� ����Ʈ
    private PointerEventData pointerEventData; //Canvas ���� ������ ��ġ �� ����
    public Camera target; //���콺 Ŀ�� ������ ����� ī�޶�

    // ������ ������ ��ü
    private GameObject preObj;
    // ���� ������ ��ü
    private GameObject curObj;


    protected override void Start()
    {
        pointerEventData = new PointerEventData(null); //pointerEventData �ʱ�ȭ
        pointerEventData.position = new Vector2(target.pixelWidth * 0.5f, target.pixelHeight * 0.5f); //ī�޶��� �߾����� ������ ����
        raycastResults = new List<RaycastResult>(); //����Ʈ �ʱ�ȭ
    }

    private void Update()
    {
        graphicRaycaster.Raycast(pointerEventData, raycastResults); //������ ��ġ�κ��� Raycast �߻�, ����� raycastResults�� ����

        if (raycastResults.Count > 0) //�浹�� UI�� ������
        {
            if (raycastResults[0].gameObject.GetComponent<TMP_Text>())
            {
                preObj = curObj;
                curObj = raycastResults[0].gameObject;
                print("�� ����" + curObj.name) ;
                curObj.GetComponent<TMP_Text>().color = new Color(1, 0, 0);
            }
        }
        else //�浹�� UI�� ������
        {
            if (preObj != null)
            {
                print("�� ����");
                curObj.GetComponent<TMP_Text>().color = new Color(0, 0, 0);
            }
            preObj = null;
            //HandlePointerExitAndEnter(pointerEventData, null); //������ ��� �� GameObject�� null�̾�� ȣ�������� ���
        }

        raycastResults.Clear(); //Raycast ��� ����Ʈ �ʱ�ȭ �� �ʼ�
    }

    public override void Process() { } //��ӹ޾ƾ� ���� �ȶ�
    protected override void OnEnable() { } //��ӹ޾ƾ� ���� �ȶ�
    protected override void OnDisable() { } //��ӹ޾ƾ� ���� �ȶ�
}

//    // ������ Canvas
//    public GameObject canvas;
//    // ���η�����
//    private LineRenderer lr;
//    // �׷��� �����ɽ���
//    private GraphicRaycaster graphicRaycaster;
//    // ����Ʈ �̺�Ʈ ������
//    private PointerEventData pointerEventData;
//    // �̺�Ʈ �ý���
//    private EventSystem eventSystem;

//    // ����
//    private bool drawLine;
//    // ������ ������ ��ü
//    private GameObject preObj;
//    // ���� ������ ��ü
//    private GameObject curObj;

//    // Start is called before the first frame update
//    void Start()
//    {
//        lr = GetComponent<LineRenderer>();
//        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
//        eventSystem = GetComponent<EventSystem>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // ���� �������� �׸��� ���� ���� ���¿� ������ ��Ʈ�ѷ��� One ��ư�� ������ ī�޶� �������� �˾�â ���� �÷��̾� ������ ����
//        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.RTouch) && !lr.enabled)
//        {
//            // ĳ���� ������ ����

//            // �˾�â ����

//            // ���� ������ �׸���
//            drawLine = true;
//        }
//        // ���� �������� �׸��� ���� �� ������ ��Ʈ�ѷ��� One ��ư�� �ٽ� ������ �˾�â ������ �÷��̾� ������ ����
//        else if(ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.RTouch) && lr.enabled)
//        {
//            // ĳ���� ������ ����

//            // �˾�â ������

//            // ���� ������ ����
//            lr.enabled = false;
//            drawLine = false;
//        }

//        if (drawLine)
//            DrawLineRenderer();
//    }

//    // ��ư�� ������ ������ �׸��� �Լ� ����
//    private void DrawLineRenderer()
//    {
//        lr.enabled = true;
//        // �̺�Ʈ �ý����� ������ �̺�Ʈ�����Ϳ� Set
//        pointerEventData = new PointerEventData(eventSystem);
//        // ������ ��Ʈ�ѷ��� ��ġ�� ������ �̺�Ʈ ������ �����ǿ� set
//        //pointerEventData.position = ARAVRInput.RHandPosition;
//        // ����ĳ��Ʈ ����Ʈ ����
//        List<RaycastResult> results = new List<RaycastResult>();
//        // ���� ĳ��Ʈ ���
//        graphicRaycaster.Raycast(pointerEventData, results);
//        // ���̿� �ε��� ��ü�� UI�� �ִٸ�
//        if(results.Count >0)
//        {        
//            // �ش� ��� ù ��° ��ü Ȯ��
//            GameObject hitObj = results[0].gameObject;
//            preObj = curObj;
//            curObj = hitObj;

//            print(results[0].gameObject.name + "��ü ������");
//            lr.SetPosition(0, pointerEventData.position);
//            lr.SetPosition(1, hitObj.transform.localPosition);

//            print(curObj.GetComponent<TMP_Text>());
//            if(curObj.GetComponent<TMP_Text>())
//            {
//                print("�� ����");
//                curObj.GetComponent<TMP_Text>().color = new Color(1, 0, 0);
//            }
//        }
//        // ���̿� �ε��� UI�� ���ٸ�
//        {
//            //if (preObj != null)
//            //{
//            //    if(preObj.GetComponent<TMP_Text>())
//            //    {
//            //        preObj.GetComponent<TMP_Text>().color = new Color(0.44f, 0.44f, 0.44f);
//            //    }
//            //}
//            preObj = null;
//            lr.SetPosition(0, ARAVRInput.RHandPosition);
//            lr.SetPosition(1, ARAVRInput.RHandPosition + (ARAVRInput.RHandDirection * 100f));
//        }





//        //lr.enabled = true;
//        //// UI���� ���� �׸���
//        //Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
//        //RaycastHit hitInfo;
//        //int layer = 1 << LayerMask.NameToLayer("UI");

//        //// Ray �浹 ����
//        //if (Physics.Raycast(ray, out hitInfo, 100, layer))
//        //{
//        //    // Ray �ε��� ������ ���� �׸���

//        //    // ��ư�� ������ ������ ��ư �� + ���� �� ����
//        //    print("UI ����");
//        //    // ������ �ȸ����� ��������

//        //    // ������ ���� ���¿��� Ʈ���� ��ư ������ OnClick() �����ϱ�

//        //}
//        //else
//        //{
//        //    lr.SetPosition(0, ray.origin);
//        //    lr.SetPosition(1, ray.origin + ARAVRInput.RHandDirection * 100f);
//        //}
//    }


//public class GraphicRaycasterEX : BaseInputModule //BaseInputModule Ŭ���� ���
//{
//}
//}
