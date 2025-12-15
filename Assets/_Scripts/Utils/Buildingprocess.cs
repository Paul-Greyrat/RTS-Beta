  


using UnityEngine;

public class Buildingprocess
{
    private BuildActionSO m_BuildAction;
    private WorkerUnit m_Worker;
    private StructureUnit m_Structure;
    private ParticleSystem m_ConstructionEffect;
    private float m_ProcessTimer;
    private bool m_IsFinished;

    private bool InProcess => HasActiveWorker && m_Worker.CurrentState == UnitState.Building;

    public bool HasActiveWorker => m_Worker != null;
    public Buildingprocess(
        BuildActionSO buildAction,
        Vector3 placementPosition,
        WorkerUnit worker,
        ParticleSystem constructionEffectPrefab
    )
    {
        m_BuildAction = buildAction;
        var effectOffset = new Vector3(0, -1f, 0);
        m_ConstructionEffect = Object.Instantiate(
            constructionEffectPrefab, 
            placementPosition + effectOffset, 
            Quaternion.identity
        );
        m_Structure = Object.Instantiate(m_BuildAction.StructurePrefab); 
        m_Structure.Renderer.sprite = m_BuildAction.FoundationSprite;
        m_Structure.transform.position = placementPosition;
        m_Structure.ResgisterProcess(this);
        worker.SendToBuild(m_Structure);

    }

    public void Update()
    {
        if (m_IsFinished) return;

        if (InProcess)
        {
            m_ProcessTimer += Time.deltaTime;

            if (!m_ConstructionEffect.isPlaying)
            {
                m_ConstructionEffect.Play();
            }
            
            if (m_ProcessTimer >= m_BuildAction.ConstructionTime)
            {
                m_IsFinished = true;
                m_Structure.Renderer.sprite = m_BuildAction.CompletionSprite;
                m_Structure.OnConstructionFinished();
                m_Worker.OnBuildingFinished();
            }

        }
    }

    public void AddWorker(WorkerUnit worker)
    {
        if (HasActiveWorker) return;
        m_Worker = worker;
    }

    public void RemoveWorker()
    {
        if (!HasActiveWorker) return;
        m_Worker = null;
        m_ConstructionEffect.Stop();
    }
}
