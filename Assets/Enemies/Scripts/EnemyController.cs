using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float limiteIzquierdo = -5f;
    [SerializeField] private float limiteDerecho = 5f;
    [SerializeField] private int danio = 10;
    [SerializeField] private Transform jugador;
    [SerializeField] private float rangoAtaque = 1.5f;
    [SerializeField] private float distanciaMinima = 2f;
    [SerializeField] private float tiempoEntreAtaques = 1f;

    private float tiempoUltimoAtaque = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // --- Movimiento hacia el jugador SOLO si está más lejos que la distancia mínima ---
        if (distancia > distanciaMinima)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                jugador.position,
                velocidad * Time.deltaTime
            );
        }
        if (distancia <= rangoAtaque)
        {
            if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
            {
                animator.SetTrigger("attack");
                tiempoUltimoAtaque = Time.time;
            }
        }

        // --- Ataque si está cerca ---
        if (Vector2.Distance(transform.position, jugador.position) < rangoAtaque)
        {
            if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
            {
                animator.SetTrigger("attack"); // dispara la animación
                tiempoUltimoAtaque = Time.time;
            }
        }
    }

    // Este método lo llamás desde un Animation Event en el frame del golpe
    public void AplicarDano()
    {
        PlayerController pc = jugador.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.RecibirDano(danio);
            Debug.Log("El Gólem golpeó al jugador y causó " + danio + " de daño.");
        }
    }

}
