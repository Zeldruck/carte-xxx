using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "P_Card", menuName = "ScriptableObjects/Player Card", order = 1)]
public class P_Card_SO : ScriptableObject
{
    [Title("Basics")]
    public string cardName;
    public string description;
    public Sprite image;
}