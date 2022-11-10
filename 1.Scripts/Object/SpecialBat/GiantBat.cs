using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantBat : MonoBehaviour
{
    private bool leftHand;              // ���� ����̸� ����ִ� ���� �޼��̸� true, �������̸� false
    public Transform activeObjectPool;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ
    public Vector3 handPos;             // �� ��ġ

    public Transform model;             // ����� �ٵ�
    public Renderer batMesh;            // ����� �Ž�

    public ParticleSystem particle;     // �Ŵ� ������ �÷��̵� ��ƼŬ

    public GameObject player;           // ���̸� ������ �÷��̾� 
    public GameObject[] hands;          // �յ� ���� Ŀ�� ����
    private float maxHandSize = 9f;          // �ִ� �� ũ��
    private Vector3 originPos;          // �÷��̾� ���� ��ġ��

    private bool readyShoot;            // �Ŵ�ȭ ���� ����
    public float playerHight;           // �Ŵ�ȭ�� �÷��̾� ����
    public float duration;              // �Ŵ�ȭ Ȥ�� ���ȭ�� �ɸ� �ð�

    private Color whiteColor = new Color(1, 1, 1);
    private Color yellowColor = new Color(1f, 0.805f, 0.0f);


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
        if (!readyShoot)
        {
            // �޼տ� ����ְ� �޼��� �ι�° ��ư�� �����ٸ� && �ٶ� �� �غ���°� �ƴ϶��
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                ReadyToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                ReadyToShoot();
        }
        else
        {
            // �ٶ� �� �غ�����ε� ��ư���� ���� �´ٸ�
            if (leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
                FinishToShoot();
            else if (!leftHand && ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
                FinishToShoot();
        }
    }

    // �Ŵ�ȭ ����
    private void ReadyToShoot()
    {
        particle.Play();
        // bool�� ����
        readyShoot = true;
        // �÷��̾� ������ �����ϱ�
        GameManager.instance.playerMove = false;
        // �ڷ�ƾ���� ���� ����ǰ� ����
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
            // �÷��̾� ��ġ�� ����
            player.transform.position = originPos + new Vector3(0, playerHight * (time/duration), 0);
            // ����� �� ����
            float r = Mathf.Lerp(yellowColor.r, whiteColor.r, time / duration);
            float g = Mathf.Lerp(yellowColor.g, whiteColor.g, time / duration);
            float b = Mathf.Lerp(yellowColor.b, whiteColor.b, time / duration);
            Color color = new Color(r, g, b);
            batMesh.materials[0].color = color;
            // �� ũ�� Ŀ����
            foreach (GameObject hand in hands)
            {
                float size = 1 + maxHandSize * (time / duration);
                hand.transform.localScale = new Vector3(size, size, size);
            }

        }
        yield return null;
    }

    // �Ŵ�ȭ ������
    private void FinishToShoot()
    {
        // �ڷ�ƾ���� ���� ����ǰ� ����
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
            // �÷��̾� ��ġ�� ����
            player.transform.position = originPos + new Vector3(0,playerHight,0) - new Vector3(0, playerHight * (time / duration), 0);
            // ����� �� ����
            float r = Mathf.Lerp(whiteColor.r, yellowColor.r, time / duration);
            float g = Mathf.Lerp(whiteColor.g, yellowColor.g, time / duration);
            float b = Mathf.Lerp(whiteColor.b, yellowColor.b, time / duration);
            Color color = new Color(r, g, b);
            batMesh.materials[0].color = color;
            // �� ũ�� �۾�����
            foreach (GameObject hand in hands)
            {
                float size = 1 + maxHandSize - maxHandSize * (time / duration);
                hand.transform.localScale = new Vector3(size, size, size);
            }

        }
        yield return null;
        // bool�� ����
        readyShoot = false;
        // �÷��� ������ ���� Ǯ��
        GameManager.instance.playerMove = true;
    }

    // SpecialBatManager���� SendMessage�� ȣ��
    private void ReturnNomal()
    {
        // �Ŵ�ȭ ���̾��ٸ� ������ ������ ����
        if(readyShoot)
        {
            foreach (GameObject hand in hands)
            {
                float size = 1;
                hand.transform.localScale = new Vector3(size, size, size);
            }
            // �÷��̾� ��ġ�� ����
            player.transform.position = originPos;
            // ����� �� ����
            batMesh.materials[0].color = yellowColor;
            // �÷��̾� ������ ���� Ǯ��
            GameManager.instance.playerMove = true;
            // bool�� ����
            readyShoot = false;
        }
    }
}
