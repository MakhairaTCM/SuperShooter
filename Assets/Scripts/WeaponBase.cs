using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Stats Globales")]
    public string weaponName;
    public int currentLevel = 1;
    public float damage;
    public float cooldown; // Cadence de tir

    protected float timer;

    protected virtual void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Attack();
            timer = cooldown;
        }
    }

    // Chaque arme définira sa propre façon d'attaquer
    protected abstract void Attack();

    // Système de Level Up générique
    public void LevelUp()
    {
        currentLevel++;
        ApplyLevelUpStats();
        Debug.Log(weaponName + " est passé au niveau " + currentLevel);
    }
    // Ajoute cette méthode pour calculer les dégâts finaux
    public float GetActualDamage()
    {
        // On cherche les stats du joueur (le parent de l'arme)
        PlayerStats stats = GetComponentInParent<PlayerStats>();
        if (stats != null)
        {
            return damage * stats.might; // Dégâts de l'arme x Force du joueur
        }
        return damage;
    }

    // Chaque arme définira quels stats augmentent
    protected abstract void ApplyLevelUpStats();
}