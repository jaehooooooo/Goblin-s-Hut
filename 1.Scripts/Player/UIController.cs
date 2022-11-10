using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIController : BaseInputModule
{
    public GraphicRaycaster graphicRaycaster; //Canvas에 있는 GraphicRaycaster
    private List<RaycastResult> raycastResults; //Raycast로 충돌한 UI들을 담는 리스트
    private PointerEventData pointerEventData; //Canvas 상의 포인터 위치 및 정보
    public Camera target; //마우스 커서 역할을 대신할 카메라

    // 이전에 감지된 물체
    private GameObject preObj;
    // 현재 감지된 물체
    private GameObject curObj;


    protected override void Start()
    {
        pointerEventData = new PointerEventData(null); //pointerEventData 초기화
        pointerEventData.position = new Vector2(target.pixelWidth * 0.5f, target.pixelHeight * 0.5f); //카메라의 중앙으로 포인터 지정
        raycastResults = new List<RaycastResult>(); //리스트 초기화
    }

    private void Update()
    {
        graphicRaycaster.Raycast(pointerEventData, raycastResults); //포인터 위치로부터 Raycast 발생, 결과는 raycastResults에 담긴다

        if (raycastResults.Count > 0) //충돌한 UI가 있으면
        {
            if (raycastResults[0].gameObject.GetComponent<TMP_Text>())
            {
                preObj = curObj;
                curObj = raycastResults[0].gameObject;
                print("색 변경" + curObj.name) ;
                curObj.GetComponent<TMP_Text>().color = new Color(1, 0, 0);
            }
        }
        else //충돌한 UI가 없으면
        {
            if (preObj != null)
            {
                print("색 복구");
                curObj.GetComponent<TMP_Text>().color = new Color(0, 0, 0);
            }
            preObj = null;
            //HandlePointerExitAndEnter(pointerEventData, null); //포인터 벗어남 → GameObject가 null이어야 호버링에서 벗어남
        }

        raycastResults.Clear(); //Raycast 결과 리스트 초기화 → 필수
    }

    public override void Process() { } //상속받아야 에러 안뜸
    protected override void OnEnable() { } //상속받아야 에러 안뜸
    protected override void OnDisable() { } //상속받아야 에러 안뜸
}

//    // 감지할 Canvas
//    public GameObject canvas;
//    // 라인렌더러
//    private LineRenderer lr;
//    // 그래픽 레이케스터
//    private GraphicRaycaster graphicRaycaster;
//    // 포인트 이벤트 데이터
//    private PointerEventData pointerEventData;
//    // 이벤트 시스템
//    private EventSystem eventSystem;

//    // 변수
//    private bool drawLine;
//    // 이전에 감지된 물체
//    private GameObject preObj;
//    // 현재 감지된 물체
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
//        // 라인 렌더러를 그리고 있지 않은 상태에 오른쪽 컨트롤러의 One 버튼을 누르면 카메라 앞쪽으로 팝업창 띄우고 플레이어 움직임 제한
//        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.RTouch) && !lr.enabled)
//        {
//            // 캐릭터 움직임 제한

//            // 팝업창 띄우기

//            // 라인 렌더러 그리기
//            drawLine = true;
//        }
//        // 라인 렌더러를 그리고 있을 때 오른쪽 컨트롤러의 One 버튼을 다시 누르면 팝업창 닫히고 플레이어 움직임 가능
//        else if(ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.RTouch) && lr.enabled)
//        {
//            // 캐릭터 움직임 해제

//            // 팝업창 내리기

//            // 라인 렌더러 끄기
//            lr.enabled = false;
//            drawLine = false;
//        }

//        if (drawLine)
//            DrawLineRenderer();
//    }

//    // 버튼을 누르면 라인을 그리는 함수 실행
//    private void DrawLineRenderer()
//    {
//        lr.enabled = true;
//        // 이벤트 시스템을 포인터 이벤트데이터에 Set
//        pointerEventData = new PointerEventData(eventSystem);
//        // 오른쪽 컨트롤러의 위치를 포인터 이벤트 데이터 포지션에 set
//        //pointerEventData.position = ARAVRInput.RHandPosition;
//        // 레이캐스트 리스트 선언
//        List<RaycastResult> results = new List<RaycastResult>();
//        // 레이 캐스트 사용
//        graphicRaycaster.Raycast(pointerEventData, results);
//        // 레이에 부딪힌 물체가 UI가 있다면
//        if(results.Count >0)
//        {        
//            // 해당 결과 첫 번째 객체 확인
//            GameObject hitObj = results[0].gameObject;
//            preObj = curObj;
//            curObj = hitObj;

//            print(results[0].gameObject.name + "물체 감지중");
//            lr.SetPosition(0, pointerEventData.position);
//            lr.SetPosition(1, hitObj.transform.localPosition);

//            print(curObj.GetComponent<TMP_Text>());
//            if(curObj.GetComponent<TMP_Text>())
//            {
//                print("색 변경");
//                curObj.GetComponent<TMP_Text>().color = new Color(1, 0, 0);
//            }
//        }
//        // 레이에 부딪힌 UI가 없다면
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
//        //// UI까지 라인 그리기
//        //Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
//        //RaycastHit hitInfo;
//        //int layer = 1 << LayerMask.NameToLayer("UI");

//        //// Ray 충돌 검출
//        //if (Physics.Raycast(ray, out hitInfo, 100, layer))
//        //{
//        //    // Ray 부딪힌 지점에 라인 그리기

//        //    // 버튼에 라인이 맞으면 버튼 색 + 글자 색 변경
//        //    print("UI 검출");
//        //    // 라인이 안맞으면 돌려놓기

//        //    // 라인이 맞은 상태에서 트리거 버튼 누르멸 OnClick() 실행하기

//        //}
//        //else
//        //{
//        //    lr.SetPosition(0, ray.origin);
//        //    lr.SetPosition(1, ray.origin + ARAVRInput.RHandDirection * 100f);
//        //}
//    }


//public class GraphicRaycasterEX : BaseInputModule //BaseInputModule 클래스 상속
//{
//}
//}
