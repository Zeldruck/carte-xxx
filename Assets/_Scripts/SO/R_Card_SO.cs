using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "R_Card", menuName = "ScriptableObjects/Receiver Card", order = 0)]
public class R_Card_SO : ScriptableObject
{
    [Title("Basics")]
    public string cardName;
    
    [Title("Dependencies")]
    public CardCombinaison_SO combinaisions;
}