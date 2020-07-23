using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected Stats stats;

    void Start()
    {
        onStart();
    }

    void Update()
    {
        onUpdate();
    }

    private void FixedUpdate()
    {
        onFixedUpdate();
    }

    protected virtual void onStart() { }
    protected virtual void onFixedUpdate() { }
    protected virtual void onUpdate() { }
}
