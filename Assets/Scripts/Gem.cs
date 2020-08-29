using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Gem : MonoBehaviour
{
    public GameObject mainCharacter;
    public SpriteRenderer sprite;
    public TypeMoveGem typeMoveGem;
    public Rigidbody2D rb;
    public float movementSpeed;
    public float accelerator;

    public bool fixDirection;
    public float speedRotate;
    public float safeRange;

    public void Start()
    {
        
    }
    public void Update()
    {
        Split();

        GemMovement();
    }
    public void Split()
    {
        transform.eulerAngles += new Vector3(0, 0, speedRotate * Time.deltaTime) ;
    }
    public void GemMovement()
    {
        Vector2 directionMove = (mainCharacter.transform.position - transform.position);
        Vector2 randomVector = new Vector2(0.2f,0.2f);
        
        if (directionMove.magnitude > safeRange)
        {
            float fixRange = directionMove.magnitude < safeRange ? 0 : directionMove.magnitude - safeRange;
            rb.velocity = (directionMove.normalized) * (movementSpeed + accelerator * fixRange);
            fixDirection = true;
        }
        else if(fixDirection)
        {
            fixDirection = false;
        }

    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sprite.sortingOrder = sprite.sortingOrder == 1 ? -1 : 1;
        }
    }
}
public enum TypeMoveGem{
    follow,
    around,
    go
}