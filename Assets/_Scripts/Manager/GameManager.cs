using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : SingertonManager<GameManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_PointToClickPrefabs;
    [SerializeField] private ActionBar m_ActionBar;
    public Unit ActiveUnit;
    private PlacementProcess m_PlacementProcess;


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
        m_PlacementProcess = new PlacementProcess(buildAction);
        m_PlacementProcess.ShowPlacementOutLine();
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


}
