using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBat : MonoBehaviour
{

    private bool leftHand;              // 현재 방망이를 잡고있는 손이 왼손이면 true, 오른손이면 false
    public Transform activeObjectPool;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Vector3 handPos;             // 손 위치


    public Transform model;             // 방망이 바디
    public Renderer batMesh;            // 방망이 매쉬

    public ParticleSystem particle;     // 바람 발사할때 플레이될 파티클
    public Transform windSphere;        // 바람게이지 모을때 보일 형태

    private bool readyShoot;            // 바람 발사 가능 상태
    private bool makeWindGauge;         // 바람 게이지 모으는 상태

    private float sphereSize;           // 특수효과 sphere크기
    private float maxSphereSize =3f;        // 특수효과 sphere 최대크기

    private Color whiteColor = new Color(1, 1, 1);
    private Color shilverColor = new Color(0.35f, 0.35f, 0.35f);

    private void Awake()
    {
        // 해당 방망이가 갖고있는 매테리얼 중에서 두번째 메테리얼의 색상을 찾아옴
        batMesh = model.gameObject.GetComponent<Renderer>();
        particle.Stop();
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
        if(!readyShoot)
        {
            // 왼손에 잡고있고 왼손의 두번째 버튼을 눌렀다면 && 바람 불 준비상태가 아니라면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }
        if (readyShoot)
        {
            // 바람 불 준비상태인데 버튼에서 손을 뗏다면
            if (leftHand && ARAVRInput.GetUp(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetUp(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }

        if (makeWindGauge)
            MakeGauge();
    }

    // 공 칠 준비됨
    private void ReadyToShoot()
    {
        readyShoot = true;
        makeWindGauge = true;

        // sphere 사이즈 초기화
        sphereSize = 0f;
        time = 0f;
    }
    private void FinishToShoot()
    {
        windSphere.gameObject.SetActive(false);

        RaycastHit[] hits;
        hits = Physics.SphereCastAll(Camera.main.transform.position, sphereSize, Vector3.up, 0f);
        // htis들에서 날아갈 수 있는 물체 걸러내기
        if (hits.Length > 0)
        {
            for (int j = 0; j < hits.Length; j++)
            {
                // 감지된 물체가 날아갈 수 있는 물체라면
                if (hits[j].transform.gameObject.GetComponent<WindableObject>())
                {
                    hits[j].transform.parent = activeObjectPool.transform;
                    Vector3 forcedir = (hits[j].transform.position - Camera.main.transform.position).normalized;
                    hits[j].transform.gameObject.GetComponent<WindableObject>().FlyHigh(forcedir * sphereSize);
                }
            }
        }
        // 이펙트 활성화
        particle.transform.position = Camera.main.transform.position + new Vector3(0, -1, 0);
        particle.transform.localScale = new Vector3(sphereSize,sphereSize,sphereSize);
        particle.Play();

        // 방망이 색 원래 상태로 복귀
        batMesh.materials[0].color = shilverColor;
        //
        readyShoot = false;
        makeWindGauge = false;
    }
    private float time;
    public float size;
    private void MakeGauge()
    {
        // 시간에 따라 장풍 특수효과 영역 확대
        time += Time.deltaTime/2;
        sphereSize = time * size;
        sphereSize = Mathf.Clamp(sphereSize, 0f, maxSphereSize);

        //방망이 색상 변경
        float r = Mathf.Lerp(shilverColor.r, whiteColor.r, sphereSize / maxSphereSize);
        float g = Mathf.Lerp(shilverColor.g, whiteColor.g, sphereSize / maxSphereSize);
        float b = Mathf.Lerp(shilverColor.b, whiteColor.b, sphereSize / maxSphereSize);
        Color color = new Color(r, g, b);
        batMesh.materials[0].color = color;

        // 영역 표시 
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
        // 방망이와 닿는 물체가 바람에 날아갈 수 있는 물체인지 확인하고 날리기
        if (other.gameObject.GetComponent<WindableObject>())
        {
            Vector3 forceDir = (other.transform.position - transform.position).normalized;
            other.transform.parent = activeObjectPool.transform;
            other.gameObject.GetComponent<WindableObject>().FlyHigh(forceDir);
        }
    }

    // SpecialBatManager에서 SendMessage로 호출
    private void ReturnNomal()
    {
        // acticeObjectPool에 있는 모든 오브젝트 원래 상태로 복귀
        int objectCount = activeObjectPool.transform.childCount;

        GameObject[] handPoolObjects = new GameObject[objectCount];
        for (int i =0; i< objectCount; i++)
        {
            handPoolObjects[i] = activeObjectPool.transform.GetChild(i).gameObject;
        }

        foreach(GameObject target in handPoolObjects)
        {
            // 상호작용한 오브젝트들에게 원래 위치값과 설정값으로 돌아가라고 전달
            if (target.GetComponent<ObjectPosition>())
                target.GetComponent<ObjectPosition>().ReturnToNomal("windableObject");
        }

    }

}
