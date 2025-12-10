


using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    protected override void UpdateBehavior()
    {
        if (CurrentTask != UnitTask.None)
        {

            CheckForCloseojects();
            
        }
    }

    protected override void OnSetDestination()
    {
        
    }

    void CheckForCloseojects()
    {
        Debug.Log("Checking");
        var hits = RunProximityOjectDetection();

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            if (CurrentTask == UnitTask.Build && hit.gameObject == Target.gameObject)
            {
                if (hit.TryGetComponent<StructureUnit>(out var unit))
                {
                    StartBuilding(unit);
                }
            }
            Debug.Log(hit.gameObject.name);
        }
    }

    void StartBuilding(StructureUnit unit)
    {
        Debug.Log("Building started..." + unit.gameObject.name);
    }
}
