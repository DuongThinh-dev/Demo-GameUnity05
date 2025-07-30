using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dino05_Pachycephalosaurus : Enemy
{
    public float invincibleDuration = 3.5f;

    private bool hasTriggeredInvincibility = false;// Bien co de dam bao chi kich hoat 1 lan duy nhat
    private bool isInvincible = false;// Bat/tat trang thai mien sat thuong

    protected override void ApplyPassive()
    {
        // Passive duoc xu ly trong TakeDamage
    }

    public override void TakeDamage(float dmg)
    {
        // Neu dang bat tu -> khong nhan sat thuong
        if (isInvincible)
        {
            return;
        }

        // Neu mau duoi 50% va chua tung kich hoat bat tu -> kich hoat
        if (!hasTriggeredInvincibility && hp <= maxHp * 0.5f)
        {
            StartCoroutine(ActivateInvincibility());
            return; // Khong nhan sat thuong tai frame nay
        }

        base.TakeDamage(dmg);
    }

    private IEnumerator ActivateInvincibility()
    {
        hasTriggeredInvincibility = true;
        isInvincible = true;

        if (rend != null)
            rend.material.color = Color.yellow;

        yield return new WaitForSeconds(invincibleDuration);

        if (rend != null)
            rend.material.color = originalColor;

        isInvincible = false;
    }
}

