using System.Collections;
using System.Collections.Generic;
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
    ContactFilter2D contactFilter;

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
        polygonCollider.OverlapCollider(contactFilter,  hits);

        detections = new List<Collider2D>();

        foreach(Collider2D hit in hits)
        {
            if (hit != null)
            {
                detections.Add(hit);
            }
        }

        hits = new Collider2D[20];
        circleCollider.OverlapCollider(contactFilter, hits);

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


}
