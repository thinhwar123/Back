using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIxBugGround : MonoBehaviour
{
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            GetComponentInParent<Transform>().position += Vector3.up;
        }
    }
    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            GetComponentInParent<Transform>().position += Vector3.up;
        }
    }
}
