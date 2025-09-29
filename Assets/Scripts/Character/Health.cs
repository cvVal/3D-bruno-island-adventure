using System;
using UnityEngine;

namespace RPG.Character
{
    public class Health : MonoBehaviour
    {
        [NonSerialized] public float HealthPoints;

        public void TakeDamage(float damageAmount)
        {
            HealthPoints = Mathf.Max(HealthPoints - damageAmount, 0);
            print(HealthPoints);
        }
    }
}