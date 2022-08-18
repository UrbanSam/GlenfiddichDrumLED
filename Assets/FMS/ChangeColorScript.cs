using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorScript : MonoBehaviour
{
    public List<ParticleSystem> ps;


    public void SetColor(Color cc)
    {
        foreach (var item in ps)
        {
            var pp = item.main;
            pp.startColor = cc; 
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
