using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Rock : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool isGround;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform checkGround;
    [SerializeField] private Transform checkHitBox;
    //[SerializeField] private Vector2 dir;
    //[SerializeField] private bool isSlide;
    [SerializeField] private float slideForce;
    [SerializeField] private float gravity;
    [SerializeField] private float timeSlide;
    [SerializeField] private Tween tweenX;
    [SerializeField] private Tween tweenY;
    [SerializeField] private float timeSlideCounter;
    //public void Update()
    //{
    //    if (isSlide && timeSlideCounter > 0)
    //    {
    //        rb.velocity = new Vector2(slideForce * dir.x, rb.velocity.y);
    //        timeSlideCounter -= Time.deltaTime;
    //    }
    //}
    public void Update()
    {
        isGround = Physics2D.OverlapBox(checkGround.position, new Vector2(2, 0.1f), 0, whatIsGround);
        if (!isGround)
        {
            tweenX.Kill();
            Drop();
        }
        else if(isGround)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;            
        }
    }
    //public void Slide2(Vector2 direction)
    //{
    //    isSlide = true;
    //    timeSlideCounter = timeSlide;
    //    dir = direction;
    //}
    public void Slide(Vector2 direction)
    {
        //rb.bodyType = RigidbodyType2D.Dynamic;
        tweenX = transform.DOMoveX(transform.position.x + direction.x * slideForce, timeSlide).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }
    public void Drop()
    {
        //tweenY = transform.DOMoveY(transform.position.y - 1 * gravity, timeSlide).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
