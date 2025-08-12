using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType { Dino_01, Dino_02, Dino_03, Dino_04, Dino_05, Dino_06, Dino_07, Dino_08, Dino_09, Dino_10, Dino_11 }

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;

    [Header("Information Dino")]
    public float speed; // toc do hien tai
    public float maxHp;
    public float armor;
    public int rewardGold;
    protected float hp;

    [Header("Unity Stuff")]
    public Image heathBar;
    public GameObject heathCanva;
    public Animator anim;
    protected Renderer rend;

    protected float originalSpeed; // toc do goc
    protected Color originalColor;
    public Color frozenColor = Color.cyan;
    private Vector3 heathCanvaToEnemy;
    private bool isSlowed = false;
    private float slowTimer = 0f;
    private float slowMultiplier = 1f;

    private GoldManager goldManager;
    private HomeManager homeManager;

    protected virtual void OnEnable()
    {
        goldManager = FindAnyObjectByType<GoldManager>();
        homeManager = FindAnyObjectByType<HomeManager>();

        EnemyDino();

        if (rend == null) rend = GetComponentInChildren<Renderer>();
        if (rend != null) originalColor = rend.material.color;

        if (heathBar != null) heathBar.fillAmount = 1f;
        if (heathCanva != null) heathCanvaToEnemy = heathCanva.transform.position - transform.position;

        anim = GetComponent<Animator>();

        ApplyPassive();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            gameObject.SetActive(false);
        }
    }

    public void EnemyDino()
    {
        switch (enemyType)
        {
            case EnemyType.Dino_01: speed = 2f; maxHp = 40; armor = 0; rewardGold = 20; break;
            case EnemyType.Dino_02: speed = 1f; maxHp = 120; armor = 4; rewardGold = 40; break;
            case EnemyType.Dino_03: speed = 1f; maxHp = 160; armor = 10; rewardGold = 40; break;
            case EnemyType.Dino_04: speed = 1f; maxHp = 200; armor = 8; rewardGold = 40; break;
            case EnemyType.Dino_05: speed = 1.5f; maxHp = 80; armor = 4; rewardGold = 30; break;
            case EnemyType.Dino_06: speed = 1.5f; maxHp = 80; armor = 4; rewardGold = 30; break;
            case EnemyType.Dino_07: speed = 1f; maxHp = 200; armor = 8; rewardGold = 50; break;
            case EnemyType.Dino_08: speed = 1.5f; maxHp = 160; armor = 6; rewardGold = 50; break;
            case EnemyType.Dino_09: speed = 2f; maxHp = 40; armor = 2; rewardGold = 30; break;
            case EnemyType.Dino_10: speed = 1.5f; maxHp = 120; armor = 6; rewardGold = 50; break;
            case EnemyType.Dino_11: speed = 1.5f; maxHp = 160; armor = 8; rewardGold = 50; break;
        }

        hp = maxHp;
        originalSpeed = speed;
    }

    protected virtual void ApplyPassive() { }

    void LateUpdate()
    {
        if (heathCanva != null)
        {
            heathCanva.transform.position = transform.position + heathCanvaToEnemy;
            if (Camera.main != null)
                heathCanva.transform.forward = Camera.main.transform.forward;
        }

        if (homeManager.gameEnded)
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        // Quan ly slow khong dung coroutine
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                // Het slow -> tra ve toc do goc
                speed = originalSpeed;
                isSlowed = false;
            }
        }
    }

    protected virtual void OnDisable()
    {
        isSlowed = false;
        slowTimer = 0f;
        speed = originalSpeed;
    }

    public virtual void TakeDamage(float dmg)
    {
        float actualDamage = Mathf.Max(dmg - armor, 1f);
        hp -= actualDamage;
        UpdateHPBar();

        if (hp <= 0f)
        {
            Die();
        }
    }

    protected virtual void UpdateHPBar()
    {
        if (heathBar != null)
            heathBar.fillAmount = hp / maxHp;
    }

    public bool IsAlive() => hp > 0f;

    protected virtual void Die()
    {
        goldManager.AddGold(rewardGold);
        gameObject.SetActive(false);
    }

    public void Slow(float multiplier, float duration)
    {
        // Neu chua bi slow hoac muon lam moi slow
        isSlowed = true;
        slowMultiplier = multiplier;
        slowTimer = duration;
        speed = originalSpeed * multiplier;
    }
}
