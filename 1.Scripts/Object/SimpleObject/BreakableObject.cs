using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    // �����ų� �μ��� �� �ִ� ������Ʈ�� �����ִ� ��ũ��Ʈ

    // ������Ʈ Ǯ�� Ȱ���Ͽ� ������ �����ִ� ������Ʈ�� ���ٰ� Ű�� ������� �۵�
    enum objectState
    {
        standard,nomal,sliceWidth,sliceLength,crush
    }

    // �μ����ų� ���� �� �ִ��� ����
    public bool sliceable;
    public bool crushable;

    public GameObject[] objectStates;
    bool isBroken = false;
    public int waitForsec;

    public Vector3 boxSize;    // �߸� ���� Ȯ�ο� �ڽ�������
    public float maxDistance;   // ���� ���� �Ÿ�
    RaycastHit hit;

    float velocity;             // �μ����ų� ���� ������ ����� �ӵ�

    public void Broken(bool isSharp, float weaponVelocity)
    {
        if (isBroken)
            return;

        velocity = weaponVelocity;
        if (isSharp && sliceable)   // ��ī�ο� ���⿡ �¾Ұ� && �߸� �� �ִ� ��ü�϶�
        {
            StartCoroutine(IFindDirectionSlice());
        }
        else if(!isSharp && crushable ) // ������ ���⿡ �¾Ұ� && �μ��� �� �ִ� ��ü�϶�
        {        
            // ���̻� �μ����� �ʵ��� bool�� ����
            isBroken = true;
            //gameObject.GetComponent<BoxCollider>().enabled = false;
            var findCollider = gameObject.GetComponent<Collider>().GetType();

            if (findCollider.Equals(typeof(BoxCollider)))
                gameObject.GetComponent<BoxCollider>().enabled = false;
            else if (findCollider.Equals(typeof(CapsuleCollider)))
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
            else
                gameObject.GetComponent<MeshCollider>().enabled = false;


            // ��ü�� �ε��� ���Ⱑ �����ϴٸ� -> ��ü �μ���
            StartCoroutine(IBrokenObject());
        }
    }

    IEnumerator IFindDirectionSlice()
    {
        // �ݶ��̴��� ������ٵ� ��Ȱ��ȭ
        GetComponent<Rigidbody>().isKinematic = true;
        var findCollider = gameObject.GetComponent<Collider>().GetType();

        if (findCollider.Equals(typeof(BoxCollider)))
            gameObject.GetComponent<BoxCollider>().enabled = false;
        else if (findCollider.Equals(typeof(CapsuleCollider)))
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        else
            gameObject.GetComponent<MeshCollider>().enabled = false;

        //While�� Vector3
        while (true)
        {
            // �߸� ������ �������� �������� �Ǵ�
            if (Physics.BoxCast(transform.position, boxSize / 2, Vector3.up, out hit, Quaternion.identity, maxDistance)
                || Physics.BoxCast(transform.position, boxSize / 2, Vector3.down, out hit, Quaternion.identity, maxDistance))
            {
                // ���� �������� �߸�
                print("���ι���");
                print("����" + (transform.position - hit.point));
                StartCoroutine(ISliceLengthObject());
                break;
            }
            else if (Physics.BoxCast(transform.position, boxSize / 2, Vector3.forward, out hit, Quaternion.identity, maxDistance)
                || Physics.BoxCast(transform.position, boxSize / 2, Vector3.back, out hit, Quaternion.identity, maxDistance)
                || Physics.BoxCast(transform.position, boxSize / 2, Vector3.left, out hit, Quaternion.identity, maxDistance)
                || Physics.BoxCast(transform.position, boxSize / 2, Vector3.right, out hit, Quaternion.identity, maxDistance))
            {
                // ���� �������� �߸�
                print("���ι���");
                print("���" + (transform.position - hit.point));

                StartCoroutine(ISliceWidthObject());
                break;
            }
            yield return null;
        }
        #region �����غ� ��� ���� �߿��� ������ Į�̶� ���ڶ� ������� �����ϴ� �ڽ� �ݶ��̴��� ������ ���⼺�� �߸���
        // While �� TransformDirection()
        //while (true)
        //{
        //    // �߸� ������ �������� �������� �Ǵ�
        //    if (Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.up), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.down), out hit, Quaternion.identity, maxDistance))
        //    {
        //        // ���� �������� �߸�
        //        print("���ι���");

        //        StartCoroutine(IsliceLengthObject());
        //        break;
        //    }
        //    else if (Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.forward), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2,transform.TransformDirection( Vector3.back), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.left), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.right), out hit, Quaternion.identity, maxDistance))
        //    {
        //        // ���� �������� �߸�
        //        print("���ι���");
        //        StartCoroutine(IsliceWidthObject());
        //        break;
        //    }
        //    yield return null;
        //}
        //}
        //while (true)
        //{
        //    // �߸� ������ �������� �������� �Ǵ�
        //    if (Physics.BoxCast(transform.position, boxSize / 2, Vector3.up, out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, Vector3.down, out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, Vector3.forward, out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2,Vector3.back, out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, Vector3.left, out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, Vector3.right, out hit, Quaternion.identity, maxDistance))
        //    {
        //        Vector3 dir = transform.position - hit.point;
        //        if(dir.y >=0.05f)
        //        {
        //            // ���� �������� �߸�
        //            print("���ι���");
        //            print("����" + (transform.position - hit.point));
        //            StartCoroutine(IsliceLengthObject());
        //            break;

        //        }
        //        else
        //        {
        //            // ���� �������� �߸�
        //            print("���ι���");
        //            print("���" + (transform.position - hit.point));

        //            StartCoroutine(IsliceWidthObject());
        //            break;
        //        }
        //    }
        //    yield return null;

        //}
        //while (true)
        //{
        //    // �߸� ������ �������� �������� �Ǵ�
        //    if (Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.up), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.down), out hit, Quaternion.identity, maxDistance)
        //        ||(Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.forward), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.back), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.left), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.right), out hit, Quaternion.identity, maxDistance)))
        //    {
        //        Vector3 dir = transform.position - hit.point;
        //        if (dir.y >= 0.05f)
        //        {
        //            // ���� �������� �߸�
        //            print("���ι���");
        //            print("����" + (transform.position - hit.point));
        //            StartCoroutine(IsliceLengthObject());
        //            break;

        //        }
        //        else
        //        {
        //            // ���� �������� �߸�
        //            print("���ι���");
        //            print("���" + (transform.position - hit.point));

        //            StartCoroutine(IsliceWidthObject());
        //            break;
        //        }
        //    }
        //    yield return null;
        //}
        #endregion
        // ���̻� �μ����� �ʵ��� bool�� ����
        isBroken = true;
    }
    
    IEnumerator ISliceWidthObject()
    {
        GetComponent<Rigidbody>().isKinematic = false;

        // ������Ʈ Ǯ���� �����̽� ������Ʈ Ȱ��ȭ �� �⺻ ��� ������Ʈ ��Ȱ��ȭ
        objectStates[(int)objectState.nomal].SetActive(false);
        objectStates[(int)objectState.sliceWidth].SetActive(true);
        // ������Ʈ Ǯ�� ����ִ� ���� Ƣ��
        BrokenStart(objectStates[(int)objectState.sliceWidth]);

        // 5�� ���� ��ٸ���
        yield return new WaitForSeconds(waitForsec);
        // ������Ʈ Ǯ���� �����̽� ������Ʈ ��Ȱ��ȭ �� �⺻ ��� ������Ʈ Ȱ��ȭ
        GetComponent<Rigidbody>().isKinematic = true;

        BrokenEnd(objectStates[(int)objectState.sliceWidth]);
        objectStates[(int)objectState.sliceWidth].SetActive(false);
        yield return null;
        objectStates[(int)objectState.nomal].SetActive(true);

        var findCollider = gameObject.GetComponent<Collider>().GetType();

        if (findCollider.Equals(typeof(BoxCollider)))
            gameObject.GetComponent<BoxCollider>().enabled = true;
        else if (findCollider.Equals(typeof(CapsuleCollider)))
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        else
            gameObject.GetComponent<MeshCollider>().enabled = true;

        // ��ġ�� ���� ��ġ�� ����
        gameObject.transform.localPosition = gameObject.GetComponent<ObjectPosition>().originPos;
        GetComponent<Rigidbody>().isKinematic = false;

        // bool�� ����
        isBroken = false;
    }
    IEnumerator ISliceLengthObject()
    {
        GetComponent<Rigidbody>().isKinematic = false;

        // ������Ʈ Ǯ���� �����̽� ������Ʈ Ȱ��ȭ �� �⺻ ��� ������Ʈ ��Ȱ��ȭ
        objectStates[(int)objectState.nomal].SetActive(false);
        objectStates[(int)objectState.sliceLength].SetActive(true);
        // ������Ʈ Ǯ�� ����ִ� ���� Ƣ��
        BrokenStart(objectStates[(int)objectState.sliceLength]);

        // 5�� ���� ��ٸ���
        yield return new WaitForSeconds(waitForsec);
        // ������Ʈ Ǯ���� �����̽� ������Ʈ ��Ȱ��ȭ �� �⺻ ��� ������Ʈ Ȱ��ȭ
        BrokenEnd(objectStates[(int)objectState.sliceLength]);
        objectStates[(int)objectState.sliceLength].SetActive(false);
        objectStates[(int)objectState.nomal].SetActive(true);

        var findCollider = gameObject.GetComponent<Collider>().GetType();

        if (findCollider.Equals(typeof(BoxCollider)))
            gameObject.GetComponent<BoxCollider>().enabled = true;
        else if (findCollider.Equals(typeof(CapsuleCollider)))
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        else
            gameObject.GetComponent<MeshCollider>().enabled = true;

        // ��ġ�� ���� ��ġ�� ����
        gameObject.transform.localPosition = gameObject.GetComponent<ObjectPosition>().originPos;
        // �ݶ��̴��� ������ٵ� ��Ȱ��ȭ
        GetComponent<Rigidbody>().isKinematic = false;

        // bool�� ����
        isBroken = false;
    }
    IEnumerator IBrokenObject()
    {
        // ������Ʈ Ǯ���� �����̽� ������Ʈ Ȱ��ȭ �� �⺻ ��� ������Ʈ ��Ȱ��ȭ
        objectStates[(int)objectState.nomal].SetActive(false);
        objectStates[(int)objectState.crush].SetActive(true);
        // ������Ʈ Ǯ�� ����ִ� ���� Ƣ��
        BrokenStart(objectStates[(int)objectState.crush]);

        // 5�� ���� ��ٸ���
        yield return new WaitForSeconds(waitForsec);
        // ������Ʈ Ǯ���� �����̽� ������Ʈ ��Ȱ��ȭ �� �⺻ ��� ������Ʈ Ȱ��ȭ
        BrokenEnd(objectStates[(int)objectState.crush]);
        objectStates[(int)objectState.crush].SetActive(false);
        objectStates[(int)objectState.nomal].SetActive(true);

        // �⺻ ���� ������ٵ� & �ݶ��̴� Ȱ��ȭ
        //gameObject.GetComponent<BoxCollider>().enabled = true;
        var findCollider = gameObject.GetComponent<Collider>().GetType();

        if (findCollider.Equals(typeof(BoxCollider)))
            gameObject.GetComponent<BoxCollider>().enabled = true;
        else if (findCollider.Equals(typeof(CapsuleCollider)))
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        else
            gameObject.GetComponent<MeshCollider>().enabled = true;
        // ��ġ�� ���� ��ġ�� ����
        gameObject.transform.localPosition = gameObject.GetComponent<ObjectPosition>().originPos;
        // �ݶ��̴��� ������ٵ� ��Ȱ��ȭ
        GetComponent<Rigidbody>().isKinematic = false;

        // bool�� ����
        isBroken = false;

    }
    private void BrokenStart(GameObject target)
    {
        // �ش� ������Ʈ Ǯ�� ����ִ� ��ü ����
        int length = target.transform.childCount;
        GameObject[] objectsPool = new GameObject[length];
        for(int i = 0; i <length; i++)
        {
            Transform brokenObject = target.transform.GetChild(i);
            // ������ ���� ����
            Vector3 direction = Random.insideUnitSphere;
            // ���ư��� 
            Rigidbody rb = brokenObject.GetComponent<Rigidbody>();
            rb.velocity = direction * velocity;
        }
    }

    private void BrokenEnd(GameObject target)
    {
        // �ش� ������Ʈ Ǯ�� ����ִ� ��ü ����
        int length = target.transform.childCount;
        GameObject[] objectsPool = new GameObject[length];
        for (int i = 0; i < length; i++)
        {
            Transform brokenObject = target.transform.GetChild(i);
            brokenObject.transform.localPosition = Vector3.zero;
        }
    }
}
