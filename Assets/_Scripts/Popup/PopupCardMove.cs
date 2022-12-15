using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupCardMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Drag&Drop

    private Vector3 decalage;
    
    private bool isDragging;
    public bool IsDragging => isDragging;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
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
        newPosition.x = hitResult.worldPosition.x - decalage.x;
        newPosition.y = transform.position.y;
        newPosition.z = hitResult.worldPosition.z - decalage.z;

        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !isDragging)
            return;

        decalage = Vector3.zero;
        isDragging = false;
    }

    #endregion
}
