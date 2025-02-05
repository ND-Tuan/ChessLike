using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    bool Freezing { get; set; }
    bool Burning { get; set; }

    void TakeDamage(int damage, Vector3 hitPoint = default(Vector3));

    void BuffsTrigger();
}
