using System.Collections.Generic;
using UnityEngine;

public class Dino04_Stegosaurus : Enemy
{
    public float debuffRadius = 2.5f;
    public float fireTimeIncreasePercent = 0.9f;
    public LayerMask towerLayer;
    public float checkInterval = 1f;

    // L?u turret v� fireTime g?c
    private readonly Dictionary<Turret, float> affectedTurrets = new Dictionary<Turret, float>();
    private float nextCheckTime;

    protected override void OnEnable()
    {
        base.OnEnable();
        nextCheckTime = Time.time;
        affectedTurrets.Clear();
    }

    protected override void Update()
    {
        base.Update();
        if (!IsAlive()) return;

        if (Time.time >= nextCheckTime)
        {
            ApplyDebuff();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    protected override void Die()
    {
        RemoveDebuff();
        base.Die();
    }

    void ApplyDebuff()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, debuffRadius, towerLayer);

        // Th�m debuff cho turret m?i
        foreach (Collider col in hits)
        {
            Turret turret = col.GetComponent<Turret>();
            if (turret != null && !affectedTurrets.ContainsKey(turret))
            {
                affectedTurrets[turret] = turret.fireTime; // l?u gi� tr? g?c
                turret.fireTime *= (1f + fireTimeIncreasePercent);
            }
        }

        // G? debuff cho turret ra kh?i v�ng
        List<Turret> toRemove = new List<Turret>();
        foreach (var kvp in affectedTurrets)
        {
            Turret t = kvp.Key;
            if (t == null || Vector3.Distance(t.transform.position, transform.position) > debuffRadius)
            {
                t.fireTime = kvp.Value; // kh�i ph?c gi� tr? g?c
                toRemove.Add(t);
            }
        }

        foreach (var t in toRemove)
        {
            affectedTurrets.Remove(t);
        }
    }

    void RemoveDebuff()
    {
        foreach (var kvp in affectedTurrets)
        {
            if (kvp.Key != null)
                kvp.Key.fireTime = kvp.Value; // kh�i ph?c gi� tr? g?c
        }
        affectedTurrets.Clear();
    }
}
