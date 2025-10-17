using UnityEngine;

public class EnemyChase : EnemyBaseState
{

     public override void onEnter(EnemyStateManager stateManager)
    {
        stateManager.DetectPlayer();
    }

    public override void onUpdate(EnemyStateManager stateManager)
    {
        stateManager.PerformChase();

        if (stateManager.isPlayerVisible)
        {
         stateManager.SwitchState(stateManager.EnemyAttack);
        }
        
    }

    public override void onExit(EnemyStateManager stateManager)
    {
        
    }
    







}
