using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunFrame : MonoBehaviour
{
    [SerializeField] private Image Icon;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Dmg;
    [SerializeField] private TextMeshProUGUI CritDmg;
    [SerializeField] private TextMeshProUGUI CoolDown;
    [SerializeField] private TextMeshProUGUI AmmoCapacity;
    [SerializeField] private TextMeshProUGUI Cost;

    public void SetGunFrame(object[] data, int SelectIndex)
    {
        GunInfo gunInfo = (GunInfo)data[0];

        //detach info for UI
        Icon.sprite         = gunInfo.Icon;
        Name.text           = gunInfo.Name;
        Dmg.text            = gunInfo.Damage.ToString();
        CritDmg.text        = gunInfo.CritDamage.ToString();
        CoolDown.text       = (gunInfo.Cooldown*100).ToString();
        AmmoCapacity.text   = gunInfo.AmmoCapacity.ToString();
        Cost.text           = gunInfo.Cost.ToString();

        //Set index for selection
        GetComponent<Button>().onClick.AddListener(() => MenuUI.Instance.GetOnSelect(SelectIndex));

    }

}
