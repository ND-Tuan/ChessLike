using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "Data/StatusEffect")]
public class StatusEffectData : ScriptableObject
{
    [Header("Burn")]
        [SerializeField] private int burnDmg;
        [SerializeField] private float burnDuration;
        [SerializeField] private float burnCooldown;

        public int BurnDmg => burnDmg;
        public float BurnDuration => burnDuration;
        public float BurnCooldown => burnCooldown;

    [Header("Freeze")]
        [SerializeField] private int freezeDuration;
        public int FreezeDuration => freezeDuration;

    [Header("Poison")]
        [SerializeField] private int poisonDmg;
        [SerializeField] private float poisonDuration;
        [SerializeField] private float poisonCooldown;

        public int PoisonDmg => poisonDmg;
        public float PoisonDuration => poisonDuration;
        public float PoisonCooldown => poisonCooldown;
    }
