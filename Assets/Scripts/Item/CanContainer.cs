using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer.Samples;
using UnityEngine;

public class CanContainer : NormalPickableItem
{
    public GameObject[] breakingStage;

    public GameObject content;

    public float breakingForce = 0.5f;

    protected int breakingIndex = 0;

    protected bool knifeEnter = false;

    public bool opened { get; protected set; } = false;

    public override void Start()
    {
        base.Start();
        breakingStage[0].SetActive(true);
        breakingIndex = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!opened && !knifeEnter && other.TryGetComponent<BzKnife>(out var _knife))
        {
            Rigidbody knifeRigidbody = _knife.GetComponent<Rigidbody>();
            if (Vector3.Dot(knifeRigidbody.velocity, -transform.up) > 0.75 && knifeRigidbody.velocity.magnitude > breakingForce)
            {
                if (content != null) content.SetActive(true);
                if (breakingStage[breakingIndex] != null) breakingStage[breakingIndex].SetActive(false);
                breakingIndex++;
                if (breakingIndex != breakingStage.Length)
                {
                    if (breakingStage[breakingIndex] != null) breakingStage[breakingIndex].SetActive(true);
                }
                else
                {
                    opened = true;
                    while (content.transform.childCount != 0)
                    {
                        Transform child = content.transform.GetChild(0);
                        child.parent = null;
                        child.gameObject.AddComponent<Rigidbody>();
                        child.gameObject.AddComponent<NormalPickableItem>();
                        child.tag = "canInteract";
                    }
                    Destroy(content);
                }
            }
            knifeEnter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BzKnife>(out var _knife))
        {
            knifeEnter = false;
        }
    }
}
