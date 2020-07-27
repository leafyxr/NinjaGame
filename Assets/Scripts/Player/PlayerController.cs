using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D PlayerBody;

    [SerializeField]
    float PlayerSpeed = 5.0f;

    [SerializeField]
    float MaxSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 5.0f;
    [SerializeField]
    float AirSpeed = 5.0f;

   

    [SerializeField]
    float CrouchDrag;

       [SerializeField]
    LayerMask groundLayer;

    bool InputEnabled = true;

    bool jumping = false;
    bool falling = false;

    bool crouched = false;
    bool isRight = false;

    Animator animator;

    

    // Start is called before the first frame update
    void Start()
    {
        PlayerBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }

    private void Update()
    {

        if (Input.GetButtonDown("Melee"))
        {
            animator.SetTrigger("Attack");

            animator.SetInteger("Attack ID", UnityEngine.Random.Range(1, 3));
        }

        if (!crouched && IsGrounded() && !jumping && Input.GetButtonDown("Jump"))
        {
            PlayerBody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            animator.SetBool("Jumping", true);
            jumping = true;
            falling = false;
        }
    }
       

    // Update is called once per frame
    void FixedUpdate()
    {
        if (InputEnabled)
        {
            if (falling && IsGrounded())
            {
                animator.SetBool("Falling", false);
                falling = false;
            }

            if (!IsGrounded() && !jumping)
            {
                animator.SetBool("Falling", true);
                falling = true;
            }

            if (jumping && PlayerBody.velocity.y < 0)
            {
                animator.SetBool("Jumping", false);
                animator.SetBool("Falling", true);
                falling = true;
                jumping = false;
            }

            if (IsGrounded())
            {
                if (Input.GetButton("Crouch"))
                {
                    crouched = true;
                    animator.SetBool("Crouched", crouched);
                    PlayerBody.velocity = new Vector2(PlayerBody.velocity.x * (1 - Time.deltaTime * CrouchDrag), PlayerBody.velocity.y);
                }
                else
                {
                    crouched = false;
                    animator.SetBool("Crouched", crouched);
                }
            }

            if (!crouched)
            {
                float direction = Input.GetAxis("Horizontal");

                if (direction != 0 && IsGrounded())
                {
                    Vector2 force = new Vector2(direction * PlayerSpeed, 0);

                    PlayerBody.AddForce(force, ForceMode2D.Force);

                    if (direction < 0) isRight = true;
                    else isRight = false;

                    animator.SetBool("Moving", true);

                }
                else if (direction != 0)
                {
                    animator.SetBool("Moving", false);
                    Vector2 force = new Vector2(direction * AirSpeed, 0);
                    PlayerBody.AddForce(force, ForceMode2D.Force);
                    if (direction < 0) isRight = true;
                    else isRight = false;
                }
                else if (IsGrounded())
                {
                    animator.SetBool("Moving", false);
                    PlayerBody.velocity = new Vector2(0, PlayerBody.velocity.y);
                }
                else
                {
                    animator.SetBool("Moving", false);
                }
            }
            else
            {

            }

            if (Mathf.Abs(PlayerBody.velocity.x) > MaxSpeed)
            {
                PlayerBody.velocity = new Vector2(MaxSpeed * Mathf.Sign(PlayerBody.velocity.x), PlayerBody.velocity.y);
            }

            animator.SetFloat("SpeedMultiplier", Mathf.Abs(PlayerBody.velocity.x) / MaxSpeed);

            GetComponent<SpriteRenderer>().flipX = isRight;
        }
    }

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;

        //Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);

        if (hit) Debug.DrawRay(position, direction, Color.green);
        else Debug.DrawRay(position, direction, Color.red);

        return hit;
    }



}
