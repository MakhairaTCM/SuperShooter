using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string name;
        public GameObject enemyPrefab;
        public float spawnStartTime;
        public float weight = 1f;
    }

    [Header("Configuration")]
    public List<Wave> enemies;
    public GameObject boss1Prefab;
    public GameObject boss2Prefab;

    [Header("Paramètres de Spawn")]
    public float spawnRadius = 12f;
    public float baseSpawnInterval = 1f; // Temps de base entre les vagues
    public float minSpawnInterval = 0.2f; // Cap pour ne pas faire crasher le jeu

    private float gameTimer;
    private float spawnTimer;
    private Transform player;

    // Gestion Boss
    private bool boss1Spawned = false;
    private bool boss2Spawned = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        gameTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // ... Gestion Boss ...
        // Boss 1 apparaît à 60 secondes (1 minute)
        if (gameTimer >= 60f && !boss1Spawned)
        {
            if (boss1Prefab != null) SpawnBoss(boss1Prefab);
            boss1Spawned = true; // La variable est maintenant "utilisée" !
        }

        // Boss 2 apparaît à 120 secondes (2 minutes)
        if (gameTimer >= 120f && !boss2Spawned)
        {
            if (boss2Prefab != null) SpawnBoss(boss2Prefab);
            boss2Spawned = true; // La variable est utilisée
        }

        // Intervalle de spawn (On revient à la réduction douce normale : 0.1f)
        float currentInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - (gameTimer / 60f) * 0.1f);

        if (spawnTimer >= currentInterval)
        {
            // C'EST ICI QU'ON APPLIQUE LE X2
            // On calcule le nombre normal, et on le double.
            int enemiesPerBatch = (1 + (int)(gameTimer / 60f)) * 2;

            for (int i = 0; i < enemiesPerBatch; i++)
            {
                SpawnRandomEnemy();
            }

            spawnTimer = 0f;
        }
    }

    void SpawnBoss(GameObject bossPrefab)
    {
        Vector2 spawnPos = GetRandomSpawnPos();
        Debug.Log(" WARNING : BOSS APPROACHING ");
        Instantiate(bossPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnRandomEnemy()
    {
        // ... (Logique inchangée pour le choix de l'ennemi) ...
        List<Wave> availableEnemies = new List<Wave>();
        float totalWeight = 0f;

        foreach (var wave in enemies)
        {
            if (gameTimer >= wave.spawnStartTime)
            {
                availableEnemies.Add(wave);
                totalWeight += wave.weight;
            }
        }

        if (availableEnemies.Count == 0) return;

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;
        GameObject selectedPrefab = availableEnemies[0].enemyPrefab;

        foreach (var wave in availableEnemies)
        {
            cumulativeWeight += wave.weight;
            if (randomValue <= cumulativeWeight)
            {
                selectedPrefab = wave.enemyPrefab;
                break;
            }
        }

        Vector2 spawnPos = GetRandomSpawnPos();
        GameObject newEnemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

        // Buff global des stats (inchangé)
        float scalingFactor = 1f + (gameTimer / 60f) * 0.1f;
        EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
        if (ai != null) ai.ApplyGlobalBuff(scalingFactor);
    }

    Vector2 GetRandomSpawnPos()
    {
        // On ajoute un petit aléatoire pour que les ennemis du même "batch" ne soient pas empilés
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        return (Vector2)player.position + randomDir * spawnRadius;
    }
}