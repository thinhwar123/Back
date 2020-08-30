using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CharacterEffect : MonoBehaviour
{
    public CharacterMovement characterMovement;
    [Header("Jump")]
    public ParticleSystem jumpEffect;
    public ParticleSystem healingEffect;

    [Header("Dash")]
    public GameObject dashEffect;
    public float timeDashSpawnStart;
    public float timeDashSpawnCounter;
    public float timeDashFade;
    public float dashFadeValue;

    [Header("Roll")]
    public GameObject rollEffect;
    public float timeRollSpawnStart;
    public float timeRollSpawnCounter;
    public float timeRollFade;
    public float rollFadeValue;

    [Header("ChangeForm")]
    public Animator changeFormAni;
    public void fixDirectionSlide()
    {
        if (characterMovement.isWallLeft)
        {
            transform.parent.transform.localPosition = new Vector3(-0.1f, 0, 0);
        }
        else if (characterMovement.isWallRight)
        {
            transform.parent.transform.localPosition = new Vector3(0.1f, 0, 0);
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
    public IEnumerator DashEffect(float dashTime)
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
            dashTime -= Time.deltaTime;
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
    public void ChangeFormEffect()
    {
        changeFormAni.GetComponent<SpriteRenderer>().flipX = characterMovement.spriteRenderer.flipX;
        changeFormAni.SetTrigger("changeForm");
    }
}
