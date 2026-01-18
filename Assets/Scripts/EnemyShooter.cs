using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // Utilise le même projectile que tes ennemis ou un nouveau rouge
    public float fireRate = 1.5f;
    public float range = 15f;

    private float timer;
    private Transform player;

    void Start() { player = GameObject.FindGameObjectWithTag("Player")?.transform; }

    void Update()
    {
        if (player == null) return;

        // Si le joueur est à portée
        if (Vector3.Distance(transform.position, player.position) < range)
        {
            timer += Time.deltaTime;
            if (timer >= fireRate)
            {
                Shoot();
                timer = 0;
            }
        }
    }

    void Shoot()
    {
        GameObject p = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // On configure le projectile pour viser le joueur
        Vector2 dir = (player.position - transform.position).normalized;

        // ATTENTION : Il faut que ton prefab de projectile ennemi ait un script qui le fait avancer
        // Si tu utilises "ProjectileBehavior" (celui du joueur), il va tuer les ennemis !
        // Il faut créer un script "EnemyProjectile" ou modifier l'existant pour ajouter une variable "targetTag".

        // Solution rapide : Utilise ProjectileBehavior mais change le Layer du prefab ProjectileEnnemi pour qu'il ne touche que le Player.
        ProjectileBehavior pb = p.GetComponent<ProjectileBehavior>();
        if (pb != null)
        {
            pb.direction = dir;
            pb.damage = 15f;
        }
    }
}