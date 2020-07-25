using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIcons : MonoBehaviour
{

    [SerializeField]
    Sprite alertIcon;
    [SerializeField]
    Sprite searchIcon;

    SpriteRenderer spriteRenderer;
    Animator animator;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void setAlert(bool alert)
    {
        animator.SetBool("Alert", alert);
        animator.SetBool("Search", false);
        if (alert) spriteRenderer.sprite = alertIcon;
        else spriteRenderer.sprite = null;
    }

    public void setSearch(bool search)
    {
        animator.SetBool("Search", search);
        animator.SetBool("Alert", false);
        if (search) spriteRenderer.sprite = searchIcon;
        else spriteRenderer.sprite = null;
    }
}
