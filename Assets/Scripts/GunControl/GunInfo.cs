using UnityEngine;

[CreateAssetMenu(fileName ="GunInfo", menuName = "ScriptableObject/GunInfo")]
public class GunInfo : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    public int Cost;
    public int Damage;
    public int CritDamage;
    public float Cooldown;
    public int AmmoCapacity;

}
