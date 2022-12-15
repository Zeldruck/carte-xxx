using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDragger : MonoBehaviour
{
    private Camera mainCam;
    private EventSystem eventSystem;

    private Vector3 draggableDBS;
    private bool isPopup;
    private IIsDraggable draggable;

    private float snapSize;

    [SerializeField] private LayerMask boardMask;

    private void Start()
    {
        mainCam = Camera.main;
        eventSystem = EventSystem.current;
        
        snapSize = GameManager.instance.board.SnapSize;
    }

    private void Update()
    {
        //LookForCard();
        
        if (Input.GetMouseButton(0))
        {
            if (draggable == null)
            {
                if (eventSystem.IsPointerOverGameObject())
                {
                    PointerEventData pointerEventData = new PointerEventData(eventSystem)
                    {
                        position = Input.mousePosition
                    };

                    List<RaycastResult> hits = new List<RaycastResult>();
                    eventSystem.RaycastAll(pointerEventData, hits);

                    if (hits.Count <= 0)
                        return;
                    
                    Transform canvasTransform = hits[0].gameObject.transform.root;
                        
                    IIsDraggable tempDraggable = canvasTransform.GetComponent<IIsDraggable>();

                    if (tempDraggable == null)
                        return;
                    
                    draggable = tempDraggable;
                    draggable.StartDragging();

                    isPopup = true;

                    draggableDBS = new Vector3(hits[0].worldPosition.x - canvasTransform.position.x, 0f, hits[0].worldPosition.z - canvasTransform.position.z);
                }
                else if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                {
                    IIsDraggable tempDraggable = hit.collider.gameObject.GetComponent<IIsDraggable>();

                    if (tempDraggable == null)
                        return;
                    
                    draggable = tempDraggable;
                    draggable.StartDragging();

                    isPopup = false;
                        
                    draggableDBS = new Vector3(hit.point.x - hit.transform.position.x, 0f, hit.point.z - hit.transform.position.z);
                }
            }
            else
            {
                if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, boardMask))
                    return;
                
                Vector3 nPos = hit.point;

                if (!isPopup)
                {
                    nPos.x = Mathf.Round((nPos.x - draggableDBS.x) / snapSize) * snapSize;
                    nPos.z = Mathf.Round((nPos.z - draggableDBS.z) / snapSize) * snapSize;
                }
                else
                {
                    nPos.x -= draggableDBS.x;
                    nPos.z -= draggableDBS.z;
                }
                    
                nPos.y = 0f;

                draggable.Drag(nPos);
            }
        }
        else if (draggable != null)
        {
            draggable.StopDragging();
            draggable = null;
            //draggableDBS = Vector3.zero;
        }
    }

    private void LookForCard()
    {
        if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            IIsDraggable tempDraggable = hit.collider.gameObject.GetComponent<IIsDraggable>();

            if (tempDraggable == null)
            {
                if (draggable != null)
                {
                    // Stop outlining
                    draggable.StopDragging();
                    draggable = null;
                }
                
                return;
            }

            if (tempDraggable == draggable)
                return;
            
            if (draggable != null)
            {
                // Stop outlining
                draggable.StopDragging();
            }

            draggable = tempDraggable;
            // Outline
        }
    }
}

public interface IIsDraggable
{
    public void Drag(Vector3 _position);
    public void DropCard();

    public void StartDragging();
    public void StopDragging();
}