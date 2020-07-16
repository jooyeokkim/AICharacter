using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class simulation : MonoBehaviour
{
    public GameObject AIdataObject;
    Rigidbody2D rigid;
    SpriteRenderer spriterender;
    Animator anim;
    private bool gojump = false;
    string ai = "";
    string aijump = "";
    int currentinsturction = -1;
    int currentinstructionjump = -1;
    float nextmove = 0;
    int doublejump = 0;
    int howmanysamples = 50;
    bool candoublejump = false;
    bool[] v;
    bool[] v2;
    // Start is called before the first frame update
    void Start()
    {
        AIdata aidata = AIdataObject.GetComponent<AIdata>();
        v2 = new bool[howmanysamples];
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();
        ai = aidata.gettopgen1();
        aijump = aidata.gettopgen2();
        Debug.Log(ai);
        Debug.Log(aijump);

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
        if (gojump && !v2[currentinstructionjump])
        {
            v2[currentinstructionjump] = true;
            if (!anim.GetBool("isjumping"))
            {
                rigid.AddForce(Vector2.up * 18, ForceMode2D.Impulse);
                doublejump = 1;
            }
            else if (rigid.velocity.y < -5 && candoublejump)
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
        if (currentinsturction >= ai.Length)
        {
            //CancelInvoke();
            nextmove = 0;
            gojump = false;
            Debug.Log("play시뮬레이션종료");
            return;
        }
        if (ai[currentinsturction] == '0')
        {
            nextmove = -3.5f;
            spriterender.flipX = true;
        }
        else if (ai[currentinsturction] == '1')
        {
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
        if (currentinstructionjump >= aijump.Length)
        {
            //CancelInvoke();
            nextmove = 0;
            gojump = false;
            Debug.Log("jump시뮬레이션종료");
            return;
        }
        if (aijump[currentinstructionjump] == '0') gojump = false;
        else gojump = true;
        Invoke("jump", 0.3f);
    }
}
