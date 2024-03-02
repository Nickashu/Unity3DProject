using UnityEngine;

[CreateAssetMenu(fileName = "enemyConfigs", menuName = "Enemy")]
public class EnemyObject : ScriptableObject {
    public float health, bulletDamage, shotCooldown;
    public Color color;
}
