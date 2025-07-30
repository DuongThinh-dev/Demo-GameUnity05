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
        Debug.Log($"[Tower2] Generated {goldPerTick} gold!");
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
            // Tower1 - Sung lien thanh
            case TowerType.Tower1:
                isExplosive = false;
                buyPrice = 80;
                switch (levelType)
                {
                    case LevelType.Lv1:
                        damage = 10f;
                        fireTime = 0.8f;
                        upgradePrice = 100;
                        sellPrice = 50;
                        rangeType = RangeType.near;
                        break;
                    case LevelType.Lv2:
                        damage = 15f;
                        fireTime = 0.7f;
                        upgradePrice = 150;
                        sellPrice = 70;
                        rangeType = RangeType.medium;
                        break;
                    case LevelType.Lv3:
                        damage = 22f;
                        fireTime = 0.6f;
                        upgradePrice = 0;
                        sellPrice = 120;
                        rangeType = RangeType.medium;
                        break;
                }
                break;

            // Tower2 - Thap nguon (Sinh vang)
            case TowerType.Tower2:
                isExplosive = false;
                buyPrice = 90;
                switch (levelType)
                {
                    case LevelType.Lv1:
                        goldPerTick = 4;
                        goldInterval = 6f;
                        upgradePrice = 100;
                        sellPrice = 60;
                        break;
                    case LevelType.Lv2:
                        goldPerTick = 6;
                        goldInterval = 6f;
                        upgradePrice = 150;
                        sellPrice = 70;
                        break;
                    case LevelType.Lv3:
                        goldPerTick = 8;
                        goldInterval = 5f;
                        upgradePrice = 0;
                        sellPrice = 120;
                        break;
                }
                break;

            // Tower3 - Thap phao don muc tieu
            case TowerType.Tower3:
                isExplosive = false;
                buyPrice = 100;
                switch (levelType)
                {
                    case LevelType.Lv1:
                        damage = 35f;
                        fireTime = 3.0f;
                        upgradePrice = 110;
                        sellPrice = 70;
                        rangeType = RangeType.medium;
                        break;
                    case LevelType.Lv2:
                        damage = 55f;
                        fireTime = 2.7f;
                        upgradePrice = 150;
                        sellPrice = 80;
                        rangeType = RangeType.medium;
                        break;
                    case LevelType.Lv3:
                        damage = 75f;
                        fireTime = 2.3f;
                        upgradePrice = 0;
                        sellPrice = 120;
                        rangeType = RangeType.medium;
                        break;
                }
                break;

            // Tower4 - Thap dai bac (dien rong)
            case TowerType.Tower4:
                isExplosive = true;
                buyPrice = 120;
                switch (levelType)
                {
                    case LevelType.Lv1:
                        damage = 25f;
                        fireTime = 3.5f;
                        explosionRadius = 1f;
                        upgradePrice = 140;
                        sellPrice = 90;
                        rangeType = RangeType.far;
                        break;
                    case LevelType.Lv2:
                        damage = 40f;
                        fireTime = 3.0f;
                        explosionRadius = 1.25f;
                        upgradePrice = 160;
                        sellPrice = 110;
                        rangeType = RangeType.far;
                        break;
                    case LevelType.Lv3:
                        damage = 60f;
                        fireTime = 2.5f;
                        explosionRadius = 1.5f;
                        upgradePrice = 0;
                        sellPrice = 130;
                        rangeType = RangeType.far;
                        break;
                }
                break;
        }

        UpdateRangeFromRangeType();

        Debug.Log(towerType.ToString() + " " + levelType.ToString());
    }

    public void Upgrade()
    {
        if (levelType == LevelType.Lv1) levelType = LevelType.Lv2;
        else if (levelType == LevelType.Lv2) levelType = LevelType.Lv3;
        else return;

        CancelInvoke();   // Dung moi Invoke truoc
        Tower();          // Gan lai chi so moi

        // Goi lai dung theo loai thap
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
