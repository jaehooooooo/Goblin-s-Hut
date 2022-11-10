using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    // 깨지거나 부셔질 수 있는 오브젝트가 갖고있는 스크립트

    // 오브젝트 풀을 활용하여 본인이 갖고있는 오브젝트를 껏다가 키는 방식으로 작동
    enum objectState
    {
        standard,nomal,sliceWidth,sliceLength,crush
    }

    // 부셔지거나 깨질 수 있는지 여부
    public bool sliceable;
    public bool crushable;

    public GameObject[] objectStates;
    bool isBroken = false;
    public int waitForsec;

    public Vector3 boxSize;    // 잘릴 방향 확인용 박스사이즈
    public float maxDistance;   // 검출 가능 거리
    RaycastHit hit;

    float velocity;             // 부셔지거나 깨진 파편이 흩어질 속도

    public void Broken(bool isSharp, float weaponVelocity)
    {
        if (isBroken)
            return;

        velocity = weaponVelocity;
        if (isSharp && sliceable)   // 날카로운 무기에 맞았고 && 잘릴 수 있는 물체일때
        {
            StartCoroutine(IFindDirectionSlice());
        }
        else if(!isSharp && crushable ) // 뭉뚝한 무기에 맞았고 && 부셔질 수 있는 물체일때
        {        
            // 더이상 부셔지지 않도록 bool값 변경
            isBroken = true;
            //gameObject.GetComponent<BoxCollider>().enabled = false;
            var findCollider = gameObject.GetComponent<Collider>().GetType();

            if (findCollider.Equals(typeof(BoxCollider)))
                gameObject.GetComponent<BoxCollider>().enabled = false;
            else if (findCollider.Equals(typeof(CapsuleCollider)))
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
            else
                gameObject.GetComponent<MeshCollider>().enabled = false;


            // 물체와 부딪힌 무기가 뭉툭하다면 -> 물체 부셔짐
            StartCoroutine(IBrokenObject());
        }
    }

    IEnumerator IFindDirectionSlice()
    {
        // 콜라이더와 리지드바디 비활성화
        GetComponent<Rigidbody>().isKinematic = true;
        var findCollider = gameObject.GetComponent<Collider>().GetType();

        if (findCollider.Equals(typeof(BoxCollider)))
            gameObject.GetComponent<BoxCollider>().enabled = false;
        else if (findCollider.Equals(typeof(CapsuleCollider)))
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        else
            gameObject.GetComponent<MeshCollider>().enabled = false;

        //While문 Vector3
        while (true)
        {
            // 잘린 방향이 세로인지 가로인지 판단
            if (Physics.BoxCast(transform.position, boxSize / 2, Vector3.up, out hit, Quaternion.identity, maxDistance)
                || Physics.BoxCast(transform.position, boxSize / 2, Vector3.down, out hit, Quaternion.identity, maxDistance))
            {
                // 세로 방향으로 잘림
                print("세로방향");
                print("세로" + (transform.position - hit.point));
                StartCoroutine(ISliceLengthObject());
                break;
            }
            else if (Physics.BoxCast(transform.position, boxSize / 2, Vector3.forward, out hit, Quaternion.identity, maxDistance)
                || Physics.BoxCast(transform.position, boxSize / 2, Vector3.back, out hit, Quaternion.identity, maxDistance)
                || Physics.BoxCast(transform.position, boxSize / 2, Vector3.left, out hit, Quaternion.identity, maxDistance)
                || Physics.BoxCast(transform.position, boxSize / 2, Vector3.right, out hit, Quaternion.identity, maxDistance))
            {
                // 가로 방향으로 잘림
                print("가로방향");
                print("까로" + (transform.position - hit.point));

                StartCoroutine(ISliceWidthObject());
                break;
            }
            yield return null;
        }
        #region 도전해본 결과 정작 중요한 문제는 칼이랑 상자랑 닿았을때 반응하는 박스 콜라이더의 문제라서 방향성이 잘못됨
        // While 문 TransformDirection()
        //while (true)
        //{
        //    // 잘린 방향이 세로인지 가로인지 판단
        //    if (Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.up), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.down), out hit, Quaternion.identity, maxDistance))
        //    {
        //        // 세로 방향으로 잘림
        //        print("세로방향");

        //        StartCoroutine(IsliceLengthObject());
        //        break;
        //    }
        //    else if (Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.forward), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2,transform.TransformDirection( Vector3.back), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.left), out hit, Quaternion.identity, maxDistance)
        //        || Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(Vector3.right), out hit, Quaternion.identity, maxDistance))
        //    {
        //        // 가로 방향으로 잘림
        //        print("가로방향");
        //        StartCoroutine(IsliceWidthObject());
        //        break;
        //    }
        //    yield return null;
        //}
        //}
        //while (true)
        //{
        //    // 잘린 방향이 세로인지 가로인지 판단
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
        //            // 세로 방향으로 잘림
        //            print("세로방향");
        //            print("세로" + (transform.position - hit.point));
        //            StartCoroutine(IsliceLengthObject());
        //            break;

        //        }
        //        else
        //        {
        //            // 가로 방향으로 잘림
        //            print("가로방향");
        //            print("까로" + (transform.position - hit.point));

        //            StartCoroutine(IsliceWidthObject());
        //            break;
        //        }
        //    }
        //    yield return null;

        //}
        //while (true)
        //{
        //    // 잘린 방향이 세로인지 가로인지 판단
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
        //            // 세로 방향으로 잘림
        //            print("세로방향");
        //            print("세로" + (transform.position - hit.point));
        //            StartCoroutine(IsliceLengthObject());
        //            break;

        //        }
        //        else
        //        {
        //            // 가로 방향으로 잘림
        //            print("가로방향");
        //            print("까로" + (transform.position - hit.point));

        //            StartCoroutine(IsliceWidthObject());
        //            break;
        //        }
        //    }
        //    yield return null;
        //}
        #endregion
        // 더이상 부셔지지 않도록 bool값 변경
        isBroken = true;
    }
    
    IEnumerator ISliceWidthObject()
    {
        GetComponent<Rigidbody>().isKinematic = false;

        // 오브젝트 풀에서 슬라이스 오브젝트 활성화 및 기본 노멸 오브젝트 비활성화
        objectStates[(int)objectState.nomal].SetActive(false);
        objectStates[(int)objectState.sliceWidth].SetActive(true);
        // 오브젝트 풀에 들어있는 파편 튀기
        BrokenStart(objectStates[(int)objectState.sliceWidth]);

        // 5초 정도 기다리기
        yield return new WaitForSeconds(waitForsec);
        // 오브젝트 풀에서 슬라이스 오브젝트 비활성화 및 기본 노멀 오브젝트 활성화
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

        // 위치값 원래 위치로 변경
        gameObject.transform.localPosition = gameObject.GetComponent<ObjectPosition>().originPos;
        GetComponent<Rigidbody>().isKinematic = false;

        // bool값 변경
        isBroken = false;
    }
    IEnumerator ISliceLengthObject()
    {
        GetComponent<Rigidbody>().isKinematic = false;

        // 오브젝트 풀에서 슬라이스 오브젝트 활성화 및 기본 노멸 오브젝트 비활성화
        objectStates[(int)objectState.nomal].SetActive(false);
        objectStates[(int)objectState.sliceLength].SetActive(true);
        // 오브젝트 풀에 들어있는 파편 튀기
        BrokenStart(objectStates[(int)objectState.sliceLength]);

        // 5초 정도 기다리기
        yield return new WaitForSeconds(waitForsec);
        // 오브젝트 풀에서 슬라이스 오브젝트 비활성화 및 기본 노멀 오브젝트 활성화
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

        // 위치값 원래 위치로 변경
        gameObject.transform.localPosition = gameObject.GetComponent<ObjectPosition>().originPos;
        // 콜라이더와 리지드바디 비활성화
        GetComponent<Rigidbody>().isKinematic = false;

        // bool값 변경
        isBroken = false;
    }
    IEnumerator IBrokenObject()
    {
        // 오브젝트 풀에서 슬라이스 오브젝트 활성화 및 기본 노멸 오브젝트 비활성화
        objectStates[(int)objectState.nomal].SetActive(false);
        objectStates[(int)objectState.crush].SetActive(true);
        // 오브젝트 풀에 들어있는 파편 튀기
        BrokenStart(objectStates[(int)objectState.crush]);

        // 5초 정도 기다리기
        yield return new WaitForSeconds(waitForsec);
        // 오브젝트 풀에서 슬라이스 오브젝트 비활성화 및 기본 노멀 오브젝트 활성화
        BrokenEnd(objectStates[(int)objectState.crush]);
        objectStates[(int)objectState.crush].SetActive(false);
        objectStates[(int)objectState.nomal].SetActive(true);

        // 기본 도형 리지드바디 & 콜라이더 활성화
        //gameObject.GetComponent<BoxCollider>().enabled = true;
        var findCollider = gameObject.GetComponent<Collider>().GetType();

        if (findCollider.Equals(typeof(BoxCollider)))
            gameObject.GetComponent<BoxCollider>().enabled = true;
        else if (findCollider.Equals(typeof(CapsuleCollider)))
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        else
            gameObject.GetComponent<MeshCollider>().enabled = true;
        // 위치값 원래 위치로 변경
        gameObject.transform.localPosition = gameObject.GetComponent<ObjectPosition>().originPos;
        // 콜라이더와 리지드바디 비활성화
        GetComponent<Rigidbody>().isKinematic = false;

        // bool값 변경
        isBroken = false;

    }
    private void BrokenStart(GameObject target)
    {
        // 해당 오브젝트 풀에 들어있는 물체 갯수
        int length = target.transform.childCount;
        GameObject[] objectsPool = new GameObject[length];
        for(int i = 0; i <length; i++)
        {
            Transform brokenObject = target.transform.GetChild(i);
            // 랜덤한 방향 설정
            Vector3 direction = Random.insideUnitSphere;
            // 날아가기 
            Rigidbody rb = brokenObject.GetComponent<Rigidbody>();
            rb.velocity = direction * velocity;
        }
    }

    private void BrokenEnd(GameObject target)
    {
        // 해당 오브젝트 풀에 들어있는 물체 갯수
        int length = target.transform.childCount;
        GameObject[] objectsPool = new GameObject[length];
        for (int i = 0; i < length; i++)
        {
            Transform brokenObject = target.transform.GetChild(i);
            brokenObject.transform.localPosition = Vector3.zero;
        }
    }
}
