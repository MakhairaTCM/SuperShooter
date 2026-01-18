using UnityEngine;

public class OrbitalContact : MonoBehaviour
{
    public WeaponOrbital parentWeapon;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();

            if (enemy != null && parentWeapon != null)
            {
                float dmg = parentWeapon.GetActualDamage();

                
                Debug.Log($" ORBITAL a touché {other.name} ! Dégâts : {dmg}");

                enemy.TakeDamage(dmg);
            }
        }
    }
}