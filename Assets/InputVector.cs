using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputVector : MonoBehaviour
{
    public InputField x, y, z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(Vector3 _val)
    {
        x.text = _val.x.ToString();
        y.text = _val.y.ToString();
        z.text = _val.z.ToString();
    }

    public Vector3 GetValue()
    {
        return new Vector3(float.Parse(x.text), float.Parse(y.text), float.Parse(z.text));
    }
}
