using UnityEngine;

public class EnemyPatrol : EnemyBaseState
{

    public override void onEnter(EnemyStateManager stateManager)
    {
        stateManager.FindPatrolPoint();
    }

    public override void onUpdate(EnemyStateManager stateManager)
    {
        stateManager.PerformPatrol();

        if (stateManager.isPlayerVisible)
        {
            stateManager.SwitchState(stateManager.EnemyChase);
        }
        
    }

    public override void onExit(EnemyStateManager stateManager)
    {
        
    }
    









}
