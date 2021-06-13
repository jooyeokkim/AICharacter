using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float score;
    public int playerid;
    public GameObject AIdataObject;
    public Button sortdata;
    Rigidbody2D rigid;
    SpriteRenderer spriterender;
    Animator anim;
    private bool gojump = false;
    string ai = ""; //좌우 이동 지시열
    string aijump = ""; //점프 지시열
    int currentinsturction = -1; //현재 좌우 이동 지시 인덱스
    int currentinstructionjump = -1; //현재 점프 지시 인덱스
    float nextmove = 0;
    int howmanysamples = 50;
    int doublejump = 0;
    bool isfinished = false;
    bool candoublejump = false;
    public GameObject[] coins;
    bool[] v; // 보상 기둥들의 방문 여부 확인, 점수 중복 부여 방지

    // Start is called before the first frame update
    void Start()
    {

        v = new bool[coins.Length];
        score = 0;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();
        if (Savegene.generation == 0)
        {
            for (int i = 0; i < howmanysamples; i++)
            {
                int ran = Random.Range(0, 3);
                int jum = Random.Range(0, 2);
                ai += ran;
                aijump += jum;
            }
        }
        else
        {
            ai = Savegene.SavedGene1[playerid];
            aijump = Savegene.SavedGene2[playerid];
        }
        Debug.Log("playerid="+playerid+"->"+ai);
        Debug.Log("playerid=" + playerid + "->" + aijump);
        play();
        jump();
    }

    // Update is called once per frame
    void Update()
    {
        // 애니메이션의 걷기모션 실행 기준 지정
        if (Mathf.Abs(rigid.velocity.x) < 2)
            anim.SetBool("iswalking", false);
        else anim.SetBool("iswalking", true);
    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextmove, rigid.velocity.y);

        // 떨어지는 동안 바닥을 탐지 
        if (rigid.velocity.y < 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("GROUND"));
            if (hit.collider != null)
            {
                if (hit.distance < 0.5f) // 탐지물 결과까지의 거리가 0.5미만일 경우 캐릭터가 바닥에 있다고 가정
                {
                    doublejump = 0;
                    anim.SetBool("isjumping", false);
                }
            }
        }
        if (anim.GetBool("isjumping") && doublejump == 1) // 더블 점프 로직
        {
            candoublejump = true;
        }
        else candoublejump = false;

        // 한 세대의 모든 지시 이행 완료
        if (currentinstructionjump >= aijump.Length && currentinsturction >= aijump.Length && !isfinished)
        {
            score += 10*this.gameObject.transform.localPosition.x; // 종료 위치의 x좌표값만큼 높은 점수 부여
            isfinished = true;
            AIdata aidata = AIdataObject.GetComponent<AIdata>();
            aidata.AddData(playerid, ai, aijump, score); // AI 학습
            sortdata.interactable = true;           
        }


        if (gojump)
        {
            if (!anim.GetBool("isjumping"))
            {
                rigid.AddForce(Vector2.up * 18, ForceMode2D.Impulse);
                doublejump = 1;
            }
            else if(rigid.velocity.y<-5 && candoublejump) // 싱글 점프 후 어느 정도 내려왔을 때 추가 점프 허용
            {
                rigid.AddForce(Vector2.up * 18, ForceMode2D.Impulse);
                doublejump = 0;
            }
            anim.SetBool("isjumping", true);
        }
    }

    void play()
    {
        currentinsturction++;
        if (currentinsturction>= ai.Length) //한 세대의 모든 지시 이행 완료
        {
            nextmove = 0;
            gojump = false;
            return;
        }
        if (ai[currentinsturction] == '0') // 좌 이동
        {
            nextmove = -4.0f;
            spriterender.flipX = true;
        }
        else if (ai[currentinsturction] == '1') // 우 이동
        {
            nextmove = 4.0f;
            spriterender.flipX = false;
        }
        else // 정지
        {
            nextmove = 0;
        }
        Invoke("play", 0.3f); // 0.3초 후에 다음 지시 이행
    }

    void jump()
    {
        currentinstructionjump++;
        if (currentinstructionjump>= aijump.Length) //한 세대의 모든 지시 이행 완료
        {
            nextmove = 0;
            gojump = false;
            Debug.Log("jump학습종료");
            return;
        }

        if (aijump[currentinstructionjump] == '0') gojump = false;
        else gojump = true;
        Invoke("jump", 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "dropcollider") // 떨어졌을 경우 매우 낮은 점수 부여
        {
            score = -100;
            return;
        }

        if (collision.gameObject.tag == "rewards") // 보상 기둥에 닿았을 경우 점수 부여
        {
            for(int i = 0; i < coins.Length; i++)
            {
                string s = "coin" + i;
                if (collision.gameObject.name == s)
                {
                    if (i == 10) Time.timeScale = 0; // 하나의 캐릭터라도 도착지에 도달 했을 경우, 모든 캐릭터 정지
                    if (!v[i]) // 방문하지 않은 보상 기둥일 경우
                    {
                        score+=i;
                        v[i] = true;
                    }
                }
            }
        }
    }
}
