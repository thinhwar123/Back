using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
public class CharacterMovement : MonoBehaviour
{
    [Header("CharacterAttribute")]
    public bool isDebug;
    public CharacterEffect characterEffect;
    public BoxCollider2D standCollider;
    public Collider2D rollCollider;
    public Rigidbody2D rb;
    public Animator ani;
    public SpriteRenderer spriteRenderer;
    public Transform leftHand;
    public Transform rightHand;
    public Transform feet;
    public Transform bodyCharacter;
    public Gem gemObject;
    public Transform gemPoint;
    public Vector2 gemPointPosition;
    public LayerMask whatIsGround;
    public LayerMask whatIsSpecialObject;
    public LayerMask whatIsRoom;
    public float normalGravity;
    public bool isWallLeft;
    public bool isWallRight;
    public bool isWall;
    public bool isGround;
    public bool isInTheAir;
    public bool isInGround;

    [Header("FindRoomAttribute")]
    public Room currentRoom;

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
    public float wallJumpForceBack;
    public float wallJumpTime;
    public float wallJumpTimeCounter;
    public bool continueJump;

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
    public float dashCountdownCanDash;
    public float dashCountdownCanDashTime;

    [Header("RollAttribute")]
    public bool canRoll;
    public bool canStand;
    public bool isRoll;
    public float rollForce;
    public float rollTime;
    public float rollTimeCounter;

