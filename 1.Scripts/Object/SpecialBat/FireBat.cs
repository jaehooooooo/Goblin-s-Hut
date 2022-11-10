using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBat : MonoBehaviour
{
    private bool leftHand;  // 현재 방망이를 잡고있는 손이 왼손이면 true, 오른손이면 false
    public Transform activeObjectPool;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform activeObjectPoolLeftHand;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform activeObjectPoolRightHand;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀

    public Vector3 handPos;

    private bool readyShoot;
    public Transform fireBalls;

    public Transform model;
    public Renderer batMesh;

    private Color whiteColor = new Color(1, 1, 1);
    private Color redColor = new Color(0.7735849f, 0.1824492f, 0.9046201f);

    // 휘두르는 순간 속도 체크
    Vector3 nowPos = new Vector3(0, 0, 0); // 현재 위치값
    Vector3 prePos = new Vector3(0, 0, 0); // 한프레임 전 위치값
    public float nowSpeed;  // 현재 프레임에서 방망이 속도


    private void Start()
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
        if(readyShoot)
        {
            // 공칠 준비상태인데 버튼을 한 번 더 눌렀다면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }
        else
        {
            // 왼손에 잡고있고 왼손의 두번째 버튼을 눌렀다면 && 공 칠 준비상태가 아니라면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }

        if (readyShoot)
            CheckBatSpeed();
    }

    // 공 칠 준비됨
    private void ReadyToShoot()
    {
        // 볼이 위치할 예정 Pos
        Vector3 ballPos = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + new Vector3(0, -0.15f, 0);
        RaycastHit hit;

        //// 만약에 그 위치에 어떤 물체가 존재한다면 공을 위치시킬 공간이 부족합니다 라는 안내 쓰고 그렇지 않으면 생성
        if (Physics.SphereCast(ballPos, 0.05f, Camera.main.transform.forward, out hit, 0.5f))
        {
            // 감지된 물체 있음
            print("해당 위치는 안됨");
        }
        else
        {
            // 감지된 물체 없음
            print("해당 위치 가능");

            if(!fireBalls.gameObject.activeSelf)
            {
                // 공한테 해당 위치에서 대기하고 있으라고 전달
                readyShoot = true;
                // 방망이 색 변경
                batMesh.materials[0].color = whiteColor;
                if (!fireBalls.gameObject.activeSelf)
                {
                    // 불 공 눈앞에 위치시키기
                    fireBalls.gameObject.SetActive(true);
                    fireBalls.gameObject.GetComponent<FireBall>().ReadyToHit();
                }
            }
            else
            {
                print("공 준비 안됬음");
            }
        }
    }
    private void FinishToShoot()
    {
        print("공 비활성화");

        // 공 비활성화
        fireBalls.gameObject.SetActive(false);
        // 방망이 색 원래 상태로 복귀
        batMesh.materials[0].color = redColor;
        // 방망이 속도체크 끄기
        readyShoot = false;
    }

    private void CheckBatSpeed()
    {
        // 이전 위치 현재 위치에 저장
        prePos = nowPos;
        // 현재 위치 저장
        nowPos = transform.position;
        // 속도 = 거리 / 시간
        nowSpeed = Mathf.Abs(Vector3.Distance(nowPos, prePos) / Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 방망이와 닿는 물체가 불에 탈 수 있는 물체인지 확인하고 불에 타기
        if (other.gameObject.GetComponent<InflammableObject>())
        {
            other.transform.parent = activeObjectPool.transform;
            other.gameObject.GetComponent<InflammableObject>().Burn();
        }

        if (other.gameObject.GetComponent<FireBall>())
        {
            // 방망이 색 원래 상태로 복귀
            batMesh.materials[0].color = redColor;
            // 공 칠준비 false
            readyShoot = false;
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
                target.GetComponent<ObjectPosition>().ReturnToNomal("inflammableObject");
        }

        // 손에 잡고있는 ObjectPool에 있는 모든 오브젝트 원래 상태로 복귀
        if(leftHand)
        {
            int objectCountRight = activeObjectPoolRightHand.transform.childCount;
            if (objectCountRight > 0)
            {
                // 왼손에 방망이를 잡고있음으로 오른쪽 손에 물체를 잡을 수 있음
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
                // 오른손에 방망이를 잡고있음으로 왼손에 물체를 잡을 수 있음
                GameObject handPoolObject = activeObjectPoolLeftHand.transform.GetChild(0).gameObject;
                if (handPoolObject.GetComponent<ObjectPosition>())
                    if (handPoolObject.GetComponent<ObjectPosition>().inflammableObject)
                        handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("inflammableObject");
            }
        }
    }
}
