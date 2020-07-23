﻿using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    [SerializeField]
    Transform Fireposition;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    Vector2 target;
    bool targetVisible = false;

    bool isAttacking = false;

    Animator animator;

    float attackDelay = 1.2f;
    float clock = 0.0f;

    Patrol patrolComponent;

    // Start is called before the first frame update
    void Start()
    {
        patrolComponent = GetComponent<Patrol>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
         ViewCheck();

        if (isAttacking && targetVisible)
        {
            if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("GunnerFire"))
            {
                Debug.Log("Animation not running");
                animationEnd();
            }
            clock += Time.deltaTime;
            if (clock >= attackDelay)
            {
                patrolComponent.pausePatrol(true);
                animator.SetTrigger("Attack");
                clock = 0.0f;
            }
        }
        else if (!isAttacking && targetVisible)
        {
            clock += Time.deltaTime;
            isAttacking = true;
        }
        else if (isAttacking && !targetVisible)
        {
            animationEnd();
            isAttacking = false;
            clock = 0.0f;
        }
        else animationEnd();

    }

    void ViewCheck()
    {
        Collider2D[] colliders = GetComponentInChildren<FOV>().getDetections();

        targetVisible = false;
        if (colliders != null)
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    targetVisible = GetComponentInChildren<FOV>().CheckObstruction(collider);
                    if (targetVisible)
                    {
                        target = collider.ClosestPoint(transform.position);
                        return;
                    }
                }
            }
    }

    public void animationEnd()
    {
        patrolComponent.pausePatrol(false);
    }

    public void Fire()
    {

    }
}
