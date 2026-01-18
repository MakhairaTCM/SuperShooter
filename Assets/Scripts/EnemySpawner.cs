using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string name;             // Nom pour t'y retrouver (ex: "Robot Basic")
        public GameObject enemyPrefab;  // Le prefab
        public float spawnStartTime;    // Apparait � partir de quand ? (secondes)
        public float weight = 1f;       // Chance d'apparition
    }

    [Header("Configuration des Vagues")]
    public List<Wave> enemies;          // Ta liste d'ennemis
    public GameObject boss1Prefab;
    public GameObject boss2Prefab;

    [Header("Param�tres de Spawn & Difficult�")]
    public float spawnRadius = 12f;
    public float baseSpawnInterval = 1f;
    public float minSpawnInterval = 0.1f;

    // NOUVEAU : Contr�le la vitesse � laquelle le jeu devient dur
    // 0.05 = Normal, 0.1 = Difficile, 0.2 = Cauchemar
    public float difficultyScaling = 0.05f;

    [Header("OUTILS DE DEBUG (Tests)")]
    public bool forceSingleType = false;    // Coche pour forcer un seul type
    public int enemyIndexToSpawn = 0;       // L'index dans la liste 'enemies' ci-dessus
    public bool spawnBossNow = false;       // Coche pour faire appara�tre Boss 1 direct

    private float gameTimer;
    private float spawnTimer;
    private Transform player;

    // Gestion Boss Auto
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
        // Boss 1 appara�t � 60 secondes (1 minute)
        if (gameTimer >= 60f && !boss1Spawned)
        {
            if (boss1Prefab != null) SpawnBoss(boss1Prefab);
            boss1Spawned = true; // La variable est maintenant "utilis�e" !
        }

        // Boss 2 appara�t � 120 secondes (2 minutes)
        if (gameTimer >= 120f && !boss2Spawned)
        {
            if (boss2Prefab != null) SpawnBoss(boss2Prefab);
            boss2Spawned = true; // La variable est utilis�e
        }

        // --- 2. GESTION BOSS AUTO (Temps) ---
        // Exemple : Boss 1 � 300s (5min), Boss 2 � 600s (10min)
        if (!boss1Spawned && gameTimer >= 300f)
        {
            SpawnBoss(boss1Prefab);
            boss1Spawned = true;
        }
        if (!boss2Spawned && gameTimer >= 600f)
        {
            SpawnBoss(boss2Prefab);
            boss2Spawned = true;
        }

        // --- 3. CALCUL DE L'INTERVALLE (Courbe Logarithmique) ---
        // Formule : Plus le temps avance, plus le diviseur est grand, donc l'intervalle petit.
        float currentInterval = baseSpawnInterval / (1f + (gameTimer * difficultyScaling));

        // On s'assure de ne pas descendre sous le minimum (sinon crash CPU)
        if (currentInterval < minSpawnInterval) currentInterval = minSpawnInterval;

        if (spawnTimer >= currentInterval)
        {
            // --- 4. CALCUL DU NOMBRE D'ENNEMIS (Batch) ---
            // On garde ta logique "x2" car elle est satisfaisante
            int enemiesPerBatch = (1 + (int)(gameTimer / 60f));

            // Optionnel : Si le jeu est tr�s avanc� (> 10min), on double encore
            if (gameTimer > 600) enemiesPerBatch *= 2;

            for (int i = 0; i < enemiesPerBatch; i++)
            {
                SpawnRandomEnemy();
            }

            spawnTimer = 0f;
        }
    }

    void SpawnBoss(GameObject bossPrefab)
    {
        if (bossPrefab == null) return;

        Vector2 spawnPos = GetRandomSpawnPos();
        Debug.Log($" BOSS SPAWN : {bossPrefab.name} !");

        // Instanciation
        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        // On buff le boss selon le temps aussi !
        float bossBuff = 1f + (gameTimer / 60f) * 0.5f; // +50% stats par minute
        EnemyAI ai = boss.GetComponent<EnemyAI>();
        if (ai != null) ai.ApplyGlobalBuff(bossBuff);
    }

    void SpawnRandomEnemy()
    {
        GameObject selectedPrefab = null;

        // --- A. LOGIQUE DEBUG (Force un type) ---
        if (forceSingleType)
        {
            if (enemies.Count > enemyIndexToSpawn)
            {
                selectedPrefab = enemies[enemyIndexToSpawn].enemyPrefab;
            }
            else
            {
                Debug.LogWarning("Index Debug invalide !");
                return;
            }
        }
        // --- B. LOGIQUE NORMALE (Poids & Temps) ---
        else
        {
            List<Wave> availableEnemies = new List<Wave>();
            float totalWeight = 0f;

            foreach (var wave in enemies)
            {
                // On ne prend que ceux qui ont le droit d'appara�tre maintenant
                if (gameTimer >= wave.spawnStartTime)
                {
                    availableEnemies.Add(wave);
                    totalWeight += wave.weight;
                }
            }

            if (availableEnemies.Count == 0) return; // Personne dispo ? On quitte.

            float randomValue = Random.Range(0, totalWeight);
            float cumulativeWeight = 0f;

            foreach (var wave in availableEnemies)
            {
                cumulativeWeight += wave.weight;
                if (randomValue <= cumulativeWeight)
                {
                    selectedPrefab = wave.enemyPrefab;
                    break;
                }
            }
        }

        if (selectedPrefab != null)
        {
            Vector2 spawnPos = GetRandomSpawnPos();
            GameObject newEnemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

            // --- C. BUFF DES STATS (Scaling) ---
            // Formule : +10% de stats toutes les minutes
            float scalingFactor = 1f + (gameTimer / 60f) * 0.1f;

            EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.ApplyGlobalBuff(scalingFactor);
            }
        }
    }

    Vector2 GetRandomSpawnPos()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        return (Vector2)player.position + randomDir * spawnRadius;
    }
}