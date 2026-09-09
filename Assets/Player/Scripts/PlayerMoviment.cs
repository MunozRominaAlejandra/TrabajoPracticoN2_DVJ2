using UnityEngine;
using UnityEngine.InputSystem; // Requerido para el nuevo Input System

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    [Tooltip("Velocidad de movimiento horizontal del jugador")]
    public float moveSpeed = 8f;

    [Header("L�mites de Pantalla (Opcional)")]
    public bool useLimits = false;
    public float minX = -8f;
    public float maxX = 8f;

    private Rigidbody2D rb;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Configuraciones de seguridad para el Rigidbody2D
        rb.gravityScale = 0f; // Evita que el jugador caiga
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Evita que el personaje se voltee al chocar
    }

    void Update()
    {
        // Leer las teclas de flechas (Izquierda / Derecha) o A / D con el nuevo Input System
        horizontalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                horizontalInput = -1f;
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                horizontalInput = 1f;
            }
        }
    }

    void FixedUpdate()
    {
        // Aplicar movimiento horizontal suave en la f�sica
        Vector2 velocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = velocity;

        // Limitar la posici�n dentro de los bordes de la pantalla si est� activado
        if (useLimits)
        {
            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }
}
