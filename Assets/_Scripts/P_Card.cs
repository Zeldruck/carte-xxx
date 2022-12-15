using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class P_Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    #region Dragging

    private Vector3 positionBeforeDragging;
    private Vector3 decalage;
    
    private bool isDragging;
    public bool IsDragging => isDragging;
    
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
    
    /*
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("RCard"))
            return;
        
        R_Card tempRCard = other.gameObject.GetComponent<R_Card>();

        if (rCard != null)
        {
            if (Vector3.Distance(transform.position, tempRCard.transform.position) >=
                Vector3.Distance(transform.position, rCard.transform.position))
            {
                // Stop outlining the actual rCard
                
                rCard = tempRCard;
            }
        }
        else
        {
            rCard = tempRCard;
        }
        
        // Outline rCard
    }

    private void OnTriggerExit(Collider other)
    {
        if (rCard == null || !other.CompareTag("RCard"))
            return;
        
        R_Card tempRCard = other.gameObject.GetComponent<R_Card>();
        
        if (tempRCard != rCard)
            return;
        
        // Stop outlining the actual rCard
        rCard = null;
    }*/

    #endregion
    
    private R_Card rCard;
    [SerializeField] private P_Card_SO pCardSo;
    public P_Card_SO CardSO => pCardSo;

    private float snapSize;

    private void OnEnable()
    {
        rCard = null;
    }

    public void DropCard()
    {
        Debug.Log("Drop Card");
        
        // Move card if in obstructed place
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, transform.localScale / 2f, transform.up, transform.rotation);

        foreach (RaycastHit raycastHit in hits)
        {
            if (raycastHit.collider.gameObject == gameObject || !raycastHit.collider.CompareTag("RCard"))
                continue;

            // Add card to rCard
            rCard = raycastHit.collider.GetComponent<R_Card>();
            
            if (rCard == null)
                continue;
            
            bool isGoodCard = rCard.AddPCard(this);

            if (isGoodCard)
            {
                //Destroy(gameObject);
                gameObject.SetActive(false);
                return;
            }
        }
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
                continue;

            if (hit.collider.gameObject.CompareTag("RCard"))
            {
                // Reset to before dragging pos
                transform.position = positionBeforeDragging;
            }
            else
            {
                // Try to push to the right
                bool hasBeenPushed = false;
                RaycastHit tHit = hit;
                
                while (!hasBeenPushed)
                {
                    Transform hitTransform = tHit.collider.transform;

                    RaycastHit[] nHits = Physics.BoxCastAll(hitTransform.position, hitTransform.localScale / 2f,
                        hitTransform.forward, hitTransform.rotation);

                    bool canBePushed = true;
                    
                    foreach (RaycastHit nHit in nHits)
                    {
                        if (nHit.collider.gameObject == gameObject || nHit.collider.gameObject == hitTransform.gameObject) continue;

                        canBePushed = false;
                        tHit = nHit;
                        
                        break;
                    }

                    if (!canBePushed)
                        continue;
                    
                    transform.position = hitTransform.position + hitTransform.right *
                        (hitTransform.localScale.x / 2f + transform.localScale.x / 2f);
                    
                    hasBeenPushed = true;
                }
            }
            
            Debug.Log("Drop card obstructed", hit.collider.gameObject);
            break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging)
            return;
        
        Debug.Log("Click");
    }
}
