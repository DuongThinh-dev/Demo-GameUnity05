using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino03_Ankylosaurus : Enemy
{
    protected override void ApplyPassive()
    {
        armor *= 1.4f;
    }

    protected override void OnDisable()
    {
        // Reset neu enemy duoc tai su dung (object pooling)
        base.OnDisable();

        armor /= 1.4f;

    }
}

