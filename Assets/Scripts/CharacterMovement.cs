using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
    public Gem gemObject;
    public Transform gemPoint;
    public Vector2 gemPointPosition;
    public LayerMask whatIsGround;
    public float normalGravity;
    public bool isWallLeft;
    public bool isWallRight;
    public bool isWall;
    public bool isGround;
    public bool isInTheAir;


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
    public bool tempCanSomersault;

    public int dashStackMax;
    public int dashStack;
    public float dashCountdown;
    public float dashCountdownTime;

    [Header("RollAttribute")]
    public bool canRoll;
    public bool isRoll;
    public float rollForce;
    public float rollTime;
    public float rollTimeCounter;

    [Header("AttackAttribute")]
    public bool canAttack;
    public bool isAttack;
    public float attatckTime;
    public float attackTimeCounter;

    [Header("AttackAttribute")]
    public bool isHeal;

    [Header("ChangeFormAttribute")]
    public bool isLight;
    public RuntimeAnimatorController LightAni;
    public RuntimeAnimatorController DarkAni;
    public bool isTranform;
    public bool isFastTranform;
    public float timeTranform;

    public bool canTranform;
    public float tranformCountdown;
    public float tranformCountdownTime;

    // Start is called before the first frame update
    void Start()
    {
        rb.gravityScale = normalGravity;
        dashStack = dashStackMax;
    }

    void Update()
    {
        //dir = new Vector2(1, 1);
        float xIndex = Input.GetAxisRaw("Horizontal");
        float yIndex = Input.GetAxisRaw("Vertical");
        dir = new Vector2(xIndex, yIndex);
        
        CheckStatus();//check trang thai nhan vat

        DashCountdown();
        TranformCountdown();
        Attack();

        Flip();
        Jump();
        WallJump();
        Somersault();
        WallSlide();




        Run();
        Roll();
        Dash8();
        //Dash2();

        Heal();
        GemLightSwitch();
        StartCoroutine(ChangeForm1());
    }
    public void CheckStatus()
    {
        isGround = Physics2D.OverlapBox(feet.position,new Vector2(0.5f, 0.05f), 0, whatIsGround);
        isWallLeft = Physics2D.OverlapBox(leftHand.position, new Vector2(0.05f, 0.6f), 0, whatIsGround);
        isWallRight = Physics2D.OverlapBox(rightHand.position, new Vector2(0.05f, 0.6f), 0, whatIsGround);
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
        bool tempFlipX = spriteRenderer.flipX;
        if (!isWall )
        {
            if (rb.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (rb.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = tempFlipX;
            }
        }
        else if(isWall )
        {
            if (isWallLeft && rb.velocity.y < 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (isWallRight && rb.velocity.y < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = tempFlipX;
            }
        }
        FixGemPointPosition();
    }
    public void Run()
    {
        if (!isWallJump && !isDash && !isRoll && !isTranform && !isHeal)
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
        if (Input.GetKeyDown(KeyCode.J) && isGround && !isDash && !isTranform && !isHeal)
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
        if ((isGround || isWall) && !isDash)
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
        if (canDash && dashStack !=0 && Input.GetKeyDown(KeyCode.K) && !(isGround && dir.y < 0) && !isRoll && !isTranform && !isHeal && !((isWallLeft && dir.x == -1) || (isWallRight && dir.x == 1)))// co the dash + an nut + dung duoi dat dash xuong duoi + bam tuong dash vao tuong + ko roll
        {
            canDash = false;
            isDash = true;
            isJumping = false;

            tempCanSomersault = canSomersault;
            canSomersault = false;
            rb.gravityScale = 0;

            dashTimeCounter = dashTime;
            

            if (dir == Vector2.zero && !(isWallLeft && spriteRenderer.flipX) && !(isWallRight && !spriteRenderer.flipX))
            {
                rb.velocity = Vector2.zero;
                dashStack--;
                rb.velocity = new Vector2((spriteRenderer.flipX ? -1 : 1), dir.y).normalized * dashForce;
            }
            else if (dir.x != 0 || (!isWall && dir.y != 0))
            {
                rb.velocity = Vector2.zero;
                dashStack--;
                rb.velocity = dir.normalized * dashForce;
            }
            else if(isWall && dir.y > 0)
            {
                rb.velocity = Vector2.zero;
                dashStack--;
                rb.velocity = new Vector2((spriteRenderer.flipX ? -1 : 1), dir.y).normalized * dashForce;
            }
            else
            {
                canDash = true;
                isDash = false;

                canSomersault = tempCanSomersault;
                rb.gravityScale = normalGravity;

                dashTimeCounter = -1;             
            }

            //ani
            if (dir.x == 0 && dir.y == 1 && isDash)
            {
                ani.SetTrigger("dashUp");
                StartCoroutine(characterEffect.DashEffect(dashTime));
            }
            else if (dir.x == 0 && dir.y == -1 && isDash)
            {
                ani.SetTrigger("dashDown");
                StartCoroutine(characterEffect.DashEffect(dashTime));
            }
            else if(isDash)
            {
                ani.SetTrigger("dash");
                StartCoroutine(characterEffect.DashEffect(dashTime));
            }
        }
        else if (dashTimeCounter >= 0)
        {
            dashTimeCounter -= Time.deltaTime;
            if ((isWallLeft && rb.velocity.x < 0) || (isWallRight && rb.velocity.x > 0)) // ngat dash khi vao tuong
            {
                canSomersault = tempCanSomersault;

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
                canSomersault = tempCanSomersault;

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
    public void Dash2()// xem xet viec dash 2 huong
    {
        if (canDash && dashStack != 0 && Input.GetKeyDown(KeyCode.K) && !isRoll && !isTranform && !isHeal && !((isWallLeft && dir.x == -1) || (isWallRight && dir.x == 1)))// co the dash + an nut + dung duoi dat dash xuong duoi + bam tuong dash vao tuong + ko roll
        {
            canDash = false;
            isDash = true;
            isJumping = false;

            tempCanSomersault = canSomersault;
            canSomersault = false;
            rb.gravityScale = 0;

            dashTimeCounter = dashTime;

            if (dir.x == 0 && !(isWallLeft && spriteRenderer.flipX) && !(isWallRight && !spriteRenderer.flipX) )
            {
                rb.velocity = Vector2.zero;
                dashStack--;
                rb.velocity = new Vector2((spriteRenderer.flipX ? -1 : 1), 0).normalized * dashForce;
            }
            else if (dir.x != 0 || (!isWall && dir.y != 0))
            {
                rb.velocity = Vector2.zero;
                dashStack--;
                rb.velocity = new Vector2(dir.x, 0).normalized * dashForce;
            }
            else if (isWall && dir.y > 0)
            {
                rb.velocity = Vector2.zero;
                dashStack--;
                rb.velocity = new Vector2((spriteRenderer.flipX ? -1 : 1), 0).normalized * dashForce;
            }
            else
            {
                canDash = true;
                isDash = false;

                canSomersault = tempCanSomersault;
                rb.gravityScale = normalGravity;

                dashTimeCounter = -1;
            }

            //ani
            if (isDash)
            {
                ani.SetTrigger("dash");
                StartCoroutine(characterEffect.DashEffect(dashTime));
            }
        }
        else if (dashTimeCounter >= 0)
        {
            dashTimeCounter -= Time.deltaTime;
            if ((isWallLeft && rb.velocity.x < 0) || (isWallRight && rb.velocity.x > 0)) // ngat dash khi vao tuong
            {
                canSomersault = tempCanSomersault;

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
                canSomersault = tempCanSomersault;

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
        if (canRoll && isGround && Input.GetKeyDown(KeyCode.L) && !isDash && !isTranform && !isHeal && !(isWallLeft && dir.x == -1) && !(isWallRight && dir.x == 1) && !(isWallLeft && spriteRenderer.flipX) && !(isWallRight && !spriteRenderer.flipX))
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
    public void Attack()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isDash && !isRoll && !isWallSlide && !isWallJump && !isHeal && !isAttack && canAttack)
        {
            canAttack = false;
            isAttack = true;
            attackTimeCounter = attatckTime;
            //ani

            ani.SetTrigger("attack");
        }
        else if (attackTimeCounter >= 0)
        {
            attackTimeCounter -= Time.deltaTime;
            if (isWall)
            {
                canAttack = true;
                isAttack = false;
                attackTimeCounter = -1;
            }
            
        }
        else
        {
            canAttack = true;
            isAttack = false;

        }
    }
    public void Heal()
    {
        if (Input.GetKeyDown(KeyCode.O) && isGround && dir == Vector2.zero)
        {
            isHeal = true;
            characterEffect.HealingEffect(true);
        }
        if(Input.GetKeyUp(KeyCode.O) && isHeal)
        {
            isHeal = false;
            characterEffect.HealingEffect(false);
        }
    }
    public void GemLightSwitch()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            gemObject.isLightUp = !gemObject.isLightUp;
        }
    }
    public IEnumerator ChangeForm1()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isFastTranform && !isTranform && isGround)
        {
            rb.velocity = Vector2.zero;
            isTranform = true;
            isLight = !isLight;
            //ani
            ani.SetTrigger("changeForm");
            characterEffect.ChangeFormEffect();

            yield return new WaitForSeconds(timeTranform);
            isTranform = false;
            ani.runtimeAnimatorController = isLight ? DarkAni : LightAni;
        }
    }
    public void DashCountdown()
    {
        if (dashStack < dashStackMax)
        {
            if (dashCountdownTime > 0)
            {
                dashCountdownTime -= Time.deltaTime;
            }
            else
            {
                dashCountdownTime = dashCountdown;
                dashStack++;
            }
        }
    }
    public void TranformCountdown()
    {
        if (!canTranform)
        {
            if (tranformCountdownTime > 0)
            {
                tranformCountdownTime -= Time.deltaTime;
            }
            else
            {
                tranformCountdownTime = tranformCountdown;
                canTranform = true;
            }
        }
    }
    public void FixGemPointPosition()
    {
        gemPoint.localPosition =  new Vector3((spriteRenderer.flipX ? -1 : 1) * gemPointPosition.x, gemPointPosition.y, 0);
    }
    int count = 0;
    public void debugCount()
    {
        Debug.Log(count);
        count++;
    }
}
