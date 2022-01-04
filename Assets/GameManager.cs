using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
using DG.Tweening;

[System.Serializable]
public class SequenceSet
{
    public List<Texture> texList;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int[] scoreList;
    public int[] seqList;
    public int scoreTarget = 200;
    public int scoreAdd = 10;
    public float seqPlaySpeed = 0.008f;
    public int seqAddFrame = 1;
    public float resetTimer = 5f;
    public float stayDuration = 10f;
    public float[] resetList;

    public SequenceSet[] seqSet;
    public int[] seqIndex;

    public RawImage baseSet;
    public DisplayUGUI[] rawImg;
    public RawImage[] dissolverawImg;
    public Material[] mats;
    public MediaPlayer[] mediaPlayer;
    public MediaPlayer[] dissolvePlayer;
    public MediaPlayer maskPlayer;
    public GameObject[] particleGroup;

    public bool gameStart;
    public bool gameEnd;

    int highest = -1;
    int scoreHigh = 0;
    int randomHitAdd = 5;

    public GameObject bg;
    public bool useDissolve;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameEnd = false;
        highest = -1;
        StartCoroutine(DelaySet());
        StartCoroutine(UpdateSeqPlayback());

        bg.SetActive(SaveManager.instance.saveData.useBackground);

        seqPlaySpeed = SaveManager.instance.saveData.seqPlaySpeed;
        scoreTarget = SaveManager.instance.saveData.scoreTarget;
        scoreAdd = SaveManager.instance.saveData.scoreAdd;
        randomHitAdd = SaveManager.instance.saveData.randomHitAdd;
        resetTimer = SaveManager.instance.saveData.resetTimer;
        stayDuration = SaveManager.instance.saveData.stayDuration;
        useDissolve = SaveManager.instance.saveData.useDissolve;
        seqAddFrame = SaveManager.instance.saveData.seqAddFrame;

        if (useDissolve)
        {
            foreach (var item in mediaPlayer)
            {
                item.Stop();
                item.gameObject.SetActive(false);
            }

            foreach (var item in dissolvePlayer)
            {
                item.gameObject.SetActive(true);
            }

            //StartCoroutine(DelaySetMat());
            foreach (var item in rawImg)
            {
                item.gameObject.SetActive(false);
            }

            foreach (var item in dissolverawImg)
            {
                item.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator DelaySetMat()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < rawImg.Length; i++)
        {
            var ii = i;
            var matt = mats[ii];
            Debug.Log(i);
            Debug.Log(rawImg[ii]);
            rawImg[ii].material = mats[ii];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ForceReset();
        }

        if(gameStart)
        {
            UpdateGame();
        }
    }

    void UpdateGame()
    {
        highest = -1;
        scoreHigh = 0;

        for (int i = 0; i < scoreList.Length; i++)
        {
            int ii = i;
            if (resetList[ii] > 0)
            {
                var r = resetList[ii] -= Time.deltaTime;
            }

            if (resetList[ii] <= 0)
            {
                resetList[ii] = resetTimer * 0.1f;
                scoreList[ii] -= 5;

                if (scoreList[ii] <= 0)
                    scoreList[ii] = 0;
            }

            if (scoreList[ii] > 0 && scoreList[ii] > scoreHigh)
            {
                scoreHigh = scoreList[ii];
                highest = ii;
            }
            //else
            //{
            //    if (seqIndex[ii] > 0 && seqIndex[ii] > scoreHigh)
            //    {
            //        scoreHigh = scoreList[ii];
            //        highest = ii;

            //    }
            //}
            //rawImg[i].texture = seqSet[i].texList[scoreList[i]];
        }
    }

    public void AddScore(int _index)
    {
        if (!gameStart || gameEnd) return;
        //rectList[_index].SetSiblingIndex(3);
        StartCoroutine(AddScoreCor(_index));
    }

    IEnumerator AddScoreCor(int _index)
    {
        yield return new WaitForEndOfFrame();
        //if (highest != _index)
        //{
        //    if (scoreHigh > scoreList[_index])
        //    {
        //        //scoreList[_index] += (scoreHigh - scoreList[_index])/4;
        //    }
        //}
        scoreList[_index] += scoreAdd + Random.Range(0, randomHitAdd);

        //Game Over
        if(scoreList[_index] >= scoreTarget)
        {
            gameStart = false;
            scoreHigh = 599;
            highest = _index;
        }

        scoreList[_index] = Mathf.Clamp(scoreList[_index], 0, seqSet[0].texList.Count - 1);
        resetList[_index] = resetTimer;
    }

