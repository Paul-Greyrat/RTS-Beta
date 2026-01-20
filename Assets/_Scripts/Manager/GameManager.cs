



using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ClickType
{
    Move, Attack, Build
}

public class GameManager : SingertonManager<GameManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_PointToMovePrefabs;
    [SerializeField] private PointToClick m_PointToBuildPrefabs;
    [SerializeField] private ActionBar m_ActionBar;
    [SerializeField] private ConfirmationBar m_ConfirmationBar;
    [SerializeField] private TextPobupController m_TextPopupController;

    [Header("Camera Settings")]
    [SerializeField] private float m_PanSpeed = 100;
    [SerializeField] private float m_MobilePanSpeed = 10;

    [Header("VFX")]
    [SerializeField] private ParticleSystem m_ConstructionEffectPrefab;

    public Unit ActiveUnit;
    private List<Unit> m_PlayerUnits = new();
    private List<Unit> m_Enemies = new();
    private PlacementProcess m_PlacementProcess;
    private CameraController m_CameraController;

    private int m_gold = 50;
    private int m_wood = 50;
    public int Gold => m_gold;
    public int Wood => m_wood;



    public bool hasActiveunit => ActiveUnit != null;

    public void Start()
    {
        m_CameraController = new CameraController(m_PanSpeed, m_MobilePanSpeed);
        ClearActionBarUI();
    }

    public void Update()
    {
        m_CameraController.Update();

        if (m_PlacementProcess != null)
        {
            m_PlacementProcess.Update();
        }
        else if (GreyUtils.TryGetShotClickposition(out Vector2 inputPosition))
        {
            DetectClick(inputPosition);
        }
    }

    public void RegisterUnit(Unit unit)
    {
        if (unit.IsPlayer)
        {
            m_PlayerUnits.Add(unit);
        }
        else
        {
            m_Enemies.Add(unit);
        }
    }

    public void UnregisterUnit(Unit unit)
    {
        if (unit.IsPlayer)
        {
            if (m_PlacementProcess != null)
            {
                CancelBuildPlacement();
            }

            if (ActiveUnit == unit)
            {
                ClearActionBarUI();
                ActiveUnit.Deselect();
                ActiveUnit = null;
            }

            unit.StopMovement();

            m_PlayerUnits.Remove(unit);
        }
        else
        {
            m_Enemies.Remove(unit);
        }
    }

    public void ShowTextPopup(string text, Color color, Vector3 position)
    {
        m_TextPopupController.Spawn(text, color, position);

    }

        public Unit FindClosestUnit(Vector3 originPosition, float maxDistance, bool isPlayer)
    {
        List<Unit> units = isPlayer ? m_PlayerUnits : m_Enemies;
        float sqrMaxDistance = maxDistance * maxDistance;
        Unit closestUnit = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (Unit unit in units)
        {
            if (unit.CurrentState == UnitState.Dead) continue;

            float sqrDistance = (unit.transform.position - originPosition).sqrMagnitude;
            if (sqrDistance < sqrMaxDistance && sqrDistance < closestDistanceSqr)
            {
                closestUnit = unit;
                closestDistanceSqr = sqrDistance;
            }
        }

        return closestUnit;
    }

    public void StartBuidProcess(BuildActionSO buildAction)
    {
        if (m_PlacementProcess != null) return;

        var tilemapManager = TilemapManager.Get();

        m_PlacementProcess = new PlacementProcess(
            buildAction,
            tilemapManager
            );
        m_PlacementProcess.ShowPlacementOutLine();
        m_ConfirmationBar.Show(buildAction.GoldCost, buildAction.WoodCost);
        m_ConfirmationBar.SetupHooks(ConfirmBuildPlacement, CancelBuildPlacement);
        m_CameraController.m_lockCamera = true;
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
            if (unit.IsPlayer)
            {
                HandleClickOnPlayerUnit(unit);
            }
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

            DisplayClickEffect(worldPoint, ClickType.Move);
            ActiveUnit.MoveTo(worldPoint);

        }
    }

    public void HandleClickOnPlayerUnit(Unit unit)
    {
        if (hasActiveunit)
        {
            if (HasClickedOnUnitActive(unit))
            {
                CancelActiveUnit();
                return;
            }
            else if (WorkerClickedUnFinishedBuiding(unit))
            {
                DisplayClickEffect(unit.transform.position, ClickType.Build);
                ((WorkerUnit)ActiveUnit).SendToBuild(unit as StructureUnit);
                return;
            }
        }
        SelectNewUnit(unit);
    }

    bool WorkerClickedUnFinishedBuiding(Unit Clickedunit)
    {
        return
            ActiveUnit is WorkerUnit &&
            Clickedunit is StructureUnit structure &&
            structure.IsUnderConstuction;
    }

    public void SelectNewUnit(Unit unit)
    {
        if (unit.CurrentState == UnitState.Dead) return;
        
        if (hasActiveunit)
        {
            ActiveUnit.Deselect();
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
        ActiveUnit.Deselect();
        ActiveUnit = null;
        ClearActionBarUI();
    }

    public void DisplayClickEffect(Vector2 worldPoint, ClickType clickType)
    {
        if (clickType == ClickType.Move)
        {
            Instantiate(m_PointToMovePrefabs, (Vector3)worldPoint, Quaternion.identity);
        }
        else if (clickType == ClickType.Build)
        {
            Instantiate(m_PointToBuildPrefabs, (Vector3)worldPoint, Quaternion.identity);
        }
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
            DisplayClickEffect(buildposition, ClickType.Build);
            m_ConfirmationBar.Hide();
            new BuildingProcess(
                m_PlacementProcess.BuildAction,
                buildposition,
                (WorkerUnit)ActiveUnit,
                m_ConstructionEffectPrefab
            );

            m_PlacementProcess = null;
            m_CameraController.m_lockCamera = false;
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
        m_CameraController.m_lockCamera = false;
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
