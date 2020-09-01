using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    public Animator ani;
    public GameObject specialObjectColl;
    public GameObject crossHair;
    public GameObject tempCrossHair;
    public void BeControlled()
    {
        StopTarget();
        specialObjectColl.GetComponent<CircleCollider2D>().enabled = false;
    }
    public void BeFree()
    {
        specialObjectColl.GetComponent<CircleCollider2D>().enabled = true;
    }
    public void StartTarget()
    {
        tempCrossHair = Instantiate(crossHair, transform.position, Quaternion.identity, transform);
    }
    public void StopTarget()
    {
        if (tempCrossHair != null)
        {
            tempCrossHair.GetComponent<CrossHair>().DestroyAim();
        }
        
    }
    public void Explosion()
    {
        ani.SetTrigger("explosion");
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
