using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Linq;

public class VoiceInputTester : MonoBehaviour
{
    [Header("UI Output (Opcional)")]
    [SerializeField] private Text uiStatusText;
    [SerializeField] private Text uiTranscriptionText;

    // Componentes nativos de Unity / Windows Speech
    private KeywordRecognizer keywordRecognizer;
    private DictationRecognizer dictationRecognizer;

    // Palabras clave para probar la detección de comandos
    private string[] keywords = new string[] { "hola", "red", "blue", "cerrar" };

    void Start()
    {
        LogToConsoleAndUI("Iniciando pruebas de voz...");

        // 1. Probar KeywordRecognizer (Comandos)
        InitKeywordRecognizer();

        // 2. Probar DictationRecognizer (Transcripción completa)
        InitDictationRecognizer();
    }

    private void InitKeywordRecognizer()
    {
        keywordRecognizer = new KeywordRecognizer(keywords);
        keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;
        keywordRecognizer.Start();
        Debug.Log("[VoiceTest] KeywordRecognizer activado. Di: 'hola', 'red' o 'blue'.");
    }

    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
    {
        string msg = $"[Comando Detectado]: {args.text} (Confianza: {args.confidence})";
        LogToConsoleAndUI(msg);

        // Cambiar color de fondo como prueba visual si hay cámara principal
        if (args.text == "red") Camera.main.backgroundColor = Color.red;
        if (args.text == "blue") Camera.main.backgroundColor = Color.blue;
    }

    private void InitDictationRecognizer()
    {
        dictationRecognizer = new DictationRecognizer();

        // Evento mientras hablas (Hipótesis)
        dictationRecognizer.DictationHypothesis += (text) =>
        {
            if (uiTranscriptionText != null)
                uiTranscriptionText.text = "Escuchando: " + text;
        };

        // Evento cuando pausas y se completa la frase
        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            string resultMsg = $"[Transcripción Final]: {text}";
            LogToConsoleAndUI(resultMsg);
            if (uiTranscriptionText != null)
                uiTranscriptionText.text = text;
        };

        // Manejo de errores/cierre
        dictationRecognizer.DictationComplete += (cause) =>
        {
            Debug.LogWarning($"[VoiceTest] Dictado completado/detenido. Causa: {cause}");
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError($"[VoiceTest] Error de dictado: {error} (HResult: {hresult})");
        };

        dictationRecognizer.Start();
        Debug.Log("[VoiceTest] DictationRecognizer iniciado. Comienza a hablar...");
    }

    private void LogToConsoleAndUI(string message)
    {
        Debug.Log(message);
        if (uiStatusText != null)
        {
            uiStatusText.text = message;
        }
    }

    private void OnDestroy()
    {
        // Liberar recursos obligatoriamente
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.OnPhraseRecognized -= OnKeywordsRecognized;
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }

        if (dictationRecognizer != null)
        {
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }
            dictationRecognizer.Dispose();
        }
    }
}