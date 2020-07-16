using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int score;
    public int playerid;
    public GameObject AIdataObject;
    public Button sortdata;
    Rigidbody2D rigid;
    SpriteRenderer spriterender;
    Animator anim;
    private bool gojump = false;
    string ai = "";
    string aijump = "";
    int currentinsturction = -1;
    int currentinstructionjump = -1;
    float nextmove = 0;
    int howmanysamples = 50;
    int doublejump = 0;
    bool isfinished = false;
    bool candoublejump = false;
    public GameObject[] coins;
    bool[] v;
    bool[] v2;
    // Start is called before the first frame update
    void Start()
    {

        v = new bool[coins.Length];
        v2 = new bool[howmanysamples];
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
        if (Mathf.Abs(rigid.velocity.x) < 2)
            anim.SetBool("iswalking", false);
        else anim.SetBool("iswalking", true);
    }
    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextmove, rigid.velocity.y);
        if (rigid.velocity.y < 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("GROUND"));
            if (hit.collider != null)
            {
                if (hit.distance < 0.5f)
                {
                    doublejump = 0;
                    anim.SetBool("isjumping", false);
                }
            }
        }
        if (anim.GetBool("isjumping") && doublejump == 1)
        {
            candoublejump = true;
        }
        else candoublejump = false;
        if (currentinstructionjump >= aijump.Length && currentinsturction >= aijump.Length && !isfinished)
        {
            isfinished = true;
            AIdata aidata = AIdataObject.GetComponent<AIdata>();
            aidata.AddData(playerid, ai, aijump, score);
            sortdata.interactable = true;           
        }
        if (gojump && !v2[currentinstructionjump])
        {
            v2[currentinstructionjump] = true;
            if (!anim.GetBool("isjumping"))
            {
                rigid.AddForce(Vector2.up * 18, ForceMode2D.Impulse);
                doublejump = 1;
            }
            else if(rigid.velocity.y<-5 && candoublejump)
            {
                rigid.AddForce(Vector2.up * 18, ForceMode2D.Impulse);
                doublejump = 0;
            }
            else
            {
                //Debug.Log(currentinstructionjump);
                //aijump = aijump.Remove(currentinstructionjump, 1); //processing missed jump
                //aijump = aijump.Insert(currentinstructionjump, "0"); //processing missed jump
                //Debug.Log(aijump);               
            }
            anim.SetBool("isjumping", true);
        }
    }
    void play()
    {
        currentinsturction++;
        if (currentinsturction>= ai.Length)
        {
            //CancelInvoke();
            nextmove = 0;
            gojump = false;
            Debug.Log("play학습종료");
            return;
        }
        if (ai[currentinsturction] == '0')
        {
            nextmove = -3.5f;
            spriterender.flipX = true;
        }
        else if (ai[currentinsturction] == '1')
        {
            score+=2;
            nextmove = 3.5f;
            spriterender.flipX = false;
        }
        else
        {
            nextmove = 0;
        }
        Invoke("play", 0.3f);
    }
    void jump()
    {
        currentinstructionjump++;
        if (currentinstructionjump>= aijump.Length)
        {
            //CancelInvoke();
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
        if (collision.gameObject.tag == "dropcollider")
        {
            score = -100;
            return;
        }
        if (collision.gameObject.tag == "rewards")
        {
            for(int i = 0; i < coins.Length; i++)
            {
                string s = "coin" + i;
                if (collision.gameObject.name == s)
                {
                    if (!v[i])
                    {
                        score ++;
                        v[i] = true;
                    }
                }
            }
        }

    }
}
