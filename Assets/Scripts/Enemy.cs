using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class Enemy : MonoBehaviour {
    public EnemyObject[] enemyConfigs;
    public Transform bulletHole;
    public ParticleSystem particlesDeath;
    [HideInInspector] public Transform playerTransform;
    public int enemyType;

    private bool isLerpingDamage = false, isDead = false, chasing = true, canShoot = true, deadFlag=false;
    private float bulletDamage, originalHealth, currentHealth, shotCooldown;
    [SerializeField] private float distShootPlayer;
    private MeshRenderer meshRenderer;
    private NavMeshAgent navMesh;
    private Color originalColor;
    private Player playerScript;

    public GameObject healthBar;
    private HealthBar scriptHealthBar;

    private void Start() {
        playerScript = playerTransform.gameObject.GetComponent<Player>();
        meshRenderer = GetComponent<MeshRenderer>();
        navMesh = GetComponent<NavMeshAgent>();
        scriptHealthBar = healthBar.GetComponent<HealthBar>();
        currentHealth = enemyConfigs[enemyType].health;
        originalHealth = currentHealth;
        bulletDamage = enemyConfigs[enemyType].bulletDamage;
        meshRenderer.material.color = enemyConfigs[enemyType].color;
        navMesh.speed = enemyConfigs[enemyType].speed;
        shotCooldown = enemyConfigs[enemyType].shotCooldown;
        originalColor = meshRenderer.material.color;
        healthBar.SetActive(true);
    }

    private void Update() {
        if (!GameController.GetInstance().gamePaused) {
            if (!playerScript.dead) {    //Se o jogador não tiver morrido
                navMesh.destination = playerTransform.position;
                transform.LookAt(playerTransform.position);   //Fazendo o inimigo sempre olhar para o player
                if ((gameObject.transform.position - playerTransform.position).magnitude <= distShootPlayer)
                    chasing = false;
                else
                    chasing = true;

                if (!chasing) {    //Se estiver perto do jogador
                    if (canShoot && !isDead) {
                        canShoot = false;
                        StartCoroutine(shoot());
                    }
                }
            }

            if (isDead) {
                if (!deadFlag) {
                    deadFlag = true;
                    enemyDeath();
                }
            }
            else {
                if (currentHealth <= 0)
                    isDead = true;
            }
        }
        else {
            navMesh.velocity = Vector3.zero;
        }
    }

    private IEnumerator shoot() {
        BulletController.GetInstance().spawnBullet(bulletHole.transform.position, (int)BulletController.typesOfGuns.enemyGun, transform.rotation, true, bulletDamage);
        yield return new WaitForSeconds(shotCooldown);
        canShoot = true;
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

    private void takeDamage(float damage) {
        if (isLerpingDamage) {
            isLerpingDamage=false;
            meshRenderer.material.color = originalColor;
        }
        currentHealth -= damage;
        isDead = currentHealth <= 0 ? true : false;
        scriptHealthBar.updateHealth(currentHealth, originalHealth, isDead);
        StartCoroutine(blinkDamage());
    }

    private IEnumerator blinkDamage() {     //Método para fazer o inimigo "piscar" ao levar dano
        isLerpingDamage = true;
        float timePassed = 0f, lerpDuration = 0.2f;
        while (timePassed < lerpDuration) {
            meshRenderer.material.color =  Color.Lerp(originalColor, Color.black, lerpDuration);
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

    private void enemyDeath() {
        Destroy(gameObject);
        GameController.GetInstance().updateCoins(enemyType+1);
        GameController.GetInstance().numEnemies--;
        ParticleSystem particles = Instantiate(particlesDeath, gameObject.transform.position, Quaternion.identity);
        ParticleSystem.MainModule particlesMain = particles.main;
        Color colorParticles = meshRenderer.material.color;
        colorParticles.a = 1f;
        particlesMain.startColor = colorParticles;
        particles.gameObject.SetActive(true);
        particles.Play();
    }
}
