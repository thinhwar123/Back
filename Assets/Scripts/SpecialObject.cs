using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    public Animator ani;
    public void Explosion()
    {
        ani.SetTrigger("explosion");
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
