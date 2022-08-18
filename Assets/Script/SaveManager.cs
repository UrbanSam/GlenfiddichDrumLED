using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class MySaveClass
{
    public List<MyData> data;
    public List<string> oscAddress;
    public float resetTimer;
    public int scoreTarget;
    public int scoreAdd;
    public int randomHitAdd;
    public float stayDuration;
    public float specialDuration;
    public float seqPlaySpeed;
    public float seqRewindPlaySpeed;
    public float multiplier;
    public int seqAddFrame;
    public bool useBackground;
    public bool useDissolve;
    public int[] hitTarget;
    public bool specialWinner;
    public float dimLED;
    public Vector3[] particlePos;
    public float sfxVolume;

    public MySaveClass()
    {
        data = new List<MyData>();
    }
}

[System.Serializable]
public class MyData
{
    public Color color;
    public Vector3 scale;
    public Vector3 direction;
    public float speed;

    public MyData(Color _color, Vector3 _scale, Vector3 _direction, float _speed)
    {
        color = _color;
        scale = _scale;
        direction = _direction;
        speed = _speed;
    }
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public MySaveClass saveData;

    public List<DrumHit> drumHit;
    public List<OSCReceiver> osc;


    private void Awake()
    {
        instance = this;
        Load();

        for (int i = 0; i < osc.Count; i++)
        {
            int ii = i;
            osc[ii].address = saveData.oscAddress[ii];
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

    public void UpdateData()
    {
        for (int i = 0; i < saveData.data.Count; i++)
        {
            try
            {
                int ii = i;
                drumHit[ii].color = saveData.data[ii].color;
                drumHit[ii].scale = saveData.data[ii].scale;
                drumHit[ii].direction = saveData.data[ii].direction;
                drumHit[ii].speed = saveData.data[ii].speed;
                drumHit[ii].shootParticle.cc.SetColor(saveData.data[ii].color);
                //Game2Manager.instance.SetLEDColor(ii, saveData.data[ii].color);
            }
            catch (System.Exception)
            {

            }
        }

    }

    public void Save()
    {
        string json = JsonUtility.ToJson(saveData);
        StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + "/save.json", false);
        writer.WriteLine(json);
        writer.Close();

        Debug.Log("SAVED"); 
    }


    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void Load()
    {
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/save.json");
        saveData = JsonUtility.FromJson<MySaveClass>(reader.ReadToEnd());
        reader.Close();
    }
}
