using UnityEngine;

public class WeaponGarlic : WeaponBase
{
    public float damageInterval = 0.5f;
    public LayerMask enemyLayer; // Sélectionne "Enemy" dans l'inspecteur

    protected override void Attack()
    {
        // On récupère tous les ennemis dans la zone
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2, enemyLayer);

        foreach (var enemyCollider in enemies)
        {
            // SÉCURITÉ : Si l'ennemi est déjà en train d'être détruit par une autre arme
            if (enemyCollider == null) continue;

            EnemyAI enemyScript = enemyCollider.GetComponent<EnemyAI>(); // ou EnemyHealth

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
            }
        }
    }

    protected override void ApplyLevelUpStats()
    {
        damage *= 1.3f;
        transform.localScale *= 1.1f;
    }
}