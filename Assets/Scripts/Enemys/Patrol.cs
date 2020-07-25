using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    float clock = 0;

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

    bool isRight = false;

    // Start is called before the first frame update
    void Start()
    {
        isRight = false;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        bool lastDir = isRight;

        ViewCheck();

        if (searching && !targetVisable)
        {
            if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().setSearch(true);
        }
        else if (following || targetVisable)
        {
            if (GetComponentInChildren<StateIcons>()) GetComponentInChildren<StateIcons>().setAlert(true);
        }
        else
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
                        following = true;
                        searching = false;
                        state = State.Follow;
                        return;
                    }
                }
            }

        if (following && !searching && !targetVisable)
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

        

        if (Vector2.Distance(target, transform.position) < MinDistance - 0.1)
        {
            direction = -direction;
            Vector2 f = new Vector2(direction.x * speed, 0);
            body.velocity = new Vector2(f.x, body.velocity.y);
            animator.SetBool("Moving", true);
            return;
        }
        else if (Vector2.Distance(target, transform.position) < MinDistance || Vector2.Distance(target, transform.position) > MaxDistance) direction = Vector2.zero;

        if (direction.x != 0) animator.SetBool("Moving", true);
        else animator.SetBool("Moving", false);

        Vector2 force = new Vector2(direction.x * speed, 0);

        body.velocity = new Vector2(force.x, body.velocity.y);

        bool lastDir = isRight;

        if (direction.x < 0) isRight = true;
        else if (direction.x > 0) isRight = false;

        if (lastDir != isRight)
        {
            GetComponent<SpriteRenderer>().flipX = isRight;
            GameObject obj = GetComponentInChildren<FOV>().gameObject;
            obj.transform.RotateAround(gameObject.transform.position, Vector3.up, 180);
        }

    }

    void Search()
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        if (Vector2.Distance(target, transform.position) < MinDistance && !searchLocationReached)
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
                searching = false;
                state = State.Idle;
            }
        }
        else
        {
            Vector2 force = new Vector2(direction.x * speed, 0);

            body.velocity = new Vector2(force.x, body.velocity.y);

            bool lastDir = isRight;

            if (direction.x < 0) isRight = true;
            else if (direction.x > 0) isRight = false;

            if (direction.x != 0) animator.SetBool("Moving", true);
            else animator.SetBool("Moving", false);

            if (lastDir != isRight)
            {
                GetComponent<SpriteRenderer>().flipX = isRight;
                GameObject obj = GetComponentInChildren<FOV>().gameObject;
                obj.transform.RotateAround(gameObject.transform.position, Vector3.up, 180);
            }
        }
    }

    void Idle()
    {
        animator.SetBool("Moving", false);
    }

    void Wander()
    {
    }

    public void flip()
    {
        isRight = !isRight;

        GetComponent<SpriteRenderer>().flipX = isRight;
        GameObject obj = GetComponentInChildren<FOV>().gameObject;
        obj.transform.RotateAround(gameObject.transform.position, Vector3.up, 180);
    }

    public bool pausePatrol(bool pause)
    {
        if (pause)
        {
            if (lastState != State.Pause)
            {
                body.velocity = new Vector2(0, body.velocity.y);
                lastState = state;
                state = State.Pause;
                return true;
            }
        }
        else
        {
            if (lastState != State.Pause)
            {
                state = State.Search;
                following = true;
                searching = false;
                ViewCheck();
                return true;
            }
        }
        return false;
    }
}
