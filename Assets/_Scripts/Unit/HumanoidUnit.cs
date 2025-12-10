using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;

public class HumanoidUnit : Unit
{
    protected Vector2 m_velocity;
    protected Vector3 m_LastPosition;

    public float CurrentSpeed => m_velocity.magnitude;

    protected void Start()
    {
        m_LastPosition = transform.position;
    }

    protected void Update()
    {
        UpdateVeVelocity();
        
        UpdateBehavior();
    }
    protected virtual void UpdateBehavior()
    {
        
    }

    protected virtual void UpdateVeVelocity()
    {
        m_velocity = new Vector2(
            (transform.position.x - m_LastPosition.x),
            (transform.position.z - m_LastPosition.z)
        ) / Time.deltaTime;

        m_LastPosition = transform.position;
        var state = m_velocity.magnitude > 0 ? UnitState.Moving : UnitState.Idle;
        SetState(state);
        
        m_Animator?.SetFloat("Speed", Mathf.Clamp01(CurrentSpeed));
    }
}
