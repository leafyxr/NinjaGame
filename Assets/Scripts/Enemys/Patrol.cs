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

    private void Update()
    {
        bool lastDir = isRight;

        ViewCheck();

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
                break;
        }


    }

    void ViewCheck()
    {
        Collider2D[] colliders = GetComponentInChildren<FOV>().getDetections();

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
                        Debug.Log("Follow");
                        target = collider.gameObject.transform.position;
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
            Debug.Log("Search");
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

        if (Vector2.Distance(target, transform.position) < MinDistance - 0.1) direction = -direction;
        else if (Vector2.Distance(target, transform.position) < MinDistance || Vector2.Distance(target, transform.position) > MaxDistance) direction = Vector2.zero;

        Vector2 force = new Vector2(direction.x * speed, 0);

        body.velocity = new Vector2(force.x, body.velocity.y);

        bool lastDir = isRight;

        if (direction.x < 0) isRight = true;
        else if (direction.x > 0) isRight = false;

        if(direction.x != 0) animator.SetBool("Moving", true);
        else animator.SetBool("Moving", false);

        if (lastDir != isRight)
        {
            Debug.Log("Flip Sprite");
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
            Debug.Log("Destination Reached");
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
                Debug.Log("Stop Search");
                animator.SetBool("Search", false);
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
                Debug.Log("Flip Sprite");
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
        Debug.Log("Flip");
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
                lastState = state;
                state = State.Pause;
                return true;
            }
        }
        else
        {
            if (lastState != State.Pause)
            {
                state = lastState;
                return true;
            }
        }
        return false;
    }
}
