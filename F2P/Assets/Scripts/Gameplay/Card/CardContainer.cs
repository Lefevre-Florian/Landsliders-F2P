using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CardContainer : MonoBehaviour
{
    [HideInInspector] public BoxCollider2D boxCollider;

    [SerializeField] public Vector2 size { get; private set; }

    public BoxCollider2D GetBoxCollider() 
    {
        if(boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

        return boxCollider;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
