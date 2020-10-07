using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;

public class Gem : MonoBehaviour
{
    [SerializeField] private GameObject gemPoint;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public bool isLightUp;
    [SerializeField] private GameObject gemLight;
    [SerializeField] private float speedChangeLight;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float accelerator;
    [SerializeField] private float speedRotate;
    [SerializeField] private float safeRange;
    [SerializeField] private GameObject explosionObject;
    [SerializeField] private LayerMask whatIsWeakWall;
    [SerializeField] private LayerMask whatIsRock;

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
        RaycastHit2D[] hitWeakWall = Physics2D.CircleCastAll(transform.position, 3f, Vector2.zero, 0, whatIsWeakWall);
        if (hitWeakWall.Length != 0)
        {
            for (int i = 0; i < hitWeakWall.Length; i++)
            {
                hitWeakWall[i].transform.GetComponent<WeakWall>().Explosion();
            }
        }
        RaycastHit2D[] hitRock = Physics2D.CircleCastAll(transform.position, 3f, Vector2.zero, 0, whatIsRock);
        if (hitRock.Length != 0)
        {
            for (int i = 0; i < hitRock.Length; i++)
            {
                hitRock[i].transform.GetComponent<Rock>().Slide(new Vector2(hitRock[i].transform.position.x - transform.position.x, 0).normalized);
            }
        }
        //ani
        GameObject tempObject = Instantiate(explosionObject, transform.position, Quaternion.identity);
        tempObject.GetComponent<Animator>().SetTrigger("fire");
        Destroy(tempObject, 2);
    }
}