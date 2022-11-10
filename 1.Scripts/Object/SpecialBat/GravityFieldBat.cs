using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldBat : MonoBehaviour
{
    private bool leftHand;              // 현재 방망이를 잡고있는 손이 왼손이면 true, 오른손이면 false
    public Transform activeObjectPool;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform activeObjectPoolLeftHand;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform activeObjectPoolRightHand;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀


    public Vector3 handPos;             // 손 위치

    public Transform model;             // 방망이 바디
    public Renderer batMesh;            // 방망이 매쉬

    public Transform fieldSphere;        // 중력장게이지 모을때 보일 형태

    private bool readyShoot;            // 중력장 생성 가능 상태
    private bool makeFieldGauge;         // 중력장 게이지 모으는 상태
    private bool useField;         // 중력장 체크하는 상태

    private float time;
    //public float size;


    private float sphereSize;           // 특수효과 sphere크기
    private float maxSphereSize = 3f;   // 특수효과 sphere 최대크기

    private Color whiteColor = new Color(1, 1, 1);
    private Color darkBlueColor = new Color(0.13f, 0.32f, 0.33f);

    private void Awake()
    {
        // 해당 방망이가 갖고있는 매테리얼 중에서 두번째 메테리얼의 색상을 찾아옴
        batMesh = model.gameObject.GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        // 현재 방망이를 잡고있는 손이 어느 손인지 판단 후 상호작용시 들어갈 물체의 부모 오브젝트 설정
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
        // 버튼 클릭으로 필드 생성 on off
        if (!readyShoot)
        {
            // 왼손에 잡고있고 왼손의 두번째 버튼을 눌렀다면 && 중력장 활성화 상태가 아니라면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }
        else if (readyShoot)
        {
            // 중력장 생성중에 손을 뗏다면 중력장 생성 멈추기
            if (leftHand && ARAVRInput.GetUp(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                makeFieldGauge = false;
            else if (!leftHand && ARAVRInput.GetUp(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                makeFieldGauge = false;

            // 중력장 활성화 상태인데 다시 버튼을 눌렀다면 중력장 해제
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

    // 공 칠 준비됨
    private void ReadyToShoot()
    {
        readyShoot = true;
        makeFieldGauge = true;

        // 특수효과 sphere 사이즈 초기화
        sphereSize = 0f;
        time = 0f;
    }
    private void MakeGauge()
    {
        // 시간에 따라 그래비디 필드 특수효과 영역 확대
        time += Time.deltaTime / 2;
        sphereSize = time /** size*/;
        sphereSize = Mathf.Clamp(sphereSize, 0f, maxSphereSize);

        //방망이 색상 변경
        float r = Mathf.Lerp(darkBlueColor.r, whiteColor.r, sphereSize / maxSphereSize);
        float g = Mathf.Lerp(darkBlueColor.g, whiteColor.g, sphereSize / maxSphereSize);
        float b = Mathf.Lerp(darkBlueColor.b, whiteColor.b, sphereSize / maxSphereSize);
        Color color = new Color(r, g, b);
        batMesh.materials[0].color = color;

        // 영역 표시 
        fieldSphere.gameObject.SetActive(true);
        fieldSphere.position = transform.position;
        fieldSphere.localScale = new Vector3(sphereSize, sphereSize, sphereSize);

        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, sphereSize/2, Vector3.up, 0f);
        // htis들에서 날아갈 수 있는 물체 걸러내기
        if (hits.Length > 0)
        {
            for (int j = 0; j < hits.Length; j++)
            {
                // 감지된 물체가 중력장에 들어갈 수 있는 물체라면
                if (hits[j].transform.gameObject.GetComponent<LevitatableObject>() && !hits[j].transform.GetComponent<ObjectPosition>().gravityFieldObject)
                {
                    // 부모 변경 및 bool값 변경
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
    // 엑티브 오브젝트 풀에 있는 물체가 1개 이상이라면 안에 있는 물체와 방망이의 높이 맞춤
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
        // 오브젝트의 중력 활성화하고
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
        // 방망이 색 원래 상태로 복귀
        batMesh.materials[0].color = darkBlueColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 방망이와 닿는 물체가 바람에 날아갈 수 있는 물체인지 확인하고 날리기
        if (other.gameObject.GetComponent<LevitatableObject>())
        {
            // 오브젝트 풀에 넣기
            other.gameObject.transform.parent = activeObjectPool;
            //
            other.transform.GetComponent<ObjectPosition>().gravityFieldObject = true;
            other.transform.GetComponent<ObjectPosition>().inGravityFieldObject = true;

        }
    }

    // SpecialBatManager에서 SendMessage로 호출
    private void ReturnNomal()
    {
        FinishToShoot();

        // acticeObjectPool에 있는 모든 오브젝트 원래 상태로 복귀
        int objectCount = activeObjectPool.transform.childCount;
        GameObject[] handPoolObjects = new GameObject[objectCount];
        for (int i = 0; i < objectCount; i++)
        {
            handPoolObjects[i] = activeObjectPool.transform.GetChild(i).gameObject;
        }

        foreach (GameObject target in handPoolObjects)
        {
            // 상호작용한 오브젝트들에게 원래 위치값과 설정값으로 돌아가라고 전달
            if (target.GetComponent<ObjectPosition>())
                target.GetComponent<ObjectPosition>().ReturnToNomal("gravityFieldObject");
        }

        // 손에 잡고있는 ObjectPool에 있는 모든 오브젝트 원래 상태로 복귀
        if (leftHand)
        {
            int objectCountRight = activeObjectPoolRightHand.transform.childCount;
            if (objectCountRight > 0)
            {
                // 왼손에 방망이를 잡고있음으로 오른쪽 손에 물체를 잡을 수 있음
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
                // 오른손에 방망이를 잡고있음으로 왼손에 물체를 잡을 수 있음
                GameObject handPoolObject = activeObjectPoolLeftHand.transform.GetChild(0).gameObject;
                if (handPoolObject.GetComponent<ObjectPosition>())
                    if (handPoolObject.GetComponent<ObjectPosition>().gravityFieldObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("gravityFieldObject");
            }
        }
    }
}


