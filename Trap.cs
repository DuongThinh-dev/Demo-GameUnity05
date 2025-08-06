using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapType { Spike, Freeze, Bomb }

public class Trap : MonoBehaviour
{
    private GridSystem gridSystem;
    private Vector3 placementPosition;

    public TrapType trapType; // Loai bay

    [Header("Trap Stats")]
    public int damage;
    public float effectDuration;
    public float trapPrice;

    [Header("Bomb Trap Settings")]
    public float explosionRadius = 1.5f;
    public LayerMask enemyLayer;

    private bool isTriggered = false;

    private void Start()
    {
        SetupTrapByType();
    }

    void SetupTrapByType()
    {
        switch (trapType)
        {
            case TrapType.Spike:
                damage = 10;
                effectDuration = 3f;
                trapPrice = 50;
                break;

            case TrapType.Freeze:
                damage = 20;
                effectDuration = 0.75f;
                trapPrice = 80;
                break;

            case TrapType.Bomb:
                damage = 40;
                effectDuration = 0f;
                trapPrice = 100;
                break;
        }
    }

    void Update()
    {
        if (isTriggered) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, 0.2f, enemyLayer);
        if (hits.Length > 0)
        {
            Enemy enemy = hits[0].GetComponent<Enemy>();
            if (enemy != null)
            {
                TriggerTrap(enemy);
                isTriggered = true;
            }
        }
    }

    void TriggerTrap(Enemy triggeredEnemy)
    {
        if (trapType == TrapType.Bomb && ObjectPooler.Instance != null)
        {
            GameObject effectGO = ObjectPooler.Instance.SpawnFromPool("ExplosiveImpact", transform.position, Quaternion.identity);

            // Goi Init de phat am thanh no
            ImpactEffect impact = effectGO.GetComponent<ImpactEffect>();
            if (impact != null)
            {
                impact.Init(TypeEffect.ImpactExplosiveEffect);
            }
        }

        switch (trapType)
        {
            case TrapType.Bomb:
                Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
                foreach (Collider col in nearbyEnemies)
                {
                    Enemy e = col.GetComponent<Enemy>();
                    if (e != null)
                        e.TakeDamage(damage);
                }
                break;

            case TrapType.Spike:
                triggeredEnemy.TakeDamage(damage);
                if (triggeredEnemy.IsAlive())
                    triggeredEnemy.Slow(0.6f, effectDuration); // Giam toc 40%
                break;

            case TrapType.Freeze:
                triggeredEnemy.TakeDamage(damage);
                if (triggeredEnemy.IsAlive())
                    triggeredEnemy.Freeze(effectDuration); // Dong bang
                break;
        }

        if (gridSystem != null)
        {
            gridSystem.ClearOccupiedCell(placementPosition);
        }

        Destroy(gameObject);
    }

    public void SetGridReference(GridSystem grid, Vector3 position)
    {
        gridSystem = grid;
        placementPosition = position;
    }
}
