using UnityEngine;

public class MenuEnemyDecoratorRB : MonoBehaviour
{
    public float speed = 1.5f;
    public float distance = 3f;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    private Vector2 startPos;
    private int direction = 1;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;

        // Important pour un menu : pas de gravit�, pas de rotation
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        float offset = rb.position.x - startPos.x;

        if (Mathf.Abs(offset) >= distance)
        {
            direction *= -1;
        }

        rb.linearVelocity = new Vector2(direction * speed, 0f);

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction > 0;
    }
}
