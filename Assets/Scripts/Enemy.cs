using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove; 
    


    void Awake()
    {
        
        rigid = GetComponent<Rigidbody2D>();
        Think();
    }

    
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove * 2, rigid.velocity.y);


        Vector2 fontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(fontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(fontVec, Vector2.down, 1, LayerMask.GetMask("Floor"));
        if (rayHit.collider == null)
        {
            nextMove *= -1;
            CancelInvoke();
            Invoke("Think", Random.Range(1f, 4f));

        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);
        float nextThinkTime = Random.Range(2f, 5f);



        
        Invoke("Think", nextThinkTime);
    }
}
