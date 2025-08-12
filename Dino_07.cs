using UnityEngine;

public class Dino07_Brachiosaurus : Enemy
{
    private float healPercent = 0.2f;   // phan tram mau hoi
    private float healCooldown = 6f;    // thoi gian cho hoi tiep
    private float checkInterval = 0.5f; // thoi gian kiem tra lai dieu kien
    private float healTimer = 0f;       // bo dem hoi mau
    private float checkTimer = 0f;      // bo dem kiem tra

    protected override void OnEnable()
    {
        base.OnEnable();
        healTimer = 0f;
        checkTimer = 0f;
    }

    protected override void Update()
    {
        if (!IsAlive()) return;

        // Neu dang trong thoi gian cooldown hoi mau
        if (healTimer > 0f)
        {
            healTimer -= Time.deltaTime;
            return;
        }

        // Kiem tra moi checkInterval giay
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = checkInterval;

            // Neu mau duoi 50% -> hoi mau
            if (hp < maxHp * 0.5f)
            {
                float healAmount = maxHp * healPercent;
                hp += healAmount;
                if (hp > maxHp) hp = maxHp;

                UpdateHPBar(); // cap nhat thanh mau

                // Bat dau cooldown
                healTimer = healCooldown;
            }
        }
    }
}
