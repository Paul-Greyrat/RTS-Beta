using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingertonManager<GameManager>
{
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
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (HasClickedOnUnit(hit, out Unit unit))
        {
            HandleClickOnUnit(unit);
        }
        else
        {
            HandleClickOnGround(worldPosition);            
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
        ActiveUnit.MoveTo(worldPoint);
    }

    public void HandleClickOnUnit(Unit unit)
    {
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
}
