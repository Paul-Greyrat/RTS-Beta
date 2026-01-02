using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
    public override bool IsPlayer => false;
    private float m_AttackCommitmentTime = 1f;
    private float m_CurrentAttackCommitmentTime = 0;

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
                        StopMovement();
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
                        SetTarget(foe);
                        MoveTo(foe.transform.position);
                    }
                }
                break;

            case UnitState.Attacking:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target.transform))
                    {
                        m_CurrentAttackCommitmentTime = m_AttackCommitmentTime;
                        tryAttackCurrenttarget();
                    }
                    else
                    {
                        m_CurrentAttackCommitmentTime -= Time.deltaTime;
                        if (m_CurrentAttackCommitmentTime <= 0)
                        {
                            SetState(UnitState.Moving);
                        }
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
 
