using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int score;
    public int playerid;
    Rigidbody2D rigid;
    SpriteRenderer spriterender;
    Animator anim;
    public Text text1;
    private bool gojump = false;
    public Button text2;
    public Button text3;
    string ai = "";
    string aijump = "";
    int currentinsturction = 0;
    int currentinstructionjump = 0;
    int nextmove = 0;
    public GameObject[] coins;
    bool[] v;
    // Start is called before the first frame update
    void Start()
    {
        v = new bool[coins.Length];
        score = 0;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();
        for(int i = 0; i < 50; i++)
        {
            int ran = Random.Range(0, 3);
            int jum = Random.Range(0, 2);
            ai += ran;
            aijump += jum;
        }
        text1.text = ai;
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
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("GROUND"));
            if (hit.collider != null)
            {
                if (hit.distance < 0.5f)
                {
                    anim.SetBool("isjumping", false);
                }
            }
        }
        if (gojump && !anim.GetBool("isjumping"))
        {
            rigid.AddForce(Vector2.up * 16, ForceMode2D.Impulse);
            anim.SetBool("isjumping", true);
        }
    }
    void play()
    {
        if (currentinsturction>= ai.Length)
        {
            CancelInvoke();
            nextmove = 0;
            gojump = false;
            return;
        }
        text2.GetComponentInChildren<Text>().text = ""+(currentinsturction+1)+"번째 명령";
        text3.GetComponentInChildren<Text>().text = "" + ai[currentinsturction];
        if (ai[currentinsturction] == '0')
        {
            nextmove = -3;
            spriterender.flipX = true;
        }
        else if (ai[currentinsturction] == '1')
        {
            nextmove = 3;
            spriterender.flipX = false;
        }
        else
        {
            nextmove = 0;
        }
        currentinsturction++;
        Invoke("play", 0.3f);
    }
    void jump()
    {
        if (currentinstructionjump>= aijump.Length)
        {
            CancelInvoke();
            nextmove = 0;
            gojump = false;
            return;
        }
        if (aijump[currentinstructionjump] == '0') gojump = false;
        else gojump = true;
        currentinstructionjump++;
        Invoke("jump", 0.3f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "dropcollider")
        {
            score = -10;
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
                        score++;
                        v[i] = true;
                    }
                }
            }
        }

    }
}
