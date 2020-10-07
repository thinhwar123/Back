using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator ani;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public EnemyStatus enemyStatus;
    [SerializeField] private GameObject character;
    [SerializeField] private float rangeAttack;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackSpeedCounter;
    [SerializeField] private float speedWalk;
    public Vector2 dir;
    [Header("demoAttribute")]
    public float rangeFindCharacter;
    public LayerMask whatIsPlayer;

    public void Start()
    {
        enemyStatus = EnemyStatus.idle;
    }
    public void Flip()
    {
        if (enemyStatus != EnemyStatus.beControll)
        {
            Vector2 tempDir= Vector2.zero;
            if (character != null)
            {
                tempDir = (character.transform.position - transform.position);
            }
            if (tempDir.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (tempDir.x < 0)
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {

            if (dir.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (dir.x < 0)
            {
                spriteRenderer.flipX = false;
            }
        }


    }
    public void Update()
    {
        Flip();
        Idle();
        Walk();
        Attack();
        Hurt();
        Death();
        BeControll();
    }
    public void Idle()
    {
        if (character != null)
        {
            if (character.GetComponentInParent<CharacterMovement>().isJoin && enemyStatus != EnemyStatus.beControll)
            {
                enemyStatus = EnemyStatus.idle;
                ani.SetBool("isWalk", false);
            }
            else if (character.GetComponentInParent<CharacterMovement>().isJoin && enemyStatus == EnemyStatus.beControll)
            {

            }
            else
            {
                enemyStatus = EnemyStatus.walk;
            }
        }

    }
    public void Walk()
    {
        if (enemyStatus == EnemyStatus.walk)
        {
            if (character != null)
            {

                if ((character.transform.position - transform.position).magnitude > rangeAttack)
                {
                    enemyStatus = EnemyStatus.walk;
                    ani.SetBool("isWalk", true);
                    rb.velocity = new Vector2((character.transform.position - transform.position).normalized.x, 0) * speedWalk;
                }
                else
                {
                    Debug.Log((character.transform.position - transform.position).magnitude);
                    ani.SetBool("isWalk", false);
                    enemyStatus = EnemyStatus.attack;
                    
                }
            }
        }
    }
    public void Attack()
    {
        if (enemyStatus == EnemyStatus.attack && attackSpeedCounter < 0)
        {
            ani.SetTrigger("attack");
            attackSpeedCounter = attackSpeed;
        }
        else if(enemyStatus == EnemyStatus.attack)
        {
            enemyStatus = EnemyStatus.walk;
            attackSpeedCounter -= Time.deltaTime;
        }
    }
    public void Death()
    {

    }
    public void Hurt()
    {

    }
    public void BeControll()
    {
        if (enemyStatus == EnemyStatus.beControll)
        {
            float xIndex = Input.GetAxisRaw("Horizontal");
            float yIndex = Input.GetAxisRaw("Vertical");
            dir = new Vector2(xIndex, yIndex);
            rb.velocity = new Vector2(xIndex * speedWalk, rb.velocity.y);

            //ani
            if (xIndex !=0)
            {
                ani.SetBool("isWalk", true);
            }
            else
            {
                ani.SetBool("isWalk", false);
            }
            
        }
    }
    public IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(1);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger && enemyStatus == EnemyStatus.idle)
        {
            character = collision.transform.gameObject;
            enemyStatus = EnemyStatus.walk;
        }
    }
}
public enum EnemyStatus
{
    idle,
    death,
    attack,
    walk,
    hurt,
    beControll
}