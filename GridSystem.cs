using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Trap Prefabs")]
    public GameObject Trap1;
    public GameObject Trap2;
    public GameObject Trap3;

    [Header("Tower Prefabs")]
    public GameObject Tower1;
    public GameObject Tower2;
    public GameObject Tower3;
    public GameObject Tower4;

    [Header("Settings")]
    public float gridSize = 1f;
    public int numberTower;// so Tower 
    public int numberTrap;// so Trap
    public bool isPlacingTower;// dang dat thap khong
    public bool isPlacingTrap;// dang dat bay khong

    [SerializeField] private LayerMask gridTowerLayer;
    [SerializeField] private LayerMask gridTrapLayer;
    private GameObject ghostObject;// object ao
    private GameObject objectToPlace;// object se duoc dat
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private GameObject[] towerPrefabs;
    private GameObject[] trapPrefabs;

    public static GridSystem activeGridSystem = null; // He thong grid đang hoat dong

    private void Start()
    {
        towerPrefabs = new GameObject[] { Tower1, Tower2, Tower3, Tower4 };
        trapPrefabs = new GameObject[] { Trap1, Trap2, Trap3 };
        isPlacingTower = false;
        isPlacingTrap = false;
    }

    private void Update()
    {
        if (!isPlacingTower && !isPlacingTrap) return;

        if (ghostObject == null)
        {
            CreateGhostObject();
        }

        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }


    }

    // goi tu UI de bat dau dat
    public void StartPlacingTower(int towerIndex)
    {
        if (towerIndex < 1 || towerIndex > towerPrefabs.Length) return;

        // Tat ghost object o GridSystem cu (neu co)
        if (activeGridSystem != null && activeGridSystem != this)
        {
            activeGridSystem.CancelPlacement();
        }

        numberTower = towerIndex;
        objectToPlace = towerPrefabs[towerIndex - 1];
        isPlacingTower = true;
        activeGridSystem = this;

        if (ghostObject != null)
            Destroy(ghostObject);

        CreateGhostObject();
    }

    public void StartPlacingTrap(int trapIndex)
    {
        if (trapIndex < 1 || trapIndex > trapPrefabs.Length) return;

        // Huỷ ghost cũ nếu có
        if (activeGridSystem != null && activeGridSystem != this)
        {
            activeGridSystem.CancelPlacement();
        }

        numberTower = trapIndex; // dùng lại biến cho chỉ số
        objectToPlace = trapPrefabs[trapIndex - 1];
        isPlacingTrap = true;
        isPlacingTower = false;
        activeGridSystem = this;

        if (ghostObject != null)
            Destroy(ghostObject);

        CreateGhostObject();
    }


    // dung dat va xoa ghost
    public void CancelPlacement()
    {
        isPlacingTower = false;
        isPlacingTrap = false;

        if (ghostObject != null)
        {
            Destroy(ghostObject);
            ghostObject = null;
        }
    }


    // Tao ghost neu chưa co.
    void CreateGhostObject()
    {
        if (objectToPlace == null) return;

        ghostObject = Instantiate(objectToPlace);
        ghostObject.name = "Ghost_" + objectToPlace.name; // de phan biet

        // Tat tat ca collider
        foreach (Collider col in ghostObject.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // Tat tat ca MonoBehaviour scripts (vi du như Turret)
        foreach (MonoBehaviour script in ghostObject.GetComponentsInChildren<MonoBehaviour>())
        {
            script.enabled = false;
        }

        // Lam mo mau vat lieu
        foreach (Renderer renderer in ghostObject.GetComponentsInChildren<Renderer>())
        {
            var mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }


    // Cap nhat vi tri ghost theo chuot.
    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Chon layer dung theo loai dang dat
        LayerMask currentLayerMask = isPlacingTower ? gridTowerLayer :
                                 isPlacingTrap ? gridTrapLayer : 0;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, currentLayerMask))
        {
            Vector3 point = hit.point;
            Vector3 snappedPosition = new Vector3(
                Mathf.Round(point.x / gridSize) * gridSize,
                Mathf.Round(point.y / gridSize) * gridSize,
                Mathf.Round(point.z / gridSize) * gridSize
            );

            ghostObject.transform.position = snappedPosition;
            ghostObject.SetActive(true);

            SetGhostColor(occupiedPositions.Contains(snappedPosition) ? Color.red : new Color(1f, 1f, 1f, 0.5f));
        }
        else
        {
            if (ghostObject != null)
                ghostObject.SetActive(false);
        }
    }

    void SetGhostColor(Color color)
    {
        foreach (Renderer renderer in ghostObject.GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = color;
        }
    }

    // Dat Tower 
    void PlaceObject()
    {
        if (ghostObject == null || !ghostObject.activeSelf) return;

        Vector3 placementPosition = ghostObject.transform.position;

        // Khong cho dat len vi tri da co tower.
        if (occupiedPositions.Contains(placementPosition)) return;

        // Tao tower that va danh dau vi tri da su dung.
        GameObject placedObj = Instantiate(objectToPlace, placementPosition, Quaternion.identity);

        // Neu la trap thi truyen thong tin ve GridSystem
        Trap trap = placedObj.GetComponent<Trap>();
        if (trap != null)
        {
            trap.SetGridReference(this, placementPosition);
        }

        occupiedPositions.Add(placementPosition);

        Destroy(ghostObject);
        isPlacingTower = false;
        isPlacingTrap = false;
        ghostObject = null;

        // Bao cho GameManager rang vua dat tower xong
        GameObject.FindObjectOfType<GameManager>().NotifyJustPlaced();
    }

    public void ClearOccupiedCell(Vector3 position)
    {
        if (occupiedPositions.Contains(position))
        {
            occupiedPositions.Remove(position);
        }
    }


}
