using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldBat : MonoBehaviour
{
    private bool leftHand;              // ���� ����̸� ����ִ� ���� �޼��̸� true, �������̸� false
    public Transform activeObjectPool;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform activeObjectPoolLeftHand;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform activeObjectPoolRightHand;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ


    public Vector3 handPos;             // �� ��ġ

    public Transform model;             // ����� �ٵ�
    public Renderer batMesh;            // ����� �Ž�

    public Transform fieldSphere;        // �߷�������� ������ ���� ����

    private bool readyShoot;            // �߷��� ���� ���� ����
    private bool makeFieldGauge;         // �߷��� ������ ������ ����
    private bool useField;         // �߷��� üũ�ϴ� ����

    private float time;
    //public float size;


    private float sphereSize;           // Ư��ȿ�� sphereũ��
    private float maxSphereSize = 3f;   // Ư��ȿ�� sphere �ִ�ũ��

    private Color whiteColor = new Color(1, 1, 1);
    private Color darkBlueColor = new Color(0.13f, 0.32f, 0.33f);

    private void Awake()
    {
        // �ش� ����̰� �����ִ� ���׸��� �߿��� �ι�° ���׸����� ������ ã�ƿ�
        batMesh = model.gameObject.GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        // ���� ����̸� ����ִ� ���� ��� ������ �Ǵ� �� ��ȣ�ۿ�� �� ��ü�� �θ� ������Ʈ ����
        if (transform.parent.gameObject.transform.parent.name.Contains("Left"))
        {
            leftHand = true;
            handPos = ARAVRInput.LHandPosition;
        }
        else
        {
            leftHand = false;
            handPos = ARAVRInput.RHandPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ��ư Ŭ������ �ʵ� ���� on off
        if (!readyShoot)
        {
            // �޼տ� ����ְ� �޼��� �ι�° ��ư�� �����ٸ� && �߷��� Ȱ��ȭ ���°� �ƴ϶��
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }
        else if (readyShoot)
        {
            // �߷��� �����߿� ���� �´ٸ� �߷��� ���� ���߱�
            if (leftHand && ARAVRInput.GetUp(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                makeFieldGauge = false;
            else if (!leftHand && ARAVRInput.GetUp(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                makeFieldGauge = false;

            // �߷��� Ȱ��ȭ �����ε� �ٽ� ��ư�� �����ٸ� �߷��� ����
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }

        if (makeFieldGauge)
        {
            MakeGauge();
        }
        if(readyShoot)
            SelectObject();
    }

    // �� ĥ �غ��
    private void ReadyToShoot()
    {
        readyShoot = true;
        makeFieldGauge = true;

        // Ư��ȿ�� sphere ������ �ʱ�ȭ
        sphereSize = 0f;
        time = 0f;
    }
    private void MakeGauge()
    {
        // �ð��� ���� �׷���� �ʵ� Ư��ȿ�� ���� Ȯ��
        time += Time.deltaTime / 2;
        sphereSize = time /** size*/;
        sphereSize = Mathf.Clamp(sphereSize, 0f, maxSphereSize);

        //����� ���� ����
        float r = Mathf.Lerp(darkBlueColor.r, whiteColor.r, sphereSize / maxSphereSize);
        float g = Mathf.Lerp(darkBlueColor.g, whiteColor.g, sphereSize / maxSphereSize);
        float b = Mathf.Lerp(darkBlueColor.b, whiteColor.b, sphereSize / maxSphereSize);
        Color color = new Color(r, g, b);
        batMesh.materials[0].color = color;

        // ���� ǥ�� 
        fieldSphere.gameObject.SetActive(true);
        fieldSphere.position = transform.position;
        fieldSphere.localScale = new Vector3(sphereSize, sphereSize, sphereSize);

        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, sphereSize/2, Vector3.up, 0f);
        // htis�鿡�� ���ư� �� �ִ� ��ü �ɷ�����
        if (hits.Length > 0)
        {
            for (int j = 0; j < hits.Length; j++)
            {
                // ������ ��ü�� �߷��忡 �� �� �ִ� ��ü���
                if (hits[j].transform.gameObject.GetComponent<LevitatableObject>() && !hits[j].transform.GetComponent<ObjectPosition>().gravityFieldObject)
                {
                    // �θ� ���� �� bool�� ����
                    hits[j].transform.parent = activeObjectPool.transform;
                    hits[j].transform.GetComponent<ObjectPosition>().gravityFieldObject = true;
                    hits[j].transform.GetComponent<ObjectPosition>().inGravityFieldObject = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sphereSize);
    }

    public GameObject[] inGravityFieldObjects;
    // ��Ƽ�� ������Ʈ Ǯ�� �ִ� ��ü�� 1�� �̻��̶�� �ȿ� �ִ� ��ü�� ������� ���� ����
    private void SelectObject()
    {
        int count = activeObjectPool.childCount;
        int num = 0;
        inGravityFieldObjects = new GameObject[count];
        if (count > 0)
        {
            for(int i = 0; i < activeObjectPool.childCount; i++)
            {
                if (activeObjectPool.GetChild(i).transform.gameObject.GetComponent<ObjectPosition>().inGravityFieldObject)
                {
                    inGravityFieldObjects[num] = activeObjectPool.GetChild(i).transform.gameObject;
                    num++;
                }
            }
            foreach (GameObject target in inGravityFieldObjects)
            {
                target.GetComponent<Rigidbody>().isKinematic = true;
                target.transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                target.transform.forward = transform.forward;
            }
        }
    }

    private void FinishToShoot()
    {        
        //
        readyShoot = false;

        fieldSphere.gameObject.SetActive(false);
        // ������Ʈ�� �߷� Ȱ��ȭ�ϰ�
        int count = inGravityFieldObjects.Length;
        if (count > 0)
        {
            foreach (GameObject target in inGravityFieldObjects)
            {
                target.GetComponent<Rigidbody>().isKinematic = false;
                target.GetComponent<ObjectPosition>().inGravityFieldObject = false;
            }
            for(int i =0; i<count; i++)
            {
                inGravityFieldObjects[i] = null;
            }
        }
        //
        inGravityFieldObjects = new GameObject[0];
        // ����� �� ���� ���·� ����
        batMesh.materials[0].color = darkBlueColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����̿� ��� ��ü�� �ٶ��� ���ư� �� �ִ� ��ü���� Ȯ���ϰ� ������
        if (other.gameObject.GetComponent<LevitatableObject>())
        {
            // ������Ʈ Ǯ�� �ֱ�
            other.gameObject.transform.parent = activeObjectPool;
            //
            other.transform.GetComponent<ObjectPosition>().gravityFieldObject = true;
            other.transform.GetComponent<ObjectPosition>().inGravityFieldObject = true;

        }
    }

    // SpecialBatManager���� SendMessage�� ȣ��
    private void ReturnNomal()
    {
        FinishToShoot();

        // acticeObjectPool�� �ִ� ��� ������Ʈ ���� ���·� ����
        int objectCount = activeObjectPool.transform.childCount;
        GameObject[] handPoolObjects = new GameObject[objectCount];
        for (int i = 0; i < objectCount; i++)
        {
            handPoolObjects[i] = activeObjectPool.transform.GetChild(i).gameObject;
        }

        foreach (GameObject target in handPoolObjects)
        {
            // ��ȣ�ۿ��� ������Ʈ�鿡�� ���� ��ġ���� ���������� ���ư���� ����
            if (target.GetComponent<ObjectPosition>())
                target.GetComponent<ObjectPosition>().ReturnToNomal("gravityFieldObject");
        }

        // �տ� ����ִ� ObjectPool�� �ִ� ��� ������Ʈ ���� ���·� ����
        if (leftHand)
        {
            int objectCountRight = activeObjectPoolRightHand.transform.childCount;
            if (objectCountRight > 0)
            {
                // �޼տ� ����̸� ����������� ������ �տ� ��ü�� ���� �� ����
                GameObject handPoolObject = activeObjectPoolRightHand.transform.GetChild(0).gameObject;
                if (handPoolObject.GetComponent<ObjectPosition>())
                    if (handPoolObject.GetComponent<ObjectPosition>().gravityFieldObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("gravityFieldObject");
            }
        }
        else
        {
            int objectCountLeft = activeObjectPoolLeftHand.transform.childCount;
            if (objectCountLeft > 0)
            {
                // �����տ� ����̸� ����������� �޼տ� ��ü�� ���� �� ����
                GameObject handPoolObject = activeObjectPoolLeftHand.transform.GetChild(0).gameObject;
                if (handPoolObject.GetComponent<ObjectPosition>())
                    if (handPoolObject.GetComponent<ObjectPosition>().gravityFieldObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("gravityFieldObject");
            }
        }
    }
}


