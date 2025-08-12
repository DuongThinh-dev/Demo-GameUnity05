using UnityEngine;

public class Dino08_Trex : Enemy
{
    [SerializeField] private GameObject reviveEffectPrefab;

    public float reviveHealthPercent = 0.5f; // phan tram mau khi hoi sinh
    public float reviveDelay = 1.5f;         // thoi gian cho hoi sinh

    private bool hasRevived = false;         // da hoi sinh chua
    private bool isReviving = false;         // dang hoi sinh
    private float reviveTimer = 0f;          // bo dem thoi gian hoi sinh
    private Collider myCollider;

    protected override void OnEnable()
    {
        base.OnEnable();
        hasRevived = false;
        isReviving = false;
        reviveTimer = 0f;
        speed = originalSpeed;

        if (myCollider == null)
            myCollider = GetComponent<Collider>();
    }

    protected override void Die()
    {
        if (!hasRevived)
        {
            // bat dau trang thai hoi sinh
            hasRevived = true;
            isReviving = true;
            reviveTimer = reviveDelay;

            // dung di chuyen va vo hieu collider
            speed = 0f;
            if (myCollider != null) myCollider.enabled = false;

            // hoi mau ngay lap tuc
            hp = maxHp * reviveHealthPercent;
            UpdateHPBar();

            // trigger animation revive neu co
            if (anim != null && anim.isActiveAndEnabled)
                anim.SetTrigger("reviveTrigger");

            return;
        }

        base.Die(); // da hoi sinh roi => chet han
    }

    public override void TakeDamage(float dmg)
    {
        if (isReviving) return; // mien nhiem khi dang hoi sinh
        base.TakeDamage(dmg);
    }

    protected override void Update()
    {
        if (isReviving)
        {
            reviveTimer -= Time.deltaTime;
            if (reviveTimer <= 0f)
            {
                // ket thuc hoi sinh
                isReviving = false;
                speed = originalSpeed;
                if (myCollider != null) myCollider.enabled = true;

                // hieu ung hoi sinh
                if (reviveEffectPrefab != null)
                    Instantiate(reviveEffectPrefab, transform.position, Quaternion.identity);

                // dam bao animation chay lai
                if (anim != null && anim.isActiveAndEnabled)
                    anim.Play("Run_08");
            }
        }
    }

    protected override void OnDisable()
    {
        hasRevived = false;
        isReviving = false;
        reviveTimer = 0f;
        speed = originalSpeed;

        base.OnDisable();
    }
}
