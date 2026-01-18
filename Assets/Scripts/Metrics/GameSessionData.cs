using System.Collections.Generic;

[System.Serializable]
public class GameSessionData
{
    // --- Identité ---
    public string sessionID;
    public string playerName;
    public string timestampDate;
    public string version;

    // --- Performance ---
    public float timeSurvived;
    public int totalKills;
    public int finalLevel;
    public float currentXp;
    public string causeOfDeath;

    // --- Position (NOUVEAU) --- 
    public float playerPosX; 
    public float playerPosY;

    // ---  Distance & Boss ---
    public float totalDistanceTraveled; 
    public List<string> bossesDefeated = new List<string>();

    // ---  Dégâts reçus par type d'ennemi ---
   
    public List<DamageSourceEntry> damageReceivedBySource = new List<DamageSourceEntry>();
    public float totalDamageTaken; 

    // --- Build ---
    public float finalMoveSpeed;
    public float finalMaxHp;
    public float finalMight;
    public List<string> upgradesHistory = new List<string>();
}

// Petite classe utilitaire pour stocker "Qui m'a fait mal et combien"
[System.Serializable]
public class DamageSourceEntry
{
    public string enemyName;
    public float damageAmount;
}