    [Header("SpecialDashAttribute")]
    public bool isInSpecialDash;
    public bool isAim;
    public bool isAimRun;
    public bool canSpecialDash;
    public bool isSpecialDash;
    public bool isJoin;
    public bool canFindObjectWhileRun;  // bien de tao khoang cach giua cac lan quet
    public bool delayTimeFind;
    public float specialDashForce;
    public float specialDashRange;
    public List<GameObject> listSpecialObject;
    public int indexListObject;
    public float timeSwitchObject; 
    public float timeSwitchObjectCounter;
    public GameObject tempCrossHair;
    public GameObject tempSpecialObject;
    public GameObject specialObjectAim;
    public GameObject specialObjectControll;
    public GameObject crossHair;
    public float joinTime;
    public float joinTimeCounter;

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
        //listObject = new RaycastHit2D[0];
        EndTranform();
    }
    void Update()
    {
        //dir = new Vector2(1, 1);
        float xIndex = Input.GetAxisRaw("Horizontal");
        float yIndex = Input.GetAxisRaw("Vertical");
        dir = new Vector2(xIndex, yIndex);
        CheckStatus();//check trang thai nhan vat

        DashCountdownStack();
        DashCountdownCanDash();
        TranformCountdown();
        Attack();
        Flip();
        Skill();
        WallSlide();
        if (isLight || isDebug)
        {
            Jump();
            WallJump1();
            Somersault();
            Heal();
            Roll();
        }
        if(!isLight || isDebug)
        {
            Dash8();
            Aim();
            SpecialDash();
            OutSpecialDash();
        }

        Run();
        //Dash2();

        GemLightSwitch();
        NormalTranform();
        QuickTranform();

    }
    public void CheckStatus()
    {
        if (!isJoin)
        {
            isGround = Physics2D.OverlapBox(feet.position, new Vector2(0.9f, 0.1f), 0, whatIsGround);
            isWallLeft = Physics2D.OverlapBox(leftHand.position, new Vector2(0.05f, 1.3f), 0, whatIsGround);
            isWallRight = Physics2D.OverlapBox(rightHand.position, new Vector2(0.05f, 1.3f), 0, whatIsGround);
            isWall = ((isWallLeft /*&& dir.x < 0*/) || (isWallRight /*&& dir.x > 0*/)) && !isGround;
            isInTheAir = !isWall && !isGround;
            canStand = !Physics2D.OverlapCircle(bodyCharacter.position, 0.1f, whatIsGround);
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
        if (!isRoll && !isDash)
        {
            if (!isWallSlide)
            {
                if (dir.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (dir.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
            }
            else if (isWallSlide)
            {
                if (isWallLeft && rb.velocity.y < 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (isWallRight && rb.velocity.y < 0)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }

        FixGemPointPosition();
    }
    public void Run()
    {
        if (!isWallJump && !isDash && !isRoll && !isTranform && !isHeal && !isJoin && !isSpecialDash)
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
        if (isWall && !isWallJump && rb.velocity.y < 0  &&((isWallLeft && dir.x <0) || (isWallRight && dir.x > 0)))
        {
            isWallSlide = true;
            rb.gravityScale = 0.1f;
            rb.velocity = Vector2.down * slideForce;
            //ani
            ani.SetBool("isWallSilde", true);
        }
        else if (isWallSlide && ((isWallLeft) || (isWallRight)) && !isGround && !isDash && !isWallJump)
        {
            isWallSlide = true;
            rb.gravityScale = 0.1f;
            rb.velocity = Vector2.down * slideForce;
            //ani
            ani.SetBool("isWallSilde", true);
        }
        else if (!isDash && !isInSpecialDash)
        {
            isWallSlide = false;
            rb.gravityScale = normalGravity;
            //ani
            ani.SetBool("isWallSilde", false);
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
    public void WallJump1()
    {
        if (isWall && Input.GetKeyDown(KeyCode.J) && !isWallJump)
        {
            isWallJump = true;
            continueJump = true;
            wallJumpTimeCounter = wallJumpTime;
            rb.gravityScale = normalGravity;
            Vector2 fixDirection = new Vector2(wallJumpDirection.x * (isWallLeft ? 1 : -1), wallJumpDirection.y);
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
        }
        if (continueJump && isInTheAir && dir.x != 0  && !isWallJump)
        {
            continueJump = false;
            //isSomersault = true;
            //canSomersault = false;

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = Vector2.up * wallJumpForceBack;

            //ani
            //ani.SetBool("isSomersault", true);
            ani.SetTrigger("somersault");
            characterEffect.JumpEffect();
        }
        else if(continueJump && !isWallJump)
        {
            continueJump = false;
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
        if (canDash && !isDash && dashStack !=0 && Input.GetKeyDown(KeyCode.K) && !(isGround && dir.y < 0) && !isRoll && !isTranform && !isHeal && !(isInSpecialDash && !isAim) && !((isWallLeft && dir.x == -1) || (isWallRight && dir.x == 1)))// co the dash + an nut + dung duoi dat dash xuong duoi + bam tuong dash vao tuong + ko roll
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
        else if (dashTimeCounter < 0 )
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
        if (canDash && !isDash && dashStack != 0 && Input.GetKeyDown(KeyCode.K) && !isRoll && !isTranform && !isHeal && /*(isInSpecialDash && isAim) && */ !((isWallLeft && dir.x == -1) || (isWallRight && dir.x == 1)))// co the dash + an nut + dung duoi dat dash xuong duoi + bam tuong dash vao tuong + ko roll
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
    public void Aim()
    {
        if (Input.GetKeyDown(KeyCode.N) ) // check luc bat dau an N
        {
            isAim = true;
            isInSpecialDash = true;

            StartCoroutine(FindSpecialObjectAround(false,0.1f));
        }
        else if (isAim /*&& rb.velocity != Vector2.zero *//*&& ((spriteRenderer.flipX && dir.x < 0) || (!spriteRenderer.flipX && dir.x > 0))*/) // check luc chuyen dong 
        {
            isAimRun = true;
            StartCoroutine(FindSpecialObjectAround(true,0.1f));
        }
        else if (isAim && isAimRun && dir.x == 0) // check luc ket thuc chuyen dong
        {
            isAimRun = false;
            StartCoroutine(FindSpecialObjectAround(false,0.1f));
        }
        if (Input.GetKeyUp(KeyCode.N) && isAim)
        {
            isAim = false;
            canSpecialDash = false;
            if (!isJoin)
            {
                isInSpecialDash = false;
            }

            tempSpecialObject = null;
            if (specialObjectAim != null)
            {

                specialObjectAim.GetComponent<SpecialObject>().StopTarget();
                specialObjectAim = null;
            }
        }
    }
    public void SpecialDash()
    {
        if (Input.GetKeyDown(KeyCode.M) && isAim && canSpecialDash && !isDash)
        {
            gameObject.transform.SetParent(null);
            specialObjectAim.GetComponent<SpecialObject>().Frezzing(true);
            isAim = false;
            canSpecialDash = false;
            if (isJoin)
            {
                //spriteRenderer.color = Color.white;
                specialObjectControll.GetComponent<SpecialObject>().Frezzing(false);
                specialObjectControll.GetComponent<SpecialObject>().BeFree();
            }

            isSpecialDash = true;
            rb.gravityScale = 0;

            //characterCollider.isTrigger = true;
            //ani
            ani.SetTrigger("dash");
            StartCoroutine(characterEffect.SpecialDashEffect());
        }
        if (isSpecialDash && (transform.position - specialObjectAim.transform.position).magnitude > 0.5f)
        {
            rb.velocity = (specialObjectAim.transform.position - transform.position).normalized * specialDashForce;
        }
        else if (isSpecialDash) // nhap vao
        {
            //Debug.Log("endBug");
            ani.SetTrigger("endDash");
            rb.velocity = Vector2.zero;
            isSpecialDash = false;
            isJoin = true;
            spriteRenderer.color = Color.clear;

            specialObjectControll = specialObjectAim;
            specialObjectControll.GetComponent<SpecialObject>().BeControlled();
            gameObject.transform.SetParent(specialObjectControll.transform);
            joinTimeCounter = joinTime;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
    public void OutSpecialDash()
    {
        if (Input.GetKeyDown(KeyCode.M) && isJoin && !Input.GetKey(KeyCode.N))
        {
            isJoin = false;
            isInSpecialDash = false;
            specialObjectControll.GetComponent<SpecialObject>().Frezzing(false);
            listSpecialObject = new List<GameObject>();
            //characterCollider.isTrigger = false;

            spriteRenderer.color = Color.white;
            gameObject.transform.SetParent(null);                                                    // pha huy object
            specialObjectControll.GetComponent<SpecialObject>().BeFree();
            tempSpecialObject = null;
            specialObjectAim = null;
            specialObjectControll = null;
            rb.bodyType = RigidbodyType2D.Dynamic;
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
        if (isJoin && joinTimeCounter >= 0) // dem nguoc tg 
        {
            joinTimeCounter -= Time.deltaTime;
        }
        else if (isJoin && joinTimeCounter < 0)
        {
            isJoin = false;
            specialObjectControll.GetComponent<SpecialObject>().Frezzing(false);

            //characterCollider.isTrigger = false;

            spriteRenderer.color = Color.white;
            gameObject.transform.SetParent(null);                                                    // pha huy object
            specialObjectControll.GetComponent<SpecialObject>().BeFree();
            if (!isAim)
            {
                listSpecialObject = new List<GameObject>();
                tempSpecialObject = null;
                specialObjectAim = null;
                specialObjectControll = null;
                isInSpecialDash = false;
            }

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = normalGravity;
        }
    }
    public IEnumerator FindSpecialObjectAround(bool isWait ,float delayTime)
    {
        if (canFindObjectWhileRun || !isWait)
        {
            if (isWait)
            {
                canFindObjectWhileRun = false;
            }
            //if (specialObjectAim != null)
            //{
            //    specialObjectAim.GetComponent<SpecialObject>().StopTarget();
            //}
            canSpecialDash = false;
            listSpecialObject = new List<GameObject>();
            Vector2 lastVector2 = Vector2.down;
            int fixVector2 = spriteRenderer.flipX ? 1 : -1;
            for (int i = 0; i < 180; i += 2)
            {
                Debug.DrawRay(transform.position, (lastVector2 + new Vector2(lastVector2.y, -lastVector2.x) *(float)Math.Tan(Math.PI / 90)).normalized * specialDashRange, Color.red,1);
                RaycastHit2D tempHit = Physics2D.Raycast(transform.position, (lastVector2 + new Vector2(lastVector2.y, -lastVector2.x) * (float)Math.Tan(Math.PI / 90)).normalized, specialDashRange, whatIsSpecialObject);
                if (tempHit)
                {
                    if (!listSpecialObject.Exists(x => x.name ==  tempHit.transform.gameObject.name))
                    {
                        if (tempHit.transform.gameObject.layer == 11)
                        {
                            listSpecialObject.Add(tempHit.transform.gameObject);
                        }
                    }

                }
                lastVector2 = (lastVector2 + new Vector2(lastVector2.y, -lastVector2.x) * (float)Math.Tan(Math.PI / 90) * fixVector2).normalized;
            }
            if (listSpecialObject.Count == 0)
            {
                for (int i = 180; i < 360; i += 2)
                {
                    Debug.DrawRay(transform.position, (lastVector2 + new Vector2(lastVector2.y, -lastVector2.x) * (float)Math.Tan(Math.PI / 90)).normalized * specialDashRange, Color.red, 1);
                    RaycastHit2D tempHit = Physics2D.Raycast(transform.position, (lastVector2 + new Vector2(lastVector2.y, -lastVector2.x) * (float)Math.Tan(Math.PI / 90)).normalized, specialDashRange, whatIsSpecialObject);
                    if (tempHit)
                    {
                        if (!listSpecialObject.Exists(x => x.name == tempHit.transform.gameObject.name))
                        {
                            if (tempHit.transform.gameObject.layer == 11)
                            {
                                listSpecialObject.Add(tempHit.transform.gameObject);
                            }
                        }

                    }
                    lastVector2 = (lastVector2 + new Vector2(lastVector2.y, -lastVector2.x) * (float)Math.Tan(Math.PI / 90) * fixVector2).normalized;
                }
            }

            if (listSpecialObject.Count != 0)
            {
                //Debug.Log(listSpecialObject[0].name);
                tempSpecialObject = listSpecialObject[0];
                for (int i = 1; i < listSpecialObject.Count; i++)
                {
                    if ((tempSpecialObject.transform.position - transform.position).magnitude > (listSpecialObject[i].transform.position - transform.position).magnitude)
                    {
                        //Debug.Log(listSpecialObject[0].name);
                        tempSpecialObject = listSpecialObject[i];
                    }
                }
                canSpecialDash = true;

                if (tempSpecialObject != specialObjectAim)
                {
                    if (specialObjectAim != null)
                    {
                        specialObjectAim.GetComponent<SpecialObject>().StopTarget();
                    }
                    
                    specialObjectAim = tempSpecialObject;
                    specialObjectAim.GetComponent<SpecialObject>().StartTarget();
                }


            }
            if (isWait)
            {
                yield return new WaitForSeconds(delayTime);
                canFindObjectWhileRun = true;
            }

        }
    }
    public void Roll()
    {
        if (canRoll && isGround && Input.GetKeyDown(KeyCode.L) && !isDash && !isTranform && !isHeal && !isInSpecialDash /*&& !(isWallLeft && dir.x == -1) && !(isWallRight && dir.x == 1) && !(isWallLeft && spriteRenderer.flipX) && !(isWallRight && !spriteRenderer.flipX)*/)
        {
            isRoll = true;
            canRoll = false;

            standCollider.isTrigger = true;
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
            if (/*isWallLeft || isWallRight ||*/ isJumping)
            {
                isRoll = false;
                rollTimeCounter = -1;


            }
        }
        else if (rollTimeCounter < 0 && canStand)
        {
            if (isRoll)
            {
                isRoll = false;
                standCollider.isTrigger = false;

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
            //if ((isInTheAir && dir.y > 0) || (isGround && dir == Vector2.up))
            //{
            //    if (spriteRenderer.flipX)
            //    {
            //        ani.SetTrigger("attackDown");
            //    }
            //    else
            //    {
            //        ani.SetTrigger("attackUp");
            //    }
                
            //}
            //else if (isInTheAir && dir == Vector2.down)
            //{
            //    if (spriteRenderer.flipX)
            //    {
            //        ani.SetTrigger("attackUp");
            //    }
            //    else
            //    {
            //        ani.SetTrigger("attackDown");
            //    }
            //}
            //else if (true)
            //{
            //    ani.SetTrigger("attack");
            //}
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
    public void DashCountdownStack()
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
    public void DashCountdownCanDash()
    {
        if (!canDash)
        {
            if (dashCountdownCanDashTime > 0)
            {
                dashCountdownCanDashTime -= Time.deltaTime;
            }
            else
            {
                dashCountdownCanDashTime = dashCountdownCanDash;
                canDash = true;
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
