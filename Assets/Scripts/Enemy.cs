using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    public EnemyObject enemyConfigs;
    public Transform bulletHole;
    public ParticleSystem particlesDeath;
    public Canvas canvasPoints;
    [HideInInspector] public Transform playerTransform;
    public int enemyType;

    private bool isLerpingDamage = false, isDead = false, chasing = true, canShoot = true, deadFlag=false;
    private float bulletDamage, originalHealth, currentHealth, shotCooldown;
    [SerializeField] private float distShootPlayer, distLookPlayer;
    private MeshRenderer meshRenderer;
    private NavMeshAgent navMesh;
    private Color originalColor;

    public GameObject healthBar;
    private HealthBar scriptHealthBar;

    private void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        navMesh = GetComponent<NavMeshAgent>();
        scriptHealthBar = healthBar.GetComponent<HealthBar>();
        currentHealth = enemyConfigs.health;
        originalHealth = currentHealth;
        bulletDamage = enemyConfigs.bulletDamage;
        meshRenderer.material.color = enemyConfigs.color;
        navMesh.speed = enemyConfigs.speed;
        shotCooldown = enemyConfigs.shotCooldown;
        originalColor = meshRenderer.material.color;
        healthBar.SetActive(true);
    }

    private void Update() {
        if (!GameController.GetInstance().gamePaused) {
            if (!GameController.GetInstance().playerDead) {    //Se o jogador não tiver morrido
                navMesh.destination = playerTransform.position;
                float distToPlayer = (gameObject.transform.position - playerTransform.position).magnitude;
                if (distToPlayer <= distShootPlayer)
                    chasing = false;
                else
                    chasing = true;
                if (distToPlayer <= distLookPlayer)
                    transform.LookAt(playerTransform.position);   //Fazendo o inimigo sempre olhar para o player

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
        BulletController.GetInstance().spawnBullet(bulletHole.transform.position, (int)Globals.typesOfGuns.enemyGun, transform.rotation, gameObject, true, bulletDamage);
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
        int amountCoins = (enemyType + 1) * GameController.GetInstance().coinsMultiplier;    //Calculando o número de moedas que o jogador ganhou ao matar o inimigo
        Globals.numCoins += amountCoins;
        GameController.GetInstance().updateCoins();
        GameController.GetInstance().numEnemiesSpawned--;
        GameController.GetInstance().numEnemiesDefeated++;
        //Pontuação:
        Vector3 ptsPosition = gameObject.transform.position;
        ptsPosition.y += 2;   //Fazendo a pontuação aparecer um pouco acima do inimigo
        Canvas canvasPts = Instantiate(canvasPoints, ptsPosition, new Quaternion(transform.rotation.x, Camera.main.transform.rotation.y, transform.rotation.z, Camera.main.transform.rotation.w));
        canvasPts.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + amountCoins.ToString();
        canvasPts.gameObject.SetActive(true);
        //Sistema de partículas:
        SoundController.GetInstance().PlaySound("explosion", gameObject);
        ParticleSystem particles = Instantiate(particlesDeath, gameObject.transform.position, Quaternion.identity);
        ParticleSystem.MainModule particlesMain = particles.main;
        Color colorParticles = meshRenderer.material.color;
        colorParticles.a = 1f;
        particlesMain.startColor = colorParticles;
        particles.gameObject.SetActive(true);
        particles.Play();
        //Power-up:
        Vector3 positionPowerUp = gameObject.transform.position;
        positionPowerUp.y += 1;
        PowerUpController.GetInstance().createPowerUp(positionPowerUp);
        Destroy(gameObject);
    }
}
