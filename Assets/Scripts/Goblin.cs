using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Goblin : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;

    public bool beingDamaged = false;

    private Animator anim;
    private SpriteRenderer spriterenderer;

    private GoblinStat _goblinStat;

    private bool isTracking;
    

    public Vector2 boxSize;

    private float normalCurTime;
    public float normalCoolTime = 0.4f;

    private Player player;

    private bool isAttacking;
    private bool stopTracking;
    

    void Awake()
    {

        anim = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        _goblinStat = GetComponent<GoblinStat>();
        isAttacking = false;
        isTracking = false;
        stopTracking = false;

        Think();
    }



    void FixedUpdate()
    {

        //몬스터가 피격 되었을 때 뒤로 살짝 밀리게 하기 위해서 움직임을 잠깐 멈추는 코드임.
        //Player 스크립트에서 코루틴으로 beingDamaged 변수의 값을 조정함.
        if (!beingDamaged)
            move();

        if(!stopTracking)
            trackPlayer();

        if (!isAttacking)
        {
            Debug.Log("In function");
            readyToAttack();
        }



    }

    void move()
    {
        rigid.velocity = new Vector2(nextMove * 5, rigid.velocity.y);
        Vector2 fontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(fontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(fontVec, Vector2.down, 1, LayerMask.GetMask("Floor"));
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
        if (nextMove != 0)
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



        rigid.velocity = new Vector2(dirc * 4, rigid.velocity.y);

        _goblinStat.Hit(PlayerStat._playerStat.atk);
        anim.SetTrigger("damaged");
    }

    //자신이 바라보는 방향의 일정거리 안에 있는 플레이어를 추적하는 함수
    void trackPlayer()
    {
        Vector2 rayVec;
        if (spriterenderer.flipX)
            rayVec = Vector2.left;
        else
            rayVec = Vector2.right;

        Debug.DrawRay(transform.position, rayVec * 10, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, rayVec, 10, LayerMask.GetMask("Player"));


        if (rayHit.collider)
        {
            CancelInvoke();
            isTracking = true;
            float distance = GameObject.Find("Player").transform.position.x - transform.position.x;
            nextMove = distance > 0 ? 1 : -1;
            anim.SetInteger("walkSpeed", nextMove);
            

            if (nextMove != 0)
                spriterenderer.flipX = nextMove == -1;
        }

        else
        {
            if (isTracking)
                Invoke("Think", Random.Range(1f, 4f));

            isTracking = false;
        }
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(new Vector3(transform.position.x + 1.4f, transform.position.y, transform.position.z),
            boxSize);
        Gizmos.DrawWireCube(new Vector3(transform.position.x - 1.4f, transform.position.y, transform.position.z),
            boxSize);
    }

    
    //공격 범위에 플레이어가 들어왔을 때 그것을 감지하고 공격을 시작하는 함수.
    void readyToAttack()
    {
        if (normalCurTime <= 0)
        {
            Collider2D[] collider2Ds;
            if (spriterenderer.flipX)
                collider2Ds = Physics2D.OverlapBoxAll(new Vector2(transform.position.x - 0.8f, transform.position.y), boxSize, 0);
            else
                collider2Ds = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + 0.8f, transform.position.y), boxSize, 0);


            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.gameObject.layer == 7)
                {
                    stopTracking = true;
                    isAttacking = true;
                    CancelInvoke();
                    
                    nextMove = 0;
                    anim.SetInteger("walkSpeed", nextMove);

                    anim.SetTrigger("attack");
                    Invoke("Attack", 0.4f);
                }
            }
        }
        else
        {
            normalCurTime -= Time.deltaTime;
        }
    }

    //실제로 공격을 하는 함수(적 플레이어의 피격)
    void Attack()
    {
        
        Collider2D[] collider2Ds;
        if (spriterenderer.flipX)
            collider2Ds = Physics2D.OverlapBoxAll(new Vector2(transform.position.x - 1.4f, transform.position.y), boxSize, 0);
        else
            collider2Ds = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + 1.4f, transform.position.y), boxSize, 0);
        
        
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Player")
            {
                collider.gameObject.GetComponent<Player>().onDamaged(transform.position);
                PlayerStat._playerStat.Hit(_goblinStat.atk);
            }
        }

        StartCoroutine(managetrackingAndAttacking());
    }
    
    
    //이 코루틴은 몬스터가 플레이어를 공격할 때 몬스터의 다른 함수를 잠깐 정지시켰던 것을 해제시키는 역할. => 공격을 종료하는 역할
    IEnumerator managetrackingAndAttacking()
    {
        yield return new WaitForSeconds(0.5f);
        Think();
        stopTracking = false;
        isAttacking = false;
    }
}

    