using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopRecordPanel : MonoBehaviour
{
    [SerializeField] private Image TopPanel;
    [SerializeField] private TextMeshProUGUI Top;
    [SerializeField] private TextMeshProUGUI Time;


    public void SetTopRecord(int top, string time)
    {
        Top.text = top.ToString();
        Time.text = time;

        TopPanel.color = top switch
        {
            1 => new Color(1, 0.7984424f, 0.0132075f, 1),
            2 => new Color(0.75f, 0.75f, 0.75f, 1),
            3 => new Color(1, 0.4082394f, 0f, 1),
            _ => new Color(0.4f, 0.4f, 0.4f, 1),
        };
    }
}
