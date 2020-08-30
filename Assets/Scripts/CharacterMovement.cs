using Cinemachine.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


public class CharacterMovement : MonoBehaviour
{
    [Header("CharacterAttribute")]
    public CharacterEffect characterEffect;
    public Collider2D characterCollider;
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
    public LayerMask whatIsSpecialObject;
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
    //public bool canDash;
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

    [Header("SpecialDashAttribute")]
    public bool isInSpecialDash;
    public bool isAim;
    public bool canSpecialDash;
    public bool isSpecialDash;
    public bool isJoin;
    public float specialDashForce;
    public float specialDashRange;
    public RaycastHit2D[] listObject;
    public int indexListObject;
    public float timeSwitchObject; 
    public float timeSwitchObjectCounter;
    public GameObject tempCrossHair;
    public GameObject tempSpecialObject;
    public GameObject crossHair;

    [Header("AttackAttribute")]
    public bool canAttack;
    public bool isAttack;
    public float attatckTime;
    public float attackTimeCounter;

    [Header("HealAndSkillAttribute")]
    public bool isHeal;
    public bool isSkill;
    public GameObject bulletObject;
    public float bulletSpeed;


    public float minimumHeldDuration = 0.25f;
    public float pressTime;
    public bool isHeldKey;



    [Header("TranformAttribute")]
    public bool isLight;
    public RuntimeAnimatorController LightAni;
    public RuntimeAnimatorController DarkAni;
    public bool isTranform;
    //public bool isFastTranform;
    //public float timeTranform;

    public bool canNormalTranform;
    public float normalTranformCountdown;
    public float normalTranformCountdownTime;

    public bool canQuickTranform;
    public float quickTranformCountdown;
    public float quickTranformCountdownTime;

