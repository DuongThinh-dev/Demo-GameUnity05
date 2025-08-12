using UnityEngine;

public class Dino06_Parasaurolophus : Enemy
{
    public float speedBoostMultiplier = 1.9f; // Tang 90%
    public float boostDuration = 3f;
    public float cooldown = 6f;

    private bool canTriggerBoost = true; // Co the kich hoat hay khong
    private bool isBoosting = false;     // Dang tang toc hay khong

    private float boostTimer = 0f;       // Thoi gian con lai khi tang toc
    private float cooldownTimer = 0f;    // Thoi gian hoi chieu con lai

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);

        if (canTriggerBoost && !isBoosting && IsAlive())
        {
            // Bat dau tang toc
            canTriggerBoost = false;
            isBoosting = true;
            boostTimer = boostDuration;

            speed = speed * speedBoostMultiplier;
        }
    }

    protected override void Update()
    {
        // Neu dang boost -> dem nguoc
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                // Het boost -> tra lai toc do goc
                speed = originalSpeed;
                isBoosting = false;
                cooldownTimer = cooldown; // Bat dau hoi chieu
            }
        }
        // Neu dang hoi chieu -> dem nguoc
        else if (!canTriggerBoost)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canTriggerBoost = true;
            }
        }
    }
}
