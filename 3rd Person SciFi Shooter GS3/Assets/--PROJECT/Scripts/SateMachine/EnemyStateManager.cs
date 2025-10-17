using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateManager : MonoBehaviour
{

                /***************************************************************************************
*    Title: How to Program in Unity: State Machines Explained
*    Author: iHeartGameDev
*    Date: 2021
*    Code version: 2021 LTS
*    Availability: https://www.youtube.com/watch?v=Vt8aZDPzRjI
*
***************************************************************************************/

    //state machince scripts
    EnemyBaseState currentState;
    public EnemyAttack EnemyAttack = new EnemyAttack();
    public EnemyChase EnemyChase = new EnemyChase();
    public EnemyPatrol EnemyPatrol = new EnemyPatrol();


    [Header("Scriptable Object Data")]
    public EnemyManager enemyData;


    [Header("References")]
    [SerializeField] public NavMeshAgent navAgent;
    [SerializeField] public Transform playerTransform;
    [SerializeField] public Transform firePoint;


    [Header("Layers")]
    [SerializeField] public LayerMask terrainLayer;
    [SerializeField] public LayerMask playerLayerMask;


    public Vector3 currentPatrolPoint;
    public bool hasPatrolPoint;
    public bool isOnAttackCooldown;
    public bool isPlayerVisible;
    public bool isPlayerInRange;
    public int currentHealth;


    public void Awake()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        if (navAgent == null)
        {
            navAgent = GetComponent<NavMeshAgent>();
        }

        // Initialize stats from the Scriptable Object
        if (enemyData != null)
        {
            currentHealth = enemyData.maxHealth;
            navAgent.speed = enemyData.moveSpeed;
        }

        SwitchState(EnemyPatrol);
    }

    void Update()
    {
        //check for player
        DetectPlayer();
        // execute the logic for current state
        currentState.onUpdate(this);
    }

    //state transitions
    public void SwitchState(EnemyBaseState state)
    {
        
        currentState = state;
        state.onEnter(this);

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Accessing engagementRange via enemyData
        if (enemyData != null) Gizmos.DrawWireSphere(transform.position, enemyData.engagementRange);

        Gizmos.color = Color.yellow;
        // Accessing visionRange via enemyData
        if (enemyData != null) Gizmos.DrawWireSphere(transform.position, enemyData.visionRange);
    }

    public void DetectPlayer()
    {
        if (enemyData == null) return;
        // Accessing visionRange and engagementRange via enemyData
        isPlayerVisible = Physics.CheckSphere(transform.position, enemyData.visionRange, playerLayerMask);
        isPlayerInRange = Physics.CheckSphere(transform.position, enemyData.engagementRange, playerLayerMask);
    }

    public void FireProjectile()
    {
        // Accessing projectilePrefab, forwardShotForce, and verticalShotForce via enemyData
        if (enemyData == null || enemyData.projectilePrefab == null || firePoint == null) return;

        Rigidbody projectileRb = Instantiate(enemyData.projectilePrefab, firePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        projectileRb.AddForce(transform.forward * enemyData.forwardShotForce, ForceMode.Impulse);
        projectileRb.AddForce(transform.up * enemyData.verticalShotForce, ForceMode.Impulse);

        Destroy(projectileRb.gameObject, 3f);
    }

    public void FindPatrolPoint()
    {
        if (enemyData == null) return;
        // Accessing patrolRadius via enemyData
        float randomX = Random.Range(-enemyData.patrolRadius, enemyData.patrolRadius);
        float randomZ = Random.Range(-enemyData.patrolRadius, enemyData.patrolRadius);

        Vector3 potentialPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(potentialPoint, -transform.up, 2f, terrainLayer))
        {
            currentPatrolPoint = potentialPoint;
            hasPatrolPoint = true;
        }
        else
    {
        
        Debug.LogWarning("Raycast failed to find a patrol point.");
    }
    }

    public IEnumerator AttackCooldownRoutine()
    {
        isOnAttackCooldown = true;
        // Accessing attackCooldown via enemyData
        if (enemyData != null)
        {
            yield return new WaitForSeconds(enemyData.attackCooldown);
        }
        isOnAttackCooldown = false;
    }
    

    public void PerformPatrol()
    {
        if (!hasPatrolPoint)
            FindPatrolPoint();

        if (hasPatrolPoint)
            navAgent.SetDestination(currentPatrolPoint);

        if (Vector3.Distance(transform.position, currentPatrolPoint) < 1f)
            hasPatrolPoint = false;
    }


    public void PerformChase()
    {
        if (playerTransform != null)
        {
            navAgent.SetDestination(playerTransform.position);
        }
    }


    public void PerformAttack()
    {
        navAgent.SetDestination(transform.position);


        if (playerTransform != null)
        {
            transform.LookAt(playerTransform);
        }


        if (!isOnAttackCooldown)
        {
            FireProjectile();
            StartCoroutine(AttackCooldownRoutine());
        }
    }

   



}
