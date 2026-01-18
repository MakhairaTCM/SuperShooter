using UnityEngine;

public class WeaponShooter : WeaponBase
{
    public GameObject projectilePrefab;
    public float range = 10f;
    public LayerMask enemyLayer; // On sélectionnera "Enemy" ici dans l'inspecteur

    protected override void Attack()
    {
        // Optimisation : On ne cherche que les objets sur le calque "Enemy"
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        // On parcourt la liste (c'est une copie, donc sûr à utiliser)
        foreach (var hit in hits)
        {
            // Sécurité : on vérifie que l'ennemi n'a pas été détruit entre temps
            if (hit == null || hit.transform == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        if (nearest != null)
        {
            GameObject p = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            ProjectileBehavior pb = p.GetComponent<ProjectileBehavior>();

            // Sécurité si le script est manquant
            if (pb != null)
            {
                pb.damage = damage;
                pb.direction = (nearest.position - transform.position).normalized;

                // Rotation visuelle de la balle
                float angle = Mathf.Atan2(pb.direction.y, pb.direction.x) * Mathf.Rad2Deg;
                p.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
        }
    }

    protected override void ApplyLevelUpStats()
    {
        damage *= 1.2f;
        cooldown *= 0.9f;
    }
}