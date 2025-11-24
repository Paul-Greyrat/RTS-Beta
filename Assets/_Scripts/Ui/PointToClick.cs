using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToClick : MonoBehaviour
{
    [SerializeField] private float m_Duration = 1f;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private AnimationCurve m_ScaleCurve;

    private float m_Timer;
    private float m_FreqTimer;
    private Vector3 m_InitialScale;

    void Start()
    {
        m_InitialScale = transform.localScale;
    }

    void Update()
    {
        m_Timer += Time.deltaTime;
        m_FreqTimer += Time.deltaTime;
        m_FreqTimer %= 1f;


        float scaleMultiplier = m_ScaleCurve.Evaluate(m_FreqTimer);
        transform.localScale = m_InitialScale * scaleMultiplier;

        if (m_Timer >= m_Duration * 0.9f)
        {
            float fadeProgess = (m_Timer - m_Duration * 0.9f) / (m_Duration * 0.1f);
            m_SpriteRenderer.color = new Color(1f, 1f, 1f, 1f - fadeProgess);
        }

        if (m_Timer >= m_Duration)
        {
            Destroy(gameObject);
        }
    }

}
