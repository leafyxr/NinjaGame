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

        detections = new List<Collider2D>();
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
        RaycastHit2D[] hits = new RaycastHit2D[100];

        Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

        //int i = Physics2D.Linecast(transform.position, target.ClosestPoint(transform.position), ObstructionFilter, hits);

        int i = Physics2D.Raycast(transform.position, direction, ObstructionFilter, hits);

        if (i == 0) return false;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
            {
            }
            else if (hit.transform.gameObject.GetInstanceID() == target.transform.gameObject.GetInstanceID())
            {
                Debug.DrawLine(transform.position, hit.point, Color.blue);
                return true;
            }
            else if (hit.transform.gameObject.GetInstanceID() != gameObject.GetInstanceID()
                && hit.transform.gameObject.GetInstanceID() != transform.parent.gameObject.GetInstanceID())
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                return false;
            }
        }
        return false;
    }

}
