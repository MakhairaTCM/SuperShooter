using UnityEngine;
using System.Collections;

public class WeaponSword : WeaponBase
{
    public Vector2 hitBoxSize = new Vector2(2, 1);

    protected override void Attack()
    {
        StartCoroutine(Swing());
    }

    IEnumerator Swing()
    {
        // Logique visuelle (Debug pour l'instant) ou instanciation d'un sprite temporaire
        // Ici on fait les dégâts direct dans la zone devant le joueur
        Vector3 impactPos = transform.position + transform.right * 1.5f; // Devant
        Collider2D[] hits = Physics2D.OverlapBoxAll(impactPos, hitBoxSize, 0);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                hit.GetComponent<EnemyAI>().TakeDamage(damage);
        }
        yield return null;
    }

    protected override void ApplyLevelUpStats()
    {
        damage *= 1.5f; // Gros boost de dégâts
        hitBoxSize *= 1.2f; // L'épée devient plus grosse
    }

    // Pour voir la zone dans l'éditeur
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.right * 1.5f, hitBoxSize);
    }
}