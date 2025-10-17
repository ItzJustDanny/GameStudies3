using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{

    /***************************************************************************************
*    Title: How to make AI in Unity
*    Author: Brockosh
*    Date: 2024
*    Code version: 2022.3.12f1
*    Lines: 19 - 183
*    Availability: https://www.youtube.com/watch?v=KZROVLPQdWc
*
***************************************************************************************/

    [Header("Scriptable Object Data")]
    public EnemyManager enemyData;

    [Header("References")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform firePoint;

    [Header("Layers")]
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private LayerMask playerLayerMask;

    private Vector3 currentPatrolPoint;
    private bool hasPatrolPoint;
    private bool isOnAttackCooldown;
    private bool isPlayerVisible;
    private bool isPlayerInRange;

    private int currentHealth;

    private void Awake()
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
    }

    private void Update()
    {
        DetectPlayer();
        UpdateBehaviourState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Accessing engagementRange via enemyData
        if (enemyData != null) Gizmos.DrawWireSphere(transform.position, enemyData.engagementRange);

        Gizmos.color = Color.yellow;
        // Accessing visionRange via enemyData
        if (enemyData != null) Gizmos.DrawWireSphere(transform.position, enemyData.visionRange);
    }

    private void DetectPlayer()
    {
        if (enemyData == null) return;
        // Accessing visionRange and engagementRange via enemyData
        isPlayerVisible = Physics.CheckSphere(transform.position, enemyData.visionRange, playerLayerMask);
        isPlayerInRange = Physics.CheckSphere(transform.position, enemyData.engagementRange, playerLayerMask);
    }

    private void FireProjectile()
    {
        // Accessing projectilePrefab, forwardShotForce, and verticalShotForce via enemyData
        if (enemyData == null || enemyData.projectilePrefab == null || firePoint == null) return;

        Rigidbody projectileRb = Instantiate(enemyData.projectilePrefab, firePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        
        projectileRb.AddForce(transform.forward * enemyData.forwardShotForce, ForceMode.Impulse);
        projectileRb.AddForce(transform.up * enemyData.verticalShotForce, ForceMode.Impulse);

        Destroy(projectileRb.gameObject, 3f);
    }

    private void FindPatrolPoint()
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
    }

    private IEnumerator AttackCooldownRoutine()
    {
        isOnAttackCooldown = true;
        // Accessing attackCooldown via enemyData
        if (enemyData != null)
        {
            yield return new WaitForSeconds(enemyData.attackCooldown);
        }
        isOnAttackCooldown = false;
    }

    
    private void PerformPatrol()
    {
        if (!hasPatrolPoint)
            FindPatrolPoint();

        if (hasPatrolPoint)
            navAgent.SetDestination(currentPatrolPoint);

        if (Vector3.Distance(transform.position, currentPatrolPoint) < 1f)
            hasPatrolPoint = false;
    }


    private void PerformChase()
    {
        if (playerTransform != null)
        {
            navAgent.SetDestination(playerTransform.position);
        }
    }


    private void PerformAttack()
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


    private void UpdateBehaviourState()
    {
        if (!isPlayerVisible && !isPlayerInRange)
        {
            PerformPatrol();
        }
        else if (isPlayerVisible && !isPlayerInRange)
        {
            PerformChase();
        }
        else if (isPlayerVisible && isPlayerInRange)
        {
            PerformAttack();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Death();
            Debug.Log("Enemy died");
        }

    }

    private void Death()
    {
        // play animation
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            currentHealth = currentHealth - 25;
        }
    }
}