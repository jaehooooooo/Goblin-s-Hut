using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyBat : MonoBehaviour
{
    public GameObject activeObjectPool;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform model;             // ����� ����
    public Renderer batMesh;            // ����� �Ž�
    public Transform[] pointParticle;   // ���� ����� Ư��ȿ���� ����� ��ƼŬ��

    public Vector3 handPos;             // �� ��ġ ����
    public Transform batHitPos;         // Ư��ȿ�� ���� ��ġ
    private bool leftHand;              // ���� ����̸� ����ִ� ���� �޼��̸� true, �������̸� false
    public bool readyShoot;            // Ư��ȿ�� ���� ���� ����
    private float minDistance = 0.1f;   // Ư��ȿ�� �� �ּ� �Ÿ�

    private Color whiteColor = new Color(1, 1, 1);
    private Color pinkColor = new Color(1, 0.49f, 0.78f);

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
        if (readyShoot)
        {
            // ���� ����Ʈ �غ�����ε� ��ư�� �� �� �� �����ٸ�
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }
        else
        {
            // �޼տ� ����ְ� �޼��� �ι�° ��ư�� �����ٸ� && ���� ����Ʈ �غ���°� �ƴ϶��
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }

    }

    // Ư�� ȿ�� �غ��
    private void ReadyToShoot()
    {        
        // ����� �� ������� ����
        batMesh.materials[0].color = whiteColor;
        // ���� ����Ʈ�� �߻��� �غ�
        readyShoot = true;

        // ����Ʈ �׸��� ����
        StartCoroutine(IDrawButterfly());
    }
    private void FinishToShoot()
    {
        // ����� �� ���� ���·� ����
        batMesh.materials[0].color = pinkColor;
        //
        readyShoot = false;
        // �ڷ�ƾ�� ������̶�� �ڷ�ƾ ���߱�
        StopAllCoroutines();
        foreach (Transform particle in pointParticle)
        {
            particle.gameObject.SetActive(false);
        }

    }

    // ���� ����Ʈ ���� ���°� �Ǹ� 5�� ���� ������ ����Ʈ�� ����
    IEnumerator IDrawButterfly()
    {
        yield return null;
        int num = 0;
        float length = pointParticle.Length;

        while (true)
        {
            yield return null;
            //���� ��ġ�� ���� ��ġ������ ������ ������ġ �̻��̸� ���ο� ��ġ�� ����
             //���� ��ġ�� ����
            Vector3 pos = batHitPos.transform.position + new Vector3(0,0.5f,0);

            if (num == 0)
            {
                //����Ʈ Ȱ��ȭ�ϰ� ����Ʈ�� ��ġ�� ���� ��ġ�� ����
                pointParticle[num].transform.position = pos;
                pointParticle[num].gameObject.SetActive(true);

                //����� ���� ����
                float r = Mathf.Lerp(whiteColor.r, pinkColor.r, num / length);
                float g = Mathf.Lerp(whiteColor.g, pinkColor.g, num / length);
                float b = Mathf.Lerp(whiteColor.b, pinkColor.b, num / length);
                Color color = new Color(r, g, b);
                batMesh.materials[0].color = color;
                num++;
            }
            else if (num > 0 && Vector3.Distance(pos, pointParticle[num - 1].transform.position) > minDistance)
            {
                //����Ʈ Ȱ��ȭ�ϰ� ����Ʈ�� ��ġ�� ���� ��ġ�� ����
                pointParticle[num].transform.position = pos;
                pointParticle[num].gameObject.SetActive(true);

                //����� ���� ����
                float r = Mathf.Lerp(pinkColor.r, whiteColor.r, (float)(num / length));
                float g = Mathf.Lerp(whiteColor.g, pinkColor.g, (float)(num / length));
                float b = Mathf.Lerp(whiteColor.b, pinkColor.b, (float)(num / length));
                Color color = new Color(r, g, b);
                batMesh.materials[0].color = color;
                num++;
            }
            else
            {
                print("�Ÿ��� �ʹ� �����");
            }

            //10���� ��ƼŬ�� �� ��ٸ� FinishToShoot()
            if (num / length == 1)
            {
                yield return new WaitForSeconds(7f);
                FinishToShoot();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����̿� ��� ��ü�� ������ �� �� �ִ� ��ü���� Ȯ���ϰ� �������� ����
        if (other.gameObject.GetComponent<ChangeButterflyObject>())
        {
            other.transform.parent = activeObjectPool.transform;
            other.gameObject.GetComponent<ChangeButterflyObject>().ChangeButterfly();
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
                target.GetComponent<ObjectPosition>().ReturnToNomal("changeButterflyObject");
        }

    }
}
