using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartFade : MonoBehaviour
{
    public Image black;

    // Start is called before the first frame update
    void Start()
    {
        var cc = black.color;

        DOTween.ToAlpha(() => cc, x => cc = x, 0, 2f).OnUpdate(() =>
        {
            black.color = cc;
        }).OnComplete(() =>
        {

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
