﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    Stats stats;

    void Start()
    {
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
    }

    public bool takeDamage(int damage)
    {
        stats.CurrentHitPoints -= Mathf.Abs(damage);

        if (stats.CurrentHitPoints <= 0) return true;
        else return false;
    }

    public void Heal(int healAmount)
    {
        stats.CurrentHitPoints += Mathf.Abs(healAmount);

        if (stats.CurrentHitPoints > stats.MaximumHitPoints) stats.CurrentHitPoints = stats.MaximumHitPoints;
    }

    public Stats GetStats()
    {
        return stats;
    }
}
