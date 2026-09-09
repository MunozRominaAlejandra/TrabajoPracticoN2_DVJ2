using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem; // Nuevo Input System
using UnityEngine.Windows.Speech;

public class VoiceProjectileLauncher : MonoBehaviour
{
    [Header("Configuración de Proyectiles")]
    public GameObject firePrefab;
    public GameObject icePrefab;
    public Transform spawnPoint;

    [Header("Configuración de Volumen")]
    [Tooltip("Umbral RMS para considerar que la voz fue fuerte (0.0 a 1.0)")]
    public float loudVolumeThreshold = 0.15f;

    [Header("Monitoreo de Estado")]
    public bool isListening = false;
    public float currentVolume;

    // Reconocimiento de voz
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Lectura de micrófono
    private AudioClip micClip;
    private string micDevice;
    private int sampleWindow = 128;

    void Start()
    {
        InitMicrophone();
        InitVoiceCommands();
    }

    void Update()
    {
        // Control Push-to-Talk con la barra espaciadora
        HandlePushToTalkInput();

        // Solo medimos el volumen si se está presionando la tecla
        if (isListening)
        {
            currentVolume = GetMicVolume();
        }
        else
        {
            currentVolume = 0f;
        }
    }

    #region Lógica Push-To-Talk (Nuevo Input System)

    private void HandlePushToTalkInput()
    {
        if (Keyboard.current == null) return;

        // Al PRESIONAR la barra espaciadora
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartListening();
        }

        // Al SOLTAR la barra espaciadora
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            StopListening();
        }
    }

    private void StartListening()
    {
        if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Start();
            isListening = true;
            Debug.Log("Escuchando... Di 'fuego' o 'hielo'");
        }
    }

    private void StopListening()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            isListening = false;
            Debug.Log("Escucha pausada.");
        }
    }

    #endregion

    #region Inicialización

    private void InitMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            micClip = Microphone.Start(micDevice, true, 10, 44100);
        }
        else
        {
            Debug.LogError("No se detectó ningún micrófono conectado.");
        }
    }

    private void InitVoiceCommands()
    {
        keywords.Add("fuego", () => LaunchElement("fuego"));
        keywords.Add("hielo", () => LaunchElement("hielo"));

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Medium);
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
    }

    #endregion

    #region Reconocimiento y Lanzamiento

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log($"Palabra detectada: {args.text} | Confianza: {args.confidence}");

        if (keywords.TryGetValue(args.text, out System.Action action))
        {
            action.Invoke();
        }
    }

    private void LaunchElement(string elementType)
    {
        int projectileCount = (currentVolume >= loudVolumeThreshold) ? 2 : 1;
        GameObject prefabToSpawn = (elementType == "fuego") ? firePrefab : icePrefab;

        if (prefabToSpawn == null || spawnPoint == null)
        {
            Debug.LogWarning("Faltan referencias de Prefabs o SpawnPoint en el Inspector.");
            return;
        }

        for (int i = 0; i < projectileCount; i++)
        {
            // Pequeña separación lateral si son 2 proyectiles
            Vector3 offset = (i == 1) ? spawnPoint.right * 0.5f : Vector3.zero;

            // Instanciar el proyectil (su propio script ProjectileMovement se encargará de dirigirse al enemigo)
            Instantiate(prefabToSpawn, spawnPoint.position + offset, spawnPoint.rotation);
        }

        Debug.Log($"Lanzado(s) {projectileCount} proyectil(es) de {elementType}. Volumen: {currentVolume:F3}");
    }

    #endregion

    #region Cálculo de Amplitud (Volumen)

    private float GetMicVolume()
    {
        if (micClip == null) return 0f;

        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(micDevice) - sampleWindow + 1;

        if (micPosition < 0) return 0f;

        micClip.GetData(waveData, micPosition);

        float sumOfSquares = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            sumOfSquares += waveData[i] * waveData[i];
        }

        return Mathf.Sqrt(sumOfSquares / sampleWindow);
    }

    #endregion

    void OnDestroy()
    {
        if (keywordRecognizer != null)
        {
            if (keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
            keywordRecognizer.OnPhraseRecognized -= OnPhraseRecognized;
            keywordRecognizer.Dispose();
        }

        if (Microphone.IsRecording(micDevice))
        {
            Microphone.End(micDevice);
        }
    }
}