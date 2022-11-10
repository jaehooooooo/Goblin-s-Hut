using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationBat : MonoBehaviour
{
    // ���η����� ����
    public LineRenderer lr;
    public float maxDistance;

    private bool leftHand;                          // ���� ����̸� ����ִ� ���� �޼��̸� true, �������̸� false
    public Transform activeObjectPool;              // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform activeObjectPoolLeftHand;      // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Transform activeObjectPoolRightHand;     // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Vector3 handPos;                         // �� ��ġ

    public Transform model;                     // ����� �ٵ�
    public Renderer batMesh;                    // ����� �Ž�

    public GameObject particle;                 // ���� �߻��Ҷ� �÷��̵� ��ƼŬ

    private bool readyShoot;                    // ���� �߻� ���� ����
    private bool checkObject;                   // ���� �߻� ���� ����
    private bool changeColor;                   // Ÿ�� ������Ʈ ���� �����ϴ� ����
    public float duration = 3f;                 // Ÿ�� ���� ���� �ð�
    private float targetDistance;               // Ÿ�� ������Ʈ�� ����� ������ ���۰Ÿ�
    private float nowTargetDistance;            // ���� Ÿ�� ������Ʈ�� ����� ������ �Ÿ�
    public float maxTargetDistance;             // Ÿ�� ������Ʈ�� ����� ������ �ִ� �Ÿ�

    private Color whiteColor = new Color(1, 1, 1);
    private Color naivyColor = new Color(0.03f, 0.06f, 0.16f);

    private void Awake()
    {
        // ���η����� ������Ʈ ���
        lr = GetComponent<LineRenderer>();
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
        // ��ư Ŭ������ �ٶ��� �غ� on off
        if (!readyShoot)
        {
            // �޼տ� ����ְ� �޼��� �ι�° ��ư�� �����ٸ� && �������� ������� �ʴٸ�
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }
        else if (readyShoot)
        {
            // �������� ��� �ִ� ���¿��� ��ư�� �ٽ� �����ٸ�
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }

        if (readyShoot && checkObject)
            ShootLine();
    }

    private void ReadyToShoot()
    {
        // ���� �׸��� ����
        readyShoot = true;
        checkObject = true;
        // ���η����� ������Ʈ Ȱ��ȭ
        lr.enabled = true;
    }

    private Vector3 hitPoint;
    public GameObject preObject;
    public GameObject nowObject;
    private Color preTargetColor;
    private Color nowTargetColor;
    
    // ���ߺξ��� �� �ִ� ��ü ã��
    private void ShootLine()
    {
        // ����̸� �������� Ray�� ����
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hitInfo;
        int layer = 1 << LayerMask.NameToLayer("Simple Object");
        // ���� ������Ʈ�� ��Ҵٸ�
        if(Physics.Raycast(ray,out hitInfo, maxDistance,layer))
        {
            // Ray�� �ε��� ������ ���� �׸���
            lr.SetPosition(0, ray.origin);
            lr.SetPosition(1, hitInfo.point);

            // �ε��� ���� Ȯ�� �G �ε��� ��ü Ȯ��
            if(hitInfo.transform.GetComponent<LevitatableObject>())
            {
                // �ε��� ������ ��ƼŬ ����
                hitPoint = hitInfo.point;
                particle.SetActive(true);
                particle.transform.position = hitPoint;

                preObject = nowObject;
                nowObject = hitInfo.transform.gameObject;
                if(preTargetColor == new Color(0,0,0))
                {
                    preTargetColor = nowTargetColor;
                    nowTargetColor = hitInfo.transform.GetComponent<LevitatableObject>().mesh.materials[0].color;
                }

                // ������ ����  ��ü üũ
                if (preObject == nowObject)
                {
                    if (!changeColor)
                    {
                        changeColor = true;
                        print(hitInfo.distance + "���� �Ÿ�");
                        StartCoroutine(IChangeObjectColor());
                    }
                }
            }
            // ����ִ� ��ü�� ���ߺξ��� �Ұ����� ��ü���
            else
            {
                // �� �����ϰ� �ִ� �ൿ ���߱�
                ResetColor();
            }


        }
        // ���� ������Ʈ�� ������ �ƴ϶��
        else
        {
            // �� �����ϰ� �ִ� �ൿ ���߱�
            ResetColor();
            particle.SetActive(false);

            lr.SetPosition(0, ray.origin);
            lr.SetPosition(1, ray.origin + transform.up * maxDistance);
        }
    }

    private void ResetColor()
    {
        changeColor = false;
        // �ڷ�ƾ ����ϱ�
        StopAllCoroutines();
        if (preObject != null)
        {
            print(preObject.name);
            // ���� �����ӿ� ��� ��ü�� ���� ������� ����
            preObject.transform.GetComponent<LevitatableObject>().mesh.materials[0].color = preTargetColor;
        }

        // object�� �ʱ�ȭ
        nowObject = null;
        preObject = null;
        preTargetColor = new Color(0, 0, 0);
        nowTargetColor = new Color(0, 0, 0);
    }

    private IEnumerator IChangeObjectColor()
    {
        float time = 0;
        while(true)
        {
            // ��ü�� ����� ���� ���� ���� �ð��� �����
            if (time / duration > 1f * nowObject.transform.GetComponent<ObjectPosition>().objectSize)
                break;

            time += Time.deltaTime;

            //��ü ���� ����
            float r = Mathf.Lerp(nowTargetColor.r, naivyColor.r, time / duration);
            float g = Mathf.Lerp(nowTargetColor.g, naivyColor.g, time / duration);
            float b = Mathf.Lerp(nowTargetColor.b, naivyColor.b, time / duration);
            Color color = new Color(r, g, b);
            nowObject.transform.GetComponent<LevitatableObject>().mesh.materials[0].color = color;
            yield return null;
        }
        yield return null;

        // ��� �Ϸ�
        SelectObject();

    }

    private void SelectObject()
    {
        // ����ĳ��Ʈ ��Ȱ��ȭ
        checkObject = false;

        // ��ƼŬ ��Ȱ��ȭ
        particle.SetActive(false);
        // ���η������� �׷��� �����ְ�
        StartCoroutine(DrawLine());
        StartCoroutine(CatchFliyingObject());
        // ������Ʈ ������Ʈ Ǯ�� �־��ְ� // ������Ʈ�� ��ġ���� ����
        if (leftHand)
            nowObject.transform.parent = activeObjectPoolLeftHand;
        else
            nowObject.transform.parent = activeObjectPoolRightHand;
        // ������Ʈ�� �߷� ��Ȱ��ȭ�ϰ�
        nowObject.transform.GetComponent<Rigidbody>().useGravity = false;
        // bool ���ߺξ� ��
        nowObject.transform.GetComponent<ObjectPosition>().levitatableObject = true;

        // ����̿� ��ü ���� �Ÿ� üũ
        targetDistance = Vector3.Distance(transform.position, nowObject.transform.position);
        
    }

    private IEnumerator DrawLine()
    {
        yield return null;
        while (true)
        {
            // ���η������� �׷��� �����ְ�
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, nowObject.transform.position);
            yield return null;
        }
    }

    private IEnumerator CatchFliyingObject()
    {
        yield return null;
        while (true)
        {
            // ���� �� ��ü ������ �Ÿ��� �ʹ� �ִٸ�
            // ����̿� ��ü ���� �Ÿ� üũ
            nowTargetDistance = Vector3.Distance(transform.position, nowObject.transform.position);
            Vector3 prePos = nowObject.transform.position;

            if (nowTargetDistance - targetDistance > maxTargetDistance)
            {
                // �θ� �ٽ� ��������
                if (leftHand)
                    nowObject.transform.parent = activeObjectPoolLeftHand;
                else
                    nowObject.transform.parent = activeObjectPoolRightHand;

                // 1�� ��ٷ��ֱ�
                yield return new WaitForSeconds(1f);

                // ���� ��ü �ӵ� ���̱�
                nowObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                // ���� ��ü ��ġ �ľ�
                Vector3 nowPos = nowObject.transform.position;

                float time = 0f;
                // ��ü �ٽ� ���ƿ���
                while (true)
                {
                    // 
                    if (time > 1f)
                        break;

                    time += Time.deltaTime;
                    nowObject.transform.position = Vector3.Lerp(nowPos, prePos, time);
                    yield return null;
                }
                // ���� ��ü �ӵ� ���̱�
                nowObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                nowObject.transform.position = prePos;
                print(nowObject.transform.position);
                print(transform.position + (transform.up * targetDistance));
                nowObject.transform.position = transform.position + (transform.up * targetDistance);
                prePos = nowObject.transform.position;


            }
            yield return null;
        }
    }

    public float sample;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (transform.up * sample),new Vector3(0.1f,0.1f,0.1f)); 
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.localPosition + (transform.up * sample),new Vector3(0.1f,0.1f,0.1f));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.up * sample, new Vector3(0.1f,0.1f,0.1f));
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.localPosition + (transform.up * sample),new Vector3(0.1f,0.1f,0.1f));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.localPosition + (transform.forward * sample),new Vector3(0.1f,0.1f,0.1f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.localPosition + (Vector3.up * sample),new Vector3(0.1f,0.1f,0.1f));
        Gizmos.color = Color.grey;
        Gizmos.DrawWireCube(transform.localPosition + (Vector3.forward * sample),new Vector3(0.1f,0.1f,0.1f));
    }

    // ��ü ���ߺξ� ������
    private void FinishToShoot()
    {
        StopAllCoroutines();
        // ���� ������ ��Ȱ��ȭ
        lr.enabled = false;
        // ����� �� ���� ���·� ����
        batMesh.materials[0].color = naivyColor;
        // select���� ��ü�� �ִٸ�
        if(changeColor)
        {
            // �θ� ����
            nowObject.transform.parent = activeObjectPool;
            // ������Ʈ�� �߷� Ȱ��ȭ�ϰ�
            nowObject.transform.GetComponent<Rigidbody>().useGravity = true;

            ResetColor();
        }

        readyShoot = false;
        changeColor = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����̿� ��� ��ü�� ���߷��� �� �� �ִ� ��ü���� Ȯ���ϰ� ������
        if (other.gameObject.GetComponent<LevitatableObject>())
        {
            // ��ü ���߷����� ����
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            // �θ� ����
            other.transform.parent = activeObjectPool;
            other.transform.GetComponent<ObjectPosition>().levitatableObject = true;

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
                target.GetComponent<ObjectPosition>().ReturnToNomal("levitatableObject");
        }

        // �տ� ����ִ� ObjectPool�� �ִ� ��� ������Ʈ ���� ���·� ����
        int objectCountLeft = activeObjectPoolLeftHand.transform.childCount;
        if (objectCountLeft > 0)
        {
            // �����տ� ����̸� ����������� �޼տ� ��ü�� ���� �� ����
            GameObject handPoolObject = activeObjectPoolLeftHand.transform.GetChild(0).gameObject;
            if (handPoolObject.GetComponent<ObjectPosition>())
                if (handPoolObject.GetComponent<ObjectPosition>().levitatableObject)
                    handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("levitatableObject");
        }
        int objectCountRight = activeObjectPoolRightHand.transform.childCount;
        if (objectCountRight > 0)
        {
            // �޼տ� ����̸� ����������� ������ �տ� ��ü�� ���� �� ����
            GameObject handPoolObject = activeObjectPoolRightHand.transform.GetChild(0).gameObject;
            if (handPoolObject.GetComponent<ObjectPosition>())
                if (handPoolObject.GetComponent<ObjectPosition>().levitatableObject)
                    handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("levitatableObject");
        }

    }
}
