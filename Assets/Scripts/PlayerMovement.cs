using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    // Ajout des références pour l'animation
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 movement;

    void Start()
    {
        // On récupère les composants automatiquement au démarrage
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Sécurité si tu as oublié de lier le RB
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Récupération des entrées (Input)
        float mx = Input.GetAxisRaw("Horizontal");
        float my = Input.GetAxisRaw("Vertical");

        movement = new Vector2(mx, my).normalized;

        // 2. GESTION DE L'ANIMATION
        if (animator != null)
        {
            // On regarde si le joueur bouge (Vecteur > 0)
            bool isMoving = movement.sqrMagnitude > 0;

            // On envoie les infos à l'Animator (correspond aux paramètres de ton image)
            animator.SetBool("moving", isMoving);
            animator.SetFloat("moveX", mx);
            animator.SetFloat("moveY", my);
        }

        // 3. FLIP DU SPRITE (Regarder à gauche ou à droite)
        // Si on va à gauche (mx < 0), on coche "Flip X". Sinon on décoche.
        //if (mx < 0)
        //{
        //    spriteRenderer.flipX = true;
        //}
        //else if (mx > 0)
        //{
        //    spriteRenderer.flipX = false;
        //}
    }

    void FixedUpdate()
    {
        // Application du mouvement physique
        rb.linearVelocity = movement * moveSpeed;
    }
}