
using UnityEngine;

[CreateAssetMenu(fileName ="SpecificBoard", menuName = "ScriptableObject/SpecificBoard")]
public class SpecificBoard : ScriptableObject
{
    public Material BoardIcon;
    public string Message;
    public GameObject Prefab;
}
