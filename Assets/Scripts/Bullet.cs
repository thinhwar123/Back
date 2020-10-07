using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Tween tween;
    public void Fire(int dir, float speed)
    {
        tween = transform.DOMoveX(transform.position.x + dir, 1 / speed).SetLoops(-1, LoopType.Incremental);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            tween.Kill();
            Destroy(gameObject);
        }
    }
}
