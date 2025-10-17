using UnityEngine;

public class EnemyAttack : EnemyBaseState
{

     public override void onEnter(EnemyStateManager stateManager)
    {
       stateManager.navAgent.SetDestination(stateManager.transform.position);
    }

    public override void onUpdate(EnemyStateManager stateManager)
    {
        stateManager.PerformAttack();

        if (!stateManager.isPlayerInRange)
        {
            stateManager.SwitchState(stateManager.EnemyChase);
        }
         else if (!stateManager.isPlayerVisible)
        {
            stateManager.SwitchState(stateManager.EnemyPatrol);
        }
        
    }

    public override void onExit(EnemyStateManager stateManager)
    {
        
    }






    
}
