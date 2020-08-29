using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("CharacterAttribute")]
    public CharacterEffect characterEffect;
    public Rigidbody2D rb;
    public Animator ani;
    public SpriteRenderer spriteRenderer;
    public Transform leftHand;
    public Transform rightHand;
    public Transform feet;
    public Transform gemPoint;
    public LayerMask whatIsGround;
    public float normalGravity;
    public bool isWallLeft;
    public bool isWallRight;
    public bool isWall;
    public bool isGround;
    public bool isInTheAir;
    public bool isDrop;


    [Header("RunAttribute")]
    public float runSpeed;
    public Vector2 dir;

    [Header("JumpAttribute")]
    public bool isJumping;
    public float jumpForce;
    public float jumpTime;
    public float jumpTimeCounter;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;

    [Header("WallSlideAttribute")]
    public bool isWallSlide;
    public float slideForce;


    [Header("WallJumpAttribute")]
    public bool isWallJump;
    public Vector2 wallJumpDirection;
    public float wallJumpForce;
    public float wallJumpTime;
    public float wallJumpTimeCounter;

    [Header("SomersaultAttribute")]
    public bool canSomersault;
    public bool isSomersault;
    public float somersaultForce;

    [Header("DashAttribute")]
    public bool canDash;
    public bool isDash;
    public float dashForce;
    public float dashTime;
    public float dashTimeCounter;

    [Header("RollAttribute")]
    public bool canRoll;
    public bool isRoll;
    public float rollForce;
    public float rollTime;
    public float rollTimeCounter;

    // Start is called before the first frame update
    void Start()
    {
        rb.gravityScale = normalGravity;
    }

    void Update()
    {
        //dir = new Vector2(1, 1);
        float xIndex = Input.GetAxisRaw("Horizontal");
        float yIndex = Input.GetAxisRaw("Vertical");
        dir = new Vector2(xIndex, yIndex);

        CheckStatus();
        Flip();
        Jump();
        WallJump();
        Somersault();
        WallSlide();




        Run();
        Roll();
        Dash8();

    }
    public void CheckStatus()
    {
        isGround = Physics2D.OverlapCircle(feet.position, 0.1f, whatIsGround);
        isWallLeft = Physics2D.OverlapCircle(leftHand.position, 0.1f, whatIsGround);
        isWallRight = Physics2D.OverlapCircle(rightHand.position, 0.1f, whatIsGround);
        isWall = (isWallLeft || isWallRight) && !isGround;
        isInTheAir = !isWall && !isGround;

        //ani
        ani.SetBool("isGround", isGround);
        ani.SetBool("isWall", isWall);
        ani.SetBool("isInTheAir", isInTheAir);
        ani.SetFloat("velocity.x", rb.velocity.x);
        ani.SetFloat("velocity.y", rb.velocity.y);
    }
    public void Flip()
    {
        if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false;

        }
        else if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;

        }
    }
    public void Run()
    {
        if (!isWallJump && !isDash && !isRoll)
        {
            if (isWallLeft && dir.x < 0 && !isGround)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else if (isWallRight && dir.x > 0 && !isGround)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(dir.x * runSpeed, rb.velocity.y);

                //ani
                if (dir.x != 0)
                {
                    ani.SetBool("isRun", true);
                }
                else
                {
                    ani.SetBool("isRun", false);
                }
            }
        }
    }
    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.J) && isGround && !isDash)
        {
            rb.gravityScale = normalGravity;
            isJumping = true;
            jumpTimeCounter = jumpTime;

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = Vector2.up * jumpForce;

            //ani
            //ani.SetBool("isJump", true);
            ani.SetTrigger("jump");
            characterEffect.JumpEffect();

        }
        if (Input.GetKey(KeyCode.J) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;

                //ani
                //ani.SetBool("isJump", false);
            }
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            isJumping = false;

            //ani
            //ani.SetBool("isJump", false);
        }
        if (isInTheAir)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.J))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

    }
    public void WallSlide()
    {
        if (isWall && !isWallJump && rb.velocity.y < 0)
        {
            isWallSlide = true;
            rb.gravityScale = 0.1f;
            rb.velocity = Vector2.down * slideForce;
        }
        else if (!isDash)
        {
            isWallSlide = false;
            rb.gravityScale = normalGravity;
        }

    }
    public void WallJump()
    {
        if (isWall && Input.GetKeyDown(KeyCode.J) && !isWallJump)
        {
            isWallJump = true;
            wallJumpTimeCounter = wallJumpTime;
            rb.gravityScale = normalGravity;
            Vector2 fixDirection = new Vector2(wallJumpDirection.x * (isWallLeft ? 1 : -1) , wallJumpDirection.y );
            rb.velocity = fixDirection.normalized * wallJumpForce;

            //ani
            //ani.SetBool("isWallJump", true);
            ani.SetTrigger("wallJump");
            characterEffect.JumpEffect();

        }
        else if (wallJumpTimeCounter >= 0)
        {
            wallJumpTimeCounter -= Time.deltaTime;
        }
        else if (wallJumpTimeCounter < 0)
        {
            isWallJump = false;

            //ani
            //ani.SetBool("isWallJump", false);
        }

    }
    public void Somersault()
    {
        if (isGround || isWall)
        {
            canSomersault = true;
            isSomersault = false;

            //ani
            //ani.SetBool("isSomersault", false);


        }
        if (isInTheAir && Input.GetKeyDown(KeyCode.J) && canSomersault)
        {
            isSomersault = true;
            canSomersault = false;

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = Vector2.up * somersaultForce;

            //ani
            //ani.SetBool("isSomersault", true);
            ani.SetTrigger("somersault");
            characterEffect.JumpEffect();
        }
    }
    public void Dash8() // xem xet viec co nen can bang vector dash ve 1 khi dash ko
    {
        if (canDash && Input.GetKeyDown(KeyCode.K) && dir != Vector2.zero && !(isGround && dir.y < 0) && !isRoll )
        {
            canDash = false;
            isDash = true;
            rb.gravityScale = 0;

            dashTimeCounter = dashTime;
            rb.velocity = Vector2.zero;
            rb.velocity = dir.normalized * dashForce;

            //ani
            if (dir.x == 0 && dir.y == 1)
            {
                ani.SetTrigger("dashUp");
            }
            else if (dir.x == 0 && dir.y == -1)
            {
                ani.SetTrigger("dashDown");
            }
            else
            {
                ani.SetTrigger("dash");
            }
            StartCoroutine(characterEffect.DashEffect(dashTime)); // nen xem lai thoi gian ani dash
        }
        else if (dashTimeCounter >= 0)
        {
            dashTimeCounter -= Time.deltaTime;
            if (isWallLeft || isWallRight)
            {
                rb.velocity = Vector2.zero;
                isDash = false;
                rb.gravityScale = normalGravity;
                dashTimeCounter = -1;

                //ani
                ani.SetTrigger("endDash");
            }
        }
        else if (dashTimeCounter < 0)
        {
            if (isDash)
            {
                rb.velocity = Vector2.zero;
                isDash = false;

                rb.gravityScale = normalGravity;

                //ani
                ani.SetTrigger("endDash");
            }            
            if (isWall || isGround)
            {
                canDash = true;
            }
        }

    }
    public void Roll()
    {
        if (canRoll && isGround && Input.GetKeyDown(KeyCode.L) && !isDash)
        {
            isRoll = true;
            canRoll = false;

            rollTimeCounter = rollTime;
            rb.velocity = Vector2.zero;
            rb.velocity = Vector2.left * (spriteRenderer.flipX ? 1 : -1) * rollForce;

            //ani
            ani.SetTrigger("roll");
            StartCoroutine(characterEffect.RollEffect(rollTime));

        }
        else if (rollTimeCounter >= 0)
        {
            rollTimeCounter -= Time.deltaTime;
            if (isWallLeft || isWallRight || isJumping)
            {
                isRoll = false;
                rollTimeCounter = -1;


            }
        }
        else if (rollTimeCounter < 0)
        {
            if (isRoll)
            {
                isRoll = false;

            }
            if (isGround)
            {
                canRoll = true;
            }
        }

    }
    int count = 0;
    public void debugCount()
    {
        Debug.Log(count);
        count++;
    }
}
