using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class WeakWall : MonoBehaviour
{
    [SerializeField] private GameObject explosionObject;
    public void Explosion()
    {
        GameObject tempObject = Instantiate(explosionObject, transform.position, Quaternion.identity);
        tempObject.GetComponent<Animator>().SetTrigger("fire");
        Destroy(tempObject, 2);
        Destroy(gameObject);
    }
}
