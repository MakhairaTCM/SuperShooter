using System.Collections.Generic;
using UnityEngine;

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

    // --- NOUVEAU : Distance & Boss ---
    public float totalDistanceTraveled; // En unités Unity (mètres)
    public List<string> bossesDefeated = new List<string>();

    // --- NOUVEAU : Dégâts reçus par type d'ennemi ---
    // On utilise une liste de cette petite classe ci-dessous pour le JSON
    public List<DamageSourceEntry> damageReceivedBySource = new List<DamageSourceEntry>();
    public float totalDamageTaken; // Somme globale

    // --- Build ---
    public float finalMoveSpeed;
    public float finalMaxHp;
    public float finalMight;
    public List<string> upgradesHistory = new List<string>();
    public List<Vector2> positionHistory = new List<Vector2>();
}

// Petite classe utilitaire pour stocker "Qui m'a fait mal et combien"
[System.Serializable]
public class DamageSourceEntry
{
    public string enemyName;
    public float damageAmount;
}