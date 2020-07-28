using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float speed = 5;

    int damage = 1;

    [SerializeField]
    ContactFilter2D ContactFilter;

    Vector2 direction;
    bool hit = false;

    Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 force = direction * speed;

        body.velocity = new Vector2(force.x, force.y);

        Collider2D[] hits = new Collider2D[1];
        int i = GetComponent<Collider2D>().GetContacts(ContactFilter, hits);
        if (i==1)
        {
            hit = true;

            if (hits[0].CompareTag("Player") && hits[0].GetComponent<PlayerManager>())
            {
                hits[0].GetComponent<PlayerManager>().takeDamage(damage);
            }
            else if (hits[0].CompareTag("Enemy") && hits[0].GetComponent<Enemy>())
            {
                hits[0].GetComponent<Enemy>().takeDamage(damage);
            }

            Destroy(gameObject);
        }
    }

    public void setTarget(Vector2 Target)
    {
        direction = (Target - (Vector2)transform.position).normalized;
    }

    public void setDamage(int i)
    {
        damage = i;
    }
}
