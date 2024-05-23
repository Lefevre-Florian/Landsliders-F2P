using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private Transform model;

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

    public void SetPlayerRot(Vector3 pos)
    {
        model.LookAt(pos);
        model.eulerAngles = new Vector3(0, model.eulerAngles.y, 0);
    }

    public void SetAnimTrig(AnimTrig pTrig)
    {
        animator.SetTrigger(pTrig.ToString());
    }
}
