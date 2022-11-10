using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool playerMove = true;  // 플레이어 움직임, 텔레포트 제한
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
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재함");
            Destroy(gameObject);
        }

        playerMove = true;
    }



}
