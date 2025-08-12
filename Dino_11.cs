using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino11_Spinosaurus : Enemy
{
    private bool isSwimming;
    private bool previousState;
    private float baseArmor;

    [Header("Environment Check")]
    public float checkRadius = 0.3f;
    public LayerMask waterLayer;

    protected override void OnEnable()
    {
        base.OnEnable();
        baseArmor = armor;

        isSwimming = true;
        previousState = true;

        ApplyPassive();
        UpdateAnimator();
    }

    protected override void ApplyPassive()
    {
        if (isSwimming)
        {
            speed = originalSpeed * 1.5f;
            armor = baseArmor * 1.5f;
        }
        else
        {
            speed = originalSpeed;
            armor = baseArmor * 1.4f;
        }
    }

    protected override void Update()
    {
        CheckEnvironment();

        if (isSwimming != previousState)
        {
            ApplyPassive();
            UpdateAnimator();
            previousState = isSwimming;
        }
    }

    void CheckEnvironment()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius, waterLayer);
        isSwimming = hits.Length > 0;
    }

    void UpdateAnimator()
    {
        if (anim != null)
        {
            anim.SetBool("isSwimming", isSwimming);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        speed = originalSpeed;
        armor = baseArmor;
        isSwimming = true;
        previousState = true;
    }
}
