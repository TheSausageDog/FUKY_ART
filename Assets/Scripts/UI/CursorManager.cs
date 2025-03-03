using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoListener
{
    public Image cursorImage;
    public Transform target;
    
    private Camera mainCamera;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        cursorImage.transform.position = mainCamera.WorldToScreenPoint(target.position);
    }

    [Listen(EventType.OnPickingItem)]
    public void SetCursorFill(float fill)
    {
        cursorImage.fillAmount = fill;
    }
}
