using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGoldObject : MonoBehaviour
{
    private ObjectPosition objectPosition;
    // 
    public GameObject model;
    // Ȳ������ ���Ҷ� ���� ��ƼŬ
    public ParticleSystem particle;
    // Ȳ�� 
    public GameObject gold;
    public float goldWeight;
    public float orginWeight;
    // ���� ��ȭ üũ
    public bool isChange = false;
    // ���� ��ȭ �����ֱ�
    private float changeDuration = 3f;
    private bool isDuration;
    

    private void Awake()
    {
        objectPosition = GetComponent<ObjectPosition>();
        particle.Stop();
    }

    public void ChangeGold()
    {
        // �ݱ� ��ȣ�ۿ� Ȱ��ȭ �˸�
        objectPosition.changeGoldObject = true;

        // ���� �پ��ִ� ���¶�� �Ҳ��� ���� ���·� �����϶�� ����
        if (objectPosition.inflammableObject)
            objectPosition.ReturnToNomal("inflammableObject");

        // ��ü�� ���� ����
        orginWeight = objectPosition.objectWeight;
        objectPosition.objectWeight = goldWeight;

        particle.Play();
        isChange = !isChange;
        // ���� ������Ʈ ��Ȱ��ȭ
        model.SetActive(!isChange);
        // Ȳ�� ������Ʈ Ȱ��ȭ
        gold.SetActive(isChange);
        isDuration = true;
        StartCoroutine(IChangeDuration());
    }
    IEnumerator IChangeDuration()
    {
        // ������ �Ⱓ �Ŀ� �ٽ� �ٲ� �� ����
        yield return changeDuration;
        isDuration = false;
    }

    public void ReturnNomal()
    {
        // ��ü�� ���� ����
        objectPosition.objectWeight = orginWeight;

        // ��ƼŬ ���߱�
        particle.Play();
        isDuration = false;
        isChange = false;
        // ��� ��Ȱ��ȭ
        gold.SetActive(false);
        // ��� ��ġ �ʱ�ȭ
        gold.transform.position = new Vector3(0, 0, 0);
        // ���� ���� Ȱ��ȭ
        model.SetActive(true);
    }

}
