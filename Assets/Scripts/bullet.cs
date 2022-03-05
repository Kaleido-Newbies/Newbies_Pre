using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed;
    private SpriteRenderer playerSprite;
    private bool isFlip;
    private BoxCollider2D col;
    private SpriteRenderer spriterenderer;
    

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(realDestroyBullet());
        Invoke("fakeDestroyBullet", 0.7f);
        
        playerSprite = GameObject.Find("Player").GetComponent<SpriteRenderer>();
        isFlip = playerSprite.flipX;
    }

    
    void Update()
    {
        if(!isFlip)
            transform.Translate(transform.right * speed * Time.deltaTime);
        else
            transform.Translate(transform.right * -1 * speed * Time.deltaTime);

    }

    void fakeDestroyBullet()
    {
        col.enabled = false;
        spriterenderer.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {

            
            string monsterName = collision.gameObject.name.Substring(0, 3);
            switch (monsterName)
            {
                case "Sli":
                    collision.gameObject.GetComponent<Enemy>().onDamaged(transform.position);
                    collision.gameObject.GetComponent<Enemy>().beingDamaged = true;
                    StartCoroutine(enemybeingDamagedFalse(collision.gameObject.GetComponent<Enemy>()));
                    break;
                            
                case "Mus":
                    collision.gameObject.GetComponent<Mushroom>().onDamaged(transform.position);
                    collision.gameObject.GetComponent<Mushroom>().beingDamaged = true;
                    StartCoroutine(mushroombeingDamagedFalse(collision.gameObject.GetComponent<Mushroom>()));
                    break;
                
                case "Gob":
                    collision.gameObject.GetComponent<Goblin>().onDamaged(transform.position);
                    collision.gameObject.GetComponent<Goblin>().beingDamaged = true;
                    StartCoroutine(goblinbeingDamagedFalse(collision.gameObject.GetComponent<Goblin>()));
                    break;
                
                case "Gho":
                    collision.gameObject.GetComponent<Ghost>().onDamaged(transform.position);
                    collision.gameObject.GetComponent<Ghost>().beingDamaged = true;
                    StartCoroutine(ghostbeingDamagedFalse(collision.gameObject.GetComponent<Ghost>()));
                    break;
            }
            
            
            
            
            fakeDestroyBullet();
        }
        
        else if(collision.gameObject.tag == "Floor")
            Destroy(gameObject);

    }
    
    IEnumerator enemybeingDamagedFalse(Enemy enemy)
    {
        yield return new WaitForSeconds(0.5f);
        enemy.beingDamaged = false;
    }
    
    IEnumerator mushroombeingDamagedFalse(Mushroom mushroom)
    {
        yield return new WaitForSeconds(0.5f);
        mushroom.beingDamaged = false;
    }
    
    IEnumerator goblinbeingDamagedFalse(Goblin goblin)
    {
        yield return new WaitForSeconds(0.5f);
        goblin.beingDamaged = false;
    }
    
    IEnumerator ghostbeingDamagedFalse(Ghost ghost)
    {
        yield return new WaitForSeconds(0.5f);
        ghost.beingDamaged = false;
    }
    
    
    
    
    
    
    IEnumerator realDestroyBullet()
    {
        yield return new WaitForSeconds(3);
            Destroy(gameObject);
    }
}