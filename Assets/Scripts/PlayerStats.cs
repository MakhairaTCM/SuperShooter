using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerStats : MonoBehaviour
{
    [Header("Stats de Combat")]
    public float moveSpeed = 5f; // Elle est de retour !
    public float might = 1.0f;   // Multiplicateur de dégâts
    public float armor = 0f;     // Réduction de dégâts
    public float recovery = 0f;  // Regen PV (optionnel pour l'instant)

    [Header("Santé")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Progression")]
    public int killCount = 0;
    public int currentLevel = 1;
    public float currentXp = 0f;
    public float xpRequired = 10f;

    void Start()
    {
        currentHealth = maxHealth;

        // Récupère le pseudo (sauvegardé dans le menu précédemment)
        string pseudo = PlayerPrefs.GetString("Pseudo", "Tester");

        if (MetricsManager.instance != null)
        {
            MetricsManager.instance.StartSession(this);
        }

        UpdateUI();
    }

    void Update()
    {
        // Petite regen de vie (optionnel)
        if (recovery > 0 && currentHealth < maxHealth)
        {
            currentHealth += recovery * Time.deltaTime;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            UpdateUI();
        }
    }

    public void TakeDamage(float dmg, string sourceName = "Inconnu")
    {
        float finalDamage = Mathf.Max(1, dmg - armor);
        currentHealth -= finalDamage;

        // --- ENVOI AU MANAGER ---
        if (MetricsManager.instance != null)
        {
            MetricsManager.instance.LogDamageTaken(finalDamage, sourceName);
        }

        if (currentHealth <= 0)
        {
            // On passe aussi le nom du tueur à la fin de session
            Die(sourceName);
        }

        UpdateUI();
    }

    public void GainExperience(float amount)
    {
        currentXp += amount;

        // Boucle while au cas où on gagne assez d'XP pour passer 2 niveaux d'un coup
        while (currentXp >= xpRequired)
        {
            LevelUp();
        }

        UpdateUI();
    }

    public void AddKill()
    {
        killCount++;
        UpdateUI();
    }

    void LevelUp()
    {
        currentLevel++;
        currentXp -= xpRequired;
        xpRequired *= 1.2f;

        Debug.Log("LEVEL UP! Appel de l'UI...");

        // APPEL DU NOUVEAU SYSTÈME
        if (LevelUpManager.instance != null)
        {
            LevelUpManager.instance.ShowLevelUpOptions();
        }
    }

    void UpdateUI()
    {
        // On vérifie que le UIManager existe pour éviter des erreurs si tu testes sans UI
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);
            UIManager.instance.UpdateXpBar(currentXp, xpRequired, currentLevel);
            UIManager.instance.UpdateKillCounter(killCount);
        }
    }

    void Die(string killer)
    {
        Debug.Log("GAME OVER");
        if (MetricsManager.instance != null)
        {
            MetricsManager.instance.EndSession(killer);
        }
        SceneManager.LoadScene("MainMenu");
    }
}