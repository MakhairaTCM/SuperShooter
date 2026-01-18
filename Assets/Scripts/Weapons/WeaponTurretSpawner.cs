using UnityEngine;
using System.Collections.Generic; // Nécessaire pour utiliser les Listes

public class WeaponTurretSpawner : WeaponBase
{
    [Header("Réglages Tourelle")]
    public GameObject turretPrefab;
    public int maxTurrets = 1; // Au niveau 1, on en a 1 seule

    // Une liste pour garder en mémoire les tourelles vivantes
    private List<GameObject> activeTurrets = new List<GameObject>();

    protected override void Update()
    {
        // On nettoie la liste à chaque frame : on enlève les tourelles qui ont été détruites (null)
        // Cela permet de savoir combien il en reste vraiment
        activeTurrets.RemoveAll(item => item == null);

        base.Update(); // Continue de gérer le timer du WeaponBase
    }

    protected override void Attack()
    {
        // On ne pose une nouvelle tourelle que si on n'a pas atteint le maximum
        if (activeTurrets.Count < maxTurrets)
        {
            SpawnTurret();
        }
    }

    void SpawnTurret()
    {
        // On la fait apparaître à la position du joueur
        GameObject newTurret = Instantiate(turretPrefab, transform.position, Quaternion.identity);

        // On l'ajoute à notre liste de surveillance
        activeTurrets.Add(newTurret);
    }

    protected override void ApplyLevelUpStats()
    {
        // À chaque niveau, on augmente le nombre max de tourelles
        maxTurrets++;

        // Optionnel : On réduit aussi un peu le délai de vérification (cooldown)
        // pour qu'elles réapparaissent plus vite si elles meurent
        cooldown *= 0.9f;

        Debug.Log("Max Turrets : " + maxTurrets);
    }
}