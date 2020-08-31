using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    public Animator ani;
    public GameObject coll;
    public void Explosion()
    {
        Destroy(coll);
        ani.SetTrigger("explosion");
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
