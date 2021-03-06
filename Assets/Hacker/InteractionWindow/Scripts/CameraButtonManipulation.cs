﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CameraButtonManipulation : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Camera associatedCamera;
    public Vector2 startPosition;
    public HotkeyCameraSwap hotkeySwap;

    bool isOverInteractionWindow = true;
    // Use this for initialization
    void Start()
    {
        hotkeySwap = GameObject.FindGameObjectWithTag("HackerCameraGlow").GetComponent<HotkeyCameraSwap>();
    }

    public void DelayedStart()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //calls HotkeyCameraSwap to swap cameras
    public void SwapCamera()
    {
        hotkeySwap.SwapCamera(associatedCamera);
    }

    // OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse
    public void OnMouseDrag()
    {
        GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().anchoredPosition += eventData.delta;
        if (eventData.hovered.Contains(GetComponent<RectTransform>().parent.gameObject))
        {
            isOverInteractionWindow = true;
        }
        else
        {
            isOverInteractionWindow = false;
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        startPosition = GetComponent<RectTransform>().position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (isOverInteractionWindow)
        {
            eventData.pointerDrag.GetComponent<CameraButtonManipulation>().startPosition = eventData.pointerDrag.GetComponent<RectTransform>().position;
        }
        else
        {
            eventData.pointerDrag.GetComponent<RectTransform>().position = eventData.pointerDrag.GetComponent<CameraButtonManipulation>().startPosition;
        }
    }
}
