using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCheckController : MonoBehaviour
{
    public Room room;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            room.StartCamera();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            room.StopCamera();
        }
    }
}
