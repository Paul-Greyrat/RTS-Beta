using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public enum UnitState
{
    Idle, Moving, Attacking, Chopping, minig, Building
}

public enum UnitTask
{
    None, Build, Attack, Chop, Mine
}

public abstract class Unit : MonoBehaviour
{

    [SerializeField] private ActionSO[] m_Actions;
    [SerializeField] protected float m_OjectDetectionRadius = 3f;

    public bool IsTargeted;
    protected Animator m_Animator;
    protected AiPawn m_Aipawn;
    protected SpriteRenderer m_SpriteRenderer;
    protected Material m_OriginalMaterial;
    private Material m_HighlightMaterial;

    public UnitState CurrentState { get; protected set; } = UnitState.Idle;
    public UnitTask CurrentTask { get; protected set; } = UnitTask.None;
    public Unit Target { get; set; }

    public ActionSO[] Actions => m_Actions;
    public SpriteRenderer Renderer => m_SpriteRenderer;
    public bool HasTarget => Target != null;

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

    public void SetState(UnitState state)
    {
        OnSetState(CurrentState, state);
    }

    public void SetTask(UnitTask task)
    {
        OnSetTask(CurrentTask, task);
    }

    public void SetTarget(Unit target)
    {
        Target = target;
    }

    public void MoveTo(Vector3 destination)
    {
        m_SpriteRenderer.flipX = destination.x < transform.position.x;
        m_Aipawn.SetDestination(destination);

        OnSetDestination();

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

    protected virtual void OnSetDestination()
    {
        // Override in child classes
    }

    protected virtual void OnSetTask( UnitTask oldTask, UnitTask newTask)
    {
        CurrentTask = newTask;
    }

    protected virtual void OnSetState(UnitState oldState, UnitState newState)
    {
        CurrentState = newState;
    }

    protected Collider2D[] RunProximityOjectDetection()
    {
        return Physics2D.OverlapCircleAll(transform.position, m_OjectDetectionRadius);
    }

    void HightLight()
    {
        m_SpriteRenderer.material = m_HighlightMaterial;
    }

    void UnHightLight()
    {
        m_SpriteRenderer.material = m_OriginalMaterial;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, m_OjectDetectionRadius);
    }
}
