using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource bgmSource, finaleSource, finishSource;

    public AudioSource[] sfxSource;
    public AudioClip sfxclip;
    public float finaleDuration = 10f;

    public float[] timerlist;
    public float resetTime = 0.5f;
    public int sfxIndex;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in sfxSource)
        {
            item.volume = SaveManager.instance.saveData.sfxVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < timerlist.Length; i++)
        {
            timerlist[i] -= Time.deltaTime;
            if (timerlist[i] <= 0)
                timerlist[i] = 0;
        }
    }

    public void PlaySFX(int _index)
    {
        if (timerlist[0] != 0) return;
        timerlist[0] = resetTime;
        sfxIndex += 1;
        if (sfxIndex >= sfxSource.Length - 1)
            sfxIndex = 0;

        sfxSource[sfxIndex].PlayOneShot(sfxclip);
    }

    public void PlayFinish()
    {
        var cc = bgmSource.volume;
        DOTween.To(() => cc, x => cc = x, 0, 4f).OnUpdate(() =>
        {
            bgmSource.volume = cc;
        }).OnComplete(() =>
        {

        });

        finishSource.Play();

    }

    public void PlayFinale()
    {
        finaleSource.Play();
        finaleSource.volume = 1;

        var cc = bgmSource.volume;
        DOTween.To(() => cc, x => cc = x, 0, 4f).OnUpdate(() =>
        {
            bgmSource.volume = cc;
        }).OnComplete(() =>
        {

        });

        var c1 = finaleSource.volume;
        DOTween.To(() => c1, x => c1 = x, 0, 5f).OnUpdate(() =>
        {
            finaleSource.volume = c1;
        }).OnComplete(() =>
        {

        }).SetDelay(10f);

        var c2 = bgmSource.volume;
        DOTween.To(() => c2, x => c2 = x, 1, 5f).OnUpdate(() =>
        {
            bgmSource.volume = c2;
        }).OnComplete(() =>
        {

        }).SetDelay(10f);


    }
}
