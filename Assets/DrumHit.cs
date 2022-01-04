using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class DrumHit : MonoBehaviour
{
    public Transform spawnParticle;
    public Color color;
    public Vector3 direction;
    public Vector3 scale;
    public float speed = 1f;
    Transform trans;

    public KeyCode key;
    public int _index;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key))
        {
            DrumParticle dp = EZ_PoolManager.Spawn(spawnParticle, trans.position, Quaternion.identity).GetComponent<DrumParticle>();
            dp.mR.material.color = color;
            dp.rigid.velocity = direction * speed;
            dp.transform.localScale = scale;
            GameManager.instance.AddScore(_index);
        }
    }
    void OnMessageArrived(string msg)
    {
        Debug.Log("Message arrived: " + msg);
        Spawn();
    }

    public void Spawn()
    {
        DrumParticle dp = EZ_PoolManager.Spawn(spawnParticle, trans.position, Quaternion.identity).GetComponent<DrumParticle>();
        dp.mR.material.color = color;
        dp.rigid.velocity = direction * speed;
        dp.transform.localScale = scale;
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }

    void OnBecameInvisible()
    {
        EZ_PoolManager.Despawn(transform);
    }
}
