using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino07_Brachiosaurus : Enemy
{
    public float healPercent = 0.4f;
    public float healCooldown = 6f;

    private bool canHeal = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        canHeal = true;
        StartCoroutine(HealRoutine());
    }

    private IEnumerator HealRoutine()
    {
        while (IsAlive())
        {
            if (canHeal && hp < maxHp * 0.5f)
            {
                float healAmount = maxHp * healPercent;
                hp += healAmount;
                if (hp > maxHp) hp = maxHp;

                Debug.Log("Brachiosaurus healed for: " + healAmount);

                canHeal = false;
                yield return new WaitForSeconds(healCooldown);
                canHeal = true;
            }
            else
            {
                yield return new WaitForSeconds(0.5f); // ki?m tra th??ng xuyên h?n
            }
        }
    }
}

