using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager instance;

    [Header("Réglages")]
    public float saveInterval = 15f; // <--- MODIFIÉ : Passé de 30 à 15 secondes
    public bool enableLogs = true;

    private GameSessionData currentSession;
    private float startTime;
    private float timer;
    private bool isSessionActive = false;

    private PlayerStats playerStats;
    private Vector3 lastPlayerPosition;

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
            float distFrame = Vector3.Distance(playerStats.transform.position, lastPlayerPosition);
            if (distFrame > 0)
            {
                currentSession.totalDistanceTraveled += distFrame;
                lastPlayerPosition = playerStats.transform.position;
            }

           
            timer += Time.deltaTime;
            if (timer >= saveInterval)
            {
               
                UpdateSessionData("In Progress...");

               
                SaveToJSON();

              
                if (uploader != null)
                {
                    
                    uploader.SendDataToDatabase(currentSession);
                }

                timer = 0f;
            }
        }
    }

    public void StartSession(PlayerStats player)
    {
        if (!enableLogs) return;

        playerStats = player;
        lastPlayerPosition = player.transform.position;

        currentSession = new GameSessionData();
        currentSession.sessionID = Guid.NewGuid().ToString();
        currentSession.playerName = PlayerPrefs.GetString("Pseudo", "Unknown");
        currentSession.timestampDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        currentSession.version = Application.version;

        startTime = Time.time;
        isSessionActive = true;
        timer = 0f;

        SaveToJSON();
        
        if (uploader != null) uploader.SendDataToDatabase(currentSession);
    }

    public void LogBossKill(string bossName)
    {
        if (!isSessionActive) return;
        string cleanName = bossName.Replace("(Clone)", "").Trim();
        currentSession.bossesDefeated.Add(cleanName);

        
        UpdateSessionData("In Progress (Boss Kill)");
        SaveToJSON();
        if (uploader != null) uploader.SendDataToDatabase(currentSession);
    }

    public void LogDamageTaken(float amount, string sourceName)
    {
        if (!isSessionActive) return;

        currentSession.totalDamageTaken += amount;
        string cleanName = sourceName.Replace("(Clone)", "").Trim();

        DamageSourceEntry entry = currentSession.damageReceivedBySource.Find(x => x.enemyName == cleanName);

        if (entry != null)
        {
            entry.damageAmount += amount;
        }
        else
        {
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

        
        UpdateSessionData("In Progress (Level Up)");

   
        SaveToJSON();

       
        if (uploader != null)
        {
            uploader.SendDataToDatabase(currentSession);
        }
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
    }

    void SaveToJSON()
    {
        if (currentSession == null) return;
        string json = JsonUtility.ToJson(currentSession, true);
        string folderPath = Path.Combine(Application.persistentDataPath, "Metrics");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        // Note : Le nom de fichier contient l'ID unique, donc on écrase le même fichier 
        string fileName = $"Log_{currentSession.timestampDate.Replace(":", "-").Replace(" ", "_")}_{currentSession.sessionID}.json";
        File.WriteAllText(Path.Combine(folderPath, fileName), json);
    }
}