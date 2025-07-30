using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;// muc tieu

    public float speed = 70f;// toc do
    public float damage;

    [Header("Explosion Settings")]
    public bool isExplosive; // Dan co lan không
    public float explosionRadius; // ban kinh lan
    public LayerMask enemyLayer; // Layer cua enemy

    // gan muc tieu ben Turret.cs sang Bullet.cs
    public void Seek(Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // Neu khoang cach den enemy nho hon quang duong di trong frame hien tai
        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    // Khi dan trung muc tieu
    void HitTarget()
    {
        if (ObjectPooler.Instance != null)
        {
            if (isExplosive)
            {
                GameObject effectGO = ObjectPooler.Instance.SpawnFromPool("ExplosiveImpact", transform.position, transform.rotation);
                ImpactEffect effect = effectGO.GetComponent<ImpactEffect>();
                if (effect != null) effect.Init(TypeEffect.ImpactExplosiveEffect);

                Explode();
            }
            else
            {
                GameObject effectGO = ObjectPooler.Instance.SpawnFromPool("Impact", transform.position, transform.rotation);
                ImpactEffect effect = effectGO.GetComponent<ImpactEffect>();
                if (effect != null) effect.Init(TypeEffect.ImpactEffect);

                if (target != null)
                {
                    Enemy enemy = target.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
            }
        }

        target = null;
        gameObject.SetActive(false);
    }


    // Tinh sat thuong tat ca enemy trong vung anh huong
    void Explode()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        foreach (Collider col in hitEnemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    // Debug ve vung anh huong trong editor
    void OnDrawGizmosSelected()
    {
        if (isExplosive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}