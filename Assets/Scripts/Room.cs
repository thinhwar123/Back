using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Room : MonoBehaviour
{
    public GameObject cinemachineVirtualCamera;
    public Cinemachine.CinemachineConfiner cinemachineConfiner;
    public bool isDebug;
    public bool isDrawAll;
    public bool isDrawRoom;
    public bool lockLeft;
    public bool lockRight;
    public bool lockUp;
    public bool lockDown;
    private Vector2 fixPosition;
    private Vector2 fixRange;
    public GameObject cameraColl;
    public GameObject roomColl;
    public Vector2 followOffset;
    public Vector2 localPosition;


    private Vector3 roomPosition;
    private Vector3 roomRange;

    private Vector3 cameraStatPosition;
    private Vector3 cameraMoveRange;
    public void Start()
    {
        
        FixCollider();
    }
    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    cameraColl.transform.position = cameraStatPosition;
        //    cameraColl.transform.localScale = cameraMoveRange;

        //    roomColl.transform.position = roomPosition;
        //    roomColl.transform.localScale = roomRange;

        //}
    }
    public void FixCollider()
    {
        if (!isDebug)
        {
            Vector2 border = calculateThershold();
            Vector2 borderCamera = calculateCamera();

            //ve room
            roomPosition = transform.position;
            roomRange = new Vector3(border.x * 2, border.y * 2, 1);

            //ve camera
            localPosition = Vector2.zero;
            if (lockLeft || lockRight)
            {
                if (lockLeft)
                {
                    localPosition.x = followOffset.x;
                }
                else
                {
                    localPosition.x = -followOffset.x;
                }
            }
            if (lockUp || lockDown)
            {
                if (lockDown)
                {
                    localPosition.y = followOffset.y;
                }
                else
                {
                    localPosition.y = -followOffset.y;
                }
            }

            //ve vung di chuyen cua camera
            fixPosition = (lockRight ? Vector2.zero : Vector2.right) + (lockLeft ? Vector2.zero : Vector2.left) + (lockUp ? Vector2.zero : Vector2.up) + (lockDown ? Vector2.zero : Vector2.down);
            fixRange = (lockRight ? Vector2.zero : Vector2.right) + (lockLeft ? Vector2.zero : Vector2.right) + (lockUp ? Vector2.zero : Vector2.up) + (lockDown ? Vector2.zero : Vector2.up);
            cameraStatPosition = transform.position + new Vector3((fixPosition.x * border.x + localPosition.x) / 2, (fixPosition.y * border.y + localPosition.y) / 2);
            cameraMoveRange = new Vector3(borderCamera.x * fixRange.x + border.x * 2, borderCamera.y * fixRange.y + border.y * 2, 1);


            cameraColl.transform.position = cameraStatPosition;
            cameraColl.transform.localScale = cameraMoveRange;

            roomColl.transform.position = roomPosition;
            roomColl.transform.localScale = roomRange;
        }        
    }
    public void StartCamera()
    {
        cinemachineVirtualCamera.SetActive(true);
        cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = GameManager.instance.character.transform;
    }
    public void StopCamera()
    {
        cinemachineVirtualCamera.SetActive(false);
    }
    public Vector3 calculateCamera()
    {
        Rect aspet = Camera.main.pixelRect;
        Vector2 temp = new Vector2(Camera.main.orthographicSize * aspet.width / aspet.height, Camera.main.orthographicSize);
        return temp;
    }
    public Vector3 calculateThershold()
    {
        Rect aspet = Camera.main.pixelRect;
        Vector2 temp = new Vector2(Camera.main.orthographicSize * aspet.width / aspet.height, Camera.main.orthographicSize);
        temp.x -= followOffset.x;
        temp.y -= followOffset.y;
        return temp;
    }
    public void OnDrawGizmos()
    {
        if (isDebug)
        {
            Gizmos.color = Color.red;


            Vector2 border = calculateThershold();
            Vector2 borderCamera = calculateCamera();

            //ve room
            roomPosition = transform.position;
            roomRange = new Vector3(border.x * 2, border.y * 2, 1);

            //ve camera
            localPosition = Vector2.zero;
            if (lockLeft || lockRight)
            {
                if (lockLeft)
                {
                    localPosition.x = followOffset.x;
                }
                else
                {
                    localPosition.x = -followOffset.x;
                }
            }
            if (lockUp || lockDown)
            {
                if (lockDown)
                {
                    localPosition.y = followOffset.y;
                }
                else
                {
                    localPosition.y = -followOffset.y;
                }
            }

            //ve vung di chuyen cua camera
            fixPosition = (lockRight ? Vector2.zero : Vector2.right) + (lockLeft ? Vector2.zero : Vector2.left) + (lockUp ? Vector2.zero : Vector2.up) + (lockDown ? Vector2.zero : Vector2.down);
            fixRange = (lockRight ? Vector2.zero : Vector2.right) + (lockLeft ? Vector2.zero : Vector2.right) + (lockUp ? Vector2.zero : Vector2.up) + (lockDown ? Vector2.zero : Vector2.up);

            cameraStatPosition = transform.position + new Vector3((fixPosition.x * border.x + localPosition.x) / 2, (fixPosition.y * border.y + localPosition.y) / 2);

            cameraMoveRange = new Vector3(borderCamera.x * fixRange.x + border.x * 2, borderCamera.y * fixRange.y + border.y * 2, 1);

            if (cameraMoveRange.x < borderCamera.x * 2)
            {
                cameraMoveRange.x = borderCamera.x * 2;
                cameraStatPosition.x = transform.position.x + (-fixPosition.x * border.x + borderCamera.x * fixPosition.x);
            }
            if (cameraMoveRange.y < borderCamera.y * 2)
            {
                cameraMoveRange.y = borderCamera.y * 2;
                cameraStatPosition.y = transform.position.y + (-fixPosition.y * border.y + borderCamera.y * fixPosition.y);
            }

            cameraColl.transform.position = cameraStatPosition;
            cameraColl.transform.localScale = cameraMoveRange;

            roomColl.transform.position = roomPosition;
            roomColl.transform.localScale = roomRange;



            //cinemachineConfiner.InvalidatePathCache();

            if (isDrawAll)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));                                                // ve room
                Gizmos.DrawWireCube(transform.position + (Vector3)localPosition, new Vector3(borderCamera.x * 2, borderCamera.y * 2, 1));           // ve camera
                Gizmos.DrawWireCube(cameraStatPosition, cameraMoveRange);                                                                           // ve vung camera
            }
            else if (isDrawRoom)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));                                                // ve room
            }
        }
        
    }
}
