using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino06_Parasaurolophus : Enemy
{
    public float speedBoostMultiplier = 1.9f; // tang 90%
    public float boostDuration = 3f;
    public float cooldown = 6f;

    private bool canTriggerBoost = true; // Dam bao hieu ung khong kich hoat lien tuc
    private bool isBoosting = false; // Dung de tranh nhieu boost chong cheo

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);

        if (canTriggerBoost && !isBoosting)
        {
            StartCoroutine(BoostSpeedRoutine());
        }
    }

    // Tang toc trong 2s, sau do hoi lai toc do goc va cho cooldown
    private IEnumerator BoostSpeedRoutine()
    {
        canTriggerBoost = false;
        isBoosting = true;

        float boostedSpeed = speed * speedBoostMultiplier;
        speed = boostedSpeed;

        yield return new WaitForSeconds(boostDuration);

        speed = originalSpeed;
        isBoosting = false;

        yield return new WaitForSeconds(cooldown - boostDuration); // Thai gian hoi con lai
        canTriggerBoost = true;
    }
}

