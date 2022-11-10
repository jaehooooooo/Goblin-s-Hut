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
    // ���� ����
    private bool isGrabbingLeft = false;    // ��ü�� ��� �ִ����� ����
    private bool isGrabbingRight = false;    // ��ü�� ��� �ִ����� ����
    public LayerMask grabbedLayer;      // ���� ��ü�� ����
    public float grabRange = 0.08f;      // ���� �� �ִ� �Ÿ�

    private Vector3 prePosLeft;            // �޼� ���� ��ġ
    private Vector3 prePosRight;            // ������ ���� ��ġ
    public float throwPower;                // ���� ��

    private Quaternion preRotLeft;          // �޼� ���� ȸ��
    private Quaternion preRotRight;          // ������ ���� ȸ��
    public float rotPower = 5;               // ȸ�� ��
    
    GameObject grabbedObjectLeft;           // ��� �ִ� ��ü
    GameObject grabbedObjectRight;           // ��� �ִ� ��ü

    int layerNumberLeft;                    // ����ִ� ��ü�� ���̾� �ѹ�
    int layerNumberRight;
    string tagNameLeft;                     // ��� �ִ� ��ü�� �±�
    string tagNameRight;
    string tagGrabbingObject = "GrabbingObject";               // ���� ��ü�� ������ �±�

    GameObject grabbedObjectParentLeft;     // ��� �ִ� ��ü�� �θ�
    GameObject grabbedObjectParentRight;     // ��� �ִ� ��ü�� �θ�

    public float velocityValue;             // �׷��� ��ü ���� �̵� �ӵ� üũ

    private void Update()
    {
        //��ü ���
        if (!isGrabbingLeft)
            TryGrab("Left");
        else
            TryUngrab("Left");

        if (!isGrabbingRight)
            TryGrab("Right");
        else
            TryUngrab("Right");
    }

    // �׷� ��ư�� ������ ���� ���� �ȿ��ִ� ��ü�� ��´�
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
        #region �׷� ���� �ڵ�
        // �׷� ��ư�� ������
        //if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        //{
        //    // ���� ���� �ȿ� ��ü�� �ִٸ�
        //    Collider[] hitObjects = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbedLayer);
        //    // Collider�� ���� ��ü �� ���� ����� ��ü ����
        //    int closest = 0;
        //    for(int i =1; i<hitObjects.Length; i++)
        //    {
        //        // �հ� ���� ����� ��ü���� �Ÿ�
        //        Vector3 closestPos = hitObjects[closest].transform.position;
        //        float closestDistance = Vector3.Distance(closestPos, ARAVRInput.RHandPosition);
        //        // ���� ��ü�� ���� �Ÿ�
        //        Vector3 nextPos = hitObjects[i].transform.position;
        //        float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);
        //        // �� �� -> ���� ����� ��ü�� �ε��� ��ü
        //        if (nextDistance < closestDistance)
        //            closest = i;
        //    }
        //    // ������ ��´�
        //    if (hitObjects.Length > 0)  // ����� ��ü�� ���� ���
        //    {
        //        // ���� ���·� ����
        //        isGrabbing = true;
        //        // ���� ��ü ���
        //        grabbedObject = hitObjects[closest].gameObject;
        //        // ���� ��ü�� ���� �θ� 
        //        grabbedObjectParent = hitObjects[closest].transform.parent.gameObject;
        //        // ���� ��ü�� ���� �ڽ����� ���
        //        grabbedObject.transform.parent = rParent.transform;
        //        // �տ� �´� ��ü ��ġ �̵�
        //        ObjectPosition objectPosition = grabbedObject.GetComponent<ObjectPosition>();
        //        grabbedObject.transform.localPosition = objectPosition.handPos;
        //        grabbedObject.transform.localEulerAngles = objectPosition.handQuat;

        //        // ������� ����
        //        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        //    }
        //}
        #endregion
    }

    private void Grab(Vector3 handPos,Quaternion handRot, GameObject touchParent, string isGrabbing, out GameObject grabbedObject, out GameObject grabbedObjectParent, out Vector3 prePos, out Quaternion preRot, out int layerNumbers, out string tagName)
    {
        // ���� ���� �ȿ� ��ü�� �ִٸ�
        Collider[] hitObjects = Physics.OverlapSphere(handPos, grabRange, grabbedLayer);
        // Collider�� ���� ��ü �� ���� ����� ��ü ����
        int closest = 0;
        for (int i = 1; i < hitObjects.Length; i++)
        {
            // ��Ʈ ������Ʈ�� �ٸ� �տ� �����ִ� ��ü���
            if(hitObjects[closest].GetComponent<ObjectPosition>().isGrabbed)
            {
                break;
            }
            else
            {
                // �հ� ���� ����� ��ü���� �Ÿ�
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, handPos);
                // ���� ��ü�� ���� �Ÿ�
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, handPos);
                // �� �� -> ���� ����� ��ü�� �ε��� ��ü
                if (nextDistance < closestDistance)
                    closest = i;
            }
        }
        // ������ ��´�
        if (hitObjects.Length > 0 && !hitObjects[closest].GetComponent<ObjectPosition>().isGrabbed)  // ����� ��ü�� ���� ���
        {
            // ���� ���·� ����
            if (isGrabbing == "Left")
                isGrabbingLeft = true;
            else if (isGrabbing == "Right")
                isGrabbingRight = true;

            // ���� ��ü ���
            grabbedObject = hitObjects[closest].gameObject;
            ObjectPosition objectPosition = grabbedObject.GetComponent<ObjectPosition>();

            // ���� ��ü�� ���̾� ���� ����
            layerNumbers = grabbedObject.layer;
            // ���� ��ü�� �±� ���� ����
            tagName = grabbedObject.tag;
            // ���� ��ü�� ���� �θ�
            grabbedObjectParent = grabbedObject.transform.parent.gameObject;
            // ���� ��ü�� ���� �ڽ����� ���
            grabbedObject.transform.parent = touchParent.transform;
            // ��ü ���� ���·� ����
            hitObjects[closest].GetComponent<ObjectPosition>().isGrabbed = true;

            grabbedObject.transform.localPosition = objectPosition.handPos;
            grabbedObject.transform.localEulerAngles = objectPosition.handQuat;
            // �ʱ� ��ġ�� ����
            prePos = handPos;
            // �ʱ� ȸ���� ����
            preRot = handRot;
            // ������� ����
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
            // �޼տ� ����� ����� ����ٰ� ��ü���
            if (layerNumberLeft == (int)LayerNumbers.SpecialBat && !GameManager.instance.getSpecialBatLeft)
                GameManager.instance.getSpecialBatLeft = true;

            //���� ���� ����
            Vector3 throwDirectionLeft = (ARAVRInput.LHandPosition - prePosLeft);
            // ��ġ ���
            prePosLeft = ARAVRInput.LHandPosition;
            // ȸ�� ����
            Quaternion deltaRotationLeft = ARAVRInput.LHand.rotation * Quaternion.Inverse(preRotLeft);
            // ���� ȸ�� ����
            preRotLeft = ARAVRInput.LHand.rotation;


            if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.LTouch))
            {
                Vector3 touch = ARAVRInput.LHandPosition;
                Ungrab(isGrabbing, grabbedObjectLeft, grabbedObjectParentLeft,throwDirectionLeft,deltaRotationLeft,tagNameLeft);
                grabbedObjectLeft = null;   // ���� ��ü�� ������ ����
            }
        }
        else
        {            
            // �����տ� ����� ����� ����ٰ� ��ü���
            if (layerNumberLeft == (int)LayerNumbers.SpecialBat && !GameManager.instance.getSpecialBatRight)
                GameManager.instance.getSpecialBatRight = true;

            //���� ���� ����
            Vector3 throwDirectionRight = (ARAVRInput.RHandPosition - prePosRight);
            // ��ġ ���
            prePosRight = ARAVRInput.RHandPosition;
            // ȸ�� ����
            Quaternion deltaRotationRight = ARAVRInput.RHand.rotation * Quaternion.Inverse(preRotRight);
            // ���� ȸ�� ����
            preRotRight = ARAVRInput.RHand.rotation;


            if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
            {
                Vector3 touch = ARAVRInput.RHandPosition;
                Ungrab(isGrabbing, grabbedObjectRight, grabbedObjectParentRight, throwDirectionRight,deltaRotationRight, tagNameRight);
                grabbedObjectRight = null;  // ���� ��ü�� ������ ����
            }
        }
        #region Ungrab �����ڵ�
        //// ��ư�� ���Ҵٸ�
        //if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        //{
        //    // ���� ���� ���·� ��ȯ
        //    isGrabbing = false;
        //    // ���� ��� Ȱ��ȭ
        //    grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
        //    // �տ��� ��ü �����
        //    grabbedObject.transform.parent = grabbedObjectParent.transform;
        //    // ���� ��ü�� ������ ����
        //    grabbedObject = null;
        //}
        #endregion
    }

    private void Ungrab(string isGrabbing, GameObject grabbedObject, GameObject grabbedObjectParent, Vector3 throwDirection, Quaternion deltaRotation, string originTagName)
    {
        // ���� ���� ���·� ����
        if (isGrabbing == "Left")
        {
            isGrabbingLeft = false;
        }
        else if(isGrabbing == "Right")
        {
            isGrabbingRight = false;
        }

        // ���� ��� Ȱ��ȭ
        grabbedObject.GetComponent<Rigidbody>().isKinematic = false;

        // ������
        grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;

        // ���ӵ� ���
        float angle;
        Vector3 axis;

        deltaRotation.ToAngleAxis(out angle, out axis);
        Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
        grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

        // ���� ��ü bool�� ����
        grabbedObject.GetComponent<ObjectPosition>().isGrabbed = false;

        // �տ��� ��ü �����
        // ���� ��� ������Ʈ�� ��ȣ�ۿ��� �ϳ��� ���� ���� ���
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
