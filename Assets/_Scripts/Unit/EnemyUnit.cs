using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
    public override bool IsPlayer => false;

    protected override void UpdateBehaviour()
    {

        switch (CurrentState)
        {
            case UnitState.Idle:
            case UnitState.Moving:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target.transform))
                    {
                        SetState(UnitState.Attacking);
                        // Stop moving
                    }
                    else
                    {
                        MoveTo(Target.transform.position);
                    }
                }
                else
                {
                    if (TryFindClosestFoe(out var foe))
                    {
                        Debug.Log(foe.gameObject.name);
                    }
                }
                break;
            case UnitState.Attacking:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target.transform))
                    {
                        Debug.Log("Attacking");
                    }
                    else
                    {
                        SetState(UnitState.Idle);
                    }
                }
                else
                {
                    SetState(UnitState.Idle);
                }

                break;
        }
    }
}
