using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el nuevo Input System
using Meta.WitAi;
using Meta.WitAi.Json;
using Meta.Voice; // Namespace actualizado de Meta Voice SDK

public class PlayerController : MonoBehaviour
{
    [Header("Referencias de Meta Voice")]
    [SerializeField] private VoiceService voiceExperience;

    [Header("Prefabs de Ataque")]
    [SerializeField] private GameObject prefabFuego;
    [SerializeField] private GameObject prefabHielo;
    [SerializeField] private Transform puntoDeDisparo; // Objeto vacío ubicado en la cabeza/parte superior del player

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Sensibilidad de Voz Alta")]
    [SerializeField] private float umbralVolumenAlto = 0.25f;

    private float volumenActualMic = 0f;

    private void OnEnable()
    {
        if (voiceExperience != null)
        {
            voiceExperience.VoiceEvents.OnResponse.AddListener(OnVoiceResponse);
            voiceExperience.VoiceEvents.OnMicAudioLevelChanged.AddListener(OnMicLevelChanged);
        }
    }

    private void OnDisable()
    {
        if (voiceExperience != null)
        {
            voiceExperience.VoiceEvents.OnResponse.RemoveListener(OnVoiceResponse);
            voiceExperience.VoiceEvents.OnMicAudioLevelChanged.RemoveListener(OnMicLevelChanged);
        }
    }

    private void Update()
    {
        Mover();
    }

    // --- MOVIMIENTO EN 2D CON NUEVO INPUT SYSTEM ---
    private void Mover()
    {
        float inputHorizontal = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) inputHorizontal = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) inputHorizontal = 1f;
        }

        Vector3 movimiento = new Vector3(inputHorizontal, 0f, 0f);
        transform.Translate(movimiento * velocidadMovimiento * Time.deltaTime);
    }

    // --- MONITOREO DE VOLUMEN ---
    private void OnMicLevelChanged(float level)
    {
        volumenActualMic = level;
    }

    // --- DETECCIÓN DE COMANDOS DE VOZ ---
    private void OnVoiceResponse(WitResponseNode response)
    {
        string transcripcion = response.GetTranscription().ToLower();
        Debug.Log($"<color=cyan>[TEXTO RECONOCIDO]:</color> {transcripcion}");

        if (string.IsNullOrEmpty(transcripcion)) return;

        // Comprobación de voz alta
        bool esVozAlta = volumenActualMic >= umbralVolumenAlto;
        int cantidadProyectiles = esVozAlta ? 2 : 1;

        if (esVozAlta)
        {
            Debug.Log($"¡Voz alta detectada! Vol: {volumenActualMic:F2}. Lanzando ataque doble.");
        }

        // Evalúa las palabras clave en la transcripción
        if (transcripcion.Contains("fuego"))
        {
            EjecutarAtaque(prefabFuego, cantidadProyectiles);
        }
        else if (transcripcion.Contains("hielo"))
        {
            EjecutarAtaque(prefabHielo, cantidadProyectiles);
        }
    }

    // --- INSTANCIAR PROYECTIL HACIA EL ENEMIGO ---
    private void EjecutarAtaque(GameObject prefabAtaque, int cantidad)
    {
        if (prefabAtaque == null)
        {
            Debug.LogError("¡ERROR!: No has asignado el Prefab en el Inspector del PlayerController.");
            return;
        }

        // Determina la posición de origen (si no hay puntoDeDisparo, usa la parte superior del player)
        Vector3 posicionOrigen = (puntoDeDisparo != null) ? puntoDeDisparo.position : transform.position + Vector3.up * 1.5f;
        posicionOrigen.z = 0f; // Asegurar plano 2D

        for (int i = 0; i < cantidad; i++)
        {
            // Pequeño desfase en Y si dispara proyectiles dobles por hablar alto
            Vector3 offset = new Vector3(0f, i * 0.4f, 0f);

            GameObject nuevoProyectil = Instantiate(prefabAtaque, posicionOrigen + offset, Quaternion.identity);
            Debug.Log($"<color=green>¡ÉXITO!</color> Se creó el proyectil {nuevoProyectil.name} en la escena.");
        }
    }

    // --- RECIBIR DAÑO AL SER ATACADO ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo") || collision.CompareTag("AtaqueEnemigo"))
        {
            RecibirDano(10);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemigo") || collision.gameObject.CompareTag("AtaqueEnemigo"))
        {
            RecibirDano(10);
        }
    }

    public void RecibirDano(int cantidad)
    {
        UIManager ui = Object.FindAnyObjectByType<UIManager>();
        if (ui != null)
        {
            ui.ModificarVida(-cantidad);
            Debug.Log($"¡Daño recibido! Restados {cantidad} de vida.");
        }
    }
}