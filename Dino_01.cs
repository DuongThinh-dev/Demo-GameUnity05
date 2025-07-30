using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino01_Velociraptor : Enemy
{
    protected override void ApplyPassive()
    {
        speed *= 1.25f;
    }

    protected override void OnDisable()
    {
        // Reset neu enemy duoc tai su dung (object pooling)
        base.OnDisable();

        speed = originalSpeed;

    }
}
