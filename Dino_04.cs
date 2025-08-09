using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino04_Stegosaurus : Enemy
{
    public float debuffRadius = 2.5f;           // pham vi tac dong den turret
    public float fireTimeIncreasePercent = 0.9f;
    public LayerMask towerLayer;

    private readonly List<Turret> affectedTurrets = new List<Turret>();

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(ApplyDebuffRoutine());
    }

    protected override void Die()
    {
        RemoveDebuff();
        base.Die();
    }

    private IEnumerator ApplyDebuffRoutine()
    {
        while (IsAlive())
        {
            ApplyDebuff();
            yield return new WaitForSeconds(1f); // kiem tra moi giay
        }
    }

    void ApplyDebuff()
    {
        // 1. Tim tat ca turret trong pham vi
        Collider[] hits = Physics.OverlapSphere(transform.position, debuffRadius, towerLayer);
        foreach (Collider col in hits)
        {
            Turret turret = col.GetComponent<Turret>();
            if (turret != null && !affectedTurrets.Contains(turret))
            {
                turret.fireTime *= (1f + fireTimeIncreasePercent);
                affectedTurrets.Add(turret);
            }
        }

        // 2. Kiem tra xem turret nao da ra khoi vung thi khoi phuc
        for (int i = affectedTurrets.Count - 1; i >= 0; i--)
        {
            Turret t = affectedTurrets[i];
            if (t == null || Vector3.Distance(t.transform.position, transform.position) > debuffRadius)
            {
                t.fireTime /= (1f + fireTimeIncreasePercent);
                affectedTurrets.RemoveAt(i);
            }
        }
    }

    void RemoveDebuff()
    {
        foreach (Turret turret in affectedTurrets)
        {
            if (turret != null)
                turret.fireTime /= (1f + fireTimeIncreasePercent);
        }
        affectedTurrets.Clear();
    }
}
