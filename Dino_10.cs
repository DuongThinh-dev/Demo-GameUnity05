using UnityEngine;

public class Dino10_Hatzegopteryx : Enemy
{
    private bool isFlying;
    private bool previousFlyingState;
    private float baseArmor;

    [Header("Raycast Settings")]
    public float groundCheckDistance;
    public LayerMask groundLayer;

    protected override void OnEnable()
    {
        base.OnEnable();
        baseArmor = armor;
        isFlying = true;
        previousFlyingState = true;

        ApplyPassive();
        UpdateAnimator();
    }

    private void Update()
    {
        CheckLandingWithOverlap();

        // Chi cap nhat khi co thay doi trang thai (bay <-> dat)
        if (isFlying != previousFlyingState)
        {
            ApplyPassive();
            UpdateAnimator();
            previousFlyingState = isFlying;
        }
    }

    void CheckLandingWithOverlap()
    {
        Vector3 checkPosition = transform.position + Vector3.down * groundCheckDistance;
        float checkRadius = 0.2f;

        Collider[] hits = Physics.OverlapSphere(checkPosition, checkRadius, groundLayer);

        isFlying = hits.Length == 0;
    }


    protected override void ApplyPassive()
    {
        if (isFlying)
        {
            speed = originalSpeed;
            armor = baseArmor * 0.5f;
        }
        else
        {
            speed = originalSpeed * 1.1f;
            armor = baseArmor * 1.5f;
        }
    }

    void UpdateAnimator()
    {
        if (anim != null)
        {
            anim.SetBool("isFlying", isFlying);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isFlying = true;
        previousFlyingState = true;
        speed = originalSpeed;
        armor = baseArmor;
    }
}
