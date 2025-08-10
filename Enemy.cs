using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType { Dino_01, Dino_02, Dino_03, Dino_04, Dino_05, Dino_06, Dino_07, Dino_08, Dino_09, Dino_10, Dino_11 }

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;

    [Header("Imformation Dino")]
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
    //private bool isFrozen = false;
    private bool isSlowed = false;
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

        // Ap dung noi tai khi kich hoat lai
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
            case EnemyType.Dino_01: // Velociraptor
                speed = 2f;
                maxHp = 40;
                armor = 0;
                rewardGold = 20;
                break;
            case EnemyType.Dino_02: // Triceratops
                speed = 1f;
                maxHp = 120;
                armor = 4;
                rewardGold = 40;
                break;
            case EnemyType.Dino_03: // Ankylosaurus
                speed = 1f;
                maxHp = 160;
                armor = 10;
                rewardGold = 40;
                break;
            case EnemyType.Dino_04: // Stegosaurus
                speed = 1f;
                maxHp = 200;
                armor = 8;
                rewardGold = 40;
                break;
            case EnemyType.Dino_05: // Pachycephalosaurus
                speed = 1.5f;
                maxHp = 80;
                armor = 4;
                rewardGold = 30;
                break;
            case EnemyType.Dino_06: // Parasaurolophus
                speed = 1.5f;
                maxHp = 80;
                armor = 4;
                rewardGold = 30;
                break;
            case EnemyType.Dino_07: // Brachiosaurus
                speed = 1f;
                maxHp = 200;
                armor = 8;
                rewardGold = 50;
                break;
            case EnemyType.Dino_08: // Tyrannosaurus Rex
                speed = 1.5f;
                maxHp = 160;
                armor = 6;
                rewardGold = 50;
                break;
            case EnemyType.Dino_09: // Pteranodon
                speed = 2f;
                maxHp = 40;
                armor = 2;
                rewardGold = 30;
                break;
            case EnemyType.Dino_10: // Hatzegopteryx
                speed = 1.5f;
                maxHp = 120;
                armor = 6;
                rewardGold = 50;
                break;
            case EnemyType.Dino_11: // Spinosaurus
                speed = 1.5f;
                maxHp = 160;
                armor = 8;
                rewardGold = 50;
                break;
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

    protected virtual void OnDisable()
    {
        // Neu can, ban co the reset hieu ung slow, freeze tai day
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

    //public void Freeze(float duration)
    //{
    //    if (!isFrozen) StartCoroutine(FreezeCoroutine(duration));
    //}

    //private IEnumerator FreezeCoroutine(float duration)
    //{
    //    isFrozen = true;
    //    float prevSpeed = speed;
    //    speed = 0f;

    //    if (rend != null) rend.material.color = frozenColor;
    //    if (anim != null)
    //    {
    //        anim.SetBool("isFrozen", true);
    //        anim.speed = 0f;
    //    }

    //    yield return new WaitForSeconds(duration);

    //    speed = prevSpeed;
    //    if (rend != null) rend.material.color = originalColor;
    //    if (anim != null)
    //    {
    //        anim.SetBool("isFrozen", false);
    //        anim.speed = 1f;
    //    }

    //    isFrozen = false;
    //}

    public void Slow(float multiplier, float duration)
    {
        if (!isSlowed) StartCoroutine(SlowCoroutine(multiplier, duration));
    }

    private IEnumerator SlowCoroutine(float multiplier, float duration)
    {
        isSlowed = true;
        float oldSpeed = speed;
        speed *= multiplier;
        yield return new WaitForSeconds(duration);
        speed = oldSpeed;
        isSlowed = false;
    }

}