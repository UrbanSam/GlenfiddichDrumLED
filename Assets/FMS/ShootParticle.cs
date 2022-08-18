using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShootParticle : MonoBehaviour
{
    public Vector3 origin;
    public Transform trans;
    public float moveAmount = 0.5f;
    public ChangeColorScript cc;

    public int amount = 500;
    public ParticleSystem[] ps;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        origin = trans.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Coroutine cor;

    void StartReturn()
    {
        if(cor != null)
        {
            StopCoroutine(cor);
        }

        cor = StartCoroutine(ResetPos());
    }

    IEnumerator ResetPos()
    {
        yield return new WaitForSeconds(0.1f);
        trans.position = origin;
    }

    public void Move()
    {
        //trans.Translate(Vector3.forward * moveAmount);

        //ps.Emit(amount);
        foreach (var item in ps)
        {
            item.Play();
        }
        //StartReturn();
    }
}
