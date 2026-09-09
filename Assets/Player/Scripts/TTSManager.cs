using UnityEngine;
using System.Speech.Synthesis; // Requiere .NET Framework / API de Windows

public class TextToSpeechManager : MonoBehaviour
{
    public static TextToSpeechManager Instance { get; private set; }

    private SpeechSynthesizer synthesizer;

    void Awake()
    {
        // Patron Singleton para acceder fácilmente desde cualquier script
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitSynthesizer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitSynthesizer()
    {
        try
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            // Intentar seleccionar una voz en español si está disponible en Windows
            foreach (var voice in synthesizer.GetInstalledVoices())
            {
                if (voice.VoiceInfo.Culture.TwoLetterISOLanguageName.Equals("es"))
                {
                    synthesizer.SelectVoice(voice.VoiceInfo.Name);
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al inicializar TTS: {e.Message}");
        }
    }

    /// <summary>
    /// Dice en voz alta el texto proporcionado de forma asíncrona para no congelar el juego.
    /// </summary>
    public void Speak(string text)
    {
        if (synthesizer != null)
        {
            // Cancela cualquier locución anterior y dice la nueva inmediatamente
            synthesizer.SpeakAsyncCancelAll();
            synthesizer.SpeakAsync(text);
        }
    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
}