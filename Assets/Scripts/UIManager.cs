using UnityEngine;
using UnityEngine.UI; // Nécessaire pour toucher aux Sliders et Textes

public class UIManager : MonoBehaviour
{
    public static UIManager instance; // Singleton

    [Header("Éléments UI")]
    public Slider healthSlider;
    public Slider xpSlider;
    public Text levelText; // Utilise TMPro.TMP_Text si tu utilises TextMeshPro
    public Text killText;

    void Awake()
    {
        // Initialisation du Singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHealthBar(float current, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    public void UpdateXpBar(float current, float required, int level)
    {
        xpSlider.maxValue = required;
        xpSlider.value = current;
        levelText.text = "Lvl : " + level;
    }

    public void UpdateKillCounter(int kills)
    {
        killText.text = "Kills : " + kills;
    }
}