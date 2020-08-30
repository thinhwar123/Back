using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrossHair : MonoBehaviour
{
    public float speedRotate;
    public float speedScale;
    public Tween tween1;
    public Tween tween2;
    // Start is called before the first frame update
    void Start()
    {
        tween1 = transform.DORotate(new Vector3(0, 0, 180), speedRotate).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        tween2 = transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), speedScale).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void DestroyAim()
    {
        tween1.Kill();
        tween2.Kill();
        Destroy(gameObject);
    }
    
}
