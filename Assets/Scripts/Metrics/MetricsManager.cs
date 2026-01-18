using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic; // Important

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager instance;

    [Header("Réglages")]
    public float saveInterval = 30f;
    public bool enableLogs = true;

    private GameSessionData currentSession;
    private float startTime;
    private float timer;
    private bool isSessionActive = false;

    private PlayerStats playerStats;
    private Vector3 lastPlayerPosition; // Pour le calcul de distance

    private NetworkUploader uploader;
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        uploader = GetComponent<NetworkUploader>();
    }

    void Update()
    {
        if (isSessionActive && playerStats != null)
        {
            // 1. CALCUL DE LA DISTANCE
            float distFrame = Vector3.Distance(playerStats.transform.position, lastPlayerPosition);
            if (distFrame > 0)
            {
                currentSession.totalDistanceTraveled += distFrame;
                lastPlayerPosition = playerStats.transform.position;
            }

            // 2. LOGIQUE DE SAUVEGARDE AUTOMATIQUE
            timer += Time.deltaTime;
            if (timer >= saveInterval)
            {
                UpdateSessionData("In Progress...");
                SaveToJSON();
                timer = 0f;
            }
        }
    }

    public void StartSession(PlayerStats player)
    {
        if (!enableLogs) return;

        playerStats = player;
        lastPlayerPosition = player.transform.position; // Initialisation position

        currentSession = new GameSessionData();
        currentSession.sessionID = Guid.NewGuid().ToString();
        currentSession.playerName = PlayerPrefs.GetString("Pseudo", "Unknown");
        currentSession.timestampDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        currentSession.version = Application.version;

        startTime = Time.time;
        isSessionActive = true;
        timer = 0f;

        SaveToJSON();
    }

    // --- NOUVEAU : Enregistrer un Boss mort ---
    public void LogBossKill(string bossName)
    {
        if (!isSessionActive) return;
        // On nettoie le nom (enlève le "(Clone)")
        string cleanName = bossName.Replace("(Clone)", "").Trim();
        currentSession.bossesDefeated.Add(cleanName);
        SaveToJSON(); // Sauvegarde immédiate pour un événement important
    }

    // --- MODIFIÉ : Enregistrer les dégâts avec la source ---
    public void LogDamageTaken(float amount, string sourceName)
    {
        if (!isSessionActive) return;

        currentSession.totalDamageTaken += amount;

        // Nettoyage du nom
        string cleanName = sourceName.Replace("(Clone)", "").Trim();

        // Chercher si on a déjà une entrée pour cet ennemi
        DamageSourceEntry entry = currentSession.damageReceivedBySource.Find(x => x.enemyName == cleanName);

        if (entry != null)
        {
            // Il existe déjà, on ajoute les dégâts
            entry.damageAmount += amount;
        }
        else
        {
            // Nouvelle menace, on crée l'entrée
            DamageSourceEntry newEntry = new DamageSourceEntry();
            newEntry.enemyName = cleanName;
            newEntry.damageAmount = amount;
            currentSession.damageReceivedBySource.Add(newEntry);
        }
    }

    public void LogUpgrade(string upgradeName)
    {
        if (!isSessionActive) return;
        string logEntry = $"[{Mathf.Round(Time.time - startTime)}s] {upgradeName}";
        currentSession.upgradesHistory.Add(logEntry);
    }

    void UpdateSessionData(string status)
    {
        if (playerStats == null) return;
        currentSession.timeSurvived = Time.time - startTime;
        currentSession.totalKills = playerStats.killCount;
        currentSession.finalLevel = playerStats.currentLevel;
        currentSession.currentXp = playerStats.currentXp;
        currentSession.causeOfDeath = status;

        currentSession.finalMoveSpeed = playerStats.moveSpeed;
        currentSession.finalMaxHp = playerStats.maxHealth;
        currentSession.finalMight = playerStats.might;
    }

    public void EndSession(string killerName)
    {
        if (!isSessionActive) return;
        UpdateSessionData(killerName);
        SaveToJSON();

        if (uploader != null)
        {
            Debug.Log(" Tentative d'upload final...");
            uploader.SendDataToDatabase(currentSession);
        }
        else
        {
            Debug.LogWarning(" Pas de NetworkUploader trouvé !");
        }

        isSessionActive = false;
        Debug.Log(" Session terminée.");

        isSessionActive = false;
    }

    void SaveToJSON()
    {
        if (currentSession == null) return;
        string json = JsonUtility.ToJson(currentSession, true);
        string folderPath = Path.Combine(Application.persistentDataPath, "Metrics");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        string fileName = $"Log_{currentSession.timestampDate.Replace(":", "-").Replace(" ", "_")}_{currentSession.sessionID}.json";
        File.WriteAllText(Path.Combine(folderPath, fileName), json);
    }
}