using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private LevelCamera cameraController;

    public void OnBeginDrag(PointerEventData eventData){}

    public void OnDrag(PointerEventData eventData)
    {
        float moveX = eventData.delta.x;
        float moveY = eventData.delta.y;

        cameraController.MoveCamera(moveX, moveY);
    }

    public void OnEndDrag(PointerEventData eventData){}
}