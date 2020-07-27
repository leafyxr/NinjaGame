using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    enum State { Follow, Search, Idle, Wander, Pause};
    [SerializeField]
    State state;
    State lastState;

    bool following = false;
    bool searching = false;

    bool searchLocationReached = false;

    [SerializeField]
    float searchTime = 10;

    float wanderRange = 10;

    float IdleTime = 1;

    float clock = 0;

    float alertTimer = 0;

    [SerializeField]
    float alertTIme = 2;

    [SerializeField]
    Vector2 target;
    bool targetVisable = false;

    [SerializeField]
    float MinDistance = 2.0f;
    [SerializeField]
    float MaxDistance = 5.0f;

    Rigidbody2D body;
    Animator animator;

    [SerializeField]
    float speed;

    bool isLeft = false;

    bool paused = false;

    GroundCheck groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        isLeft = false;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundCheck = GetComponent<GroundCheck>();
    }

    private void FixedUpdate()
    {
        if (state == State.Wander && lastState != state)
        {
            Debug.Log("Wander Start");
            clock = 0;
            float dist = (UnityEngine.Random.Range(0, 2) * 2 - 1) * UnityEngine.Random.Range(1, wanderRange + 1);

            if ((Mathf.Sign(dist) == 1 && !groundCheck.checkGroundR())
           || (Mathf.Sign(dist) == -1 && !groundCheck.checkGroundL()))
            {
                Debug.Log("Flip Wander Direction");
                dist *= -1;
            }

            target = new Vector2(transform.position.x + dist, transform.position.y);

            searchLocationReached = false;
        }
        else if (state == State.Idle && lastState != state)
        {
            Debug.Log("Idle Start");
            clock = 0;
            IdleTime = UnityEngine.Random.Range(2, 6);
            animator.SetBool("Moving", false);
        }

        lastState = state;

        bool lastDir = isLeft;

        ViewCheck();

        if (searching && !targetVisable && !searchLocationReached)
        {
            if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().setSearch(true);
        }
        else if (following && targetVisable)
        {
            if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().setAlert(true);
        }
        else if (alertTimer == 0 && !searching)
        {
            if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().setAlert(false);
        }

        switch (state)
        {
            case State.Follow:
                Follow();
                break;
            case State.Search:
                Search();
                break;
            case State.Idle:
                Idle();
                break;
            case State.Wander:
                Wander();
                break;
            case State.Pause:
                body.velocity = new Vector2(0, body.velocity.y);
                break;
        }


    }

    void ViewCheck()
    {
        Collider2D[] colliders = GetComponentInChildren<FOV>().getDetections();

        if (state == State.Pause)
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider != null)
                {
                    if (collider.CompareTag("Player"))
                    {
                        targetVisable = GetComponentInChildren<FOV>().CheckObstruction(collider);
                        if (targetVisable)
                        {
                            target = collider.ClosestPoint(transform.position);
                            target.y = transform.position.y;
                            return;
                        }
                    }
                }
            }
            return;
        }

        targetVisable = false;
        if (colliders != null)
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    targetVisable = GetComponentInChildren<FOV>().CheckObstruction(collider);
                    if (targetVisable)
                    {
                        animator.SetBool("Search", false);
                        target = collider.ClosestPoint(transform.position);
                        target.y = transform.position.y;
                        if (!following)
                        {
                            alertTimer += Time.deltaTime;
                            if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().alertTimer((float)(alertTimer / alertTIme));

                            Debug.Log("Alert Timer: " + alertTimer);

                            if (alertTimer >= alertTIme) following = true;
                        }
                        else
                        {
                            searching = false;
                            state = State.Follow;
                            return;
                        }
                    }
                }
            }

        if (!targetVisable && alertTimer != 0 && !searching)
        {
            alertTimer -= Time.deltaTime;

            if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().alertTimer((float)(alertTimer / alertTIme));

            if (alertTimer <= 0)
            {
                alertTimer = 0;
                if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().setAlert(false);
            }
        }
        else if (!targetVisable && alertTimer != 0) alertTimer = 0;

        Debug.Log("Following == " + following + " | Searching == " + searching + " | Visible == " + targetVisable);

        if (following && !searching && !targetVisable && state != State.Wander)
        {
            Debug.Log("Lost Target");
            following = false;
            searching = true;
            searchLocationReached = false;
            state = State.Search;
            return;
        }
    }

    // Update is called once per frame
    void Follow()
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        if (!isLeft && !groundCheck.checkGroundR())
        {
            Debug.Log("Right Edge");
            direction = Vector2.zero;
        }
        if (isLeft && !groundCheck.checkGroundL())
        {
            Debug.Log("Left Edge");
            direction = Vector2.zero; 
        }

        if (Vector2.Distance(target, transform.position) < MinDistance - 0.1)
        {
            if (isLeft && !groundCheck.checkGroundR()) direction = Vector2.zero;
            else if (!isLeft && !groundCheck.checkGroundL()) direction = Vector2.zero;
            else
            {
                direction = -direction;
                Vector2 f = new Vector2(direction.x * speed, 0);
                body.velocity = new Vector2(f.x, body.velocity.y);
                animator.SetBool("Moving", true);
                return;
            }
        }
        else if (Vector2.Distance(target, transform.position) < MinDistance || Vector2.Distance(target, transform.position) > MaxDistance) direction = Vector2.zero;

        if (direction.x != 0) animator.SetBool("Moving", true);
        else animator.SetBool("Moving", false);

        Vector2 force = new Vector2(direction.x * speed, 0);

        body.velocity = new Vector2(force.x, body.velocity.y);

        bool lastDir = isLeft;

        if (direction.x < 0) isLeft = true;
        else if (direction.x > 0) isLeft = false;

        if (lastDir != isLeft)
        {
            GetComponent<SpriteRenderer>().flipX = isLeft;
            GameObject obj = GetComponentInChildren<FOV>().gameObject;
            obj.transform.RotateAround(gameObject.transform.position, Vector3.up, 180);
        }

    }

    void Search()
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        if ((!isLeft && !groundCheck.checkGroundR() && !searchLocationReached)
            || (isLeft && !groundCheck.checkGroundL() && !searchLocationReached) 
            || (Vector2.Distance(target, transform.position) < MinDistance && !searchLocationReached))
        {
            animator.SetBool("Search", true);
            searchLocationReached = true;
            clock = 0;
            body.velocity = new Vector2(0, body.velocity.y);
        }

        if (searchLocationReached)
        {
            clock += Time.deltaTime;
            if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().searchTimer(1.0f - (float)(clock / searchTime));
            if (clock >= searchTime)
            {
                animator.SetBool("Search", false);
                searching = false;
                state = State.Idle;
            }
        }
        else
        {
            Vector2 force = new Vector2(direction.x * speed, 0);

            body.velocity = new Vector2(force.x, body.velocity.y);

            bool lastDir = isLeft;

            if (direction.x < 0) isLeft = true;
            else if (direction.x > 0) isLeft = false;

            if (direction.x != 0) animator.SetBool("Moving", true);
            else animator.SetBool("Moving", false);

            if (lastDir != isLeft)
            {
                GetComponent<SpriteRenderer>().flipX = isLeft;
                GameObject obj = GetComponentInChildren<FOV>().gameObject;
                obj.transform.RotateAround(gameObject.transform.position, Vector3.up, 180);
            }
        }
    }

    void Idle()
    {
        clock += Time.deltaTime;
        if (clock >= IdleTime) state = State.Wander;
    }

    void Wander()
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        if ((!isLeft && !groundCheck.checkGroundR() && !searchLocationReached)
            || (isLeft && !groundCheck.checkGroundL() && !searchLocationReached)
            || (Vector2.Distance(target, transform.position) < MinDistance && !searchLocationReached))
        {
            animator.SetBool("Search", true);
            searchLocationReached = true;
            clock = 0;
            body.velocity = new Vector2(0, body.velocity.y);
        }

        if (searchLocationReached)
        {
            clock += Time.deltaTime;

            if (clock >= searchTime)
            {
                animator.SetBool("Search", false);
                state = State.Idle;
            }
        }
        else
        {
            Vector2 force = new Vector2(direction.x * speed, 0);

            body.velocity = new Vector2(force.x, body.velocity.y);

            bool lastDir = isLeft;

            if (direction.x < 0) isLeft = true;
            else if (direction.x > 0) isLeft = false;

            if (direction.x != 0) animator.SetBool("Moving", true);
            else animator.SetBool("Moving", false);

            if (lastDir != isLeft)
            {
                GetComponent<SpriteRenderer>().flipX = isLeft;
                GameObject obj = GetComponentInChildren<FOV>().gameObject;
                obj.transform.RotateAround(gameObject.transform.position, Vector3.up, 180);
            }
        }
    }

    public void flip()
    {
        isLeft = !isLeft;

        GetComponent<SpriteRenderer>().flipX = isLeft;
        GameObject obj = GetComponentInChildren<FOV>().gameObject;
        obj.transform.RotateAround(gameObject.transform.position, Vector3.up, 180);
    }

    public bool getAlert() { return following; }

    public bool pausePatrol(bool pause)
    {
        if (pause)
        {
            if (!paused)
            {
                paused = true;
                body.velocity = new Vector2(0, body.velocity.y);
                lastState = state;
                state = State.Pause;
                return true;
            }
        }
        else
        {
            if (paused)
            {
                paused = false;
                state = State.Search;
                following = false;
                searching = false;
                ViewCheck();
                return true;
            }
        }
        return false;
    }
}
