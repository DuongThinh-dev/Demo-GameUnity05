using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int curentLevel;
    public TextMeshProUGUI textLevel;

    [Header("UI References")]
    public GameObject NodeUI;
    public GameObject CanvaButtons;
    public GameObject StopCanva;

    public TextMeshProUGUI sellPriceText;
    public TextMeshProUGUI upgradePriceText;

    [Header("Tower & Trap Prices")]
    public int[] towerPrices = new int[4];
    public int[] trapPrices = new int[3];

    [Header("Tower Selection")]
    public LayerMask towerLayer; // Layer can kiem tra raycast
    
    private bool justPlacedTower = false;
    private GridSystem[] gridSystem;
    private GameObject selectedTower;
    private GoldManager goldManager;
    private HomeManager homeManager;
    private bool isLocked = false; // Ngan viec chon lai tower/trap khi dang dat

    private void Start()
    {
        gridSystem = FindObjectsOfType<GridSystem>();
        goldManager = FindAnyObjectByType<GoldManager>();
        homeManager = FindAnyObjectByType<HomeManager>();
        NodeUI.SetActive(false); // An NodeUI luc dau
        StopCanva.SetActive(false);

        towerPrices = new int[] { 80, 90, 100, 120 };
        trapPrices = new int[] { 50, 80, 100 };

        textLevel.text = $"Level {curentLevel}";
    }

    public void isTower(int numberTower)
    {
        if (isLocked) return;

        int index = numberTower - 1; // chuyen sang index 0-based

        if (index >= 0 && index < towerPrices.Length && goldManager.TrySpendGold(towerPrices[index]))
        {
            if (gridSystem.Length > 0)
            {
                gridSystem[0].StartPlacingTower(numberTower); // van truyen 1-based cho GridSystem
                isLocked = true;
            }
        }
    }


    public void isTrap(int numberTrap)
    {
        if (isLocked) return;

        int index = numberTrap - 1; // chuyen tu 1-based ve 0-based

        if (index >= 0 && index < trapPrices.Length && goldManager.TrySpendGold(trapPrices[index]))
        {
            foreach (var item in gridSystem)
            {
                item.StartPlacingTrap(numberTrap); // van truyen 1-based cho GridSystem
                isLocked = true;
                break;
            }
        }
    }


    private void Update()
    {

        // Neu chuot dang nam tren UI thi bo qua raycast
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Neu dang dat hoac vua dat xong => khong raycast bat NodeUI
        if (IsPlacing() || justPlacedTower) return;

        if (Input.GetMouseButtonDown(0))
            HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, towerLayer))
        {
            Turret turret = hit.collider.GetComponent<Turret>();
            if (turret == null) turret = hit.collider.GetComponentInParent<Turret>();

            if (turret != null)
            {
                selectedTower = turret.gameObject;
                UpdateNodeUI();
                NodeUI.SetActive(true);
            }
        }
    }

    // Kiem tra xem GridSystem nao dang dat tower hoac trap khong
    private bool IsPlacing()
    {
        foreach (var gs in gridSystem)
        {
            if (gs.isPlacingTower || gs.isPlacingTrap)
                return true;
        }
        return false;
    }

    public void NotifyJustPlaced()
    {
        justPlacedTower = true;
        StartCoroutine(ResetJustPlaced());
    }

    private IEnumerator ResetJustPlaced()
    {
        yield return null; // Cho 1 frame
        justPlacedTower = false;
        isLocked = false;  // Cho phep chon lai sau khi dat xong
    }


    public void isUpGrade()
    {
        if (selectedTower != null)
        {
            Turret turret = selectedTower.GetComponent<Turret>();
            if (turret != null)
            {
                turret.Upgrade();
                UpdateNodeUI();
            }
        }
    }

    private void UpdateNodeUI()
    {
        if (selectedTower != null)
        {
            Turret turret = selectedTower.GetComponent<Turret>();
            if (turret != null)
            {
                sellPriceText.text = "Sell\n" + turret.sellPrice.ToString();

                // Neu da max cap, hien thi thong bao
                if (turret.IsMaxLevel())
                {
                    upgradePriceText.text = "Max Level";
                }
                else
                {
                    upgradePriceText.text = "Upgrade\n" + turret.upgradePrice.ToString();
                }

                NodeUI.transform.position = selectedTower.transform.position;

                if (CanvaButtons != null && Camera.main != null)
                {
                    CanvaButtons.transform.forward = Camera.main.transform.forward;
                }
            }
        }
    }

    public void isSell()
    {
        if (selectedTower != null)
        {
            Turret turret = selectedTower.GetComponent<Turret>();
            if (turret != null)
            {
                turret.Sell();
                selectedTower = null;
                NodeUI.SetActive(false);
            }
        }
    }

    public void CancelNodeUI()
    {
        NodeUI.SetActive(false);
    }

    public void CancelStopCanva()
    {
        StopCanva.SetActive(false);
    }

    public void isStopButton()
    {
        StopCanva.SetActive(true);
    }

    public void isGiveIn()
    {
        StopCanva.SetActive(false);
        homeManager.EndGame(false);
    }

    public void RestartLevel()
    {
        // Tai lai scene hien tai
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void WinLevel()
    {
        PlayerPrefs.SetInt("levedReached", curentLevel + 1);
    }
}
