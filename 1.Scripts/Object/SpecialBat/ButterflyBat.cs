using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyBat : MonoBehaviour
{
    public GameObject activeObjectPool;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Transform model;             // 방망이 형태
    public Renderer batMesh;            // 방망이 매쉬
    public Transform[] pointParticle;   // 나비 방망이 특수효과에 사용할 파티클들

    public Vector3 handPos;             // 손 위치 저장
    public Transform batHitPos;         // 특수효과 발현 위치
    private bool leftHand;              // 현재 방망이를 잡고있는 손이 왼손이면 true, 오른손이면 false
    public bool readyShoot;            // 특수효과 실행 가능 여부
    private float minDistance = 0.1f;   // 특수효과 간 최소 거리

    private Color whiteColor = new Color(1, 1, 1);
    private Color pinkColor = new Color(1, 0.49f, 0.78f);

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
        if (readyShoot)
        {
            // 나비 이펙트 준비상태인데 버튼을 한 번 더 눌렀다면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }
        else
        {
            // 왼손에 잡고있고 왼손의 두번째 버튼을 눌렀다면 && 나비 이펙트 준비상태가 아니라면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }

    }

    // 특수 효과 준비됨
    private void ReadyToShoot()
    {        
        // 방망이 색 흰색으로 설정
        batMesh.materials[0].color = whiteColor;
        // 나비 이펙트를 발사할 준비
        readyShoot = true;

        // 이펙트 그리기 시작
        StartCoroutine(IDrawButterfly());
    }
    private void FinishToShoot()
    {
        // 방망이 색 원래 상태로 복귀
        batMesh.materials[0].color = pinkColor;
        //
        readyShoot = false;
        // 코루틴이 재생중이라면 코루틴 멈추기
        StopAllCoroutines();
        foreach (Transform particle in pointParticle)
        {
            particle.gameObject.SetActive(false);
        }

    }

    // 나비 이펙트 발현 상태가 되면 5초 동안 궤적에 이펙트가 생김
    IEnumerator IDrawButterfly()
    {
        yield return null;
        int num = 0;
        float length = pointParticle.Length;

        while (true)
        {
            yield return null;
            //현재 위치와 이전 위치사이의 간격이 일정수치 이상이면 새로운 위치값 저장
             //현재 위치값 저장
            Vector3 pos = batHitPos.transform.position + new Vector3(0,0.5f,0);

            if (num == 0)
            {
                //이펙트 활성화하고 이펙트의 위치에 현재 위치값 저장
                pointParticle[num].transform.position = pos;
                pointParticle[num].gameObject.SetActive(true);

                //방망이 색상 변경
                float r = Mathf.Lerp(whiteColor.r, pinkColor.r, num / length);
                float g = Mathf.Lerp(whiteColor.g, pinkColor.g, num / length);
                float b = Mathf.Lerp(whiteColor.b, pinkColor.b, num / length);
                Color color = new Color(r, g, b);
                batMesh.materials[0].color = color;
                num++;
            }
            else if (num > 0 && Vector3.Distance(pos, pointParticle[num - 1].transform.position) > minDistance)
            {
                //이펙트 활성화하고 이펙트의 위치에 현재 위치값 저장
                pointParticle[num].transform.position = pos;
                pointParticle[num].gameObject.SetActive(true);

                //방망이 색상 변경
                float r = Mathf.Lerp(pinkColor.r, whiteColor.r, (float)(num / length));
                float g = Mathf.Lerp(whiteColor.g, pinkColor.g, (float)(num / length));
                float b = Mathf.Lerp(whiteColor.b, pinkColor.b, (float)(num / length));
                Color color = new Color(r, g, b);
                batMesh.materials[0].color = color;
                num++;
            }
            else
            {
                print("거리가 너무 가까움");
            }

            //10개의 파티클을 다 썼다면 FinishToShoot()
            if (num / length == 1)
            {
                yield return new WaitForSeconds(7f);
                FinishToShoot();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 방망이와 닿는 물체가 꽃잎이 될 수 있는 물체인지 확인하고 꽃잎으로 변경
        if (other.gameObject.GetComponent<ChangeButterflyObject>())
        {
            other.transform.parent = activeObjectPool.transform;
            other.gameObject.GetComponent<ChangeButterflyObject>().ChangeButterfly();
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
                target.GetComponent<ObjectPosition>().ReturnToNomal("changeButterflyObject");
        }

    }
}
