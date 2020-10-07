using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGameObject : MonoBehaviour
{
    [SerializeField] private GameObject spwanGameObject;
    [SerializeField] private float delayTime;
    void Start()
    {
        StartCoroutine(AutoSpawnGameObject());
    }

    // Update is called once per frame
    void Update()
    {
    }
    public IEnumerator AutoSpawnGameObject()
    {
        yield return new WaitForSeconds(delayTime);
        SimplePool.Spawn(spwanGameObject, transform.position, Quaternion.identity);
        StartCoroutine(AutoSpawnGameObject());
    }
}
