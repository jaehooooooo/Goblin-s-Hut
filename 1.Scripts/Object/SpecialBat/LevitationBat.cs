using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationBat : MonoBehaviour
{
    // 라인렌더러 관련
    public LineRenderer lr;
    public float maxDistance;

    private bool leftHand;                          // 현재 방망이를 잡고있는 손이 왼손이면 true, 오른손이면 false
    public Transform activeObjectPool;              // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform activeObjectPoolLeftHand;      // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform activeObjectPoolRightHand;     // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Vector3 handPos;                         // 손 위치

    public Transform model;                     // 방망이 바디
    public Renderer batMesh;                    // 방망이 매쉬

    public GameObject particle;                 // 레이 발사할때 플레이될 파티클

    private bool readyShoot;                    // 레이 발사 가능 상태
    private bool checkObject;                   // 레이 발사 가능 상태
    private bool changeColor;                   // 타겟 오브젝트 색상 변경하는 상태
    public float duration = 3f;                 // 타겟 색상 변경 시간
    private float targetDistance;               // 타겟 오브젝트와 방망이 사이의 시작거리
    private float nowTargetDistance;            // 현재 타겟 오브젝트와 방망이 사이의 거리
    public float maxTargetDistance;             // 타겟 오브젝트와 방망이 사이의 최대 거리

    private Color whiteColor = new Color(1, 1, 1);
    private Color naivyColor = new Color(0.03f, 0.06f, 0.16f);

    private void Awake()
    {
        // 라인렌더러 컴포넌트 얻기
        lr = GetComponent<LineRenderer>();
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
        // 버튼 클릭으로 바람불 준비 on off
        if (!readyShoot)
        {
            // 왼손에 잡고있고 왼손의 두번째 버튼을 눌렀다면 && 레이저를 쏘고있지 않다면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }
        else if (readyShoot)
        {
            // 레이저를 쏘고 있는 상태에서 버튼을 다시 눌렀다면
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
        // 라인 그리기 시작
        readyShoot = true;
        checkObject = true;
        // 라인렌더러 컴포넌트 활성화
        lr.enabled = true;
    }

    private Vector3 hitPoint;
    public GameObject preObject;
    public GameObject nowObject;
    private Color preTargetColor;
    private Color nowTargetColor;
    
    // 공중부양할 수 있는 물체 찾기
    private void ShootLine()
    {
        // 방망이를 기준으로 Ray를 만듬
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hitInfo;
        int layer = 1 << LayerMask.NameToLayer("Simple Object");
        // 심플 오브젝트에 닿았다면
        if(Physics.Raycast(ray,out hitInfo, maxDistance,layer))
        {
            // Ray가 부딪힌 지점에 라인 그리기
            lr.SetPosition(0, ray.origin);
            lr.SetPosition(1, hitInfo.point);

            // 부딪힌 지점 확인 밎 부딪힌 물체 확인
            if(hitInfo.transform.GetComponent<LevitatableObject>())
            {
                // 부딪힌 지점에 파티클 생성
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

                // 프레임 단위  물체 체크
                if (preObject == nowObject)
                {
                    if (!changeColor)
                    {
                        changeColor = true;
                        print(hitInfo.distance + "레이 거리");
                        StartCoroutine(IChangeObjectColor());
                    }
                }
            }
            // 닿고있는 물체가 공중부양이 불가능한 물체라면
            else
            {
                // 색 변경하고 있는 행동 멈추기
                ResetColor();
            }


        }
        // 심플 오브젝트에 닿은게 아니라면
        else
        {
            // 색 변경하고 있는 행동 멈추기
            ResetColor();
            particle.SetActive(false);

            lr.SetPosition(0, ray.origin);
            lr.SetPosition(1, ray.origin + transform.up * maxDistance);
        }
    }

    private void ResetColor()
    {
        changeColor = false;
        // 코루틴 취소하기
        StopAllCoroutines();
        if (preObject != null)
        {
            print(preObject.name);
            // 이전 프레임에 쏘던 물체는 색상 원래대로 복귀
            preObject.transform.GetComponent<LevitatableObject>().mesh.materials[0].color = preTargetColor;
        }

        // object들 초기화
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
            // 물체의 사이즈에 따라 색상 변경 시간이 길어짐
            if (time / duration > 1f * nowObject.transform.GetComponent<ObjectPosition>().objectSize)
                break;

            time += Time.deltaTime;

            //물체 색상 변경
            float r = Mathf.Lerp(nowTargetColor.r, naivyColor.r, time / duration);
            float g = Mathf.Lerp(nowTargetColor.g, naivyColor.g, time / duration);
            float b = Mathf.Lerp(nowTargetColor.b, naivyColor.b, time / duration);
            Color color = new Color(r, g, b);
            nowObject.transform.GetComponent<LevitatableObject>().mesh.materials[0].color = color;
            yield return null;
        }
        yield return null;

        // 등록 완료
        SelectObject();

    }

    private void SelectObject()
    {
        // 레이캐스트 비활성화
        checkObject = false;

        // 파티클 비활성화
        particle.SetActive(false);
        // 라인렌더러는 그래도 보여주고
        StartCoroutine(DrawLine());
        StartCoroutine(CatchFliyingObject());
        // 오브젝트 오브젝트 풀에 넣어주고 // 오브젝트의 위치값을 통제
        if (leftHand)
            nowObject.transform.parent = activeObjectPoolLeftHand;
        else
            nowObject.transform.parent = activeObjectPoolRightHand;
        // 오브젝트의 중력 비활성화하고
        nowObject.transform.GetComponent<Rigidbody>().useGravity = false;
        // bool 공중부양 중
        nowObject.transform.GetComponent<ObjectPosition>().levitatableObject = true;

        // 방망이와 물체 사이 거리 체크
        targetDistance = Vector3.Distance(transform.position, nowObject.transform.position);
        
    }

    private IEnumerator DrawLine()
    {
        yield return null;
        while (true)
        {
            // 라인렌더러는 그래도 보여주고
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
            // 만약 두 물체 사이의 거리가 너무 멀다면
            // 방망이와 물체 사이 거리 체크
            nowTargetDistance = Vector3.Distance(transform.position, nowObject.transform.position);
            Vector3 prePos = nowObject.transform.position;

            if (nowTargetDistance - targetDistance > maxTargetDistance)
            {
                // 부모를 다시 돌려놓음
                if (leftHand)
                    nowObject.transform.parent = activeObjectPoolLeftHand;
                else
                    nowObject.transform.parent = activeObjectPoolRightHand;

                // 1초 기다려주기
                yield return new WaitForSeconds(1f);

                // 현재 물체 속도 줄이기
                nowObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                // 현재 물체 위치 파악
                Vector3 nowPos = nowObject.transform.position;

                float time = 0f;
                // 물체 다시 돌아오기
                while (true)
                {
                    // 
                    if (time > 1f)
                        break;

                    time += Time.deltaTime;
                    nowObject.transform.position = Vector3.Lerp(nowPos, prePos, time);
                    yield return null;
                }
                // 현재 물체 속도 줄이기
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

    // 물체 공중부양 끝내기
    private void FinishToShoot()
    {
        StopAllCoroutines();
        // 라인 렌더러 비활성화
        lr.enabled = false;
        // 방망이 색 원래 상태로 복귀
        batMesh.materials[0].color = naivyColor;
        // select중인 물체가 있다면
        if(changeColor)
        {
            // 부모 변경
            nowObject.transform.parent = activeObjectPool;
            // 오브젝트의 중력 활성화하고
            nowObject.transform.GetComponent<Rigidbody>().useGravity = true;

            ResetColor();
        }

        readyShoot = false;
        changeColor = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 방망이와 닿는 물체가 무중력이 될 수 있는 물체인지 확인하고 날리기
        if (other.gameObject.GetComponent<LevitatableObject>())
        {
            // 믈체 무중력으로 변경
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            // 부모 변경
            other.transform.parent = activeObjectPool;
            other.transform.GetComponent<ObjectPosition>().levitatableObject = true;

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
                target.GetComponent<ObjectPosition>().ReturnToNomal("levitatableObject");
        }

        // 손에 잡고있는 ObjectPool에 있는 모든 오브젝트 원래 상태로 복귀
        int objectCountLeft = activeObjectPoolLeftHand.transform.childCount;
        if (objectCountLeft > 0)
        {
            // 오른손에 방망이를 잡고있음으로 왼손에 물체를 잡을 수 있음
            GameObject handPoolObject = activeObjectPoolLeftHand.transform.GetChild(0).gameObject;
            if (handPoolObject.GetComponent<ObjectPosition>())
                if (handPoolObject.GetComponent<ObjectPosition>().levitatableObject)
                    handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("levitatableObject");
        }
        int objectCountRight = activeObjectPoolRightHand.transform.childCount;
        if (objectCountRight > 0)
        {
            // 왼손에 방망이를 잡고있음으로 오른쪽 손에 물체를 잡을 수 있음
            GameObject handPoolObject = activeObjectPoolRightHand.transform.GetChild(0).gameObject;
            if (handPoolObject.GetComponent<ObjectPosition>())
                if (handPoolObject.GetComponent<ObjectPosition>().levitatableObject)
                    handPoolObject.GetComponent<ObjectPosition>().ReturnToNomal("levitatableObject");
        }

    }
}
