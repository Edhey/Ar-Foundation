using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText; 

    private int collectedPackages = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddPackage()
    {
        collectedPackages++;
        UpdateUI();
        
        Debug.Log("Paquete recogido. Total: " + collectedPackages);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Paquetes: {collectedPackages}";
        }
    }
}
