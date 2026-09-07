using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.InputSystem; // Add this namespace

public class DictationTester : MonoBehaviour
{
    [Header("UI Opcional (Asigna campos UI.Text si usas Canvas)")]
    [SerializeField] private Text textHypothesis; // Para ver la transcripción en vivo
    [SerializeField] private Text textFinalResult; // Para ver el resultado al pausar

    private DictationRecognizer dictationRecognizer;

    void Start()
    {
        // 1. Crear el reconocedor de dictado
        dictationRecognizer = new DictationRecognizer();

        // 2. Suscribir eventos

        // OCURRE MIENTRAS HABLAS: Muestra la hipótesis en tiempo real
        dictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.Log($"[En vivo]: {text}");
            if (textHypothesis != null) textHypothesis.text = "Escuchando: " + text;
        };

        // OCURRE AL HACER UNA PAUSA: Entrega la frase confirmada
        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.Log($"<color=green>[Resultado Final]: {text} (Confianza: {confidence})</color>");
            if (textFinalResult != null) textFinalResult.text = "Resultado: " + text;
        };

        // MANEJO DE ERRORES O EVENTOS DEL SISTEMA
        dictationRecognizer.DictationComplete += (cause) =>
        {
            Debug.LogWarning($"Dictado detenido. Razón: {cause}");
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError($"Error en la transcripción: {error} (HResult: {hresult})");
        };

        // 3. Iniciar el micrófono y reconocimiento
        dictationRecognizer.Start();
        Debug.Log(">>> Micrófono listo. Comienza a hablar para probar la transcripción... <<<");
    }

    void Update()
    {
        // Check if the recognizer stopped and restart it using the new Input System
        if (dictationRecognizer != null && dictationRecognizer.Status != SpeechSystemStatus.Running)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log("Reiniciando reconocedor de transcripción...");
                dictationRecognizer.Start();
            }
        }
    }

    private void OnDestroy()
    {
        // Liberar el servicio de voz al salir
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