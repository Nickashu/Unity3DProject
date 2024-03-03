using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    public EnemyObject[] enemyConfigs;
    public Transform bulletHole;
    [HideInInspector] public Transform playerTransform;
    public int enemyType;

    private bool isLerpingDamage = false, isDead = false, chasing = true, canShoot = true;
    private float bulletDamage, originalHealth, currentHealth, shotCooldown;
    private MeshRenderer meshRenderer;
    private NavMeshAgent navMesh;
    private Color originalColor;

    public GameObject healthBar;
    private EnemyHealthBar scriptHealthBar;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        navMesh = GetComponent<NavMeshAgent>();
        currentHealth = enemyConfigs[enemyType].health;
        originalHealth = currentHealth;
        bulletDamage = enemyConfigs[enemyType].bulletDamage;
        meshRenderer.material.color = enemyConfigs[enemyType].color;
        navMesh.speed = enemyConfigs[enemyType].speed;
        shotCooldown = enemyConfigs[enemyType].shotCooldown;
        originalColor = meshRenderer.material.color;
        scriptHealthBar = healthBar.GetComponent<EnemyHealthBar>();
        healthBar.SetActive(true);
    }

    private void Update() {
        navMesh.destination = playerTransform.position;
        transform.LookAt(playerTransform.position);   //Fazendo o inimigo sempre olhar para o player
        //Debug.Log("hasPath: " + navMesh.hasPath + "    isStopped: " + navMesh.isStopped + "    velocity: " + navMesh.velocity);
        if (navMesh.velocity.magnitude <= 0.1f)
            chasing = false;
        else
            chasing = true;

        if (!chasing) {    //Se estiver perto do jogador
            if (canShoot) {
                canShoot = false;
                StartCoroutine(shoot());
            }
        }

        if (!isDead) {
            if (currentHealth <= 0)
                isDead = true;
        }
    }

    private void takeDamage(float damage) {
        if (isLerpingDamage) {
            isLerpingDamage=false;
            meshRenderer.material.color = originalColor;
        }
        currentHealth -= damage;
        bool dead = currentHealth <= 0 ? true : false;
        scriptHealthBar.updateHealth(currentHealth, originalHealth, damage, dead);
        StartCoroutine(blinkDamage());
    }


    private void OnCollisionEnter(Collision collision) {
        if (!isDead) {
            if (collision.gameObject.CompareTag("bullet")) {
                Bullet scriptBullet = collision.gameObject.GetComponent<Bullet>();
                if (!scriptBullet.enemyBullet)    //Se não for um tiro de outro inimigo
                    takeDamage(scriptBullet.damage);
            }
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

    private void finishDieAnimation() {
        Destroy(gameObject);
        GameController.GetInstance().numEnemies--;
    }



    private IEnumerator shoot() {
        BulletController.GetInstance().spawnBullet(bulletHole.transform.position, (int)BulletController.typesOfGuns.enemyGun, transform.rotation, true, bulletDamage);
        yield return new WaitForSeconds(shotCooldown);
        canShoot = true;
    }

}
