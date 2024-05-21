using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator animator;

    public enum AnimTrig
    {
        Transition,
        LoseCard,
        GainCard,
        Departure
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetAnimTrig(AnimTrig pTrig)
    {
        animator.SetTrigger(pTrig.ToString());
    }
}