    IEnumerator UpdateSeqPlayback()
    {
        while (true)
        {
            if (!gameEnd)
            {
                yield return new WaitForSeconds(seqPlaySpeed);
                yield return new WaitForEndOfFrame();
                if (seqIndex[0] > scoreHigh)
                {
                    seqIndex[0] -= 1;
                }
                else if (seqIndex[0] < scoreHigh)
                {
                    seqIndex[0] += seqAddFrame;
                }

                seqIndex[0] = Mathf.Clamp(seqIndex[0], 0, seqSet[0].texList.Count - 1);
                baseSet.texture = seqSet[0].texList[seqIndex[0]];

                if (!gameStart && seqIndex[0] >= seqSet[0].texList.Count - 1)
                    gameEnd = true;

                //Game Over
                if (!gameEndBegin && !gameStart && seqIndex[0] >= seqSet[0].texList.Count - 180)
                {
                    //gameEnd = true;
                    gameEndBegin = true;
                    Debug.Log("Game OVeR");
                    SetHighest(highest);
                    //mediaPlayer[highest].Play();
                }
            }
            
            yield return null;
            //for (int i = 0; i < scoreList.Length; i++)
            //{
            //    if(seqIndex[i] > scoreList[i])
            //    {
            //        seqIndex[i] -= 1;
            //    }
            //    else if(seqIndex[i] < scoreList[i])
            //    {
            //        seqIndex[i] += 1;
            //    }

            //    seqIndex[i] = Mathf.Clamp(seqIndex[i], 0, 199);
            //    rawImg[i].texture = seqSet[i].texList[seqIndex[i]];
            //}
        }
    }

    IEnumerator DelaySet()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            //if (highest >= 0)
            //{
            //    rawImg[highest].CrossFadeAlpha(1f, 0.3f, false);
            //    yield return new WaitForSeconds(0.2f);
            //}
            

            //for (int i = 0; i < rawImg.Length; i++)
            //{
            //    if (highest != i)
            //        rawImg[i].CrossFadeAlpha(0f, 0.3f, false);
            //}

            //
            //SetHighest(highest);
        }
    }

    void ResetGame()
    {
        highest = -1;
        scoreHigh = 0;
        seqIndex[0] = 0;
        for (int i = 0; i < scoreList.Length; i++)
        {
            scoreList[i] = 0;
        }
    }

    public void ForceReset()
    {
        DOTween.CompleteAll(true);

        StartCoroutine(DelayReset());
        
        //StopAllCoroutines();
    }

    IEnumerator DelayReset()
    {
        yield return new WaitForEndOfFrame();
        DOTween.CompleteAll(true);
        yield return new WaitForEndOfFrame();

        gameStart = true;
        gameEnd = false;

        for (int i = 0; i < scoreList.Length; i++)
        {
            scoreList[i] = 0;
            seqIndex[i] = 0;
        }

        
    }

    IEnumerator PauseMaskVideo()
    {
        print(maskPlayer.Info.GetDurationMs() / 1000f);
        yield return new WaitForSeconds((maskPlayer.Info.GetDurationMs()/1000f) - 0.15f);
        maskPlayer.Control.Pause();
        print("paused");
    }

    bool gameEndBegin;

    void SetHighest(int _index)
    {
        if(useDissolve)
        {
            Color cc = dissolverawImg[_index].color;
            //particleGroup[_index].SetActive(true);
            maskPlayer.Control.Play();
            //StartCoroutine(PauseMaskVideo());

            DOTween.ToAlpha(() => cc, x => cc = x, 1, 0.6f).OnUpdate(() =>
            {
                dissolverawImg[_index].color = cc;
                mats[_index].SetColor("MainColor", cc);
            }).OnComplete(() =>
            {
                DOTween.ToAlpha(() => cc, x => cc = x, 0, 2).OnUpdate(() =>
                {
                    dissolverawImg[_index].color = cc;
                    mats[_index].SetColor("MainColor", cc);
                }).OnComplete(() => {
                    //particleGroup[_index].SetActive(false);

                    gameEnd = false;
                    gameStart = true;
                    gameEndBegin = false;

                    maskPlayer.Control.Pause();
                    maskPlayer.Control.Seek(0);
                    maskPlayer.Control.Stop();

                }).SetDelay(stayDuration).OnStart(()=>
                {
                    Debug.Log("RESET");
                    ResetGame();
                    baseSet.texture = seqSet[0].texList[0];
                });
            });
        }
        else
        {
            Color cc = rawImg[_index].color;
            particleGroup[_index].SetActive(true);

            DOTween.ToAlpha(() => cc, x => cc = x, 1, 2).OnUpdate(() =>
            {
                rawImg[_index].color = cc;
            }).OnComplete(() =>
            {
                ResetGame();
                baseSet.texture = seqSet[0].texList[0];

                DOTween.ToAlpha(() => cc, x => cc = x, 0, 2).OnUpdate(() =>
                {
                    rawImg[_index].color = cc;

                }).OnComplete(() => {
                    particleGroup[_index].SetActive(false);

                    gameEnd = false;
                    gameStart = true;
                    gameEndBegin = false;

                }).SetDelay(stayDuration);
            });
        }
        

        //for (int i = 0; i < rawImg.Length; i++)
        //{
        //    int ii = i;
        //    if(highest == ii)
        //    {
        //        rawImg
        //    }
        //        rawImg[ii].CrossFadeAlpha(1f, 0.3f, false);
        //    else
        //        rawImg[ii].CrossFadeAlpha(0f, 0.3f, false);
        //}
    }
}
