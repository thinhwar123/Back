using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;

public class Gem : MonoBehaviour
{
    public GameObject gemPoint;
    public SpriteRenderer sprite;
    public Rigidbody2D rb;
    public bool isLightUp;
    public GameObject gemLight;
    public float speedChangeLight;
    public float movementSpeed;
    public float accelerator;
    public float speedRotate;
    public float safeRange;
    public GameObject explosionObject;
    public LayerMask whatIsWeakWall;

    public void Start()
    {
    }
    public void Update()
    {
        LightUp();
        Split();

        GemMovement();
    }
    public void Split()
    {
        transform.eulerAngles += new Vector3(0, 0, speedRotate * Time.deltaTime) ;
    }
    public void GemMovement()
    {
        Vector2 directionMove = (gemPoint.transform.position - transform.position);
        
        if (directionMove.magnitude > safeRange)
        {
            float fixRange = directionMove.magnitude < safeRange ? 0 : directionMove.magnitude - safeRange;
            rb.velocity = (directionMove.normalized) * (movementSpeed + accelerator * fixRange);
        }

    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sprite.sortingOrder = sprite.sortingOrder == 1 ? -1 : 1;
        }
    }
    public void LightUp()
    {
        if (isLightUp )
        {
            gemLight.SetActive(true);
        }
        else if(!isLightUp )
        {
            gemLight.SetActive(false);
        }
    }
    public void Explosion()
    {
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, 2f, Vector2.zero, 0, whatIsWeakWall);
        Debug.Log(hit.Length);
        if (hit.Length != 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                hit[i].transform.GetComponent<WeakWall>().Explosion();
            }
        }
        //ani
        GameObject tempObject = Instantiate(explosionObject, transform.position, Quaternion.identity);
        tempObject.GetComponent<Animator>().SetTrigger("fire");
        Destroy(tempObject, 2);
    }
}