    // Start is called before the first frame update
    void Start()
    {
        rb.gravityScale = normalGravity;
        dashStack = dashStackMax;
        listObject = new RaycastHit2D[0];
        EndTranform();
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

        Skill();
        Heal();

        SpecialDash();
        GemLightSwitch();
        NormalTranform();
        QuickTranform();

    }
    public void CheckStatus()
    {
        if (!isInSpecialDash)
        {
            isGround = Physics2D.OverlapBox(feet.position, new Vector2(0.1f, 0.05f), 0, whatIsGround);
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


    }
    public void Flip()
    {
        bool tempFlipX = spriteRenderer.flipX;
        if (!isWall && !isAim)
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
        else if(isWall && !isAim)
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
        else if (isAim)
        {
            if (dir.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (dir.x < 0)
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
        if (!isWallJump && !isDash && !isRoll && !isTranform && !isHeal && !isInSpecialDash)
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
        if (Input.GetKeyDown(KeyCode.J) && isGround && !isDash && !isTranform && !isHeal && !isInSpecialDash)
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
        if (isInTheAir && !isInSpecialDash)
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
        if (isWall && !isWallJump && rb.velocity.y < 0 )
        {
            isWallSlide = true;
            rb.gravityScale = 0.1f;
            rb.velocity = Vector2.down * slideForce;
        }
        else if (!isDash && !isInSpecialDash)
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
        if (/*canDash && */dashStack !=0 && Input.GetKeyDown(KeyCode.K) && !(isGround && dir.y < 0) && !isRoll && !isTranform && !isHeal && !isInSpecialDash && !((isWallLeft && dir.x == -1) || (isWallRight && dir.x == 1)))// co the dash + an nut + dung duoi dat dash xuong duoi + bam tuong dash vao tuong + ko roll
        {
            //canDash = false;
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
                //canDash = true;
                isDash = false;

                canSomersault = tempCanSomersault;
                rb.gravityScale = normalGravity;

                dashTimeCounter = -1;             
            }

            //ani
            if (dir.x == 0 && dir.y == 1 && isDash)
            {
                ani.SetTrigger("dashUp");
                StartCoroutine(characterEffect.DashEffect());
            }
            else if (dir.x == 0 && dir.y == -1 && isDash)
            {
                ani.SetTrigger("dashDown");
                StartCoroutine(characterEffect.DashEffect());
            }
            else if(isDash)
            {
                ani.SetTrigger("dash");
                StartCoroutine(characterEffect.DashEffect());
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
                ////canDash = true;
            }
        }
    }
    public void Dash2()// xem xet viec dash 2 huong
    {
        if (/*canDash && */dashStack != 0 && Input.GetKeyDown(KeyCode.K) && !isRoll && !isTranform && !isHeal && !isInSpecialDash &&  !((isWallLeft && dir.x == -1) || (isWallRight && dir.x == 1)))// co the dash + an nut + dung duoi dat dash xuong duoi + bam tuong dash vao tuong + ko roll
        {
            //canDash = false;
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
                //canDash = true;
                isDash = false;

                canSomersault = tempCanSomersault;
                rb.gravityScale = normalGravity;

                dashTimeCounter = -1;
            }

            //ani
            if (isDash)
            {
                ani.SetTrigger("dash");
                StartCoroutine(characterEffect.DashEffect());
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
                //canDash = true;
            }
        }
    }
    public void SpecialDash()
    {
        if (Input.GetKeyDown(KeyCode.N) && isGround && dir == Vector2.zero && !isInSpecialDash)                                                 // target
        {
            isInSpecialDash = true;
            isAim = true;
            indexListObject = 0;
            listObject = Physics2D.CircleCastAll((Vector2)transform.position, specialDashRange, Vector2.zero, 0, whatIsSpecialObject);
            if (listObject.Length != 0)
            {
                canSpecialDash = true;
                tempSpecialObject = listObject[indexListObject].transform.gameObject;
                tempCrossHair = Instantiate(crossHair, listObject[indexListObject].transform.position, Quaternion.identity);
            }
        }
        if (isAim && listObject.Length > 1 && dir.x != 0 && timeSwitchObjectCounter < 0)                                                                                     // doi vat the (nen xem xet co can thiet hay ko);
        {
            timeSwitchObjectCounter = timeSwitchObject;
            indexListObject = (listObject.Length + (int)dir.x + indexListObject) % listObject.Length;
            tempSpecialObject = listObject[indexListObject].transform.gameObject;
            tempCrossHair.transform.position = tempSpecialObject.transform.position;
        }
        else if(isAim)
        {
            timeSwitchObjectCounter -= Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.N) && isInSpecialDash && isAim)                                                                                  // bo target
        {
            isInSpecialDash = false;
            listObject = new RaycastHit2D[0];
            if (canSpecialDash)
            {
                tempCrossHair.GetComponent<CrossHair>().DestroyAim();
                canSpecialDash = false;
            }            

        }
        if (Input.GetKeyDown(KeyCode.M) && canSpecialDash)                                                                                      // dash vao
        {
            isAim = false;
            canSpecialDash = false;
            //lao toi vat the va nhap vao do 
            
            isSpecialDash = true;
            rb.gravityScale = 0;
            characterCollider.isTrigger = true;
            //ani
            ani.SetTrigger("dash");
            StartCoroutine(characterEffect.SpecialDashEffect());
        }
        if (isSpecialDash && (transform.position - tempSpecialObject.transform.position).magnitude > 0.5f)
        {
            rb.velocity = (tempSpecialObject.transform.position - transform.position).normalized * specialDashForce;
            
        }
        else if(isSpecialDash) // nhap vao
        {
            ani.SetTrigger("endDash");
            rb.velocity = Vector2.zero;
            isSpecialDash = false;
            isJoin = true;
            spriteRenderer.color = Color.clear;
            gameObject.transform.SetParent(tempSpecialObject.transform);
            tempCrossHair.GetComponent<CrossHair>().DestroyAim();
            
        }
        if (Input.GetKeyDown(KeyCode.M) && isJoin)                                                                              // thoat ra
        {
            isJoin = false;
            isInSpecialDash = false;
            listObject = new RaycastHit2D[0];
            characterCollider.isTrigger = false;

            spriteRenderer.color = Color.white;
            gameObject.transform.SetParent(null);
            tempSpecialObject.GetComponent<SpecialObject>().Explosion();                                                            // pha huy object
            tempCrossHair = null;
            tempSpecialObject = null;
            if (dir != Vector2.zero)
            {
                                                                                                         // su dung tham so cua dash de khien nhan vat dash ra ngoai
                isDash = true;
                isJumping = false;

                tempCanSomersault = canSomersault;
                canSomersault = false;
                rb.gravityScale = 0;

                dashTimeCounter = dashTime;

                rb.velocity = dir.normalized * dashForce;

                //ani
                ani.SetTrigger("dash");
                StartCoroutine(characterEffect.DashEffect());
                                                                                                        // ket thuc tham so dash (nen xem lai dash de hieu tai sao)
            }
            else
            {
                rb.gravityScale = normalGravity;
            }
            //thoat ra khoi do
        }
    }
    public void Roll()
    {
        if (canRoll && isGround && Input.GetKeyDown(KeyCode.L) && !isDash && !isTranform && !isHeal && !isInSpecialDash && !(isWallLeft && dir.x == -1) && !(isWallRight && dir.x == 1) && !(isWallLeft && spriteRenderer.flipX) && !(isWallRight && !spriteRenderer.flipX))
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
        if (Input.GetKeyDown(KeyCode.I) && !isDash && !isRoll && !isWallSlide && !isWallJump && !isHeal && !isAttack && canAttack && !isInSpecialDash)
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
        if (Input.GetKey(KeyCode.O) && isGround && dir == Vector2.zero && !isInSpecialDash)
        {
            if (Time.timeSinceLevelLoad - pressTime > minimumHeldDuration && !isHeal)
            {
                isHeal = true;
                ani.SetTrigger("isHeal");
                //ani
                characterEffect.HealingEffect(true);

                isHeldKey = true;
            }
        }
    }
    public void Skill()
    {
        if (Input.GetKeyDown(KeyCode.O) && !isInSpecialDash)
        {
            pressTime = Time.timeSinceLevelLoad;
            isHeldKey = false;
        }
        else if (Input.GetKeyUp(KeyCode.O) && !isInSpecialDash)
        {
            if (!isHeldKey)
            {
                Debug.Log("skill");// cast skill
                if (isLight)
                {
                    gemObject.Explosion();
                }
                else if(!isLight && isGround && dir == Vector2.zero)
                {
                    //ani
                    ani.SetTrigger("fire");
                    GameObject tempObject = Instantiate(bulletObject, transform.position, Quaternion.identity);
                    tempObject.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
                    tempObject.GetComponent<Bullet>().Fire(tempObject.GetComponent<SpriteRenderer>().flipX ? -1 : 1, bulletSpeed);
                }
            }
            if (isHeal)
            {
                ani.SetTrigger("endHeal");
                isHeal = false;
                characterEffect.HealingEffect(false);
            }
            isHeldKey = false;
        }

    }
    public void GemLightSwitch()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            gemObject.isLightUp = !gemObject.isLightUp;
        }
    }
    public void NormalTranform()
    {
        if (Input.GetKeyDown(KeyCode.Q) /*&& !isFastTranform*/ && !isTranform && isGround)
        {
            rb.velocity = Vector2.zero;
            isTranform = true;
            canNormalTranform = false;
            //ani
            ani.SetTrigger("tranform");
            characterEffect.TranformEffect();

            isLight = !isLight;
        }
    }
    public void QuickTranform()
    {
        if (Input.GetKeyDown(KeyCode.E) && canNormalTranform /*&& isFastTranform*/)
        {
            isTranform = true;
            canQuickTranform = false;
            //ani
            //ani.SetTrigger("tranform");
            characterEffect.QuickTranformEffect();

            isLight = !isLight;

            EndTranform();
        }
    }
    public void EndTranform()
    {
        ani.runtimeAnimatorController = isLight ? LightAni : DarkAni;
        isTranform = false;
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
        if (!canNormalTranform)
        {
            if (normalTranformCountdownTime > 0)
            {
                normalTranformCountdownTime -= Time.deltaTime;
            }
            else
            {
                normalTranformCountdownTime = normalTranformCountdown;
                canNormalTranform = true;
            }
        }
        if (!canQuickTranform)
        {
            if (quickTranformCountdownTime > 0)
            {
                quickTranformCountdownTime -= Time.deltaTime;
            }
            else
            {
                quickTranformCountdownTime = quickTranformCountdown;
                canQuickTranform = true;
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
