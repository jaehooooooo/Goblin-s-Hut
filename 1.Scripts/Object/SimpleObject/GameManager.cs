using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool playerMove = true;  // �÷��̾� ������, �ڷ���Ʈ ����
    public bool getSpecialBatLeft;
    public bool getSpecialBatRight;
    public bool getGravityBat;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }    
        else
        {
            Debug.LogWarning("���� �ΰ� �̻��� ���� �Ŵ����� ������");
            Destroy(gameObject);
        }

        playerMove = true;
    }



}
