using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mushroom : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;

    public bool beingDamaged = false;

    private Animator anim;
    private SpriteRenderer spriterenderer;

    private MushroomStat _mushroomStat;

    

    void Awake()
    {
        
        anim = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        _mushroomStat = GetComponent<MushroomStat>();
        
        Think();
    }
    


    void FixedUpdate()
    {
        
        //몬스터가 피격 되었을 때 뒤로 살짝 밀리게 하기 위해서 움직임을 잠깐 멈추는 코드임.
        //Player 스크립트에서 코루틴으로 beingDamaged 변수의 값을 조정함.
        if (!beingDamaged)
            move();
        
        



    }

    void move()
    {
        rigid.velocity = new Vector2(nextMove * 7, rigid.velocity.y);
        Vector2 fontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(fontVec, Vector3.down * 1.4f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(fontVec, Vector2.down , 1.4f, LayerMask.GetMask("Floor"));
        // Debug.Log(rayHit.collider);
        if (rayHit.collider == null)
        {
            nextMove *= -1;
            spriterenderer.flipX = nextMove == -1;
            
            CancelInvoke();
            Invoke("Think", Random.Range(1f, 4f));
        
        }
    }

    
    void Think()
    {
        //Set next active
        nextMove = Random.Range(-1, 2);
        
        //Sprite Animation
        anim.SetInteger("walkSpeed", nextMove);
        
        //Flip Sprite
        if(nextMove != 0)
            spriterenderer.flipX = nextMove == -1;
        
        //Recursive
        float nextThinkTime = Random.Range(0f, 2f);
        Invoke("Think", nextThinkTime);
    }
    
    public void onDamaged(Vector2 targetPos)
    {
        int dirc = targetPos.x - transform.position.x > 0 ? -1 : 1;
        Debug.Log(dirc);
        
        spriterenderer.flipX = dirc == 1;
        CancelInvoke();
        Invoke("Think", 0.5f);
        

        
        rigid.velocity = new Vector2(dirc*4, rigid.velocity.y);
        
        
        _mushroomStat.Hit(PlayerStat._playerStat.atk);
        anim.SetTrigger("damaged");

        
    }
}