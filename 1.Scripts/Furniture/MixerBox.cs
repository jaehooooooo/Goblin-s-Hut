using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MixerBox : MonoBehaviour
{
    // ������ ���� ��ũ��Ʈ 
    // Overlap Box�� ����

    // ����
    private enum MixedItem
    {
        LightBrownBat = 1, DarkBrownBat, RedStone, BlueStone, HeartPiece, SkullPiece
    }
    private MixedItem nowMixedItem;

    public int[] mixerArray = { 0, 0, 0 };
    int arrayNumber;

    public LayerMask mixedItemLayer;    // ������ ������ ������ ���̾�

    public Transform mixedObjectPool;
    public WeaponStandManager weaponStandManager;


    public Vector3 overlapBoxPosition;
    public Vector3 overlapBoxSize;
    public Vector3 mixedBoxCenterPosision;

    GameObject selletedMixedItem;       // ���մ뿡 ���� ���� ������

    public float duration;              // ���մ� �߾����� �̵��� �ð�
    public float dropSpeed;
    // ���� �ӵ��� �ƴ϶� ������������ ȿ���� ���� �ִϸ��̼� Ŀ�� ����
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public float resetDurarion;         // ����� ������ ������ ���ο� ����� ���ձ��� �ɸ� �ð�

    public GameObject[] inputParticle;     // ���վ������� ���� ���� ��ƼŬ
    public ParticleSystem[] inputParticles; // ���� �������� �� �� ���� ��ƼŬ
    public ParticleSystem[] makeParticles;  // ����̰� �ϼ��Ǿ����� ���� ��ƼŬ

    // �ʱ�ȭ
    private void Start()
    {
        foreach (ParticleSystem particle in makeParticles)
        {
            particle.Stop();
        }
    }
    private void Update()
    {
        SelletTouchedItem();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(overlapBoxPosition, overlapBoxSize);

    }
    // ���

    // ������ �ڽ��� ���˵� ��� �ݶ��̴��� ���� // �ͼ��ڽ��� ���� �� �ִ� ������ �ƴ��� // �̹� ����ִ� ������ �ƴ��� 
    private void SelletTouchedItem()
    {
        Collider[] touchedColliders = Physics.OverlapBox(overlapBoxPosition, overlapBoxSize/2,Quaternion.identity,mixedItemLayer);

        // �ݶ��̴��� ����� ��ü�� �ִ� ���
        if (touchedColliders.Length > 0)
        {
            // ��� �ݶ��̴� ��
            foreach (Collider touchedCollider in touchedColliders)
            {
                string tag = touchedCollider.tag;

                // ���տ� ��� �ִ� ������ ������Ʈ���
                if (touchedCollider.GetComponent<ObjectPosition>().isGrabbed)
                {
                    return;
                }
                //else if (touchedCollider.GetComponent<ObjectPosition>())
                //{
                //    if (!touchedCollider.GetComponent<ObjectPosition>().CheckObjectSituation())
                //        return;
                //}
                // ������ ������ �������� �±׶��
                else if (tag == "Mixable")
                {
                    selletedMixedItem = touchedCollider.gameObject;
                    // ���ϴ� ���� ���������� �Ǵ��ϴ� �Լ��� �̵�
                    SelletedMixedItem(selletedMixedItem);
                }
                else if (tag == "Grabbable")
                {
                    // �̰Ŵ� �����Ҽ� ���ٰ� �˷��ְ�
                    print("�̰Ŵ� ������ �Ұ��� �������̾�");
                    // �� ��ġ�� �����ϴ� �Լ��� �̵�
                    ReturnObjectPosition(touchedCollider.gameObject);
                }
            }
        }
    }
    
    // �ͼ��ڽ��� ���� �� �ְ� ���� �迭 �� �����ִ� ���������� �Ǵ�
    private void SelletedMixedItem( GameObject selletedMixedItem)
    {
        
        // ������ �������� �� �迭 ã��
        if(selletedMixedItem.name.Contains("Bat"))
        {
            arrayNumber = 0;
        }
        else if (selletedMixedItem.name.Contains("Stone"))
        {
            arrayNumber = 1;
        }
        else if (selletedMixedItem.name.Contains("Piece"))
        {
            arrayNumber = 2;
        }

        // �迭�� ���� ���¿� ��
        if(mixerArray[arrayNumber] == 0)    // �ش� �迭�� ����ִٸ�
        {
            // ���� enum�� ������Ʈ �̸��� �ش��ϴ� �� �Ҵ�
            nowMixedItem = (MixedItem)System.Enum.Parse(typeof(MixedItem), selletedMixedItem.name);
            // �迭�� �� �� �Ҵ�
            mixerArray[arrayNumber] = (int)nowMixedItem;
            // �ش� ������Ʈ ���̻� ���� ���ϰ� �ݶ��̴� ��Ȱ��ȭ
            var findCollider = selletedMixedItem.GetComponent<Collider>().GetType();

            if (findCollider.Equals(typeof(BoxCollider)))
                selletedMixedItem.GetComponent<BoxCollider>().enabled = false;
            else if (findCollider.Equals(typeof(CapsuleCollider)))
                selletedMixedItem.GetComponent<CapsuleCollider>().enabled = false;
            else
                selletedMixedItem.GetComponent<MeshCollider>().enabled = false;

            // �ش� ������Ʈ�� �������� �ִ� ��� ����
            StartCoroutine(MoveSelectedItem(selletedMixedItem,arrayNumber));

            // �迭�� 3���� �� á�ٸ� �迭 �� Ȯ���ϴ� �Լ��� �̵�
            if (mixerArray[0] != 0 && mixerArray[1] != 0 && mixerArray[2] != 0)
            {
                CompareArray();
            }
        }
        else // �ش� �迭�� ������� �ʴٸ�
        {
            // �ش� ������ �������� �̹� ����ִٰ� �޼��� ����
            print("�̹� �� ������ ������ ����־�");
            // �ش� ������Ʈ�� ���� ��ġ�� �ǵ����� �Լ� ����
            ReturnObjectPosition(selletedMixedItem);
        }
    }

    IEnumerator MoveSelectedItem( GameObject selectedItem, int arrayNumber)
    {
        // �ش� �������� �ӵ� �ʱ�ȭ
        selletedMixedItem.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        // �ش� �������� ������ٵ� �ʱ�ȭ
        selletedMixedItem.GetComponent<Rigidbody>().isKinematic = true;
        // �ͼ��ڽ����� ���� ���� �ð�
        inputParticle[arrayNumber].SetActive(true);
        inputParticle[arrayNumber].transform.position = mixedBoxCenterPosision;

        // �ش� ������ ��ġ�� ���ϴ� ��ġ�� ����
        var beginTime = Time.time;
        Vector3 originPos = selectedItem.transform.position;
        while (true)
        {
            var t = (Time.time - beginTime) / duration;

            if (t >= 1f) // ������ ��� �ð��� ������ ��
                break;
            t = curve.Evaluate(t);
            
            if(Vector3.Distance(selectedItem.transform.position, mixedBoxCenterPosision) > 0.03)
                selectedItem.transform.position = Vector3.Lerp(selectedItem.transform.position, mixedBoxCenterPosision, t);
            else
                selectedItem.transform.position = mixedBoxCenterPosision;        // ��ġ�� ����

            yield return null;
        }
        yield return new WaitForSeconds(1f);
        beginTime = Time.time;
        // �ͼ��ڽ��� ���������� ��ġ�� ����
        while (true)
        {
            var t = (Time.time - beginTime) / duration;

            if (t >= 1f) // ������ ��� �ð��� ������ ��
                break;
            t = curve.Evaluate(t);
            inputParticle[arrayNumber].transform.position -= new Vector3(0, dropSpeed, 0) * Time.deltaTime;
            selectedItem.transform.position -= new Vector3(0, dropSpeed, 0) * Time.deltaTime;

            yield return null;
        }
        // Ư�� ����Ʈ ����
        inputParticle[arrayNumber].SetActive(false);
        // ������ ��Ȱ��ȭ
        selectedItem.SetActive(false);        
        // ��ƼŬ ��ġ �ʱ�ȭ
        inputParticle[arrayNumber].transform.position = new Vector3(0, 0, 0);
        // �ش� �������� �θ� �ͼ��ڽ� ������ ����
        selectedItem.transform.parent = mixedObjectPool.transform;
    }

    private void ReturnObjectPosition( GameObject targetObject)
    {
        // ���� ��ġ�� ����
        targetObject.transform.position = targetObject.GetComponent<ObjectPosition>().originPos;
        // �ӵ� �ʱ�ȭ
        targetObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
    enum SpecialBats
    {
        giantBat, windBat, butterflyBat, drawingBat, fireBat, levitationBat, gravityFieldBat, goldBarBat
    }

    // ����� �� �迭
    int [] giantBat = { (int)MixedItem.LightBrownBat , (int)MixedItem.RedStone, (int)MixedItem.HeartPiece };
    int [] windBat = { (int)MixedItem.LightBrownBat, (int)MixedItem.BlueStone, (int)MixedItem.SkullPiece };
    int [] butterflyBat = { (int)MixedItem.LightBrownBat, (int)MixedItem.BlueStone, (int)MixedItem.HeartPiece };
    int[] drawingBat = { (int)MixedItem.LightBrownBat, (int)MixedItem.RedStone, (int)MixedItem.SkullPiece }; 

    int[] fireBat = { (int)MixedItem.DarkBrownBat, (int)MixedItem.RedStone, (int)MixedItem.HeartPiece };
    int [] levitationBat = { (int)MixedItem.DarkBrownBat, (int)MixedItem.BlueStone, (int)MixedItem.SkullPiece };
    int [] gravityFieldBat = { (int)MixedItem.DarkBrownBat, (int)MixedItem.BlueStone, (int)MixedItem.HeartPiece }; 
    int [] goldBarBat = { (int)MixedItem.DarkBrownBat, (int)MixedItem.RedStone, (int)MixedItem.SkullPiece };

    private void CompareArray()
    {
        // ȿ�� ����Ʈ IEnumerator�� ����
        StartCoroutine(IMakeSpecialBat());
    }

    IEnumerator IMakeSpecialBat()
    {
        // ������Ʈ Ǯ�� ������ 3������ ���� ���Դ��� üũ
        while (true)
        {
            if (mixedObjectPool.childCount > 2)
                break;
            yield return null;
        }
        yield return null;
        foreach (ParticleSystem particle in makeParticles)
        {
            particle.Play();
        }
        // �ϼ��� �迭�� �� �� ������ ��ġ�� ��ũ��Ʈ ����
        SendMessageWeaponStand();
    }

    IEnumerator IResetArray()
    {
        // �迭 �ʱ�ȭ
        for (int i = 0; i < mixerArray.Length; i++)
        {
            mixerArray[i] = 0;
        }
        // MixedObjectPool�� ����ִ� ������Ʈ ����ġ
        Transform[] children = new Transform[mixedObjectPool.childCount];
        for (int i = 0; i < mixedObjectPool.childCount; i++)
        {
            children[i] = mixedObjectPool.GetChild(i);
        }
        float count = mixedObjectPool.childCount;

        for (int i = 0; i < count; i++)
        {
            children[i] = mixedObjectPool.GetChild(0);
            children[i].gameObject.SetActive(true);

            // �ش� ������Ʈ �ݶ��̴� Ȱ��ȭ
            var findCollider = children[i].GetComponent<Collider>().GetType();

            if (findCollider.Equals(typeof(BoxCollider)))
                children[i].GetComponent<BoxCollider>().enabled = true;
            else if (findCollider.Equals(typeof(CapsuleCollider)))
                children[i].GetComponent<CapsuleCollider>().enabled = true;
            else
                children[i].GetComponent<MeshCollider>().enabled = true;

            // ��ġ�� ����
            children[i].transform.localPosition = children[i].GetComponent<ObjectPosition>().originPos;
            children[i].GetComponent<Rigidbody>().isKinematic = false;
            children[i].parent = children[i].GetComponent<ObjectPosition>().originParentObject.transform;
        }
        yield return null;
    }

    private void SendMessageWeaponStand()
    {
        if (mixerArray.SequenceEqual(giantBat))
        {
            weaponStandManager.MadeSpecialBat((int)SpecialBats.giantBat);
        }
        else if (mixerArray.SequenceEqual(windBat))
        {
            weaponStandManager.MadeSpecialBat((int)SpecialBats.windBat);
        }
        else if (mixerArray.SequenceEqual(butterflyBat))
        {
            weaponStandManager.MadeSpecialBat((int)SpecialBats.butterflyBat);
        }
        else if (mixerArray.SequenceEqual(drawingBat))
        {
            weaponStandManager.MadeSpecialBat((int)SpecialBats.drawingBat);
        }
        else if (mixerArray.SequenceEqual(fireBat))
        {
            weaponStandManager.MadeSpecialBat((int)SpecialBats.fireBat);
        }
        else if (mixerArray.SequenceEqual(levitationBat))
        {
            weaponStandManager.MadeSpecialBat((int)SpecialBats.levitationBat);
        }
        else if (mixerArray.SequenceEqual(gravityFieldBat))
        {
            weaponStandManager.MadeSpecialBat((int)SpecialBats.gravityFieldBat);
        }
        else if (mixerArray.SequenceEqual(goldBarBat))
        {
            weaponStandManager.MadeSpecialBat((int)SpecialBats.goldBarBat);
        }

        // �迭 �ʱ�ȭ
        StartCoroutine(IResetArray());
    }
}
