using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponColliderManager : MonoBehaviour
{
    // PlayerGrabObject ��ũ��Ʈ���� ���� �ӵ� �̻����� ���⸦ �ֵθ��� ���� ��ũ��Ʈ

    // isTrigger�� Ȱ���ؼ� ���� ��ü�� üũ
    // ������ ��ü���� �ش��ϴ� ��ȣ�ۿ��� �����϶�� �ڵ带 ����

    // ����� ���� ��ü�� ������ Colliders
    public Collider[] colliders;
    private Vector3 prePos;
    private float velocityValue = 2f;

    private void Update()
    {
        // �տ� ���� ���°� �ƴϸ� return
        if (!transform.parent.name.Contains("Hand"))
            return;
        else
        {
            // �տ� ���� ���¶�� �����Ӵ� �ӵ� üũ
            float velocity = Vector3.Distance(transform.position, prePos) / Time.deltaTime;
            prePos = transform.position;

            // ���� �ӵ��� 1 �̻��϶� -> �ݶ��̴��� ��� ��ü�� �ִٸ�(isTrigger) �߸��� ������ ���� �϶�� �˷��ֱ�
            if (velocity > velocityValue)
            {
                // �ݶ��̴��� ��� ��ü�� �ִ��� �ľ�
                DetectOtherCollider(velocity);
            }
            else if (velocity <= velocityValue)
            {
                DisDetectOtherCollider();
            }
        }
    }

    // �ε��� �ݶ��̴� ����
    public void DetectOtherCollider(float velocity)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
            collider.gameObject.GetComponent<WeaponColider>().velocity = velocity;
        }
    }
    // �ε��� �ݶ��̴� ��Ȱ��ȭ
    public void DisDetectOtherCollider()
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
