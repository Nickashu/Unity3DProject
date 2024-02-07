using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public EnemyObject enemyConfigs;

    private bool isLerpingDamage = false;
    private float speed, attackDamage, originalHealth, currentHealth;
    private MeshRenderer meshRenderer;
    private Color originalColor;

    public GameObject healthBar;
    private EnemyHealthBar scriptHealthBar;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        currentHealth = enemyConfigs.health;
        originalHealth = currentHealth;
        speed = enemyConfigs.speed;
        attackDamage = enemyConfigs.attackDamage;
        meshRenderer.material.color = enemyConfigs.color;
        originalColor = meshRenderer.material.color;
        scriptHealthBar = healthBar.GetComponent<EnemyHealthBar>();
        healthBar.SetActive(true);
    }

    private void Update() {
        if (currentHealth <= 0) {
            Destroy(gameObject);
        }
    }

    private void takeDamage(float damage) {
        if (isLerpingDamage) {
            isLerpingDamage=false;
            meshRenderer.material.color = originalColor;
        }
        currentHealth -= damage;
        scriptHealthBar.updateHealth(currentHealth, originalHealth, damage);
        StartCoroutine(blinkDamage());
    }


    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("bullet")) {
            Bullet scriptBullet = collision.gameObject.GetComponent<Bullet>();
            takeDamage(scriptBullet.damage);
        }
    }


    private IEnumerator blinkDamage() {     //Método para fazer o inimigo "piscar" ao levar dano
        isLerpingDamage = true;
        float timePassed = 0f, lerpDuration = 0.1f;
        while (timePassed < lerpDuration) {
            meshRenderer.material.color =  Color.Lerp(originalColor, Color.white, lerpDuration);
            timePassed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        timePassed = 0f;
        while (timePassed < lerpDuration) {
            meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, originalColor, lerpDuration);
            timePassed += Time.deltaTime;
            yield return null;
        }
        isLerpingDamage = false;
    }

}
