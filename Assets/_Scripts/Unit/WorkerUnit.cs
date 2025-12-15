


using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    protected override void UpdateBehavior()
    {
        if (CurrentTask == UnitTask.Build && HasTarget)
        {

            CheckForConstructionSite();
            
        }
    }

    protected override void OnSetDestination() => ResetState();


    public void OnBuildingFinished() => ResetState();

    public void SendToBuild(StructureUnit structure)
    {
        MoveTo(structure.transform.position);
        SetTarget(structure);
        SetTask(UnitTask.Build);
    }

    void CheckForConstructionSite()
    {
        var distanceToConstruction = Vector3.Distance(transform.position, Target.transform.position);
        if ( distanceToConstruction <= m_OjectDetectionRadius && CurrentState == UnitState.Idle)
        {
            StartBuilding(Target as StructureUnit);
        }
    }

    void StartBuilding(StructureUnit structure)
    {
        SetState(UnitState.Building);
        m_Animator.SetBool("IsBuilding", true);
        structure.AssignWorkerToBuildProcess(this);
    }

    void ResetState()
    {
        SetTask(UnitTask.None);
        if (HasTarget) CleanupTarger();
        m_Animator.SetBool("IsBuilding", false);
    }

    void CleanupTarger()
    {
        if (Target is StructureUnit structure)
        {
            structure.UnassignWorkerFromBuildProcess();
        }
        SetTarget(null);
    }
}



    // void CheckForCloseojects()
    // {
    //     Debug.Log("Checking");
    //     var hits = RunProximityOjectDetection();

    //     foreach (var hit in hits)
    //     {
    //         if (hit.gameObject == this.gameObject) continue;

    //         if (CurrentTask == UnitTask.Build && hit.gameObject == Target.gameObject)
    //         {
    //             if (hit.TryGetComponent<StructureUnit>(out var unit))
    //             {
    //                 StartBuilding(unit);
    //             }
    //         }
    //         Debug.Log(hit.gameObject.name);
    //     }
    // }
