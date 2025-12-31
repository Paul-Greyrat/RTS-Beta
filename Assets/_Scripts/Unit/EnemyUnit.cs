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
                        Debug.Log("Changing to Attacking State");
                        SetState(UnitState.Attacking);
                        //stop movement
                    }
                    else
                    {
                        Debug.Log("Moving to Target");
                        MoveTo(Target.transform.position);
                    }
                }
                else
                {
                    if (TryFindClosestFoe(out var foe))
                    {
                        SetTarget(foe);
                        MoveTo(foe.transform.position);
                        Debug.Log("Target Detected - Move to target!");
                    }
                }
                break;

            case UnitState.Attacking:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target.transform))
                    {
                        tryAttackCurrenttarget();
                    }
                    else
                    {
                        Debug.Log("Back to Moving target");
                        SetState(UnitState.Moving);
                    }
                }
                else
                {
                    Debug.Log("back to Idle state");
                    SetState(UnitState.Idle);
                }

                break;
        }
    } 
}
 
