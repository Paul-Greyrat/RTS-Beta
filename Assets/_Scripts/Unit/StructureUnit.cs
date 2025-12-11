using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureUnit : Unit
{
    private Buildingprocess m_Buildingprocess;

    public bool IsUnderConstruction => m_Buildingprocess != null;

    void Update()
    {
        if (IsUnderConstruction)
        {
            m_Buildingprocess.Update();
            
        }
    }

    public void OnConstructionFinished() => m_Buildingprocess = null;

    public void ResgisterProcess(Buildingprocess process)
    {
        m_Buildingprocess = process;
    }

    public void AssignWorkerToBuildProcess(WorkerUnit worker)
    {
        m_Buildingprocess?.AddWorker(worker);
    }

    public void UnassignWorkerFromBuildProcess()
    {
        m_Buildingprocess?.RemoveWorker();
    }
}
