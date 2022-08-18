using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable] public class VoidEvent : UnityEvent { }

public class OSCReceiver : MonoBehaviour
{
    [SerializeField] VoidEvent voidEvent = null;
    public OSC osc;
    public string address;


    // Use this for initialization
    void Start()
    {
        osc.SetAddressHandler(address, OnReceive);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnReceive(OscMessage message)
    {
                Debug.Log(address+"   :  "+message.values);
        if (message.values.Count > 0)
        {
            for (int i = 0; i < message.values.Count; i++)
            {
                voidEvent?.Invoke();
                Debug.Log(address+"   :  "+message.values[i]);
            }
        }
    }

    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }

}
