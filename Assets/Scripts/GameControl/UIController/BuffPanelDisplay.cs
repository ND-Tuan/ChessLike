using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ObserverPattern;

public class BuffPanelDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _BuffName;
    [SerializeField] private TextMeshProUGUI _BuffDescription;
    [SerializeField] private GameObject UpgradedIcon;
    [SerializeField] private int id ;

    public void DisplayBuff(string name, string description, bool isUpgraded)
    {
        _BuffName.text = name;
        _BuffDescription.text = description;
        UpgradedIcon.SetActive(isUpgraded);

    }

    public void SetBuffID(int id)
    {
        this.id = id;
    }   

    public void OnClick()
    {
       Observer.PostEvent(EvenID.ReturnBuffSelected, id);
    }
}
