using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MixerBox : MonoBehaviour
{
    // 조합을 위한 스크립트 
    // Overlap Box로 검출

    // 변수
    private enum MixedItem
    {
        LightBrownBat = 1, DarkBrownBat, RedStone, BlueStone, HeartPiece, SkullPiece
    }
    private MixedItem nowMixedItem;

    public int[] mixerArray = { 0, 0, 0 };
    int arrayNumber;

    public LayerMask mixedItemLayer;    // 조합이 가능한 아이템 레이어

    public Transform mixedObjectPool;
    public WeaponStandManager weaponStandManager;


    public Vector3 overlapBoxPosition;
    public Vector3 overlapBoxSize;
    public Vector3 mixedBoxCenterPosision;

    GameObject selletedMixedItem;       // 조합대에 들어온 조합 아이템

    public float duration;              // 조합대 중앙으로 이동할 시간
    public float dropSpeed;
    // 같은 속도가 아니라 점점빨라지는 효과를 위해 애니메이션 커브 생성
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public float resetDurarion;         // 방망이 조합이 끝나고 새로운 방망이 조합까지 걸릴 시간

    public GameObject[] inputParticle;     // 조합아이템이 들어갈때 사용될 파티클
    public ParticleSystem[] inputParticles; // 조합 아이템이 들어갈 때 사용될 파티클
    public ParticleSystem[] makeParticles;  // 방망이가 완성되었을때 사용될 파티클

    // 초기화
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
    // 기능

    // 오버랩 박스에 접촉된 모든 콜라이더를 비교함 // 믹서박스에 넣을 수 있는 것인지 아닌지 // 이미 들어있는 것인지 아닌지 
    private void SelletTouchedItem()
    {
        Collider[] touchedColliders = Physics.OverlapBox(overlapBoxPosition, overlapBoxSize/2,Quaternion.identity,mixedItemLayer);

        // 콜라이더에 검출된 물체가 있는 경우
        if (touchedColliders.Length > 0)
        {
            // 모든 콜라이더 비교
            foreach (Collider touchedCollider in touchedColliders)
            {
                string tag = touchedCollider.tag;

                // ＠손에 쥐고 있는 상태의 오브젝트라면
                if (touchedCollider.GetComponent<ObjectPosition>().isGrabbed)
                {
                    return;
                }
                //else if (touchedCollider.GetComponent<ObjectPosition>())
                //{
                //    if (!touchedCollider.GetComponent<ObjectPosition>().CheckObjectSituation())
                //        return;
                //}
                // 조합이 가능한 아이템의 태그라면
                else if (tag == "Mixable")
                {
                    selletedMixedItem = touchedCollider.gameObject;
                    // 원하던 조합 아이템인지 판단하는 함수로 이동
                    SelletedMixedItem(selletedMixedItem);
                }
                else if (tag == "Grabbable")
                {
                    // 이거는 조합할수 없다고 알려주고
                    print("이거는 조합이 불가한 아이템이야");
                    // 원 위치로 변경하는 함수로 이동
                    ReturnObjectPosition(touchedCollider.gameObject);
                }
            }
        }
    }
    
    // 믹서박스에 넣을 수 있고 현재 배열 상에 빠져있는 아이템인지 판단
    private void SelletedMixedItem( GameObject selletedMixedItem)
    {
        
        // 셀렉된 아이템이 들어갈 배열 찾기
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

        // 배열의 현재 상태와 비교
        if(mixerArray[arrayNumber] == 0)    // 해당 배열이 비어있다면
        {
            // 현재 enum에 오브젝트 이름에 해당하는 값 할당
            nowMixedItem = (MixedItem)System.Enum.Parse(typeof(MixedItem), selletedMixedItem.name);
            // 배열에 그 값 할당
            mixerArray[arrayNumber] = (int)nowMixedItem;
            // 해당 오브젝트 더이상 감지 못하게 콜라이더 비활성화
            var findCollider = selletedMixedItem.GetComponent<Collider>().GetType();

            if (findCollider.Equals(typeof(BoxCollider)))
                selletedMixedItem.GetComponent<BoxCollider>().enabled = false;
            else if (findCollider.Equals(typeof(CapsuleCollider)))
                selletedMixedItem.GetComponent<CapsuleCollider>().enabled = false;
            else
                selletedMixedItem.GetComponent<MeshCollider>().enabled = false;

            // 해당 오브젝트를 움직여서 넣는 모션 실행
            StartCoroutine(MoveSelectedItem(selletedMixedItem,arrayNumber));

            // 배열값 3개가 다 찼다면 배열 값 확인하는 함수로 이동
            if (mixerArray[0] != 0 && mixerArray[1] != 0 && mixerArray[2] != 0)
            {
                CompareArray();
            }
        }
        else // 해당 배열이 비어있지 않다면
        {
            // 해당 종류의 아이템은 이미 들어있다고 메세지 전달
            print("이미 그 아이템 종류는 들어있어");
            // 해당 오브젝트를 원래 위치로 되도리는 함수 실행
            ReturnObjectPosition(selletedMixedItem);
        }
    }

    IEnumerator MoveSelectedItem( GameObject selectedItem, int arrayNumber)
    {
        // 해당 아이템의 속도 초기화
        selletedMixedItem.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        // 해당 아이템의 리지드바디 초기화
        selletedMixedItem.GetComponent<Rigidbody>().isKinematic = true;
        // 믹서박스에서 빛이 나올 시간
        inputParticle[arrayNumber].SetActive(true);
        inputParticle[arrayNumber].transform.position = mixedBoxCenterPosision;

        // 해당 아이템 위치를 원하는 위치로 변경
        var beginTime = Time.time;
        Vector3 originPos = selectedItem.transform.position;
        while (true)
        {
            var t = (Time.time - beginTime) / duration;

            if (t >= 1f) // 지정한 경과 시간이 지나면 끝
                break;
            t = curve.Evaluate(t);
            
            if(Vector3.Distance(selectedItem.transform.position, mixedBoxCenterPosision) > 0.03)
                selectedItem.transform.position = Vector3.Lerp(selectedItem.transform.position, mixedBoxCenterPosision, t);
            else
                selectedItem.transform.position = mixedBoxCenterPosision;        // 위치값 보정

            yield return null;
        }
        yield return new WaitForSeconds(1f);
        beginTime = Time.time;
        // 믹서박스로 떨어지도록 위치값 변경
        while (true)
        {
            var t = (Time.time - beginTime) / duration;

            if (t >= 1f) // 지정한 경과 시간이 지나면 끝
                break;
            t = curve.Evaluate(t);
            inputParticle[arrayNumber].transform.position -= new Vector3(0, dropSpeed, 0) * Time.deltaTime;
            selectedItem.transform.position -= new Vector3(0, dropSpeed, 0) * Time.deltaTime;

            yield return null;
        }
        // 특수 이펙트 끄기
        inputParticle[arrayNumber].SetActive(false);
        // 아이템 비활성화
        selectedItem.SetActive(false);        
        // 파티클 위치 초기화
        inputParticle[arrayNumber].transform.position = new Vector3(0, 0, 0);
        // 해당 아이템의 부모를 믹서박스 하위로 변경
        selectedItem.transform.parent = mixedObjectPool.transform;
    }

    private void ReturnObjectPosition( GameObject targetObject)
    {
        // 원래 위치로 변경
        targetObject.transform.position = targetObject.GetComponent<ObjectPosition>().originPos;
        // 속도 초기화
        targetObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
    enum SpecialBats
    {
        giantBat, windBat, butterflyBat, drawingBat, fireBat, levitationBat, gravityFieldBat, goldBarBat
    }

    // 방망이 별 배열
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
        // 효과 이펙트 IEnumerator로 생성
        StartCoroutine(IMakeSpecialBat());
    }

    IEnumerator IMakeSpecialBat()
    {
        // 오브젝트 풀에 아이템 3종류가 전부 들어왔는지 체크
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
        // 완성된 배열과 비교 후 맞으면 거치대 스크립트 실행
        SendMessageWeaponStand();
    }

    IEnumerator IResetArray()
    {
        // 배열 초기화
        for (int i = 0; i < mixerArray.Length; i++)
        {
            mixerArray[i] = 0;
        }
        // MixedObjectPool에 들어있는 오브젝트 원위치
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

            // 해당 오브젝트 콜라이더 활성화
            var findCollider = children[i].GetComponent<Collider>().GetType();

            if (findCollider.Equals(typeof(BoxCollider)))
                children[i].GetComponent<BoxCollider>().enabled = true;
            else if (findCollider.Equals(typeof(CapsuleCollider)))
                children[i].GetComponent<CapsuleCollider>().enabled = true;
            else
                children[i].GetComponent<MeshCollider>().enabled = true;

            // 위치값 변경
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

        // 배열 초기화
        StartCoroutine(IResetArray());
    }
}
