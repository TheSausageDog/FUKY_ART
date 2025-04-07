using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using UnityEngine;

public class HandInteractionController : MonoListener
{
    public GameObject fukyCursor;
    public GameObject normalCursor;
    private bool funkyControl;

    public PickUpAndInteract pick;

    protected override void Awake()
    {
        base.Awake();
        funkyControl = false;
        normalCursor.SetActive(!funkyControl);
        fukyCursor.SetActive(funkyControl);
    }

    void Update()
    {
        if (PlayerInputController.IsLeftShiftPressed())
        {
            funkyControl = !funkyControl;
            if (!PlayerBlackBoard.isHeldObj)
            {
                normalCursor.SetActive(!funkyControl);
                fukyCursor.SetActive(funkyControl);
            }
        }
    }

    [Listen(EventType.OnItemPicked)]
    private void OnItemPicked(BasePickableItem item)
    {
        if (funkyControl)
        {
            fukyCursor.SetActive(false);
        }
        else
        {
            normalCursor.SetActive(false);
        }
    }

    [Listen(EventType.OnItemDrop)]
    private void OnItemDrop()
    {
        if (funkyControl)
        {
            fukyCursor.SetActive(true);
        }
        else
        {
            normalCursor.SetActive(true);
        }
    }


    RaycastHit[] hits = new RaycastHit[128];
    private void FixedUpdate()
    {
        Vector3 dir = funkyControl ? (fukyCursor.transform.position - Camera.main.transform.position) : Camera.main.transform.forward;
        int count = Physics.RaycastNonAlloc(Camera.main.transform.position, dir, hits, pick.pickUpRange);

        Array.Sort(hits, 0, count, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

        for (int i = 0; i < count; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("canInteract"))
            {
                pick.OnHandTriggerEnter(hits[i].collider.gameObject);
                //                Debug.Log(hits[i].collider.gameObject.name);
                return;
            }
        }
        if (pick.currentHandObj != null) pick.OnHandTriggerExit(pick.currentHandObj._transform.gameObject);
    }
}
