using UnityEngine;

[CreateAssetMenu(fileName = "enemyConfigs", menuName = "Enemy")]
public class EnemyObject : ScriptableObject {
    public float health, speed, attackDamage;
    public Color color;
}
