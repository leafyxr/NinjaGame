using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    float distBetweenPoints;
    [SerializeField]
    float distToGround;

    bool groundL;
    bool groundC;
    bool groundR;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkGroundC();
        checkGroundL();
        checkGroundR();
    }

    public bool checkGroundL()
    {
        Vector2 position = new Vector2(transform.position.x - distBetweenPoints, transform.position.y);
        Vector2 direction = Vector2.down;

        Debug.DrawRay(position, direction, Color.cyan);
        groundL = Physics2D.Raycast(position, direction, distToGround, (1 << LayerMask.NameToLayer("Ground")));

        if (groundL) Debug.DrawRay(position, direction, Color.cyan);
        else Debug.DrawRay(position, direction, Color.red);

        return groundL;
    }
    public bool checkGroundC()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;

        Debug.DrawRay(position, direction, Color.cyan);
        groundC = Physics2D.Raycast(position, direction, distToGround, (1 << LayerMask.NameToLayer("Ground")));

        if (groundC) Debug.DrawRay(position, direction, Color.cyan);
        else Debug.DrawRay(position, direction, Color.red);

        return groundC;
    }
    public bool checkGroundR()
    {
        Vector2 position = new Vector2(transform.position.x + distBetweenPoints, transform.position.y);
        Vector2 direction = Vector2.down;

        Debug.DrawRay(position, direction, Color.cyan);
        groundR = Physics2D.Raycast(position, direction, distToGround, (1 << LayerMask.NameToLayer("Ground")));

        if (groundR) Debug.DrawRay(position, direction, Color.cyan);
        else Debug.DrawRay(position, direction, Color.red);

        return groundR;
    }
}
