using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove: MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRender;
    CapsuleCollider2D capCollider;
    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRender = GetComponent<SpriteRenderer>();
        capCollider = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 5);

    }

    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);


        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y);
        Debug.DrawRay(frontVec ,Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null)
        {
            Turn();
        }

    }

    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);
        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
        {
            spriteRender.flipX = nextMove == 1;
        }

        //Recursive 는 가장 아래에
        float nextThinkTime = Random.Range(2, 5);
        Invoke("Think", nextThinkTime);
    }
    void Turn()
    {
        nextMove *= -1;
        spriteRender.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 5);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriteRender.color = new Color(1, 1, 1, 0.4f);
        spriteRender.flipY = true;
        capCollider.enabled = false;
        rigid.AddForce(Vector2.up*3, ForceMode2D.Impulse);
        Invoke("DeActive", 5);
    }
    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
