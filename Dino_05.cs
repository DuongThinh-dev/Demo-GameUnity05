using UnityEngine;

public class Dino05_Pachycephalosaurus : Enemy
{
    public float invincibleDuration = 3.5f;

    private bool hasTriggeredInvincibility = false; // Chi kich hoat 1 lan
    private bool isInvincible = false;              // Dang trong trang thai mien sat thuong
    private float invincibleTimer = 0f;             // Bo dem thoi gian

    protected override void ApplyPassive()
    {
        // Passive xu ly trong TakeDamage
    }

    public override void TakeDamage(float dmg)
    {
        if (isInvincible)
            return; // Dang bat tu thi bo qua sat thuong

        if (!hasTriggeredInvincibility && hp <= maxHp * 0.5f && IsAlive())
        {
            // Kich hoat bat tu
            hasTriggeredInvincibility = true;
            isInvincible = true;
            invincibleTimer = invincibleDuration;

            if (rend != null)
                rend.material.color = Color.yellow;

            return; // Frame nay khong nhan damage
        }

        base.TakeDamage(dmg);
    }

    protected override void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;

                if (rend != null)
                    rend.material.color = originalColor;
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // Reset khi bat lai object
        isInvincible = false;
        hasTriggeredInvincibility = false;
        invincibleTimer = 0f;

        if (rend != null)
            rend.material.color = originalColor;
    }

    protected override void OnDisable()
    {
        // Reset khi tat object
        isInvincible = false;
        hasTriggeredInvincibility = false;
        invincibleTimer = 0f;

        if (rend != null)
            rend.material.color = originalColor;
    }
}
