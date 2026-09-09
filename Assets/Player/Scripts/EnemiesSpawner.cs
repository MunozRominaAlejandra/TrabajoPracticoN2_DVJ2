
using UnityEngine;

public class EnemySpawner2D : MonoBehaviour
{
    [Header("Prefabs de Enemigos")]
    [Tooltip("Arrastra aquí tus 2 prefabs de enemigos")]
    public GameObject[] enemyPrefabs;

    [Header("Configuración de Tiempos")]
    [Tooltip("Tiempo en segundos entre cada aparición de enemigo")]
    public float spawnInterval = 2.5f;
    [Tooltip("Tiempo de espera antes de que empiece a generar el primer enemigo")]
    public float initialDelay = 1.0f;

    [Header("Límites de Aparición (Eje X)")]
    [Tooltip("Si es verdadero, los enemigos aparecerán en una posición X aleatoria entre MinX y MaxX")]
    public bool randomXPosition = true;
    public float minX = -6f;
    public float maxX = 6f;

    private float timer;

    void Start()
    {
        // El temporizador inicia con el retraso inicial
        timer = spawnInterval - initialDelay;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnRandomEnemy();
            timer = 0f; // Reiniciar temporizador
        }
    }

    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No se han asignado prefabs de enemigos en el Spawner.");
            return;
        }

        // 1. Elegir un enemigo al azar del arreglo de prefabs (entre los 2 que asignes)
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[randomIndex];

        if (selectedPrefab == null) return;

        // 2. Determinar la posición de aparición
        Vector3 spawnPosition = transform.position;

        if (randomXPosition)
        {
            float randomX = Random.Range(minX, maxX);
            spawnPosition = new Vector3(randomX, transform.position.y, transform.position.z);
        }

        // 3. Instanciar el enemigo en la posición calculada
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    // Dibujar una línea visual en la vista de escena para ajustar el rango X fácilmente
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (randomXPosition)
        {
            Vector3 leftLimit = new Vector3(minX, transform.position.y, transform.position.z);
            Vector3 rightLimit = new Vector3(maxX, transform.position.y, transform.position.z);
            Gizmos.DrawLine(leftLimit, rightLimit);
            Gizmos.DrawWireSphere(leftLimit, 0.3f);
            Gizmos.DrawWireSphere(rightLimit, 0.3f);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}

