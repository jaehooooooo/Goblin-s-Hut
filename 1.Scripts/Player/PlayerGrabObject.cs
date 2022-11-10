using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabObject : MonoBehaviour
{
    enum LayerNumbers
    {
        weapon = 7,
        simple_object = 8,
        SpecialBat = 9
    }

    public GameObject lParent;
    public GameObject rParent;
    // 변수 선언
    private bool isGrabbingLeft = false;    // 물체를 잡고 있는지의 여부
    private bool isGrabbingRight = false;    // 물체를 잡고 있는지의 여부
    public LayerMask grabbedLayer;      // 잡을 물체의 종류
    public float grabRange = 0.08f;      // 잡을 수 있는 거리

    private Vector3 prePosLeft;            // 왼손 이전 위치
    private Vector3 prePosRight;            // 오른손 이전 위치
    public float throwPower;                // 던질 힘

    private Quaternion preRotLeft;          // 왼손 이전 회전
    private Quaternion preRotRight;          // 오른손 이전 회전
    public float rotPower = 5;               // 회전 력
    
    GameObject grabbedObjectLeft;           // 잡고 있는 물체
    GameObject grabbedObjectRight;           // 잡고 있는 물체

    int layerNumberLeft;                    // 잡고있는 물체의 레이어 넘버
    int layerNumberRight;
    string tagNameLeft;                     // 잡고 있는 물체의 태그
    string tagNameRight;
    string tagGrabbingObject = "GrabbingObject";               // 잡힌 물체에 적용할 태그

    GameObject grabbedObjectParentLeft;     // 잡고 있는 물체의 부모
    GameObject grabbedObjectParentRight;     // 잡고 있는 물체의 부모

    public float velocityValue;             // 그랩한 물체 순간 이동 속도 체크

    private void Update()
    {
        //물체 잡기
        if (!isGrabbingLeft)
            TryGrab("Left");
        else
            TryUngrab("Left");

        if (!isGrabbingRight)
            TryGrab("Right");
        else
            TryUngrab("Right");
    }

    // 그랩 버튼을 누르면 일정 영역 안에있는 물체를 잡는다
    private void TryGrab(string isGrabbing)
    {
        if(isGrabbing == "Left")
        {
            if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.LTouch))
            {
                Vector3 LHandPos = ARAVRInput.LHandPosition;
                Quaternion LHandRot = ARAVRInput.LHand.rotation;
                Grab(LHandPos, LHandRot, lParent, isGrabbing, out grabbedObjectLeft, out grabbedObjectParentLeft, out prePosLeft, out preRotLeft, out layerNumberLeft, out tagNameLeft);
            }
        }
        else
        {
            if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
            {
                Vector3 RHandPos = ARAVRInput.RHandPosition;
                Quaternion RHandRot = ARAVRInput.RHand.rotation;
                Grab(RHandPos, RHandRot, rParent, isGrabbing, out grabbedObjectRight, out grabbedObjectParentRight, out prePosRight, out preRotRight, out layerNumberRight,out tagNameRight);
            }
        }
        #region 그랩 본래 코드
        // 그랩 버튼을 누르면
        //if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        //{
        //    // 일정 영역 안에 물체가 있다면
        //    Collider[] hitObjects = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbedLayer);
        //    // Collider에 들어온 물체 중 가장 가까운 물체 선택
        //    int closest = 0;
        //    for(int i =1; i<hitObjects.Length; i++)
        //    {
        //        // 손과 가장 가까운 물체와의 거리
        //        Vector3 closestPos = hitObjects[closest].transform.position;
        //        float closestDistance = Vector3.Distance(closestPos, ARAVRInput.RHandPosition);
        //        // 다음 물체와 손의 거리
        //        Vector3 nextPos = hitObjects[i].transform.position;
        //        float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);
        //        // 비교 후 -> 가장 가까운 물체의 인덱스 교체
        //        if (nextDistance < closestDistance)
        //            closest = i;
        //    }
        //    // 물건을 잡는다
        //    if (hitObjects.Length > 0)  // 검출된 물체가 있을 경우
        //    {
        //        // 잡은 상태로 변경
        //        isGrabbing = true;
        //        // 잡은 물체 기억
        //        grabbedObject = hitObjects[closest].gameObject;
        //        // 잡은 물체의 원래 부모 
        //        grabbedObjectParent = hitObjects[closest].transform.parent.gameObject;
        //        // 잡은 물체를 손의 자식으로 등록
        //        grabbedObject.transform.parent = rParent.transform;
        //        // 손에 맞는 물체 위치 이동
        //        ObjectPosition objectPosition = grabbedObject.GetComponent<ObjectPosition>();
        //        grabbedObject.transform.localPosition = objectPosition.handPos;
        //        grabbedObject.transform.localEulerAngles = objectPosition.handQuat;

        //        // 물리기능 정지
        //        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        //    }
        //}
        #endregion
    }

    private void Grab(Vector3 handPos,Quaternion handRot, GameObject touchParent, string isGrabbing, out GameObject grabbedObject, out GameObject grabbedObjectParent, out Vector3 prePos, out Quaternion preRot, out int layerNumbers, out string tagName)
    {
        // 일정 영역 안에 물체가 있다면
        Collider[] hitObjects = Physics.OverlapSphere(handPos, grabRange, grabbedLayer);
        // Collider에 들어온 물체 중 가장 가까운 물체 선택
        int closest = 0;
        for (int i = 1; i < hitObjects.Length; i++)
        {
            // 히트 오브젝트가 다른 손에 잡혀있는 물체라면
            if(hitObjects[closest].GetComponent<ObjectPosition>().isGrabbed)
            {
                break;
            }
            else
            {
                // 손과 가장 가까운 물체와의 거리
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, handPos);
                // 다음 물체와 손의 거리
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, handPos);
                // 비교 후 -> 가장 가까운 물체의 인덱스 교체
                if (nextDistance < closestDistance)
                    closest = i;
            }
        }
        // 물건을 잡는다
        if (hitObjects.Length > 0 && !hitObjects[closest].GetComponent<ObjectPosition>().isGrabbed)  // 검출된 물체가 있을 경우
        {
            // 잡은 상태로 변경
            if (isGrabbing == "Left")
                isGrabbingLeft = true;
            else if (isGrabbing == "Right")
                isGrabbingRight = true;

            // 잡은 물체 기억
            grabbedObject = hitObjects[closest].gameObject;
            ObjectPosition objectPosition = grabbedObject.GetComponent<ObjectPosition>();

            // 잡은 물체의 레이어 정보 저장
            layerNumbers = grabbedObject.layer;
            // 잡은 물체의 태그 정보 저장
            tagName = grabbedObject.tag;
            // 잡은 물체의 현재 부모
            grabbedObjectParent = grabbedObject.transform.parent.gameObject;
            // 잡은 물체를 손의 자식으로 등록
            grabbedObject.transform.parent = touchParent.transform;
            // 물체 잡힌 상태로 변경
            hitObjects[closest].GetComponent<ObjectPosition>().isGrabbed = true;

            grabbedObject.transform.localPosition = objectPosition.handPos;
            grabbedObject.transform.localEulerAngles = objectPosition.handQuat;
            // 초기 위치값 지정
            prePos = handPos;
            // 초기 회전값 지정
            preRot = handRot;
            // 물리기능 정지
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            layerNumbers = 0;
            grabbedObject = null;
            grabbedObjectParent = null;
            prePos = Vector3.zero;
            preRot = Quaternion.identity;
            tagName = tagGrabbingObject;
        }
    }

    private void TryUngrab(string isGrabbing)
    {
        if (isGrabbing == "Left")
        {
            // 왼손에 스페셜 방망이 들었다고 전체방송
            if (layerNumberLeft == (int)LayerNumbers.SpecialBat && !GameManager.instance.getSpecialBatLeft)
                GameManager.instance.getSpecialBatLeft = true;

            //던질 방향 설정
            Vector3 throwDirectionLeft = (ARAVRInput.LHandPosition - prePosLeft);
            // 위치 기억
            prePosLeft = ARAVRInput.LHandPosition;
            // 회전 방향
            Quaternion deltaRotationLeft = ARAVRInput.LHand.rotation * Quaternion.Inverse(preRotLeft);
            // 이전 회전 저장
            preRotLeft = ARAVRInput.LHand.rotation;


            if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.LTouch))
            {
                Vector3 touch = ARAVRInput.LHandPosition;
                Ungrab(isGrabbing, grabbedObjectLeft, grabbedObjectParentLeft,throwDirectionLeft,deltaRotationLeft,tagNameLeft);
                grabbedObjectLeft = null;   // 잡은 물체가 없도록 설정
            }
        }
        else
        {            
            // 오른손에 스페셜 방망이 들었다고 전체방송
            if (layerNumberLeft == (int)LayerNumbers.SpecialBat && !GameManager.instance.getSpecialBatRight)
                GameManager.instance.getSpecialBatRight = true;

            //던질 방향 설정
            Vector3 throwDirectionRight = (ARAVRInput.RHandPosition - prePosRight);
            // 위치 기억
            prePosRight = ARAVRInput.RHandPosition;
            // 회전 방향
            Quaternion deltaRotationRight = ARAVRInput.RHand.rotation * Quaternion.Inverse(preRotRight);
            // 이전 회전 저장
            preRotRight = ARAVRInput.RHand.rotation;


            if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
            {
                Vector3 touch = ARAVRInput.RHandPosition;
                Ungrab(isGrabbing, grabbedObjectRight, grabbedObjectParentRight, throwDirectionRight,deltaRotationRight, tagNameRight);
                grabbedObjectRight = null;  // 잡은 물체가 없도록 설정
            }
        }
        #region Ungrab 원래코드
        //// 버튼을 놓았다면
        //if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        //{
        //    // 잡지 않은 상태로 전환
        //    isGrabbing = false;
        //    // 물리 기능 활성화
        //    grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
        //    // 손에서 물체 떼어내기
        //    grabbedObject.transform.parent = grabbedObjectParent.transform;
        //    // 잡은 물체가 없도록 설정
        //    grabbedObject = null;
        //}
        #endregion
    }

    private void Ungrab(string isGrabbing, GameObject grabbedObject, GameObject grabbedObjectParent, Vector3 throwDirection, Quaternion deltaRotation, string originTagName)
    {
        // 잡지 않은 상태로 변경
        if (isGrabbing == "Left")
        {
            isGrabbingLeft = false;
        }
        else if(isGrabbing == "Right")
        {
            isGrabbingRight = false;
        }

        // 물리 기능 활성화
        grabbedObject.GetComponent<Rigidbody>().isKinematic = false;

        // 던지기
        grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;

        // 각속도 계산
        float angle;
        Vector3 axis;

        deltaRotation.ToAngleAxis(out angle, out axis);
        Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
        grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

        // 잡은 물체 bool값 변경
        grabbedObject.GetComponent<ObjectPosition>().isGrabbed = false;

        // 손에서 물체 떼어내기
        // 현재 대상 오브젝트가 상호작용을 하나라도 실행 중인 경우
        if (!grabbedObject.GetComponent<ObjectPosition>().CheckObjectSituation())
        {
            grabbedObject.transform.parent = grabbedObjectParent.transform;
        }
        else
        {
            grabbedObject.transform.parent = grabbedObject.transform.GetComponent<ObjectPosition>().originParentObject.transform;
        }
    }
}
