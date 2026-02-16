using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStepCollider : MonoBehaviour
{
    public PlaySteps ps;

    [SerializeField]
    public int nextIndex;

    private void Awake()
    {
        ps = FindAnyObjectByType(typeof(PlaySteps)) as PlaySteps;
    }

    private void OnTriggerEnter(Collider other)
    {
        ps.PlayStep(nextIndex);
    }
}
