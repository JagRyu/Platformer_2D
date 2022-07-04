using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRender;
    Animator anim;
    CapsuleCollider2D capCollider;

    public float maxSpeed;
    public float jumpPower;
    public GameManager gameManager;

    

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update() // *단발적 구현은 Update가 좋다
    {
        //Jump
        if (Input.GetButtonDown("Jump")&& !anim.GetBool("isJump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRender.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation
        if(Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("isWalk", false);
        }
        else
        {
            anim.SetBool("isWalk", true);
        }
    }

    void FixedUpdate()
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if(rigid.velocity.x > maxSpeed) // Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < maxSpeed * (-1)) // Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        //Landing Platform
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.45f)
                {
                    anim.SetBool("isJump", false);
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }
    void OnAttack(Transform enemy)
    {
        //Point

        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        gameManager.stagePoint += 100;

    }

    void OnDamaged(Vector2 targetPos)
    {
        gameObject.layer = 11;

        spriteRender.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7,ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("doDamaged");
        //Health Down
        gameManager.HpDown();
        Invoke("OffDamaged", 2f);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRender.color = new Color(1, 1, 1,1);
    }

    public void OnDie()
    {
        spriteRender.color = new Color(1, 1, 1, 0.4f);
        spriteRender.flipY = true;
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
        gameObject.SetActive(false);
    }
    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Point
        

        //Deactive Item
        if(collision.gameObject.tag == "Item")
        {

            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSliver = collision.gameObject.name.Contains("Sliver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSliver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;

            collision.gameObject.SetActive(false);

        }

        if(collision.gameObject.tag == "Finish")
        {
            //Next Stage
            gameManager.stagePoint += 100;
            gameManager.NextStage();

        }
    }
}
