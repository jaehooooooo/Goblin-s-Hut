using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBat : MonoBehaviour
{

    private bool leftHand;              // ���� ����̸� ����ִ� ���� �޼��̸� true, �������̸� false
    public Transform activeObjectPool;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Vector3 handPos;             // �� ��ġ


    public Transform model;             // ����� �ٵ�
    public Renderer batMesh;            // ����� �Ž�

    public ParticleSystem particle;     // �ٶ� �߻��Ҷ� �÷��̵� ��ƼŬ
    public Transform windSphere;        // �ٶ������� ������ ���� ����

    private bool readyShoot;            // �ٶ� �߻� ���� ����
    private bool makeWindGauge;         // �ٶ� ������ ������ ����

    private float sphereSize;           // Ư��ȿ�� sphereũ��
    private float maxSphereSize =3f;        // Ư��ȿ�� sphere �ִ�ũ��

    private Color whiteColor = new Color(1, 1, 1);
    private Color shilverColor = new Color(0.35f, 0.35f, 0.35f);

    private void Awake()
    {
        // �ش� ����̰� �����ִ� ���׸��� �߿��� �ι�° ���׸����� ������ ã�ƿ�
        batMesh = model.gameObject.GetComponent<Renderer>();
        particle.Stop();
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
        // ��ư Ŭ������ �ٶ��� �غ� on off
        if(!readyShoot)
        {
            // �޼տ� ����ְ� �޼��� �ι�° ��ư�� �����ٸ� && �ٶ� �� �غ���°� �ƴ϶��
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }
        if (readyShoot)
        {
            // �ٶ� �� �غ�����ε� ��ư���� ���� �´ٸ�
            if (leftHand && ARAVRInput.GetUp(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetUp(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }

        if (makeWindGauge)
            MakeGauge();
    }

    // �� ĥ �غ��
    private void ReadyToShoot()
    {
        readyShoot = true;
        makeWindGauge = true;

        // sphere ������ �ʱ�ȭ
        sphereSize = 0f;
        time = 0f;
    }
    private void FinishToShoot()
    {
        windSphere.gameObject.SetActive(false);

        RaycastHit[] hits;
        hits = Physics.SphereCastAll(Camera.main.transform.position, sphereSize, Vector3.up, 0f);
        // htis�鿡�� ���ư� �� �ִ� ��ü �ɷ�����
        if (hits.Length > 0)
        {
            for (int j = 0; j < hits.Length; j++)
            {
                // ������ ��ü�� ���ư� �� �ִ� ��ü���
                if (hits[j].transform.gameObject.GetComponent<WindableObject>())
                {
                    hits[j].transform.parent = activeObjectPool.transform;
                    Vector3 forcedir = (hits[j].transform.position - Camera.main.transform.position).normalized;
                    hits[j].transform.gameObject.GetComponent<WindableObject>().FlyHigh(forcedir * sphereSize);
                }
            }
        }
        // ����Ʈ Ȱ��ȭ
        particle.transform.position = Camera.main.transform.position + new Vector3(0, -1, 0);
        particle.transform.localScale = new Vector3(sphereSize,sphereSize,sphereSize);
        particle.Play();

        // ����� �� ���� ���·� ����
        batMesh.materials[0].color = shilverColor;
        //
        readyShoot = false;
        makeWindGauge = false;
    }
    private float time;
    public float size;
    private void MakeGauge()
    {
        // �ð��� ���� ��ǳ Ư��ȿ�� ���� Ȯ��
        time += Time.deltaTime/2;
        sphereSize = time * size;
        sphereSize = Mathf.Clamp(sphereSize, 0f, maxSphereSize);

        //����� ���� ����
        float r = Mathf.Lerp(shilverColor.r, whiteColor.r, sphereSize / maxSphereSize);
        float g = Mathf.Lerp(shilverColor.g, whiteColor.g, sphereSize / maxSphereSize);
        float b = Mathf.Lerp(shilverColor.b, whiteColor.b, sphereSize / maxSphereSize);
        Color color = new Color(r, g, b);
        batMesh.materials[0].color = color;

        // ���� ǥ�� 
        windSphere.gameObject.SetActive(true);
        windSphere.position = Camera.main.transform.position;
        windSphere.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(Camera.main.transform.position, sphereSize);
    //}
    private void OnTriggerEnter(Collider other)
    {        
        // ����̿� ��� ��ü�� �ٶ��� ���ư� �� �ִ� ��ü���� Ȯ���ϰ� ������
        if (other.gameObject.GetComponent<WindableObject>())
        {
            Vector3 forceDir = (other.transform.position - transform.position).normalized;
            other.transform.parent = activeObjectPool.transform;
            other.gameObject.GetComponent<WindableObject>().FlyHigh(forceDir);
        }
    }

    // SpecialBatManager���� SendMessage�� ȣ��
    private void ReturnNomal()
    {
        // acticeObjectPool�� �ִ� ��� ������Ʈ ���� ���·� ����
        int objectCount = activeObjectPool.transform.childCount;

        GameObject[] handPoolObjects = new GameObject[objectCount];
        for (int i =0; i< objectCount; i++)
        {
            handPoolObjects[i] = activeObjectPool.transform.GetChild(i).gameObject;
        }

        foreach(GameObject target in handPoolObjects)
        {
            // ��ȣ�ۿ��� ������Ʈ�鿡�� ���� ��ġ���� ���������� ���ư���� ����
            if (target.GetComponent<ObjectPosition>())
                target.GetComponent<ObjectPosition>().ReturnToNomal("windableObject");
        }

    }

}
