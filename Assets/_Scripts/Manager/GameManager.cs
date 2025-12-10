



using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : SingertonManager<GameManager>
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_OverlayTilemap;
    [SerializeField] private Tilemap[] m_UnreachableTilemap;


    [Header("UI")]
    [SerializeField] private PointToClick m_PointToClickPrefabs;
    [SerializeField] private ActionBar m_ActionBar;
    [SerializeField] private ConfirmationBar m_ConfirmationBar;
    public Unit ActiveUnit;
    private PlacementProcess m_PlacementProcess;

    private int m_gold = 50;
    private int m_wood = 50;
    public int Gold => m_gold;
    public int Wood => m_wood;



    public bool hasActiveunit => ActiveUnit != null;

    public void Start()
    {
        ClearActionBarUI();
    }

    public void Update()
    {

        if (m_PlacementProcess != null)
        {
            m_PlacementProcess.Update();
        }
        else if (GreyUtils.TryGetShotClickposition(out Vector2 inputPosition))
        {
            DetectClick(inputPosition);
        }
    }

    public void StartBuidProcess(BuildActionSO buildAction)
    {
        if (m_PlacementProcess != null) return;

        m_PlacementProcess = new PlacementProcess(
            buildAction,
            m_WalkableTilemap,
            m_OverlayTilemap,
            m_UnreachableTilemap
            );
        m_PlacementProcess.ShowPlacementOutLine();
        m_ConfirmationBar.Show( buildAction.GoldCost, buildAction.WoodCost);
        m_ConfirmationBar.SetupHooks(ConfirmBuildPlacement, CancelBuildPlacement);
    }

    public void DetectClick(Vector2 inputPosition)
    {

        if (GreyUtils.IsPointerOverUiElement())
        {
            return;
        }
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (HasClickedOnUnit(hit, out Unit unit))
        {
            HandleClickOnUnit(unit);
        }
        else
        {
            HandleClickOnGround(worldPoint);
        }
    }

    public bool HasClickedOnUnit(RaycastHit2D hit, out Unit unit)
    {
        if (hit.collider != null && hit.collider.TryGetComponent<Unit>(out var clickedUnit))
        {
            unit = clickedUnit;
            return true;
        }
        unit = null;
        return false;
    }

    public void HandleClickOnGround(Vector2 worldPoint)
    {
        if (hasActiveunit && IsHumannoid(ActiveUnit))
        {

            DisplayClickEffect(worldPoint);
            ActiveUnit.MoveTo(worldPoint);

        }
    }

    public void HandleClickOnUnit(Unit unit)
    {
        if (hasActiveunit)
        {
            if (HasClickedOnUnitActive(unit))
            {
                CancelActiveUnit();
                return;
            }
        }
        SelectNewUnit(unit);
    }

    public void SelectNewUnit(Unit unit)
    {
        if (hasActiveunit)
        {
            ActiveUnit.DeSelect();
        }
        ActiveUnit = unit;
        ActiveUnit.Select();
        ShowUnitAction(unit);
    }

    public bool HasClickedOnUnitActive(Unit unit)
    {
        return hasActiveunit && ActiveUnit == unit;
    }

    public bool IsHumannoid(Unit unit)
    {
        return unit is HumanoidUnit;
    }

    public void CancelActiveUnit()
    {
        ActiveUnit.DeSelect();
        ActiveUnit = null;
        ClearActionBarUI();
    }

    public void DisplayClickEffect(Vector2 worldPoint)
    {
        Instantiate(m_PointToClickPrefabs, (Vector3)worldPoint, Quaternion.identity);
    }

    public void ShowUnitAction(Unit unit)
    {
        ClearActionBarUI();

        if (unit.Actions.Length == 0) return;

        m_ActionBar.Show();

        foreach (var action in unit.Actions)
        {
            m_ActionBar.RegisterAction(
                action.Icon,
                () => action.Execute(this)
            );
        }
    }

    public void ClearActionBarUI()
    {
        m_ActionBar.ClearActions();
        m_ActionBar.Hiden();
    }

    public void ConfirmBuildPlacement()
    {
        if (!TryDeductResources(m_PlacementProcess.goldCost, m_PlacementProcess.woodCost))
        {
            Debug.Log("Not enough resources to build");
            return;
        }

        if (m_PlacementProcess.TryFinalizePlacement(out Vector3 buildposition))
        {
            m_ConfirmationBar.Hide();
            new Buildingprocess(
                m_PlacementProcess.BuildAction,
                buildposition,
                (WorkerUnit)ActiveUnit
            );

            m_PlacementProcess = null;
        }
        else
        {
            RevertResources(m_PlacementProcess.goldCost, m_PlacementProcess.woodCost);
        }
    }

    public void RevertResources(int goldAmount, int woodAmount)
    {
        m_gold += goldAmount;
        m_wood += woodAmount;
    }
    public void CancelBuildPlacement()
    {
        m_ConfirmationBar.Hide();
        m_PlacementProcess.Cleanup();
        m_PlacementProcess = null;
    }

    bool TryDeductResources(int goldCost, int woodCost)
    {
        if (m_gold >= goldCost && m_wood >= woodCost)
        {
            m_gold -= goldCost;
            m_wood -= woodCost;
            return true;
        }

        return false;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 40, 200, 20), "Gold: " + m_gold.ToString(), new GUIStyle { fontSize = 30 });
        GUI.Label(new Rect(20, 80, 200, 20), "Wood: " + m_wood.ToString(), new GUIStyle { fontSize = 30 });

        if (ActiveUnit != null)
        {
            GUI.Label(new Rect(20, 120, 200, 20), "State: " + ActiveUnit.CurrentState.ToString(), new GUIStyle { fontSize = 30 });
            GUI.Label(new Rect(20, 160, 200, 20), "task: " + ActiveUnit.CurrentTask.ToString(), new GUIStyle { fontSize = 30 });
        }
    }

}
