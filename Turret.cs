using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RangeType { near, medium, far }
public enum TowerType { Tower1, Tower2, Tower3, Tower4 }
public enum LevelType { Lv1, Lv2, Lv3 }

public class Turret : MonoBehaviour
{
    [Header("Tower Information")]
    public TowerType towerType;
    public LevelType levelType;

    [Header("General Attributes")]
    public GameObject TurretPrefab;              // Prefab mo hinh turret quay
    public Transform firePoint;                  // Diem ban dan
    public LayerMask enemyLayer;                 // Layer ke dich
    public float fireTime = 1f;                  // Thoi gian giua 2 phat ban

    [Header("Gold Generator Attributes")]
    [Tooltip("Gold generated per interval (Tower2 only)")]
    [SerializeField] private int goldPerTick = 4;     // luong vang cung cap
    [SerializeField] private float goldInterval = 6f; // Thoi gian giua 2 lan cung cap

    [Header("Combat Attributes")]
    [SerializeField] private float turnSpeed = 10f;   // Toc do quay turret
    [SerializeField] private float damage = 10f;      // Sat thuong moi phat ban
    [SerializeField] private bool isExplosive = false;// Co gay sat thuong lan không
    [SerializeField] private float explosionRadius = 1f;  // Ban kinh no (chi dung khi isExplosive = true)

    [Header("Range Attributes")]
    [SerializeField] private RangeType rangeType = RangeType.near;
    [SerializeField] private float range = 1.5f;

    [Header("Internal State")]
    public int buyPrice = 0;                   // Gia mua
    public int upgradePrice = 0;               // Gia nang cap
    public int sellPrice = 0;                  // Gia ban
    private float fireCountDown = 0f;           // Bo dem thoi gian ban
    private Transform target;                   // Ke dich dang nham toi

    void Start()
    {
        Tower();

        if (towerType == TowerType.Tower2)
        {
            InvokeRepeating(nameof(GenerateGold), goldInterval, goldInterval);
        }
        else
        {
            InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
        }
    }


    // ham sinh vang
    void GenerateGold()
    {
        // Goi he thong quan ly vang de cong (gia dinh co GoldManager.Instance.AddGold(int amount))
        GoldManager.Instance.AddGold(goldPerTick);
    }

