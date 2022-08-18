using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityRawInput;

public class Game2Manager : MonoBehaviour
{
    public static Game2Manager instance;
    public float testduration = 19.5f;
    public float testseek = 19.5f;
    public int curGameIndex = 0;
    public int currentHitAmount;
    public int[] hitTarget;
    bool gotHit;

    public bool resetting;
    public Coroutine resetCor;
    public float resetDuration = 60f;

    public float resetHitTimer;

    public int[] scoreList;
    public int[] pscoreList;

    public int scoreAdd = 10;
    public float seqPlaySpeed = 0.008f;
    public float seqRewindPlaySpeed = 0.008f;
    public int seqAddFrame = 1;
    public float resetTimer = 1f;
    public float stayDuration = 10f;
    public float specialDuration = 15f;

    public List<Texture> maskSeq;
    public List<SequenceSet> transSeq;
    public int maskSeqIndex;

    public DisplayUGUI[] rawImg;
    public RawImage[] maskImg;
    public RawImage resultImg;
    public Material maskMat;
    public MediaPlayer maskMediaPlayer;
    public MediaPlayer winnerTransMediaPlayer;

    public Material[] mats;
    public MediaPlayer[] mediaPlayer;
    public GameObject[] particleGroup;
    public RawImage transRawImg;

    public float transitionStay = 2f;

    [Header("Transition")]
    public DisplayUGUI[] tansDisplay;
    public MediaPlayer[] transMediaPlayer;

    public DisplayUGUI d4SpecialIn, d4SpecialLoop;
    public MediaPlayer m4SpecialIn, m4SpecialLoop;

    [Header("Mask")]
    public MediaPlayer[] dmaskMediaPlayer;

    public bool transitioning;
    public bool gameStart;
    public bool gameEnd;

    int highest = -1;
    int scoreHigh = 0;
    int randomHitAdd = 5;

    public GameObject bg;
    public bool useDissolve;

    float _multiplier = 1f;
    float multiplier = 0.5f;

    public Image black;

    bool specialWinner;
    public List<Image> winnerLED;
    public List<Transform> particlePos;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        RawKeyInput.Start(true);
        RawKeyInput.OnKeyUp += HandleKeyUp;
        RawKeyInput.OnKeyDown += HandleKeyDown;



        Application.targetFrameRate = 60;
        gameEnd = false;
        transitioning = false;

