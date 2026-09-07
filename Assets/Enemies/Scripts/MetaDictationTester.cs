using UnityEngine;
using UnityEngine.UI;
using Meta.WitAi.Dictation;
using UnityEngine.InputSystem;

public class MetaDictationTester : MonoBehaviour
{
    [Header("Servicio de Dictado")]
    [SerializeField] private DictationService dictationService;

    [Header("UI Opcional (Asigna componentes Text de UI)")]
    [SerializeField] private Text textHypothesis;   // Texto en tiempo real mientras hablas
    [SerializeField] private Text textFinalResult;  // Texto final al pausar

    private void OnEnable()
    {
        if (dictationService == null)
            dictationService = FindAnyObjectByType<DictationService>();

        if (dictationService != null)
        {
            dictationService.DictationEvents.OnPartialTranscription.AddListener(OnTranscriptionUpdated);
            dictationService.DictationEvents.OnFullTranscription.AddListener(OnDictationResult);
            dictationService.DictationEvents.OnError.AddListener(OnError);

            // EVENTOS DE DIAGNÓSTICO
            dictationService.DictationEvents.OnStartListening.AddListener(() => Debug.Log("<color=yellow>[Meta Voice] Micrófono ESCUCHANDO (habla ahora...)</color>"));
            dictationService.DictationEvents.OnStoppedListening.AddListener(() => Debug.Log("[Meta Voice] Micrófono DETENIDO."));
            dictationService.DictationEvents.OnResponse.AddListener((response) => Debug.Log($"[Meta Voice Raw Response]: {response}"));
        }
    }

    private void OnDisable()
    {
        if (dictationService != null)
        {
            dictationService.DictationEvents.OnPartialTranscription.RemoveListener(OnTranscriptionUpdated);
            dictationService.DictationEvents.OnFullTranscription.RemoveListener(OnDictationResult);
            dictationService.DictationEvents.OnError.RemoveListener(OnError);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log(">>> TECLA ESPACIO DETECTADA <<<");
            ToggleDictation();
        }
    }

    public void ToggleDictation()
    {
        // Si la variable está vacía, la busca automáticamente en la escena activa
        if (dictationService == null)
        {
            dictationService = FindAnyObjectByType<DictationService>();
        }

        if (dictationService == null)
        {
            Debug.LogError("¡No hay DictationService asignado ni tampoco se encontró un AppDictationExperience en la escena!");
            return;
        }

        // Resto de tu código normal...
        if (!dictationService.Active)
        {
            Debug.Log("[Meta Voice] Activando micrófono...");

            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("¡NO SE DETECTÓ NINGÚN MICRÓFONO EN EL SISTEMA!");
                return;
            }
            else
            {
                Debug.Log($"Micrófono usado: {Microphone.devices[0]}");
            }

            dictationService.Activate();
        }
        else
        {
            Debug.Log("[Meta Voice] Deteniendo micrófono...");
            dictationService.Deactivate();
        }
    }

    private void OnTranscriptionUpdated(string text)
    {
        Debug.Log($"[Meta Voice - En vivo]: {text}");
        if (textHypothesis != null) textHypothesis.text = "Escuchando: " + text;
    }

    private void OnDictationResult(string text)
    {
        Debug.Log($"<color=green>[Meta Voice - Final]: {text}</color>");
        if (textFinalResult != null) textFinalResult.text = "Resultado: " + text;
    }

    private void OnError(string error, string message)
    {
        Debug.LogError($"[Meta Voice Error]: {error} - {message}");
    }

    [ContextMenu("Probar Dictado Manual")]
    public void TestToggle()
    {
        ToggleDictation();
    }
}