    // cap nhat muc tieu
    void UpdateTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, range, enemyLayer);

        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach(Collider enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            Vector3 direction = (enemy.transform.position - transform.position).normalized;

            if(Physics.Raycast(transform.position, direction, out RaycastHit hit, range, enemyLayer))
            {
                if (hit.collider.transform == enemy.transform && distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.transform;
                }
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (towerType == TowerType.Tower2)
            return;

        if (target == null)
        {
            fireCountDown -= Time.deltaTime;
            return;
        }

        RotateTowardsTarget();

        fireCountDown -= Time.deltaTime;

        if (fireCountDown <= 0f)
        {
            Shoot();
            fireCountDown = fireTime;
        }
    }


    // ham quay Turret
    void RotateTowardsTarget()
    {
        Vector3 dir = target.position - TurretPrefab.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        TurretPrefab.transform.rotation = Quaternion.Lerp(
            TurretPrefab.transform.rotation,
            lookRotation,
            Time.deltaTime * turnSpeed
        );
    }

    // Ham ban dan
    void Shoot()
    {
        if (ObjectPooler.Instance != null)
        {
            GameObject BulletGO = ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
            Bullet bullet = BulletGO.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.Seek(target);
                bullet.damage = damage;
                bullet.isExplosive = isExplosive;
                bullet.explosionRadius = explosionRadius;
            }
        }
        else return;
    }

    // ve gizmos trong scene view
    void OnDrawGizmosSelected()
    {
        switch (towerType)
        {
            case TowerType.Tower1: Gizmos.color = Color.green; break;
            case TowerType.Tower2: Gizmos.color = Color.yellow; break;
            case TowerType.Tower3: Gizmos.color = Color.red; break;
            case TowerType.Tower4: Gizmos.color = Color.magenta; break;
        }
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void UpdateRangeFromRangeType()
    {
        switch (rangeType)
        {
            case RangeType.near:
                range = 1.5f;
                break;
            case RangeType.medium:
                range = 2.0f;
                break;
            case RangeType.far:
                range = 2.5f;
                break;
        }
    }

    public void Tower()
    {
        switch (towerType)
        {
            case TowerType.Tower1: // Sung lien thanh
                isExplosive = false;
                buyPrice = 50;
                switch (levelType)
                {
                    case LevelType.Lv1:
                        damage = 20f;
                        fireTime = 0.8f;
                        upgradePrice = 70;
                        sellPrice = 30;
                        rangeType = RangeType.near;
                        break;
                    case LevelType.Lv2:
                        damage = 25f;
                        fireTime = 0.7f;
                        upgradePrice = 90;
                        sellPrice = 50;
                        rangeType = RangeType.medium;
                        break;
                    case LevelType.Lv3:
                        damage = 30f;
                        fireTime = 0.6f;
                        upgradePrice = 0;
                        sellPrice = 70;
                        rangeType = RangeType.medium;
                        break;
                }
                break;

            case TowerType.Tower2: // Thap nguon (sinh vang)
                isExplosive = false;
                buyPrice = 70;
                switch (levelType)
                {
                    case LevelType.Lv1:
                        goldPerTick = 5;
                        goldInterval = 5f;
                        upgradePrice = 90;
                        sellPrice = 50;
                        break;
                    case LevelType.Lv2:
                        goldPerTick = 7;
                        goldInterval = 4.5f;
                        upgradePrice = 110;
                        sellPrice = 70;
                        break;
                    case LevelType.Lv3:
                        goldPerTick = 10;
                        goldInterval = 4f;
                        upgradePrice = 0;
                        sellPrice = 90;
                        break;
                }
                break;

            case TowerType.Tower3: // Thap phao don muc tieu
                isExplosive = false;
                buyPrice = 100;
                switch (levelType)
                {
                    case LevelType.Lv1:
                        damage = 45f;
                        fireTime = 2.4f;
                        upgradePrice = 120;
                        sellPrice = 80;
                        rangeType = RangeType.medium;
                        break;
                    case LevelType.Lv2:
                        damage = 70f;
                        fireTime = 2.0f;
                        upgradePrice = 140;
                        sellPrice = 100;
                        rangeType = RangeType.medium;
                        break;
                    case LevelType.Lv3:
                        damage = 100f;
                        fireTime = 1.6f;
                        upgradePrice = 0;
                        sellPrice = 120;
                        rangeType = RangeType.medium;
                        break;
                }
                break;

            case TowerType.Tower4: // Thap phao dai bac (sat thuong lan)
                isExplosive = true;
                buyPrice = 120;
                switch (levelType)
                {
                    case LevelType.Lv1:
                        damage = 25f;
                        fireTime = 3.5f;
                        explosionRadius = 1f;
                        upgradePrice = 140;
                        sellPrice = 100;
                        rangeType = RangeType.far;
                        break;
                    case LevelType.Lv2:
                        damage = 40f;
                        fireTime = 3.0f;
                        explosionRadius = 1.25f;
                        upgradePrice = 160;
                        sellPrice = 120;
                        rangeType = RangeType.far;
                        break;
                    case LevelType.Lv3:
                        damage = 60f;
                        fireTime = 2.5f;
                        explosionRadius = 1.5f;
                        upgradePrice = 0;
                        sellPrice = 140;
                        rangeType = RangeType.far;
                        break;
                }
                break;
        }

        UpdateRangeFromRangeType();
    }

    public void Upgrade()
    {
        if (IsMaxLevel()) return;

        // Neu khong du vang -> khong nang cap
        if (!GoldManager.Instance.TrySpendGold(upgradePrice))
        {
            return;
        }

        // Cap nhat cap do
        if (levelType == LevelType.Lv1) levelType = LevelType.Lv2;
        else if (levelType == LevelType.Lv2) levelType = LevelType.Lv3;
        else return;

        CancelInvoke();   // Dung cac invoke cu
        Tower();          // Cap nhat lai chi so theo cap do

        // Goi lai invoke phu hop theo loai thap
        if (towerType == TowerType.Tower2)
            InvokeRepeating(nameof(GenerateGold), goldInterval, goldInterval);
        else
            InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }


    public bool IsMaxLevel()
    {
        return levelType >= LevelType.Lv3;
    }

    public void Sell()
    {
        CancelInvoke();

        // Cong vang
        GoldManager.Instance.AddGold(sellPrice);

        // Giai phong o
        GridSystem gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem != null)
        {
            gridSystem.ClearOccupiedCell(transform.position);
        }

        Destroy(gameObject);
    }
}
