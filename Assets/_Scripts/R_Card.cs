using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class R_Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    #region Dragging
    
    private Vector3 positionBeforeDragging;
    private Vector3 decalage;
    
    private bool isDragging;
    public bool IsDragging => isDragging;

    public Action<R_Card> onClikcCard;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        positionBeforeDragging = transform.position;
        snapSize = GameManager.instance.board.SnapSize;

        RaycastResult hitResult = eventData.pointerPressRaycast;
        
        decalage = new Vector3(hitResult.worldPosition.x - transform.position.x, 0f, hitResult.worldPosition.z - transform.position.z);

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !isDragging)
            return;

        RaycastResult hitResult = eventData.pointerCurrentRaycast;
        
        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.Round((hitResult.worldPosition.x - decalage.x) / snapSize) * snapSize;
        newPosition.y = 3f;
        newPosition.z = Mathf.Round((hitResult.worldPosition.z - decalage.z) / snapSize) * snapSize;

        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !isDragging)
            return;
        
        transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
        
        decalage = Vector3.zero;
        isDragging = false;
        
        DropCard();
    }
    
    public void DropCard()
    {
        // Move card if in obstructed place
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, transform.localScale / 2f, transform.up,
            transform.rotation);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;

            transform.position = positionBeforeDragging;

            Debug.Log("Receiver card obstructed", hit.collider.gameObject);
            break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging)
            return;
        
        onClikcCard?.Invoke(this);
    }

    #endregion

    private float snapSize;

    [SerializeField] private R_Card_SO rCardInformation;
    private bool hasSelectedCombinaison;
    private Combinaison selectedCombinaison;
    public Combinaison SelectedCombinaison => selectedCombinaison;
    private RCE_Base[] rCardEvents;
    
    private List<P_Card> pCards;
    private List<P_Card_SO> pCardsSo;
    
    private List<P_Card> extraCards;
    private List<P_Card_SO> extraCardsSo;

    private bool canLaunch = false, hasActionStarted = false, hasActionEnded = false;
    private float cardActionTimer = 0f;

    public Action<bool> onCanLaunch;
    public Action<string> onTimerPhase;
    private Action<P_Card_SO> onAddPCard, onRemovePCard;
    private Action<P_Card_SO> onAddECard, onRemoveECard;

    public Action<List<P_Card_SO>, List<P_Card_SO>> onCardsChange;

    public Action<R_Card> onDestroyCard;

    private void Awake()
    {
        pCards = new List<P_Card>();
        pCardsSo = new List<P_Card_SO>();

        extraCards = new List<P_Card>();
        extraCardsSo = new List<P_Card_SO>();
    }

    public bool AddPCard(P_Card _pCard)
    {
        if (_pCard == null)
            return false;

        if (!hasSelectedCombinaison)
        {
            pCards.Add(_pCard);
            pCardsSo.Add(_pCard.CardSO);

            (hasSelectedCombinaison, selectedCombinaison) = rCardInformation.combinaisions.GetPotentialCombinaison(pCardsSo.ToArray());

            if (!hasSelectedCombinaison)
            {
                pCards.Remove(_pCard);
                pCardsSo.Remove(_pCard.CardSO);
                
                return false;
            }
        }
        else
        {
            if (extraCards.Count >= selectedCombinaison.extraPlayerCards.Length)
                return false;

            bool isGoodExtra = Array.Exists(selectedCombinaison.extraPlayerCards, (x) => x == _pCard.CardSO);
        
            if (!isGoodExtra || extraCards.Contains(_pCard))
                return false;
        
            extraCards.Add(_pCard);
            extraCardsSo.Add(_pCard.CardSO);
        }

        Combinaison _temp;
        
        // Test if all cards are good and we can launch
        (canLaunch, _temp) = rCardInformation.combinaisions.GetCombinaison(pCardsSo.ToArray(), extraCardsSo.ToArray());
        
        // Delegate for board, to tell it that's ready or not
        onCanLaunch?.Invoke(canLaunch);
        onCardsChange?.Invoke(pCardsSo, extraCardsSo);
        
        return true;
    }

    /*
     * TODO
     */
    public bool RemoveCard(bool _isPlayerCard, int _cardIndex)
    {
        if (pCards == null)
            return false;

        P_Card card;
        
        if (_isPlayerCard)
        {
            if (_cardIndex < pCards.Count)
            {
                card = pCards[_cardIndex];
                
                pCards.RemoveAt(_cardIndex);
                pCardsSo.RemoveAt(_cardIndex);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (_cardIndex < extraCards.Count)
            {
                card = extraCards[_cardIndex];
                
                extraCards.RemoveAt(_cardIndex);
                extraCards.RemoveAt(_cardIndex);
            }
            else
            {
                return false;
            }
        }
        
        // Event cards change
        onCardsChange?.Invoke(pCardsSo, extraCardsSo);

        /*
         * TODO
         */
        // set last position
        card.gameObject.SetActive(true);
        card.transform.position = Vector3.zero;
        card.DropCard();

        return true;
    }

    /*
     * TODO
     * Add R card event timers (maybe in a r event manager)
     */
    private void Update()
    {
        if (hasActionStarted && !hasActionEnded)
        {
            RDuring();
        }
    }

    /*
     * TODO
     */
    public void RStart()
    {
        
    }

    /*
     * TODO
     */
    private void RDuring()
    {
        if (cardActionTimer > 0f)
        {
            cardActionTimer -= Time.deltaTime;
            onTimerPhase?.Invoke("During");
        }
        else
        {
            REnd();
        }
    }

    /*
     * TODO
     */
    private void REnd()
    {
        hasActionEnded = true;
        cardActionTimer = 0f;
        
        onTimerPhase?.Invoke("End");
    }

    /*
     * TODO
     */
    public void RCollect()
    {
        
    }
}
