using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

public class DebugManager : MonoBehaviour
{
    public ColorPickerControl colorPicker;
    public List<Image> color;
    public InputField speed;
    public InputVector scale, direction;

    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        SaveManager.instance.UpdateData();
        UpdateUI();

        for (int i = 0; i < color.Count; i++)
        {
            int ii = i;
            color[ii].GetComponent<Button>().onClick.AddListener(()=>SetLEDColor(ii));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            canvas.SetActive(!canvas.activeSelf);
        }


        if (Input.GetKey(KeyCode.F2) & Input.GetKeyDown(KeyCode.R))
        {
            SaveManager.instance.Reset();
        }
    }

    public void Save()
    {
        if(SaveManager.instance.saveData == null)
        {
            SaveManager.instance.saveData = new MySaveClass();
        }

        for (int i = 0; i < 4; i++)
        {
            int ii = i;
            SaveManager.instance.saveData.data[ii].color = color[ii].color;
            SaveManager.instance.saveData.data[ii].speed = float.Parse(speed.text);
            SaveManager.instance.saveData.data[ii].scale = scale.GetValue();
            SaveManager.instance.saveData.data[ii].direction = direction.GetValue();
        }

        SaveManager.instance.Save();
        SaveManager.instance.UpdateData();
    }

    public void SetLEDColor(int _index)
    {
        colorPicker.onValueChanged.RemoveAllListeners();
        colorPicker.onValueChanged.AddListener((cc) =>
        {
            color[_index].color = cc;
        });

        colorPicker.gameObject.SetActive(true);
        colorPicker.CurrentColor = color[_index].color;

        //StartCoroutine(DelayUpdateUI());
    }

    public void UpdateUI()
    {
        for (int i = 0; i < color.Count; i++)
        {
            int ii = i;
            color[ii].color = SaveManager.instance.saveData.data[ii].color;
            speed.text = SaveManager.instance.saveData.data[ii].speed.ToString();
            scale.SetValue(SaveManager.instance.saveData.data[ii].scale);
            direction.SetValue(SaveManager.instance.saveData.data[ii].direction);
        }
    }
}
