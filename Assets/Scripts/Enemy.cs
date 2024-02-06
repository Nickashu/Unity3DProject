using UnityEngine;

public class Enemy : MonoBehaviour {
    public EnemyObject enemyConfigs;

    private float health, speed, attackDamage;
    private MeshRenderer meshRenderer;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        health = enemyConfigs.health;
        speed = enemyConfigs.speed;
        attackDamage = enemyConfigs.attackDamage;
        meshRenderer.material.color = enemyConfigs.color;
    }

}
