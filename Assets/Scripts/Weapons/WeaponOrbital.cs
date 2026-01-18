using UnityEngine;
using System.Collections.Generic;

public class WeaponOrbital : WeaponBase
{
    [Header("Réglages Orbital")]
    public GameObject projectilePrefab; // Le visuel de la boule
    public float rotationSpeed = 100f;  // Vitesse de rotation (degrés par seconde)
    public float radius = 2f;           // Distance par rapport au joueur
    public int projectileCount = 1;     // Nombre de boules initial

    private List<GameObject> activeProjectiles = new List<GameObject>();

    void Start()
    {
        SpawnProjectiles();
    }

    // On remplace l'Update par défaut pour gérer la rotation continue
    protected override void Update()
    {
        // Fait tourner l'objet Pivot sur lui-même (axe Z)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // On garde quand même la logique de base (timer) si jamais on veut ajouter un effet pulsé plus tard
        base.Update();
    }

    // Cette fonction ne sert pas pour cette arme car elle est "toujours active", 
    // mais WeaponBase nous oblige à l'avoir. On la laisse vide.
    protected override void Attack() { }

    // Fonction pour placer les boules en cercle parfait
    void SpawnProjectiles()
    {
        // 1. Nettoyer les anciennes boules
        foreach (var p in activeProjectiles) Destroy(p);
        activeProjectiles.Clear();

        // 2. Créer les nouvelles
        for (int i = 0; i < projectileCount; i++)
        {
            // Calcul de l'angle pour bien les répartir (ex: 2 boules = 0° et 180°)
            float angle = i * (360f / projectileCount);

            // On crée un conteneur vide temporaire pour positionner l'objet
            // Astuce mathématique : On utilise la rotation locale pour placer l'objet
            GameObject newBall = Instantiate(projectilePrefab, transform);

            // On le place au centre, on le tourne, puis on l'avance
            newBall.transform.localPosition = Vector3.zero;
            newBall.transform.localRotation = Quaternion.Euler(0, 0, angle);
            newBall.transform.Translate(Vector3.right * radius); // Éloigne la balle du centre

            // IMPORTANT : On remet la rotation de la balle à 0 pour qu'elle ne tourne pas sur elle-même bizarrement
            // (Optionnel selon le visuel voulu, mais mieux pour des sprites)
            newBall.transform.localRotation = Quaternion.identity;

            // On configure le script de contact
            OrbitalContact contact = newBall.GetComponent<OrbitalContact>();
            if (contact != null) contact.parentWeapon = this;

            activeProjectiles.Add(newBall);
        }
    }

    protected override void ApplyLevelUpStats()
    {
        // Logique d'amélioration sympa :
        // Niveau pair (2, 4...) : + de Dégâts et Vitesse
        // Niveau impair (3, 5...) : +1 Boule

        if (currentLevel % 2 != 0) // Si niveau impair (3, 5...)
        {
            projectileCount++;
            SpawnProjectiles(); // On redessine le cercle avec la nouvelle balle
            Debug.Log("Orbital : Ajout d'une boule !");
        }
        else // Si niveau pair
        {
            damage *= 1.2f;
            rotationSpeed += 30f;
            Debug.Log("Orbital : Plus rapide et plus fort !");
        }
    }
}