using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingertonManager<GameManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_PointToClickPrefabs;
    public Unit ActiveUnit;
    private Vector2 m_InitialTouchPosition;
    public bool hasActiveunit => ActiveUnit != null;

    public void Update()
    {
        Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            m_InitialTouchPosition = inputPosition;
        }

        if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (Vector2.Distance(m_InitialTouchPosition, inputPosition) < 10f)
            {
                DetectClick(inputPosition);
            }
        }
    }

    public void DetectClick(Vector2 inputPosition)
    {
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

    public void SelectNewUnit( Unit unit)
    {
        if (hasActiveunit)
        {
            ActiveUnit.DeSelect();
        }
        ActiveUnit = unit;
        ActiveUnit.Select();
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
    }

    public void DisplayClickEffect(Vector2 worldPoint)
    {
        Instantiate(m_PointToClickPrefabs, (Vector3)worldPoint, Quaternion.identity);
    }
}
