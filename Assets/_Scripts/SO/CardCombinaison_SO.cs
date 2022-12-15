using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Combinaisions", menuName = "ScriptableObjects/Card Combinaisions", order = 2)]
public class CardCombinaison_SO : ScriptableObject
{
    [Title("1 Card")]
    public CombinaisonInformationGroup[] combinaisonGroups1;
    
    [Title("2 Cards")]
    public CombinaisonInformationGroup[] combinaisonGroups2;
    
    [Title("3 Cards")]
    public CombinaisonInformationGroup[] combinaisonGroups3;
    
    [Title("4 Cards")]
    public CombinaisonInformationGroup[] combinaisonGroups4;

    #region Cards Check

    private (bool, Combinaison) GetPCombinaison(CombinaisonInformationGroup[] _group, P_Card_SO[] _mainP)
    {
        for (int i = 0; i < _group.Length; i++)
        {
            List<P_Card_SO> baseCards = _group[i].combinaison.basePlayerCards.ToList();
                    
            List<P_Card_SO> goodCards = new List<P_Card_SO>();

            foreach (P_Card_SO mainPCard in _mainP)
            {
                P_Card_SO pCard = baseCards.Find(plCard => plCard == mainPCard);

                if (pCard == null) continue;
                
                baseCards.Remove(pCard);
                goodCards.Add(pCard);

                if (goodCards.Count == _mainP.Length)
                {
                    return (true, _group[i].combinaison);
                }
            }
        }

        return (false, new Combinaison());
    }
    
    private bool IsExtraCardsGood(Combinaison _combinaison, P_Card_SO[] _mainE)
    {
        List<P_Card_SO> extraCards = _combinaison.extraPlayerCards.ToList();
        
        List<P_Card_SO> goodCards = new List<P_Card_SO>();

        foreach (P_Card_SO mainECard in _mainE)
        {
            P_Card_SO eCard = extraCards.Find(exCard => exCard == mainECard);

            if (eCard == null) continue;
            
            extraCards.Remove(eCard);
            goodCards.Add(eCard);

            if (goodCards.Count == _mainE.Length)
            {
                return true;
            }
        }
        
        return false;
    }

    #endregion
    
    public (bool, Combinaison) GetPotentialCombinaison(P_Card_SO[] _mainP)
    {
        bool isGood = false;
        Combinaison goodCombinaison = new Combinaison();
        
        switch (_mainP.Length)
        {
            case 1:
                (isGood, goodCombinaison) = GetPCombinaison(combinaisonGroups1, _mainP);
                break;
            
            case 2:
                (isGood, goodCombinaison) = GetPCombinaison(combinaisonGroups2, _mainP);
                break;
            
            case 3:
                (isGood, goodCombinaison) = GetPCombinaison(combinaisonGroups3, _mainP);
                break;
            
            case 4:
                (isGood, goodCombinaison) = GetPCombinaison(combinaisonGroups4, _mainP);
                break;
            
            default:
                break;
        }

        return (isGood, goodCombinaison);
    }
    
    public (bool, Combinaison) GetCombinaison(P_Card_SO[] _mainP, P_Card_SO[] _extraP)
    {
        Combinaison nComb = new Combinaison();
        bool arePGood = false;
        
        switch (_mainP.Length)
        {
            case 1:
                (arePGood, nComb) = GetPCombinaison(combinaisonGroups1, _mainP);
                
                if (!arePGood)
                    break;

                if (IsExtraCardsGood(nComb, _extraP))
                    break;

                // If not the good extra cards, set combinaison to "null"
                nComb = new Combinaison();
                arePGood = false;
                
                break;

            case 2:
                (arePGood, nComb) = GetPCombinaison(combinaisonGroups2, _mainP);
                
                if (!arePGood)
                    break;

                if (IsExtraCardsGood(nComb, _extraP))
                    break;

                // If not the good extra cards, set combinaison to "null"
                nComb = new Combinaison();
                arePGood = false;
                
                break;
            
            case 3:
                (arePGood, nComb) = GetPCombinaison(combinaisonGroups3, _mainP);
                
                if (!arePGood)
                    break;

                if (IsExtraCardsGood(nComb, _extraP))
                    break;

                // If not the good extra cards, set combinaison to "null"
                nComb = new Combinaison();
                arePGood = false;
                
                break;
            
            case 4:
                (arePGood, nComb) = GetPCombinaison(combinaisonGroups4, _mainP);
                
                if (!arePGood)
                    break;

                if (IsExtraCardsGood(nComb, _extraP))
                    break;

                // If not the good extra cards, set combinaison to "null"
                nComb = new Combinaison();
                arePGood = false;
                
                break;
            
            default:
                break;
        }

        return (arePGood, nComb);
    }
}

[Serializable]
public struct CombinaisonInformationGroup
{
    public Combinaison combinaison;
}

[Serializable]
public struct Combinaison
{
    [FoldoutGroup("Details"), HideLabel]
    public CombinaisonDetails details;
    
    [FoldoutGroup("Basics"), ReadOnly]
    public R_Card_SO receiverCard;
    [FoldoutGroup("Basics"), PropertySpace(SpaceAfter = 25)]
    public float timerDuration;
    [FoldoutGroup("Basics")] 
    public P_Card_SO[] basePlayerCards;
    [FoldoutGroup("Basics")] 
    public P_Card_SO[] extraPlayerCards;
    
    [FoldoutGroup("Rewards")]
    public R_Card_Reward_SO[] rewardReceiverCards;
    [FoldoutGroup("Rewards")]
    public P_Card_SO[] rewardPlayerCards;
}

[Serializable]
public struct CombinaisonDetails
{
    [Title("Basics")]
    public string name;
    [TextArea] public string description;
    public Sprite image;

    [Title("During Timer")]
    [TextArea] public string descriptionDuringTimer;
    public Sprite imageDuringTimer;
    // Or animation?
    
    [Title("At End")]
    [TextArea] public string descriptionAtEnd;
    public Sprite imageAtEnd;
    // Or animation?
}