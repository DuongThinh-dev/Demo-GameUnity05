using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino02_Triceratops : Enemy
{
    public override void TakeDamage(float dmg)
    {
        float reducedDamage = dmg * 0.6f;
        base.TakeDamage(reducedDamage);
    }
}