        highest = -1;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetTexture("Mask", null);
        }

        StartCoroutine(UpdateSeqPlayback());
        StartCoroutine(UpdateMultiplier());

        //bg.SetActive(SaveManager.instance.saveData.useBackground);

        seqPlaySpeed = SaveManager.instance.saveData.seqPlaySpeed;
        seqRewindPlaySpeed = SaveManager.instance.saveData.seqRewindPlaySpeed;
        //scoreAdd = SaveManager.instance.saveData.scoreAdd;
        //randomHitAdd = SaveManager.instance.saveData.randomHitAdd;
        resetTimer = SaveManager.instance.saveData.resetTimer;
        stayDuration = SaveManager.instance.saveData.stayDuration;
        specialDuration = SaveManager.instance.saveData.specialDuration;
        //useDissolve = SaveManager.instance.saveData.useDissolve;
        seqAddFrame = SaveManager.instance.saveData.seqAddFrame;
        hitTarget = SaveManager.instance.saveData.hitTarget;
        multiplier = SaveManager.instance.saveData.multiplier;
        specialWinner = SaveManager.instance.saveData.specialWinner;

        for (int i = 0; i < particlePos.Count; i++)
        {
            particlePos[i].localPosition = SaveManager.instance.saveData.particlePos[i];
        }


        for (int i = 0; i < transMediaPlayer.Length; i++)
        {
            transMediaPlayer[i].Events.AddListener((m, et, e) =>
            {
                MediaPlayerEventListener(m, et, e);
            });
        }

    }


    public void SetLEDColor(int _index, Color _cc)
    {
        _cc.a = 0f;
        winnerLED[_index].color = _cc;
    }

    private void HandleKeyUp(RawKey key)
    {
    
    }
    private void HandleKeyDown(RawKey key)
    {
        if(key == RawKey.Prior || key == RawKey.Next)
        {
            RestartScene();
        }
    }

    public void MediaPlayerEventListener(MediaPlayer _player, MediaPlayerEvent.EventType _event, ErrorCode e)
    {
        switch (_event)
        {
            case MediaPlayerEvent.EventType.MetaDataReady:
                break;
            case MediaPlayerEvent.EventType.ReadyToPlay:
                break;
            case MediaPlayerEvent.EventType.Started:
                break;
            case MediaPlayerEvent.EventType.FirstFrameReady:
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                transitioning = false;
                _player.Control.Seek(0);
                _player.Control.Pause();
                break;
            case MediaPlayerEvent.EventType.Closing:
                break;
            case MediaPlayerEvent.EventType.Error:
                break;
            case MediaPlayerEvent.EventType.SubtitleChange:
                break;
            case MediaPlayerEvent.EventType.Stalled:
                break;
            case MediaPlayerEvent.EventType.Unstalled:
                break;
            case MediaPlayerEvent.EventType.ResolutionChanged:
                break;
            case MediaPlayerEvent.EventType.StartedSeeking:
                break;
            case MediaPlayerEvent.EventType.FinishedSeeking:
                break;
            case MediaPlayerEvent.EventType.StartedBuffering:
                break;
            case MediaPlayerEvent.EventType.FinishedBuffering:
                break;
            case MediaPlayerEvent.EventType.PropertiesChanged:
                break;
            case MediaPlayerEvent.EventType.PlaylistItemChanged:
                break;
            case MediaPlayerEvent.EventType.PlaylistFinished:
                break;
            default:
                break;
        }
    }

    bool isreverse;
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

        resultImg.material = maskMat;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageDown))
        {
            RestartScene();
        }

        if (gameStart)
        {
            UpdateGame();
        }

        //if(Input.GetKeyDown(KeyCode.U))
        //{
        //    isreverse = !isreverse;

        //    transMediaPlayer[0].Control.Pause();
        //    dmaskMediaPlayer[0].Control.Pause();
        //}

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    transMediaPlayer[0].Control.Play();
        //    dmaskMediaPlayer[0].Control.Play();
        //}


        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < winnerLED.Count; i++)
            {
                winnerLED[i].rectTransform.DOScaleX(0f, Random.Range(2f, 3f)).SetEase(Ease.InBack);
            }


        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < winnerLED.Count; i++)
            {
                winnerLED[i].rectTransform.DOScaleX(1f, Random.Range(2f, 3f)).SetEase(Ease.InOutBack);
            }
        }
    }

    bool restarting;
    public void RestartScene()
    {

        if (restarting) return;
        restarting = true;

        var cc = black.color;

        DOTween.ToAlpha(() => cc, x => cc = x, 1, 1f).OnUpdate(() =>
        {
            black.color = cc;
        }).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public float reverseTimes;

    private void FixedUpdate()
    {

       if(isreverse)
        {
            var timems = transMediaPlayer[0].Control.GetCurrentTimeMs();

            transMediaPlayer[0].Control.Seek(timems - reverseTimes);
            dmaskMediaPlayer[0].Control.Seek(timems - reverseTimes);

            //Debug.Log("Seeking " + timems);
        }
    }

    public void StartResetCor()
    {
        if(!resetting)
        {
            resetting = true;
            if (resetCor != null)
                StopCoroutine(resetCor);
            resetCor = StartCoroutine(ResetGameCor());
        }
    }

    public void StopResetCor()
    {
        resetting = false;
        if (resetCor != null)
            StopCoroutine(resetCor);
    }

    IEnumerator ResetGameCor()
    {
        yield return new WaitForSeconds(resetDuration);

        RestartScene();

        //if (curGameIndex > 0)
        //{
        //    curGameIndex -= 1;
        //    maskSeqIndex = maskSeq.Count - 1;
        //}
        //resetting = false;
    }

    void UpdateGame()
    {
        highest = -1;
        scoreHigh = 0;

        resetHitTimer -= Time.deltaTime;
        //if (resetHitTimer <= resetTimer - 0.3f)
        //{
        //    gotHit = false;
        //}

        if (resetHitTimer <= 0f)
        {
            gotHit = false;
            resetHitTimer = resetTimer;
            if (currentHitAmount > 0)
            {
                currentHitAmount -= 1;
                if (currentHitAmount < 0)
                    currentHitAmount = 0;
            }
        }
    }

    public void AddScore(int _index)
    {
        if (!gameStart) return;

        if(!transitioning)
        {
            //transitioning = true;
            //transMediaPlayer[0].Control.Play();
            //dmaskMediaPlayer[0].Control.Play();

            var cc = tansDisplay[0].color;
            DOTween.ToAlpha(() => cc, x => cc = x, 1, 2f).OnUpdate(() =>
            {
                tansDisplay[0].color = cc;
            }).OnComplete(() =>
            {

            });
        }

        SoundManager.instance.PlaySFX(_index);

        currentHitAmount += (int)Mathf.Round(1 * Multiplier());
        scoreList[_index] += scoreAdd;
        resetHitTimer = resetTimer;
        gotHit = true;
        StopResetCor();
    }

    public int GetCurrentMaskProgress()
    {
        return (int)Mathf.Ceil(currentHitAmount * maskSeq.Count / hitTarget[curGameIndex]);
    }

    public float Multiplier()
    {
        return _multiplier;
    }

    public IEnumerator UpdateMultiplier()
    {
        while (true)
        {

            float multiply = 1f;
            for (int i = 0; i < scoreList.Length; i++)
            {
                var ii = i;
                if (scoreList[ii] != pscoreList[ii])
                {
                    pscoreList[ii] = scoreList[ii];
                    multiply += 1;
                }
            }

            if(multiply > 2f)
            {
                multiply *= multiplier;
            }
            else
            {
                multiply = 1f;
            }

            _multiplier = multiply;
            yield return new WaitForSeconds(0.5f);
        }
    }


    IEnumerator UpdateSeqPlayback()
    {
        while (true)
        {
            if (!resetting && curGameIndex <= scoreList.Length-1)
            {
                yield return new WaitForSeconds(gotHit?seqPlaySpeed : seqRewindPlaySpeed);
                if (gotHit && maskSeqIndex < GetCurrentMaskProgress())
                {
                    maskSeqIndex += (int)Mathf.Round(seqAddFrame * Multiplier());
                }
                else
                {
                    gotHit = false;
                    maskSeqIndex -= 1;
                }

                maskSeqIndex = Mathf.Clamp(maskSeqIndex, 0, maskSeq.Count-1);
                mats[curGameIndex].SetTexture("Mask", maskSeq[maskSeqIndex]);
                transRawImg.texture = transSeq[curGameIndex].texList[maskSeqIndex];

                if (maskSeqIndex >= maskSeq.Count -1)
                {
                    curGameIndex += 1;
                    maskSeqIndex = 0;
                    currentHitAmount = 0;

                    //Reveal Winner
                    if (curGameIndex >= hitTarget.Length)
                    {
                        gameEnd = true;
                        gameStart = false;

                        PlaySpecial();
                    }
                }
                else
                {
                    if (curGameIndex > 0 && maskSeqIndex <= 0)
                    {
                        StartResetCor();
                    }
                }
            }

            yield return null;
        }
    }


    void ResetGame()
    {
        gameStart = false;
        gameEnd = false;
        transitioning = false;

        curGameIndex = 0;
        currentHitAmount = 0;
        maskSeqIndex = 0;

        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetTexture("Mask", null);

        }

        for (int i = 0; i < scoreList.Length; i++)
        {
            scoreList[i] = 0;
            pscoreList[i] = 0;
        }
    }

    public void ForceReset()
    {
        DOTween.CompleteAll(true);

        StartCoroutine(DelayReset(true));
        StopResetCor();
        StopAndPlayCoroutine(ref specialCor);
        //StopAllCoroutines();
    }



    IEnumerator DelayReset(bool _gameStart = false)
    {
        yield return new WaitForEndOfFrame();
        DOTween.CompleteAll(true);
        yield return new WaitForEndOfFrame();

        ResetGame();
        gameStart = _gameStart;

        StopCountdown();
        Color cc = maskMat.GetColor("MainColor");
        cc.a = 0f;
        maskMat.SetColor("MainColor", cc);
        maskMediaPlayer.Control.Pause();
        maskMediaPlayer.Control.Seek(0);
        maskMediaPlayer.Control.Stop();


        winnerTransMediaPlayer.Control.Pause();
        winnerTransMediaPlayer.Control.Seek(0);
        winnerTransMediaPlayer.Control.Stop();

        m4SpecialIn.Control.Pause();
        m4SpecialIn.Control.Seek(0);
        m4SpecialIn.Control.Stop();

        m4SpecialLoop.Control.Pause();
        m4SpecialLoop.Control.Seek(0);
        m4SpecialLoop.Control.Stop();

        var c1 = d4SpecialIn.color;
        c1.a = 0f;
        d4SpecialIn.color = c1;

        c1 = d4SpecialLoop.color;
        c1.a = 0f;
        d4SpecialLoop.color = c1;
    }

    bool gameEndBegin;

    Coroutine restartCor;

    public IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(stayDuration);
        var ledc = winnerLED[0].color;

        for (int i = 0; i < winnerLED.Count; i++)
        {
            winnerLED[i].color = ledc;
            winnerLED[i].rectTransform.DOScaleX(0f, Random.Range(2f, 3f)).SetEase(Ease.InOutBack);
        }

        yield return new WaitForSeconds(3f);
       //yield return DOTween.ToAlpha(() => ledc, x => ledc = x, 0, 1f).OnUpdate(() =>
       // {
       //     for (int i = 0; i < winnerLED.Count; i++)
       //     {
       //         winnerLED[i].color = ledc;
       //     }

       // }).OnComplete(() =>
       // {

       // }).WaitForCompletion();

        

        m4SpecialIn.Control.Pause();
        m4SpecialIn.Control.Seek(0);
        m4SpecialIn.Control.Stop();

        var c1 = d4SpecialIn.color;
        c1.a = 0f;
        d4SpecialIn.color = c1;

        c1 = d4SpecialLoop.color;
        c1.a = 0f;
        d4SpecialLoop.color = c1;

        ResetGame();

        Color cc = maskMat.GetColor("MainColor");

        DOTween.ToAlpha(() => cc, x => cc = x, 0, 2f).OnUpdate(() =>
        {
            maskMat.color = cc;
            maskMat.SetColor("MainColor", cc);
        }).OnComplete(() =>
        {
            maskMediaPlayer.Control.Pause();
            maskMediaPlayer.Control.Seek(0);
            maskMediaPlayer.Control.Stop();

            winnerTransMediaPlayer.Control.Pause();
            winnerTransMediaPlayer.Control.Seek(0);
            winnerTransMediaPlayer.Control.Stop();

            gameStart = true;
        });
    }

    public void StartCountdownCor()
    {
        StopCountdown();

        restartCor = StartCoroutine(StartCountdown());
    }

    public void StopCountdown()
    {
        if(restartCor != null)
        {
            StopCoroutine(restartCor);
            restartCor = null;
        }
    }

    IEnumerator DelayPlayMask()
    {
        yield return new WaitForSeconds(2.4f);
        maskMediaPlayer.Control.Play();
    }

    [ContextMenu("END")]
    public void PlayResult()
    {
        StartCoroutine(DelayPlayMask());
        winnerTransMediaPlayer.Control.Play();
    }

    Coroutine specialCor;

    public void StopAndPlayCoroutine(ref Coroutine _cor, IEnumerator _ie = null)
    {
        if (_cor != null)
        {
            StopCoroutine(_cor);
        }

        _cor = _ie == null? null : StartCoroutine(_ie);
    }

    [ContextMenu("SPECIAL")]
    public void PlaySpecial()
    {
        StopAndPlayCoroutine(ref specialCor, SpecialIn());
    }

    public IEnumerator SpecialIn()
    {
        Color cc = d4SpecialIn.color;
        m4SpecialIn.Control.Play();

        DOTween.ToAlpha(() => cc, x => cc = x, 1, 1f).OnUpdate(() =>
        {
            d4SpecialIn.color = cc;
        }).OnComplete(() =>
        {
            SoundManager.instance.PlayFinish();
        });


        yield return new WaitForSeconds(specialDuration);

        //yield return new WaitForSeconds(testduration);

        //m4SpecialLoop.Control.SeekFast(testseek);

        //var c2 = d4SpecialLoop.color;
        //yield return DOTween.ToAlpha(() => c2, x => c2 = x, 1f, 0.5f).OnUpdate(() =>
        //{
        //    d4SpecialLoop.color = c2;
        //}).OnComplete(() =>
        //{

        //}).WaitForCompletion();

        //DOTween.ToAlpha(() => cc, x => cc = x, 0f, 0.5f).OnUpdate(() =>
        //{
        //    d4SpecialIn.color = cc;
        //}).OnComplete(() =>
        //{

        //});

        

        yield return new WaitForSeconds(5f);

        SetHighest();
    }


    void SetHighest()
    {
        var highest = 0;
        var score = 0;

        for (int i = 0; i < scoreList.Length; i++)
        {
            if (scoreList[i] > score)
            {
                score = scoreList[i];
                highest = i;
            }
        }
        var ledc = SaveManager.instance.saveData.data[highest].color;
        //ledc.a = 0f;

        for (int i = 0; i < winnerLED.Count; i++)
        {
            winnerLED[i].color = ledc;
            winnerLED[i].rectTransform.DOScaleX(1f, Random.Range(2f, 3f)).SetEase(Ease.InBack).SetDelay(4f);
        }

        //DOTween.ToAlpha(() => ledc, x => ledc = x, 1, 0f).OnUpdate(() =>
        //{
        //    for (int i = 0; i < winnerLED.Count; i++)
        //    {
        //        winnerLED[i].color = ledc;
        //    }

        //}).OnComplete(() =>
        //{

        //});


        if (highest >= 3)
            highest = 4;


        maskMat.SetTexture("MainTex", mats[highest].GetTexture("MainTex"));
        StartCoroutine(DelayPlayMask());
        winnerTransMediaPlayer.Control.Play();

        Color cc = maskMat.GetColor("MainColor");

        DOTween.ToAlpha(() => cc, x => cc = x, 1, 2f).OnUpdate(() =>
        {
            maskMat.color = cc;
            maskMat.SetColor("MainColor", cc);
        }).OnComplete(() =>
        {

            SoundManager.instance.PlayFinale();

        });
        
        StartCountdownCor();

    }
}
