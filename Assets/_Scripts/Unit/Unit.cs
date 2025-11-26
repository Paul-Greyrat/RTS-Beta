using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{

    [SerializeField] private ActionSO[] m_Actions;

    public bool isMoving;
    public bool IsTargeted;
    protected Animator m_Animator;
    protected AiPawn m_Aipawn;
    protected SpriteRenderer m_SpriteRenderer;
    protected Material m_OriginalMaterial;
    private Material m_HighlightMaterial;

    public ActionSO[] Actions => m_Actions;

    protected virtual void Awake()
    {

        if (TryGetComponent<Animator>(out var animator))
        {
            m_Animator = animator;
        }

        if (TryGetComponent<AiPawn>(out var aiPawn))
        {
            m_Aipawn = aiPawn;
        }
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        m_OriginalMaterial = m_SpriteRenderer.material;
        m_HighlightMaterial = Resources.Load<Material>("Materials/OutLine");
        

    }

    public void MoveTo(Vector3 destination)
    {
        m_SpriteRenderer.flipX = destination.x < transform.position.x;
        m_Aipawn.SetDestination(destination);
    }

    public void Select()
    {
        HightLight();
        IsTargeted = true;
    }

    public void DeSelect()
    {
        UnHightLight();
        IsTargeted = false;
    }

    void HightLight()
    {
        m_SpriteRenderer.material = m_HighlightMaterial;
    }

        void UnHightLight()
    {
        m_SpriteRenderer.material = m_OriginalMaterial;
    }
}
