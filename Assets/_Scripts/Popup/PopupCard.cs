using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupCard : MonoBehaviour
{
    

    public Action onCloseEvent;

    private R_Card currentRCard;

    [SerializeField] private GameObject popupObject;

    [Header("UI")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image descImage;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button startButton;
    [Space]
    [SerializeField] private CardUIContainer[] playerCardsUI;
    [SerializeField] private CardUIContainer[] extraCardsUI;

    private void Start()
    {
        for (int i = 0; i < playerCardsUI.Length; i++)
        {
            int ind = i;
            playerCardsUI[i].cardEvents.onRemoveCard += () => RemovePCard(ind);
        }
        
        for (int i = 0; i < extraCardsUI.Length; i++)
        {
            int ind = i;
            extraCardsUI[i].cardEvents.onRemoveCard += () => RemoveECard(ind);
        }
    }

    public void OpenPopUp(R_Card _rCard)
    {
        popupObject.SetActive(true);

        _rCard.onCardsChange += CardsChange;
        _rCard.onTimerPhase += TimerChange;
        _rCard.onDestroyCard += ClosePopUp;
        _rCard.onCanLaunch += SetCanLaunch;

        currentRCard = _rCard;
    }

    private void UnlockCardSlots()
    {
        UnlockPCardSlots();
        
        UnlockECardSlots();
        
        // Reward Cards
        // TODO
    }

    private void UnlockPCardSlots()
    {
        // P Cards
        for (int i = 0; i < playerCardsUI.Length; i++)
        {
            if (i >= currentRCard.SelectedCombinaison.basePlayerCards.Length)
                playerCardsUI[i].cardUIObject.SetActive(false);

            playerCardsUI[i].cardUIObject.SetActive(true);
            playerCardsUI[i].cardImage.sprite = null;
        }
    }
    
    private void UnlockECardSlots()
    {
        // E Cards
        for (int i = 0; i < extraCardsUI.Length; i++)
        {
            if (i >= currentRCard.SelectedCombinaison.extraPlayerCards.Length)
                extraCardsUI[i].cardUIObject.SetActive(false);
            
            extraCardsUI[i].cardUIObject.SetActive(true);
            extraCardsUI[i].cardImage.sprite = null;
        }
    }
    
    // TODO
    // To optimize
    private void CardsChange(List<P_Card_SO> _playerCards, List<P_Card_SO> _extraCards)
    {
        // Reset P cards
        UnlockPCardSlots();
        
        for (int i = 0; i < _playerCards.Count; i++)
        {
            playerCardsUI[i].cardImage.sprite = _playerCards[i].image;
        }
        
        // Reset E cards
        UnlockECardSlots();
        
        for (int i = 0; i < _extraCards.Count; i++)
        {
            extraCardsUI[i].cardImage.sprite = _extraCards[i].image;
        }
    }

    private void PhaseChange(string _name, string _description, Sprite _image)
    {
        // fade
        
        nameText.text = _name;
        descriptionText.text = _description;
        descImage.sprite = _image;
    }

    private void TimerChange(string _timer)
    {
        timerText.text = _timer;
    }

    private void SetCanLaunch(bool _canLaunch)
    {
        startButton.interactable = _canLaunch;
    }

    private void ClosePopUp(R_Card _rCard)
    {
        if (_rCard)
        {
            _rCard.onCardsChange -= CardsChange;
            _rCard.onDestroyCard -= ClosePopUp;
            _rCard.onTimerPhase -= TimerChange;
            _rCard.onCanLaunch -= SetCanLaunch;
        }

        onCloseEvent?.Invoke();

        popupObject.SetActive(false);
    }

    public void ClosePopUp()
    {
        ClosePopUp(currentRCard);
    }

    private void RemoveCard(bool _isPlayerCard, int _cardIndex)
    {
        // protection if already launched

        currentRCard.RemoveCard(_isPlayerCard, _cardIndex);
    }
    
    private void RemovePCard(int _cardIndex)
    {
        RemoveCard(true, _cardIndex);
    }
    
    private void RemoveECard(int _cardIndex)
    {
        RemoveCard(false, _cardIndex);
    }
    
    [Serializable]
    private struct CardUIContainer
    {
        public GameObject cardUIObject;
        public PU_CardUI cardEvents;
        public Image cardImage;

        public CardUIContainer(GameObject _cardUIObject, Image _cardImage)
        {
            cardUIObject = _cardUIObject;
            cardImage = _cardImage;

            cardEvents = cardUIObject.GetComponent<PU_CardUI>();
        }
    }
}
