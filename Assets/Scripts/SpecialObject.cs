using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    [SerializeField] private Enemy enemyCharacter;
    [SerializeField] private bool isEnemy;
    [SerializeField] private Animator ani;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool isAim;
    [SerializeField] private GameObject specialObjectColl;
    [SerializeField] private GameObject crossHair;
    [SerializeField] private GameObject tempCrossHair;
    public void BeControlled()
    {
        StopTarget();
        specialObjectColl.GetComponent<CircleCollider2D>().enabled = false;
        if (isEnemy)
        {
            enemyCharacter.enemyStatus = EnemyStatus.beControll;
        }
        
    }
    public void BeFree()
    {

        specialObjectColl.GetComponent<CircleCollider2D>().enabled = true;
        if (isEnemy)
        {
            enemyCharacter.enemyStatus = EnemyStatus.walk;
        }
    }
    public void Frezzing(bool isFrezz)
    {
        if (isEnemy)
        {
            if (isFrezz)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        else
        {

            if (isFrezz)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                Debug.Log("dai bang");
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

    }
    public void StartTarget()
    {
        if (!isAim)
        {
            isAim = true;
            tempCrossHair = Instantiate(crossHair, transform.position, Quaternion.identity, transform);
        }       
    }
    public void StopTarget()
    {
        if (isAim)
        {
            isAim = false;
            if (tempCrossHair != null)
            {
                tempCrossHair.GetComponent<CrossHair>().DestroyAim();
            }
        }

    }
    public void Explosion()
    {
        ani.SetTrigger("explosion");
    }
    //public void DestroyObject()
    //{
    //    Destroy(gameObject);
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && !isEnemy)
        {
            SimplePool.Despawn(gameObject);
        }
    }
}
