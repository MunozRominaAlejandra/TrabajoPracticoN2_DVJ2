using UnityEngine;
using TMPro; // Necesario para trabajar con TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("Referencias de la UI (TextMeshPro)")]
    [SerializeField] private TMP_Text textVida;
    [SerializeField] private TMP_Text textTiempo;
    [SerializeField] private TMP_Text textPuntos;

    [Header("Configuración Inicial")]
    [SerializeField] private int vidaActual = 100;
    [SerializeField] private int puntosActuales = 0;
    [SerializeField] private float tiempoRestante = 60f; // Tiempo inicial en segundos

    private bool tiempoActivo = true;

    private void Start()
    {
        ActualizarUI();
    }

    private void Update()
    {
        ManejarTiempo();
    }

    private void ManejarTiempo()
    {
        if (!tiempoActivo) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoTiempo();
        }
        else
        {
            tiempoRestante = 0;
            tiempoActivo = false;
            ActualizarTextoTiempo();
            TiempoAgotado();
        }
    }

    private void ActualizarUI()
    {
        ActualizarTextoVida();
        ActualizarTextoPuntos();
        ActualizarTextoTiempo();
    }

    private void ActualizarTextoVida()
    {
        if (textVida != null)
            textVida.text = "Vida: " + vidaActual;
    }

    private void ActualizarTextoPuntos()
    {
        if (textPuntos != null)
            textPuntos.text = "Puntos: " + puntosActuales;
    }

    private void ActualizarTextoTiempo()
    {
        if (textTiempo != null)
        {
            // Formatea el tiempo en minutos y segundos (00:00)
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    private void TiempoAgotado()
    {
        Debug.Log("¡El tiempo se ha terminado!");
        // Agrega aquí la lógica de Game Over si la necesitas
    }

    // --- MÉTODOS PÚBLICOS PARA LLAMAR DESDE OTROS SCRIPTS ---

    public void ModificarVida(int cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Max(0, vidaActual); // Evita vidas negativas
        ActualizarTextoVida();
    }

    public void AgregarPuntos(int puntos)
    {
        puntosActuales += puntos;
        ActualizarTextoPuntos();
    }

    public void PausarTiempo(bool pausado)
    {
        tiempoActivo = !pausado;
    }
}