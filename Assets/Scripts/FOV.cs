using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class FOV : MonoBehaviour
{
    [SerializeField]
    float Angle;
    float halfBase;
    [SerializeField]
    float length;

    [SerializeField]
    float radius;

    [SerializeField]
    Vector2 Offset;

    [SerializeField]
    ContactFilter2D ViewFilter;

    [SerializeField]
    ContactFilter2D ObstructionFilter;

    PolygonCollider2D polygonCollider;
    CircleCollider2D circleCollider;

    List<Collider2D> detections;

    void Start()
    {
        polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider.offset = Offset;
        polygonCollider.isTrigger = true;

        halfBase = length / Mathf.Tan(Mathf.Deg2Rad*Angle);

        Debug.Log(halfBase);

        polygonCollider.points = new Vector2[]{
            new Vector2(0,0),
            new Vector2(length,halfBase),
            new Vector2(length,-halfBase)
        };

        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.offset = Offset;
        circleCollider.isTrigger = true;
        circleCollider.radius = radius;
    }

    private void Update()
    {
        Collider2D[] hits = new Collider2D[20];
        polygonCollider.OverlapCollider(ViewFilter,  hits);

        detections = new List<Collider2D>();

        foreach(Collider2D hit in hits)
        {
            if (hit != null)
            {
                detections.Add(hit);
            }
        }

        hits = new Collider2D[20];
        circleCollider.OverlapCollider(ViewFilter, hits);

        foreach (Collider2D hit in hits)
        {
            if (hit != null)
            {
                detections.Add(hit);
            }
        }
    }

    public Collider2D[] getDetections()
    {
        if (detections.Count != 0)
            return detections.ToArray();
        else
            return null;
    }

    public bool CheckObstruction(Collider2D target)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        Physics2D.Linecast(transform.position, target.transform.position, ObstructionFilter, hits);
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log("Hit : " + hit.transform.gameObject.name);
            if (hit.transform.gameObject.GetInstanceID() == target.transform.gameObject.GetInstanceID())
            {
                Debug.Log("Hit Success");
                Debug.DrawLine(transform.position, hit.point, Color.blue);
                Debug.Log("Target Visable");
                return true;
            }
            else if (hit.transform.gameObject.GetInstanceID() != gameObject.GetInstanceID() 
                && hit.transform.gameObject.GetInstanceID() != transform.parent.gameObject.GetInstanceID())
            {
                Debug.Log("Hit Failure");
                Debug.DrawLine(transform.position, hit.point, Color.red);
                return false;
            }
            else
            {
                Debug.Log("Self Detection, Continue");
            }
        }
        return false;
    }

}
