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
        m_velocity = new Vector2(
            (transform.position.x - m_LastPosition.x),
            (transform.position.z - m_LastPosition.z)
        ) / Time.deltaTime;

        m_LastPosition = transform.position;
        isMoving = m_velocity.magnitude > 0;
        m_Animator.SetFloat("Speed", Mathf.Clamp01(CurrentSpeed));
    }
}
