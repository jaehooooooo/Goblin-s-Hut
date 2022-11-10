using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflammableObject : MonoBehaviour
{
    private ObjectPosition objectPosition;
    private Rigidbody rig;
    private BoxCollider boxCollider;
    public GameObject activeObjectPool;  // ����̿� ��ȣ�ۿ�� �� �θ� ������Ʈ Ǯ // �ӽ÷� �۵� (���߿� ��ĥ���� ��ü �޼����� �Ҳ���� �˷���� �ҵ�?)

    public GameObject model;                    // ���� ����
    public GameObject ash;                      // ����� ����
    public ParticleSystem[] burnningParitcles;  // ��Ÿ�� ��ƼŬ
    public bool burnEnough = false;             // ���� ���� ����
    private bool isBurning = false;             // ��Ÿ�� ������ üũ
    public float burnEnoughTime;                // ���� ���ҵ� �ð� (��ü���� �ٸ� �ӵ�)
    public float catchFireTime;                 // ���� ������ �Űܺٱ� ������ �ð�
    public Vector3 boxcastSize;                 // �ڽ�ĳ��Ʈ ũ��
    public Vector3 nomalColliderSize = new Vector3(0.1f,0.1f,0.1f);             // ���� ���� �� �ݶ��̴� ũ��
    public Vector3 ashColliderSize = new Vector3(0.1f,0.03f,0.1f);             // ���� ���� �� �ݶ��̴� ũ��

    // �� �ֺ� ������Ʈ üũ �� ��Ż �� �ִ� ��ü�� �� �ִٸ� �� ��ü�� ��Ÿ�� �����
    // ��Ÿ�� �ൿ�� ����

    private void Awake()
    {
        objectPosition = GetComponent<ObjectPosition>();
        rig = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        foreach (ParticleSystem particle in burnningParitcles) 
        {
            particle.Stop();
        }
    }
    public void Burn()
    {
        // ���� ��Ÿ�� ���� �ʰ� && Ȳ�� ���°� �ƴҶ� (Ȳ���� ���� �Ⱥ���)
        if(!objectPosition.inflammableObject)
        {
            if(objectPosition.changeGoldObject)
            {
                if (GetComponent<ChangeGoldObject>().isChange)
                    return;
            }
            // ��Ÿ�� ��ȣ�ۿ� Ȱ��ȭ �˸�
            objectPosition.inflammableObject = true;

            isBurning = true;
            // ��Ÿ�� ����Ʈ Ȱ��ȭ
            foreach (ParticleSystem particle in burnningParitcles)
            {
                particle.Play();
            }
            // �� ������Ʈ�� �� �Űܺ���
            StartCoroutine(IFineInflammableObject());

            // ���� ���ҵǴ� ��ü�� ��� ���� ����
            Invoke("BurnEnough", burnEnoughTime);
        }
    }

    IEnumerator IFineInflammableObject()
    {
        // �� ������Ʈ�� ���� �Űܺ��� �ð�
        yield return new WaitForSeconds(catchFireTime);

        // ���α��� RaycastBox�� ���� �ֺ��� �� ��Ż �� �ִ� ��ü �ִ��� ����
        Collider[] hitObjects = Physics.OverlapBox(transform.position, boxcastSize);

        if (hitObjects.Length > 0)
        {
            foreach (Collider collider in hitObjects)
            {
                // ���࿡ ���� ��ü�� ��Ż �� �ִ� ��ü���
                if (collider.gameObject.GetComponent<InflammableObject>())
                {
                    collider.transform.parent = activeObjectPool.transform;
                    collider.gameObject.GetComponent<InflammableObject>().Burn();
                }
            }
        }
    }

    // ���� ���ҵ� �����϶�
    private void BurnEnough()
    {
        
        // ������ٵ� ��Ȱ��ȭ
        rig.isKinematic = true;
        // ���� ���� ��Ȱ��ȭ
        model.SetActive(false);
        // ���� ������ ����
        transform.localEulerAngles = new Vector3(0, 0, 0);
        // �ڽ��ݶ��̴� ������ ����
        boxCollider.size = ashColliderSize;
        // �����ִ� �� ���� Ȱ��ȭ
        ash.SetActive(true);
        // ������ٵ� Ȱ��ȭ
        rig.isKinematic = false;

        // ��Ÿ�� ����Ʈ ��Ȱ��ȭ
        foreach (ParticleSystem particle in burnningParitcles)
        {
            particle.Stop();
        }
    }

    public void ReturnNomal()
    {
        StopAllCoroutines();
        CancelInvoke();
        // �ڽ��ݶ��̴� ������ ����
        boxCollider.size = nomalColliderSize;
        // ������ٵ� Ȱ��ȭ
        rig.isKinematic = false;
        // ���� ���� ��Ȱ��ȭ
        model.SetActive(true);
        // �����ִ� �� ���� Ȱ��ȭ
        ash.SetActive(false);
        // ��Ÿ�� ����Ʈ ��Ȱ��ȭ
        foreach (ParticleSystem particle in burnningParitcles)
        {
            particle.Stop();
        }
    }
}
