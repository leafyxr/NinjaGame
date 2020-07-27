using System.Collections;
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
        if (patrolComponent.getAlert())
        {
            ViewCheck();

            if (isAttacking && targetVisible)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("GunnerFire"))
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9)
                    {
                        animationEnd();
                    }
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
        }
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
        GameObject projectile = Instantiate(bulletPrefab, Fireposition.position, Fireposition.rotation);
        projectile.GetComponent<Projectile>().setTarget(target);
        projectile.GetComponent<Projectile>().setDamage(GetComponent<Enemy>().GetStats().RangedAttackDamage);
    }
}
