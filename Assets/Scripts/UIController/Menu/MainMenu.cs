using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _RecordPanel;
    [SerializeField] private List<GameObject> _TopRecordPrefab = new List<GameObject>();

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Back()
    {
        _RecordPanel.SetActive(false);
    }

    public void RecordPanel()
    {
        _RecordPanel.SetActive(true);

        List<float> records = GetRecordList();
        for (int i = 0; i < records.Count; i++)
        {      
            int minutes = Mathf.FloorToInt(records[i] / 60f);
            int seconds = Mathf.RoundToInt(records[i] % 60f);

            if (seconds == 60)
            {
                seconds = 0;
                minutes += 1;
            }
            string Str = minutes.ToString("00") + "m" + seconds.ToString("00") + "s";

            if (minutes == 0) Str = seconds.ToString("00") + "s";
        
            _TopRecordPrefab[i].SetActive(true);
            _TopRecordPrefab[i].GetComponent<TopRecordPanel>().SetTopRecord(i + 1, Str);
            
        }

    }

     private List<float> GetRecordList(){
        string path = Application.dataPath + "/Record.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            RecordData data = JsonUtility.FromJson<RecordData>(json);
            return data.times;
        }
        else
        {
            return new List<float>();
        }
    }


}
