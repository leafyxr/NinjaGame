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
    [SerializeField]
    SpriteRenderer spriteRendererMeter;
    SpriteMask spriteMask;
    Animator animator;

    [SerializeField]
    Color alertColour;
    [SerializeField]
    Color searchColour;

    private void Start()
    {
        spriteMask = GetComponent<SpriteMask>();
        spriteRenderer = GetComponent<SpriteRenderer>();
       // animator = GetComponent<Animator>();
    }

    public void setAlert(bool alert)
    {
        //animator.SetBool("Alert", alert);
       // animator.SetBool("Search", false);
        if (alert)
        {
            spriteRenderer.sprite = alertIcon;
            spriteMask.sprite = alertIcon;

            spriteRendererMeter.color = alertColour;
            spriteRendererMeter.transform.localScale = new Vector3(spriteRendererMeter.transform.localScale.x, 1, spriteRendererMeter.transform.localScale.z);
        }
        else
        {
            spriteRenderer.sprite = null;
            spriteMask.sprite = null;

            spriteRendererMeter.color = alertColour;
            spriteRendererMeter.transform.localScale = new Vector3(spriteRendererMeter.transform.localScale.x, 0, spriteRendererMeter.transform.localScale.z);
        }
    }

    public void setSearch(bool search)
    {
       // animator.SetBool("Search", search);
        //animator.SetBool("Alert", false);
        if (search)
        {
            spriteRenderer.sprite = searchIcon;
            spriteMask.sprite = searchIcon;

            spriteRendererMeter.color = searchColour;
            spriteRendererMeter.transform.localScale = new Vector3(spriteRendererMeter.transform.localScale.x, 1, spriteRendererMeter.transform.localScale.z);
        }
        else
        {
            spriteRenderer.sprite = null;
            spriteMask.sprite = null;

            spriteRendererMeter.color = alertColour;
            spriteRendererMeter.transform.localScale = new Vector3(spriteRendererMeter.transform.localScale.x, 0 , spriteRendererMeter.transform.localScale.z);
        }
    }

    public void alertTimer(float progress)
    {
       // animator.SetBool("Alert", true);
        //animator.SetBool("Search", false);
        spriteRenderer.sprite = alertIcon;
        spriteMask.sprite = alertIcon;

        spriteRendererMeter.color = alertColour;
        spriteRendererMeter.transform.localScale = new Vector3(spriteRendererMeter.transform.localScale.x, progress, spriteRendererMeter.transform.localScale.z);
    }

    public void searchTimer(float progress)
    {
       // animator.SetBool("Alert", false);
       // animator.SetBool("Search", true);
        spriteRenderer.sprite = searchIcon;
        spriteMask.sprite = searchIcon;

        spriteRendererMeter.color = searchColour;
        spriteRendererMeter.transform.localScale = new Vector3(spriteRendererMeter.transform.localScale.x, progress, spriteRendererMeter.transform.localScale.z);
    }
}
