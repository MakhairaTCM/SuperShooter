using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStats stats; // R�f�rence � tes stats
    private Vector2 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();

        // S�curit� : si tu as oubli� de mettre le script PlayerStats
        if (stats == null)
        {
            Debug.LogError("Le script PlayerStats est manquant sur le joueur !");
            stats = gameObject.AddComponent<PlayerStats>();
        }
    }

    void Update()
    {
        // 1. On r�cup�re les touches (ZQSD ou Fl�ches)
        // GetAxisRaw permet un arr�t imm�diat (plus nerveux/arcade)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 2. On cr�e un vecteur et on le "normalise"
        // Normaliser emp�che d'aller plus vite en diagonale
        movementInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // 3. On applique le mouvement physique
        // On utilise stats.moveSpeed qu'on a d�fini dans l'�tape d'avant
        rb.linearVelocity = movementInput * stats.moveSpeed;
    }
}