using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //점수와 스테이지 관리

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    // player hp
    public int hp;
    public GameObject[] stages;

    public PlayerMove player;

    public void NextStage()
    {

        if (stageIndex < stages.Length-1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        else
        {
            //Game Clear
            Time.timeScale = 0;

            Debug.Log("게임 클리어");

        }

        totalPoint += stagePoint;
        stagePoint = 0;


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (hp > 1)
            {
                PlayerReposition();
            }
            HpDown();
        }

    }

    public void HpDown()
    {
        if (hp > 1)
            hp--;
        else
        {
            player.OnDie();

            Debug.Log("앙");
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }
}
