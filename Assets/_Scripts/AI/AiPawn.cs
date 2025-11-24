using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPawn : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 10f;

    private Vector3? m_Destinastion;

    public Vector3? Destination => m_Destinastion;

    void Update()
    {
        if (m_Destinastion.HasValue)
        {
            var direction = m_Destinastion.Value - transform.position;
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            var distanceToDestination = Vector3.Distance(transform.position, m_Destinastion.Value);
            if ( distanceToDestination < 0.1f)
            {
                m_Destinastion = null;
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        m_Destinastion = destination;
    }
    
}
