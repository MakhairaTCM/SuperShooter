using UnityEngine;
using UnityEngine.UI; // Nécessaire pour contrôler le Slider

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 5000f;
    public float damage = 10f;
    public float speed = 2f;
    public float xpValue = 100f;

    [Header("Type")]
    public bool isBoss = false;

    [Header("UI")]
    public Slider healthSlider; // Glisse le Slider ici dans l'inspecteur

    private float currentHp;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Initialisation de la barre de vie locale
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = maxHp;
            healthSlider.gameObject.SetActive(true); // On s'assure qu'elle est visible
        }
    }

    public void ApplyGlobalBuff(float multiplier)
    {
        maxHp *= multiplier;
        currentHp = maxHp;
        damage *= multiplier;
        xpValue *= multiplier;

        // Mettre à jour la barre si les HP max changent
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = currentHp;
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats pStats = collision.gameObject.GetComponent<PlayerStats>();

            if (pStats != null)
            {
                // ON PASSE LE NOM DE L'OBJET (ex: "Enemy_Basic(Clone)")
                pStats.TakeDamage(damage * Time.deltaTime, gameObject.name);
            }
        }
    }
    public void TakeDamage(float amount)
    {
        currentHp -= amount;

        // Mise à jour visuelle immédiate
        if (healthSlider != null)
        {
            healthSlider.value = currentHp;
        }

        if (currentHp <= 0) Die();
    }

    void Die()
    {
        PlayerStats pStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();

        if (isBoss && MetricsManager.instance != null)
        {
            MetricsManager.instance.LogBossKill(gameObject.name);
        }

        if (isBoss && UIBossManager.instance != null) UIBossManager.instance.HideBossHealth();

        Destroy(gameObject);

        if (pStats != null)
        {
            pStats.GainExperience(xpValue);
            pStats.AddKill();
        }
        Destroy(gameObject);


    }
}