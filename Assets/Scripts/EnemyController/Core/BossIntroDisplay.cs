using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntroDisplay : MonoBehaviour
{
    [SerializeField] private Sprite bossSprite;
    [SerializeField] private string bossName;

    void OnEnable()
    {
        MenuUI.Instance.OnDisplayBossIntro(bossName, bossSprite);
    }
}
