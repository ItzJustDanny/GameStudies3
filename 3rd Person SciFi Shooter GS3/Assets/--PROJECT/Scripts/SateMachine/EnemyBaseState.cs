using UnityEngine;

public abstract class EnemyBaseState
{


    public abstract void onEnter(EnemyStateManager stateManager);
    public abstract void onUpdate(EnemyStateManager stateManager);
    public abstract void onExit(EnemyStateManager stateManager);

    







}
