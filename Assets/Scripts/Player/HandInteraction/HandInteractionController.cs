using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using UnityEngine;

public class HandInteractionController : MonoListener
{
    public GameObject fukyCursor;
    public GameObject normalCursor;

    public PickUpAndInteract pick;

    protected override void Awake()
    {
        base.Awake();
        // normalCursor.SetActive(!PlayerInputController.IsMoveHandHeld());
        // fukyCursor.SetActive(PlayerInputController.IsMoveHandHeld());
        normalCursor.SetActive(true);
    }

    // void Update()
    // {
    //     if (!PlayerBlackBoard.isHeldObj)
    //     {
    //         normalCursor.SetActive(!funkyControl);
    //         fukyCursor.SetActive(funkyControl);
    //     }
    // }

    [Listen(EventType.OnItemPicked)]
    private void OnItemPicked()
    {
        // if (PlayerInputController.IsMoveHandHeld())
        // {
        //     fukyCursor.SetActive(false);
        // }
        // else
        // {
        normalCursor.SetActive(false);
        // }
    }

    [Listen(EventType.OnItemDrop)]
    private void OnItemDrop()
    {
        // if (PlayerInputController.IsMoveHandHeld())
        // {
        //     fukyCursor.SetActive(true);
        // }
        // else
        // {
        normalCursor.SetActive(true);
        // }
    }


    RaycastHit[] hits = new RaycastHit[128];
    private void FixedUpdate()
    {
        Vector3 dir = Camera.main.transform.forward;
        // funkyControl ? (fukyCursor.transform.position - Camera.main.transform.position) : Camera.main.transform.forward;
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
