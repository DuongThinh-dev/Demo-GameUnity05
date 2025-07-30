using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino08_Trex : Enemy
{
    // (Optional) Hieu ung hoi sinh
    [SerializeField] private GameObject reviveEffectPrefab;

    public float reviveHealthPercent = 0.5f; // luong mau sau hoi sinh

    private bool hasRevived = false; // Da duoc hoi sinh chua
    private bool isReviving = false; // Trang thai dang hoi sinh

    protected override void Die()
    {
        // Neu chua hoi sinh thi hoi sinh
        if (!hasRevived)
        {
            hasRevived = true;
            isReviving = true;

            // Neu co animation hay hieu ung delay, ban co the cho 0.5s truoc khi cho di tiep
            StartCoroutine(ReviveDelay());

            return;
        }

        // Neu da hoi sinh 1 lan -> chet that
        base.Die();
    }

    public override void TakeDamage(float dmg)
    {
        if (isReviving)
        {
            return;
        }

        base.TakeDamage(dmg); // Goi logic mac dinh
    }

    private IEnumerator ReviveDelay()
    {
        speed = 0f;

        // Hoi mau ngay lap tuc (de thanh mau kip cap nhat)
        hp = maxHp * reviveHealthPercent;
        UpdateHPBar(); // goi lai thanh mau neu can

        if (anim != null) anim.SetTrigger("reviveTrigger");

        yield return new WaitForSeconds(1.5f);

        speed = originalSpeed;
        isReviving = false;

        // Co the them hieu ung hoi sinh tai day
        if (reviveEffectPrefab != null)
        {
            Instantiate(reviveEffectPrefab, transform.position, Quaternion.identity);
        }

        // Bat buoc ve lai chay neu trigger khong kip kich hoat
        if (anim != null) anim.Play("Run_08");

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // Reset neu enemy duoc tai su dung (vi du qua object pooling)
        hasRevived = false;
        isReviving = false;
        speed = originalSpeed;

    }
}
