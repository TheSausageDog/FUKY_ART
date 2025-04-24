using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer.Samples;
using UnityEngine;

public class CanContainer : NormalPickableItem
{
    public GameObject[] breakingStage;

    public GameObject content;

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
                    content.transform.GetChild(0).parent = null;
                }
                Destroy(content);
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
