using UnityEngine;
using UnityEngine.UI;

public class UIBossManager : MonoBehaviour
{
    public static UIBossManager instance;
    public GameObject panel; // Le parent du slider
    public Slider hpSlider;
    public Text bossNameText;

    void Awake() { instance = this; HideBossHealth(); }

    public void ShowBossHealth(float maxHp, string name)
    {
        panel.SetActive(true);
        hpSlider.maxValue = maxHp;
        hpSlider.value = maxHp;
        bossNameText.text = name;
    }

    public void UpdateBossHealth(float currentHp)
    {
        hpSlider.value = currentHp;
    }

    public void HideBossHealth()
    {
        panel.SetActive(false);
    }
}