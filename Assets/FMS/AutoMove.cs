using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoMove : MonoBehaviour
{
    public Transform trans;
    public Vector3 range;
    public float duration;
    public Ease ease;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        trans.Translate(-range, Space.Self);
        trans.DOLocalMove(range, duration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
