using UnityEngine;
using UnityEngine.InputSystem;
using Meta.WitAi;
using Meta.Voice;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias de Meta Voice")]
    [SerializeField] private VoiceService voiceExperience;

    [Header("Prefabs de Ataque")]
    [SerializeField] private GameObject prefabFuego;
    [SerializeField] private GameObject prefabHielo;
    [SerializeField] private Transform puntoDeDisparo;

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Sensibilidad de Voz Alta")]
    [SerializeField] private float umbralVolumenAlto = 0.25f;

    private float volumenMaximoRegistrado = 0f;
    private UIManager uiManager;

    private void Start()
    {
        // Cacheamos la referencia del UI al iniciar
        uiManager = Object.FindAnyObjectByType<UIManager>();
    }

    private void OnEnable()
    {
        if (voiceExperience != null)
        {
            voiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscriptionReceived);
            voiceExperience.VoiceEvents.OnMicAudioLevelChanged.AddListener(OnMicLevelChanged);
            voiceExperience.VoiceEvents.OnStartListening.AddListener(OnStartListening);
        }
    }

    private void OnDisable()
    {
        if (voiceExperience != null)
        {
            voiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscriptionReceived);
            voiceExperience.VoiceEvents.OnMicAudioLevelChanged.RemoveListener(OnMicLevelChanged);
            voiceExperience.VoiceEvents.OnStartListening.RemoveListener(OnStartListening);
        }
    }

    private void Update()
    {
        Mover();
    }

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

    // --- MÉTODOS DE VOICE SDK (Cambiados a Public) ---

    public void OnStartListening()
    {
        volumenMaximoRegistrado = 0f;
    }

    public void OnMicLevelChanged(float level)
    {
        if (level > volumenMaximoRegistrado)
        {
            volumenMaximoRegistrado = level;
        }
    }

    public void OnFullTranscriptionReceived(string transcripcion)
    {
        if (string.IsNullOrEmpty(transcripcion)) return;

        // 1. Convertir a minúsculas y quitar espacios en los extremos
        string textoLimpio = transcripcion.ToLower().Trim();

        // 2. Limpiar signos de puntuación comunes que Meta Voice suele añadir al final
        textoLimpio = textoLimpio.Replace(".", "")
                                 .Replace(",", "")
                                 .Replace("!", "")
                                 .Replace("?", "")
                                 .Replace("á", "a")
                                 .Replace("é", "e")
                                 .Replace("í", "i")
                                 .Replace("ó", "o")
                                 .Replace("ú", "u");

        // Imprimimos el texto exacto ya procesado entre corchetes para depurar
        Debug.Log($"<color=cyan>[TEXTO RECONOCIDO PROCESADO]:</color> \"{textoLimpio}\"");

        // Evalúa si el volumen fue alto
        bool esVozAlta = volumenMaximoRegistrado >= umbralVolumenAlto;
        int cantidadProyectiles = esVozAlta ? 2 : 1;

        if (esVozAlta)
        {
            Debug.Log($"¡Voz alta detectada! Vol Máx: {volumenMaximoRegistrado:F2}. Lanzando ataque doble.");
        }

        // 3. Comprobación usando Contains
        if (textoLimpio.Contains("fuego"))
        {
            Debug.Log("<color=yellow>-> Entró al IF de FUEGO</color>");
            EjecutarAtaque(prefabFuego, cantidadProyectiles);
        }
        else if (textoLimpio.Contains("hielo"))
        {
            Debug.Log("<color=yellow>-> Entró al IF de HIELO</color>");
            EjecutarAtaque(prefabHielo, cantidadProyectiles);
        }
        else
        {
            Debug.LogWarning($"<color=orange>[AVISO]:</color> El texto \"{textoLimpio}\" no contiene ni 'fuego' ni 'hielo'.");
        }
    }

    private void EjecutarAtaque(GameObject prefabAtaque, int cantidad)
    {
        if (prefabAtaque == null)
        {
            Debug.LogError("¡ERROR!: No has asignado el Prefab en el Inspector del PlayerController.");
            return;
        }

        Vector3 posicionOrigen = (puntoDeDisparo != null) ? puntoDeDisparo.position : transform.position + Vector3.up * 1.5f;
        posicionOrigen.z = 0f;

        for (int i = 0; i < cantidad; i++)
        {
            Vector3 offset = new Vector3(0f, i * 0.4f, 0f);
            GameObject nuevoProyectil = Instantiate(prefabAtaque, posicionOrigen + offset, Quaternion.identity);
            Debug.Log($"<color=green>¡ÉXITO!</color> Se creó el proyectil {nuevoProyectil.name} en la escena.");
        }
    }

    // --- COLISIONES ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo") || collision.CompareTag("AtaqueEnemigo"))
        {
            RecibirDano(10);
        }
    }

   

    public void RecibirDano(int cantidad)
    {
        if (uiManager != null)
        {
            uiManager.ModificarVida(-cantidad);
            Debug.Log($"¡Daño recibido! Restados {cantidad} de vida.");
        }
        else
        {
            // Reintenta buscar si no se asignó en Start
            uiManager = Object.FindAnyObjectByType<UIManager>();
            if (uiManager != null) uiManager.ModificarVida(-cantidad);
        }
    }
}