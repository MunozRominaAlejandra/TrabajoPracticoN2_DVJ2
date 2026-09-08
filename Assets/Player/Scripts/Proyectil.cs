using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [SerializeField] private float velocidad = 12f;
    [SerializeField] private float tiempoDeVida = 3f;

    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        // Al usar Vector3.up el proyectil viajará hacia ARRIBA (hacia el enemigo)
        transform.Translate(Vector3.up * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            // Opcional: Destruir proyectil al chocar con el enemigo
            Destroy(gameObject);
        }
    }
}