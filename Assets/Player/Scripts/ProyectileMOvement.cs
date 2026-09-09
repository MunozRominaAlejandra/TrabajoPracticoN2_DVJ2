using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileMovement2D : MonoBehaviour
{
    [Header("Configuración de Vuelo 2D")]
    public float speed = 12f;
    public float lifetime = 4f;

    private Transform targetEnemy;
    private Vector2 flyDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Desactivamos la gravedad por código para asegurar que no caiga
        rb.gravityScale = 0f;

        // Auto-destrucción tras unos segundos
        Destroy(gameObject, lifetime);

        // Buscar enemigo con Tag 'Enemy'
        targetEnemy = FindClosestEnemy();

        if (targetEnemy != null)
        {
            // Vector de dirección 2D hacia el enemigo (X e Y)
            flyDirection = ((Vector2)targetEnemy.position - (Vector2)transform.position).normalized;

            // Orientar el gráfico hacia el enemigo
            float angle = Mathf.Atan2(flyDirection.y, flyDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90 si la imagen original mira hacia arriba
        }
        else
        {
            // Si no hay enemigo, vuela hacia arriba en la pantalla (+Y)
            flyDirection = Vector2.up;
        }

        // Asignar velocidad en el Rigidbody2D
        rb.linearVelocity = flyDirection * speed;
    }

    void Update()
    {
        // Respaldo por si el Rigidbody2D no se desplaza
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            transform.Translate(flyDirection * speed * Time.deltaTime, Space.World);
        }
    }

    private Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(enemy.transform.position, transform.position);
            if (distance < minDistance)
            {
                closest = enemy;
                minDistance = distance;
            }
        }

        return closest != null ? closest.transform : null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"¡Impacto en enemigo 2D!: {other.name}");
            Destroy(gameObject);
        }
    }
}
