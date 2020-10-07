using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CharacterEffect : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [Header("Jump")]
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private ParticleSystem healingEffect;

    [Header("Dash")]
    [SerializeField] private GameObject dashEffect;
    [SerializeField] private float timeDashSpawnStart;
    [SerializeField] private float timeDashSpawnCounter;
    [SerializeField] private float timeDashFade;
    [SerializeField] private float dashFadeValue;

    [Header("SpecialDash")]
    [SerializeField] private GameObject specialDashEffect;
    [SerializeField] private float timeSpecialDashSpawnStart;
    [SerializeField] private float timeSpecialDashSpawnCounter;
    [SerializeField] private float timeSpecialDashFade;
    [SerializeField] private float specialDashFadeValue;

    [Header("Roll")]
    [SerializeField] private GameObject rollEffect;
    [SerializeField] private float timeRollSpawnStart;
    [SerializeField] private float timeRollSpawnCounter;
    [SerializeField] private float timeRollFade;
    [SerializeField] private float rollFadeValue;

    [Header("ChangeForm")]
    public Animator changeFormAni;
    public void fixDirectionSlide()
    {
        if (characterMovement.isWallLeft)
        {
            transform.parent.transform.localPosition = new Vector3(-0.2f, 0, 0);
        }
        else if (characterMovement.isWallRight)
        {
            transform.parent.transform.localPosition = new Vector3(0.2f, 0, 0);
        }
    }
    public void resetDirection()
    {
        transform.parent.transform.localPosition = new Vector3(0, 0, 0);
    }
    public void JumpEffect()
    {
        jumpEffect.Play();
    }
    public void HealingEffect(bool check)
    {
        if (check)
        {
            healingEffect.Play();
        }
        else
        {
            healingEffect.Stop();
        }

    }
    public IEnumerator DashEffect()
    {
        timeDashSpawnCounter = 0.05f;
        while (characterMovement.isDash)
        {

            if (timeDashSpawnCounter < 0)
            {
                timeDashSpawnCounter = timeDashSpawnStart;
                //spwan echo
                GameObject tempObject = Instantiate(dashEffect,transform.position , Quaternion.identity);
                tempObject.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
                tempObject.GetComponent<SpriteRenderer>().flipX = this.GetComponent<SpriteRenderer>().flipX;
                tempObject.GetComponent<SpriteRenderer>().DOFade(dashFadeValue, timeDashFade);
                Destroy(tempObject, timeDashFade + 0.1f);
            }
            else
            {
                timeDashSpawnCounter -= Time.deltaTime;
            }


            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator SpecialDashEffect()
    {
        timeSpecialDashSpawnCounter = 0.05f;
        while (characterMovement.isSpecialDash)
        {

            if (timeSpecialDashSpawnCounter < 0)
            {
                timeSpecialDashSpawnCounter = timeSpecialDashSpawnStart;
                //spwan echo
                GameObject tempObject = Instantiate(specialDashEffect, transform.position , Quaternion.identity);
                tempObject.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
                tempObject.GetComponent<SpriteRenderer>().flipX = this.GetComponent<SpriteRenderer>().flipX;
                tempObject.GetComponent<SpriteRenderer>().DOFade(specialDashFadeValue, timeSpecialDashFade);
                Destroy(tempObject, timeSpecialDashFade + 0.1f);
            }
            else
            {
                timeSpecialDashSpawnCounter -= Time.deltaTime;
            }


            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator RollEffect(float rollTime)
    {
        timeRollSpawnCounter = 0.05f;
        while (characterMovement.isRoll)
        {

            if (timeRollSpawnCounter < 0)
            {
                timeRollSpawnCounter = timeRollSpawnStart;
                //spwan echo
                GameObject tempObject = Instantiate(rollEffect, transform.position, Quaternion.identity);
                tempObject.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
                tempObject.GetComponent<SpriteRenderer>().flipX = this.GetComponent<SpriteRenderer>().flipX;
                tempObject.GetComponent<SpriteRenderer>().DOFade(rollFadeValue, timeRollFade);
                Destroy(tempObject, timeRollFade + 0.1f);
            }
            else
            {
                timeRollSpawnCounter -= Time.deltaTime;
            }


            yield return new WaitForEndOfFrame();
            rollTime -= Time.deltaTime;
        }
    }
    public void TranformEffect()
    {
        changeFormAni.GetComponent<SpriteRenderer>().flipX = characterMovement.spriteRenderer.flipX;
        changeFormAni.SetBool("isLight", characterMovement.isLight);
        changeFormAni.SetTrigger("normalTranform");
    }
    public void QuickTranformEffect()
    {
        changeFormAni.GetComponent<SpriteRenderer>().flipX = characterMovement.spriteRenderer.flipX;
        changeFormAni.SetBool("isLight", characterMovement.isLight);
        changeFormAni.SetTrigger("quickTranform");
    }
    public void EndNormalTranform()
    {
        characterMovement.EndTranform();
    }
}
