using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    public Vector3 direction;

    [Header("Durée de vie")]
    public float lifetime = 15f; // Tu peux changer ça dans l'inspector

    void Start()
    {
        // C'est la ligne magique :
        // "Programme mon auto-destruction dans 15 secondes"
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Déplacement
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // Si on touche, on se détruit tout de suite (ce qui annule le timer du Start)
                Destroy(gameObject);
            }
        }
    }
}