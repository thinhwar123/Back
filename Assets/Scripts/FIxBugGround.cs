using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBugGround : MonoBehaviour
{
    public bool canStand = true;
    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Ground"))
    //    {
    //        canStand = false;
    //    }
    //}
    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Ground"))
    //    {
    //        canStand = true;
    //    }
    //}
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            canStand = true;
        }
        else
        {
            canStand = false;
        }
    }
}
