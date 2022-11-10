using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBat : MonoBehaviour
{
    private bool leftHand;  // ���� ����̸� ����ִ� ���� �޼��̸� true, �������̸� false
    public Transform activeObjectPool;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform activeObjectPoolLeftHand;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform activeObjectPoolRightHand;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ

    public Vector3 handPos;

    private bool readyShoot;
    public Transform fireBalls;

    public Transform model;
    public Renderer batMesh;

    private Color whiteColor = new Color(1, 1, 1);
    private Color redColor = new Color(0.7735849f, 0.1824492f, 0.9046201f);

    // �ֵθ��� ���� �ӵ� üũ
    Vector3 nowPos = new Vector3(0, 0, 0); // ���� ��ġ��
    Vector3 prePos = new Vector3(0, 0, 0); // �������� �� ��ġ��
    public float nowSpeed;  // ���� �����ӿ��� ����� �ӵ�


    private void Start()
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
        if(readyShoot)
        {
            // ��ĥ �غ�����ε� ��ư�� �� �� �� �����ٸ�
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }
        else
        {
            // �޼տ� ����ְ� �޼��� �ι�° ��ư�� �����ٸ� && �� ĥ �غ���°� �ƴ϶��
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }

        if (readyShoot)
            CheckBatSpeed();
    }

    // �� ĥ �غ��
    private void ReadyToShoot()
    {
        // ���� ��ġ�� ���� Pos
        Vector3 ballPos = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0, -0.15f, 0);
        RaycastHit hit;

        //// ���࿡ �� ��ġ�� � ��ü�� �����Ѵٸ� ���� ��ġ��ų ������ �����մϴ� ��� �ȳ� ���� �׷��� ������ ����
        if (Physics.SphereCast(ballPos, 0.05f, Camera.main.transform.forward, out hit, 0.5f))
        {
            // ������ ��ü ����
            print("�ش� ��ġ�� �ȵ�");
        }
        else
        {
            // ������ ��ü ����
            print("�ش� ��ġ ����");

            if(!fireBalls.gameObject.activeSelf)
            {
                // ������ �ش� ��ġ���� ����ϰ� ������� ����
                readyShoot = true;
                // ����� �� ����
                batMesh.materials[0].color = whiteColor;
                if (!fireBalls.gameObject.activeSelf)
                {
                    // �� �� ���տ� ��ġ��Ű��
                    fireBalls.gameObject.SetActive(true);
                    fireBalls.gameObject.GetComponent<FireBall>().ReadyToHit();
                }
            }
            else
            {
                print("�� �غ� �ȉ���");
            }
        }
    }
    private void FinishToShoot()
    {
        print("�� ��Ȱ��ȭ");

        // �� ��Ȱ��ȭ
        fireBalls.gameObject.SetActive(false);
        // ����� �� ���� ���·� ����
        batMesh.materials[0].color = redColor;
        // ����� �ӵ�üũ ����
        readyShoot = false;
    }

    private void CheckBatSpeed()
    {
        // ���� ��ġ ���� ��ġ�� ����
        prePos = nowPos;
        // ���� ��ġ ����
        nowPos = transform.position;
        // �ӵ� = �Ÿ� / �ð�
        nowSpeed = Mathf.Abs(Vector3.Distance(nowPos, prePos) / Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����̿� ��� ��ü�� �ҿ� Ż �� �ִ� ��ü���� Ȯ���ϰ� �ҿ� Ÿ��
        if (other.gameObject.GetComponent<InflammableObject>())
        {
            other.transform.parent = activeObjectPool.transform;
            other.gameObject.GetComponent<InflammableObject>().Burn();
        }

        if (other.gameObject.GetComponent<FireBall>())
        {
            // ����� �� ���� ���·� ����
            batMesh.materials[0].color = redColor;
            // �� ĥ�غ� false
            readyShoot = false;
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
                target.GetComponent<ObjectPosition>().ReturnToNomal("inflammableObject");
        }

        // �տ� ����ִ� ObjectPool�� �ִ� ��� ������Ʈ ���� ���·� ����
        if(leftHand)
        {
            int objectCountRight = activeObjectPoolRightHand.transform.childCount;
            if (objectCountRight > 0)
            {
                // �޼տ� ����̸� ����������� ������ �տ� ��ü�� ���� �� ����
                GameObject handPoolObject = activeObjectPoolRightHand.transform.GetChild(0).gameObject;
                if (handPoolObject.GetComponent<ObjectPosition>())
                    if (handPoolObject.GetComponent<ObjectPosition>().inflammableObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("inflammableObject");
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
                    if (handPoolObject.GetComponent<ObjectPosition>().inflammableObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("inflammableObject");
            }
        }
    }
}
