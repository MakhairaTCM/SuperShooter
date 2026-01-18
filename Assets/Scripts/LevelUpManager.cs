using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // Nécessaire pour manipuler les listes facilement

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager instance;

    [Header("UI")]
    public GameObject levelUpPanel;
    public Button[] optionButtons;
    public Text[] titleTexts; // Les textes de titre des 3 boutons
    public Text[] descTexts;  // Les textes de description des 3 boutons

    [Header("Data")]
    public List<GameObject> allWeaponPrefabs; // Glisse tes 5 armes ici

    private PlayerStats playerStats;

    // Une petite classe interne pour définir une "Option"
    private class UpgradeOption
    {
        public string title;
        public string description;
        public System.Action action; // La fonction à exécuter quand on clique
    }

    void Awake()
    {
        if (instance == null) instance = this;
        levelUpPanel.SetActive(false);
    }

    void Start()
    {
        // On trouve le joueur une fois pour toutes
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerStats = p.GetComponent<PlayerStats>();
    }

    public void ShowLevelUpOptions()
    {
        Time.timeScale = 0; // Pause le jeu
        levelUpPanel.SetActive(true);

        List<UpgradeOption> pool = GeneratePossibleUpgrades();

        // Sécurité : Si on a moins de 3 options dispos, on prend tout ce qu'il y a
        int optionsCount = Mathf.Min(pool.Count, 3);

        // Mélange la liste et prend les 3 premiers
        // (C'est une façon simple de mélanger une liste en C#)
        var selectedOptions = pool.OrderBy(x => Random.value).Take(optionsCount).ToList();

        // Assigner les boutons
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < selectedOptions.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                UpgradeOption opt = selectedOptions[i];

                titleTexts[i].text = opt.title;
                descTexts[i].text = opt.description;

                // Nettoyer les anciens clics et ajouter le nouveau
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => SelectOption(opt));
            }
            else
            {
                // Cache le bouton s'il n'y a pas assez d'options
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SelectOption(UpgradeOption opt)
    {
        opt.action.Invoke();

        // LOG ICI
        if (MetricsManager.instance != null)
        {
            MetricsManager.instance.LogUpgrade(opt.title);
        }

        levelUpPanel.SetActive(false);
        Time.timeScale = 1;
    }

    List<UpgradeOption> GeneratePossibleUpgrades()
    {
        List<UpgradeOption> pool = new List<UpgradeOption>();

        // ------------------------------------------------
        // 1. LES STATS GLOBALES (Toujours dispos)
        // ------------------------------------------------

        pool.Add(new UpgradeOption
        {
            title = "Soins",
            description = "Rend 30 PV",
            action = () => { playerStats.currentHealth += 30; if (playerStats.currentHealth > playerStats.maxHealth) playerStats.currentHealth = playerStats.maxHealth; }
        });

        pool.Add(new UpgradeOption
        {
            title = "Santé Max",
            description = "+20 PV Max",
            action = () => { playerStats.maxHealth += 20; playerStats.currentHealth += 20; }
        });

        pool.Add(new UpgradeOption
        {
            title = "Armure",
            description = "+1 Armure (Réduit dégâts reçus)",
            action = () => { playerStats.armor += 1; }
        });

        pool.Add(new UpgradeOption
        {
            title = "Vitesse",
            description = "+10% Vitesse de déplacement",
            action = () => { playerStats.moveSpeed *= 1.1f; }
        });

        pool.Add(new UpgradeOption
        {
            title = "Puissance",
            description = "+10% Dégâts globaux",
            action = () => { playerStats.might += 0.1f; }
        });


        // ------------------------------------------------
        // 2. GESTION DES ARMES
        // ------------------------------------------------

        // Récupérer les armes actuelles du joueur
        WeaponBase[] currentWeapons = playerStats.GetComponentsInChildren<WeaponBase>();

        // A. UPGRADE DES ARMES EXISTANTES
        foreach (WeaponBase weapon in currentWeapons)
        {
            pool.Add(new UpgradeOption
            {
                title = "Améliorer " + weapon.weaponName,
                description = "Niveau " + (weapon.currentLevel + 1),
                action = () => { weapon.LevelUp(); }
            });
        }

        // B. NOUVELLES ARMES (Seulement si < 3 armes)
        if (currentWeapons.Length < 3)
        {
            foreach (GameObject weaponPrefab in allWeaponPrefabs)
            {
                WeaponBase script = weaponPrefab.GetComponent<WeaponBase>();

                // Vérifier si le joueur possède DÉJÀ cette arme (par son nom)
                bool alreadyHasIt = false;
                foreach (var owned in currentWeapons)
                {
                    if (owned.weaponName == script.weaponName) alreadyHasIt = true;
                }

                // Si on ne l'a pas, on l'ajoute au pool
                if (!alreadyHasIt)
                {
                    // Copie locale pour la closure (très important dans les boucles for/foreach)
                    GameObject prefabRef = weaponPrefab;

                    pool.Add(new UpgradeOption
                    {
                        title = "Nouvelle Arme : " + script.weaponName,
                        description = "Niveau 1",
                        action = () => {
                            GameObject newW = Instantiate(prefabRef, playerStats.transform.position, Quaternion.identity);
                            newW.transform.SetParent(playerStats.transform);
                        }
                    });
                }
            }
        }

        return pool;
    }
}