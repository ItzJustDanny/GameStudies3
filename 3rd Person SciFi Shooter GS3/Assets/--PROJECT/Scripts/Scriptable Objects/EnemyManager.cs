using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObject/EnemyData")]
public class EnemyManager : ScriptableObject
{
    [Header("Enemy Stats")]
    public int maxHealth;
    public int attackDamage;
    public float patrolRadius = 10f;
    public float attackCooldown = 1f;
    public float forwardShotForce = 10f;
    public float verticalShotForce = 5f;
    public float visionRange = 20f;
    public float engagementRange = 10f;
    public float moveSpeed = 3.5f;

    [Header("References")]
    public GameObject projectilePrefab;
}