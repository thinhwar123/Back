using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject cinemachineVirtualCamera;
    [SerializeField] private Cinemachine.CinemachineConfiner cinemachineConfiner;
    [SerializeField] private GameObject cameraColl;
    [SerializeField] private GameObject roomColl;
    [SerializeField] private float cameraSize;
    [SerializeField] private bool isDebug;
    [SerializeField] private bool isDrawAll;
    [SerializeField] private bool isDrawRoom;
    [SerializeField] private bool lockLeft;
    [SerializeField] private bool lockRight;
    [SerializeField] private bool lockUp;
    [SerializeField] private bool lockDown;
    private Vector2 fixPosition;
    private Vector2 fixRange;

    public Vector2 roomRangeOffset;


    private Vector3 roomPosition;
    private Vector3 roomRange;

    private Vector3 cameraStatPosition;
    private Vector3 cameraMoveRange;

    private Vector3 staticCameraPosition;
    public void Start()
    {
        
        FixCollider();
    }
    public void Update()
    {

    }
    public void FixCollider()
    {
        if (!isDebug)
        {
            Vector2 borderRoom = roomRangeOffset;
            Vector2 borderCamera = calculateCamera();

            //ve room
            roomPosition = transform.position;
            roomRange = new Vector3(roomRangeOffset.x * 2, roomRangeOffset.y * 2, 1);

            // fix vi tri va do lon
            fixPosition = (lockRight ? Vector2.zero : Vector2.right) + (lockLeft ? Vector2.zero : Vector2.left) + (lockUp ? Vector2.zero : Vector2.up) + (lockDown ? Vector2.zero : Vector2.down);
            fixRange = (lockRight ? Vector2.zero : Vector2.right) + (lockLeft ? Vector2.zero : Vector2.right) + (lockUp ? Vector2.zero : Vector2.up) + (lockDown ? Vector2.zero : Vector2.up);

            //ve camera
            staticCameraPosition = new Vector3(fixPosition.x * (borderCamera.x - roomRangeOffset.x), fixPosition.y * (borderCamera.y - roomRangeOffset.y), 1);

            //ve vung di chuyen cua camera
            cameraStatPosition = transform.position + new Vector3(fixPosition.x * (borderCamera.x / 2), fixPosition.y * (borderCamera.y / 2));

            cameraMoveRange = new Vector3(borderCamera.x * fixRange.x + roomRangeOffset.x * 2, borderCamera.y * fixRange.y + roomRangeOffset.y * 2, 1);

            if (cameraMoveRange.x < borderCamera.x * 2)
            {
                cameraMoveRange.x = borderCamera.x * 2;
                cameraStatPosition.x = transform.position.x + staticCameraPosition.x;
            }
            if (cameraMoveRange.y < borderCamera.y * 2)
            {
                cameraMoveRange.y = borderCamera.y * 2;
                cameraStatPosition.y = transform.position.y + staticCameraPosition.y;
            }




            cameraColl.transform.position = cameraStatPosition;
            cameraColl.transform.localScale = cameraMoveRange;

            roomColl.transform.position = roomPosition;
            roomColl.transform.localScale = roomRange;
        }        
    }
    public void StartCamera()
    {
        cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = GameManager.instance.character.transform;
        cinemachineVirtualCamera.SetActive(true);
        cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = cameraSize;
    }
    public void StopCamera()
    {
        cinemachineVirtualCamera.SetActive(false);
    }
    public IEnumerator DelayStopCamera()
    {
        yield return new WaitForSeconds(1.25f);

    }
    public Vector3 calculateCamera()
    {
        Rect aspet = Camera.main.pixelRect;
        Vector2 temp = new Vector2(Camera.main.orthographicSize * aspet.width / aspet.height, Camera.main.orthographicSize);
        return temp;
    }
    public void FixTranform()
    {

    }

    public void OnDrawGizmos()
    {

        if (isDebug )
        {
            cinemachineVirtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = cameraSize;
            Gizmos.color = Color.red;


            Vector2 borderRoom = roomRangeOffset;
            Vector2 borderCamera = calculateCamera();

            //ve room
            roomPosition = transform.position;
            roomRange = new Vector3(roomRangeOffset.x * 2, roomRangeOffset.y * 2, 1);

            // fix vi tri va do lon
            fixPosition = (lockRight ? Vector2.zero : Vector2.right) + (lockLeft ? Vector2.zero : Vector2.left) + (lockUp ? Vector2.zero : Vector2.up) + (lockDown ? Vector2.zero : Vector2.down);
            fixRange = (lockRight ? Vector2.zero : Vector2.right) + (lockLeft ? Vector2.zero : Vector2.right) + (lockUp ? Vector2.zero : Vector2.up) + (lockDown ? Vector2.zero : Vector2.up);

            //ve camera
            staticCameraPosition = new Vector3(fixPosition.x * (borderCamera.x - roomRangeOffset.x), fixPosition.y * (borderCamera.y - roomRangeOffset.y), 1);

            //ve vung di chuyen cua camera
            cameraStatPosition = transform.position + new Vector3(fixPosition.x *( borderCamera.x / 2), fixPosition.y  * (borderCamera.y / 2));

            cameraMoveRange = new Vector3(borderCamera.x * fixRange.x + roomRangeOffset.x * 2, borderCamera.y * fixRange.y + roomRangeOffset.y * 2, 1);

            if (cameraMoveRange.x < borderCamera.x * 2)
            {
                cameraMoveRange.x = borderCamera.x * 2;
                cameraStatPosition.x = transform.position.x + staticCameraPosition.x;
            }
            if (cameraMoveRange.y < borderCamera.y * 2)
            {
                cameraMoveRange.y = borderCamera.y * 2;
                cameraStatPosition.y = transform.position.y + staticCameraPosition.y;
            }




            cameraColl.transform.position = cameraStatPosition;
            cameraColl.transform.localScale = cameraMoveRange;

            roomColl.transform.position = roomPosition;
            roomColl.transform.localScale = roomRange;



            cinemachineConfiner.InvalidatePathCache();

            if (isDrawAll)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(roomRangeOffset.x * 2, roomRangeOffset.y * 2, 1));                                                // ve room
                Gizmos.DrawWireCube(transform.position + staticCameraPosition, new Vector3(borderCamera.x * 2, borderCamera.y * 2, 1));           // ve camera
                Gizmos.DrawWireCube(cameraStatPosition, cameraMoveRange);                                                                           // ve vung camera
            }
            else if (isDrawRoom)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(roomRangeOffset.x * 2, roomRangeOffset.y * 2, 1));                                                // ve room
            }
            
        }
        
    }
}
