using UnityEngine;

[CreateAssetMenu(fileName = "R Card Reward", menuName = "ScriptableObjects/R Card Reward", order = 3)]
public class R_Card_Reward_SO : ScriptableObject
{
    public R_Card_SO rewardReceiverCard;
    public RCE_Base[] rewardCardEvents;
}