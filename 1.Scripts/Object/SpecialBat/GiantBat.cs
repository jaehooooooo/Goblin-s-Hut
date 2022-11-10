using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantBat : MonoBehaviour
{
    private bool leftHand;              // 현재 방망이를 잡고있는 손이 왼손이면 true, 오른손이면 false
    public Transform activeObjectPool;  // 방망이와 상호작용시 들어갈 부모 오브젝트 풀
    public Vector3 handPos;             // 손 위치

    public Transform model;             // 방망이 바디
    public Renderer batMesh;            // 방망이 매쉬

    public ParticleSystem particle;     // 거대 해질때 플레이될 파티클

    public GameObject player;           // 높이를 제어할 플레이어 
    public GameObject[] hands;          // 손도 같이 커질 예정
    private float maxHandSize = 9f;          // 최대 손 크기
    private Vector3 originPos;          // 플레이어 원래 위치값

    private bool readyShoot;            // 거대화 가능 상태
    public float playerHight;           // 거대화시 플레이어 높이
    public float duration;              // 거대화 혹은 축소화시 걸릴 시간

    private Color whiteColor = new Color(1, 1, 1);
    private Color yellowColor = new Color(1f, 0.805f, 0.0f);


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
        if (!readyShoot)
        {
            // 왼손에 잡고있고 왼손의 두번째 버튼을 눌렀다면 && 바람 불 준비상태가 아니라면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }
        else
        {
            // 바람 불 준비상태인데 버튼에서 손을 뗏다면
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }
    }

    // 거대화 시작
    private void ReadyToShoot()
    {
        particle.Play();
        // bool값 변경
        readyShoot = true;
        // 플레이어 움직임 제한하기
        GameManager.instance.playerMove = false;
        // 코루틴으로 점차 변경되게 적용
        StartCoroutine(IGrowUp());
    }
    private IEnumerator IGrowUp()
    {
        float time = 0f;
        originPos = player.transform.position;
        while (true)
        {
            yield return null;

            time += Time.deltaTime;
            if (time / duration > 1f)
                break;
            // 플레이어 위치값 번경
            player.transform.position = originPos + new Vector3(0, playerHight * (time/duration), 0);
            // 방망이 색 변경
            float r = Mathf.Lerp(yellowColor.r, whiteColor.r, time / duration);
            float g = Mathf.Lerp(yellowColor.g, whiteColor.g, time / duration);
            float b = Mathf.Lerp(yellowColor.b, whiteColor.b, time / duration);
            Color color = new Color(r, g, b);
            batMesh.materials[0].color = color;
            // 손 크기 커지기
            foreach (GameObject hand in hands)
            {
                float size = 1 + maxHandSize * (time / duration);
                hand.transform.localScale = new Vector3(size, size, size);
            }

        }
        yield return null;
    }

    // 거대화 끝내기
    private void FinishToShoot()
    {
        // 코루틴으로 점차 변경되게 적용
        StartCoroutine(IShotter());
    }
    private IEnumerator IShotter()
    {
        particle.Play();
        float time = 0f;
        while (true)
        {
            yield return null;

            time += Time.deltaTime;
            if (time / duration > 1f)
                break;
            // 플레이어 위치값 번경
            player.transform.position = originPos + new Vector3(0,playerHight,0) - new Vector3(0, playerHight * (time / duration), 0);
            // 방망이 색 변경
            float r = Mathf.Lerp(whiteColor.r, yellowColor.r, time / duration);
            float g = Mathf.Lerp(whiteColor.g, yellowColor.g, time / duration);
            float b = Mathf.Lerp(whiteColor.b, yellowColor.b, time / duration);
            Color color = new Color(r, g, b);
            batMesh.materials[0].color = color;
            // 손 크기 작아지기
            foreach (GameObject hand in hands)
            {
                float size = 1 + maxHandSize - maxHandSize * (time / duration);
                hand.transform.localScale = new Vector3(size, size, size);
            }

        }
        yield return null;
        // bool값 변경
        readyShoot = false;
        // 플레이 움직임 제한 풀기
        GameManager.instance.playerMove = true;
    }

    // SpecialBatManager에서 SendMessage로 호출
    private void ReturnNomal()
    {
        // 거대화 중이었다면 강제로 원상태 복귀
        if(readyShoot)
        {
            foreach (GameObject hand in hands)
            {
                float size = 1;
                hand.transform.localScale = new Vector3(size, size, size);
            }
            // 플레이어 위치값 변경
            player.transform.position = originPos;
            // 방망이 색 변경
            batMesh.materials[0].color = yellowColor;
            // 플레이어 움직임 제한 풀기
            GameManager.instance.playerMove = true;
            // bool값 변경
            readyShoot = false;
        }
    }
}
