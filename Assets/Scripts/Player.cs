using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private float vertical, horizontal, turnSmoothVelocity, originalHealth, originalMovementSpeed, originalMovementSpeedBoost, referenceOriginalSpeed;
    private bool canShoot = true, aiming = false, enableAiming = true, isDead=false, isLerpingDamage=false;
    private bool powerUpVelocity = false, powerUpTimesTwo = false;   //Essas variáveis manterão os estados dos power-ups
    private int selectedGun=0;
    private Rigidbody rb;
    private HealthBar scriptHealthBar;
    private MeshRenderer meshRenderer;
    private Color originalColor;
    private Queue<bool> queuePowerUpVelocity = new Queue<bool>(), queuePowerUpTimesTwo = new Queue<bool>();   //Estas filas serão usadas na lógica dos power-ups
    [SerializeField] private float jumpPower, movementSpeed, jumpSmoothness, turnSmoothTime, maxMovementVelocity, currentHealth, healthIncrease;

    [HideInInspector] public bool showAiming = false;
    public Transform groundCheck, cam, bulletHole;
    public LayerMask groundLayer;
    public GameObject healthBar, canvasDeath, iconPowerUpVelocity, iconPowerUpTimesTwo;
    [SerializeField] private GameObject[] imgsWeapons;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        originalMovementSpeed = movementSpeed;
        originalMovementSpeedBoost = originalMovementSpeed * 2;
        scriptHealthBar = healthBar.GetComponent<HealthBar>();
        originalHealth = currentHealth;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
        imgsWeapons[selectedGun].SetActive(true);
        updatePowerUps();
        InvokeRepeating("IncreaseHealth", 0f, 1f);   //A vida do jogador aumentará constantemente
    }

    private void Update() {
        if (!GameController.GetInstance().gamePaused) {    //Se o jogo não estiver pausado
            aiming = Input.GetKey(KeyCode.Mouse1) && isOnGround();
            if (aiming) {
                transform.rotation = Quaternion.Euler(transform.rotation.x, cam.transform.eulerAngles.y, transform.rotation.z);
                vertical = 0;
                horizontal = 0;
            }
            else {   //Só poderá correr se não estiver mirando
                if (Input.GetKey(KeyCode.LeftShift))
                    movementSpeed = referenceOriginalSpeed * 1.5f;
                else
                    movementSpeed = referenceOriginalSpeed;
            }

            if (aiming && enableAiming)
                showAiming = true;
            else
                showAiming = false;

            //Detecções de botões:
            if (Input.GetKeyDown(KeyCode.R)) {    //Para trocar de arma
                imgsWeapons[selectedGun].SetActive(false);
                selectedGun = selectedGun == Globals.shotCoolDownGuns.Count - 1 ? 0 : selectedGun + 1;
                if (selectedGun == (int)Globals.typesOfGuns.misteryGun) {
                    if (!Globals.hasMisteryGun)
                        selectedGun = selectedGun == Globals.shotCoolDownGuns.Count - 1 ? 0 : selectedGun + 1;
                }
                canShoot = true;
                imgsWeapons[selectedGun].SetActive(true);
                //Debug.Log("Mudou para a arma: " + Enum.GetName(typeof(Globals.typesOfGuns), selectedGun));
            }

            if (Input.GetKey(KeyCode.Mouse0)) {
                if (canShoot) {
                    canShoot = false;
                    enableAiming = false;
                    StartCoroutine(shoot());
                }
            }
        }
    }

    private void FixedUpdate() {
        if (!GameController.GetInstance().gamePaused) {
            if (!aiming) {
                Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
                if (direction.magnitude >= 0.1f) {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;   //Pegando o ângulo da movimentação em graus (levando em consideração a câmera)
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);    //Fazendo a rotação acontecer de forma suave
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                    if (rb.velocity.magnitude < maxMovementVelocity) {
                        Vector3 moveDirection = (Quaternion.Euler(0f, angle, 0f) * Vector3.forward).normalized;
                        rb.AddForce(moveDirection * movementSpeed);    //Para usar o AddForce, lembrar de adicionar um drag no inspetor para limitar a velocidade!!!!
                    }
                    //characterController.Move(moveDirection * movementSpeed * Time.deltaTime);   //Usando o characterController para mover o personagem de acordo com a posição da câmera
                    //rb.velocity = new Vector3(moveDirection.x * movementSpeed, moveDirection.y, moveDirection.x * movementSpeed);
                    //transform.Translate(moveDirection * movementSpeed * Time.deltaTime);
                    //rb.MovePosition(transform.position + moveDirection * movementSpeed * Time.deltaTime);
                }
                else {
                    float xVelocity = Mathf.Lerp(rb.velocity.x, 0f, 0.05f);
                    float zVelocity = Mathf.Lerp(rb.velocity.z, 0f, 0.05f);
                    rb.velocity = new Vector3(xVelocity, rb.velocity.y, zVelocity);
                }
            }
        }
    }

    public void SetMovement(InputAction.CallbackContext value) {
        if (!GameController.GetInstance().gamePaused) {
            if (!aiming) {
                horizontal = value.ReadValue<Vector3>().x;
                vertical = value.ReadValue<Vector3>().z;
                if (value.canceled) {    //Se o evento for cancelado
                    horizontal = 0;
                    vertical = 0;
                }
            }
        }
    }
    public void SetJump(InputAction.CallbackContext value) {
        if (!GameController.GetInstance().gamePaused) {
            if (value.performed) {
                if (isOnGround()) {
                    SoundController.GetInstance().PlaySound("jump");
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                    //rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
                }
            }
            if (value.canceled) {    //Se o evento for cancelado
                if (rb.velocity.y > 0.1f) 
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * jumpSmoothness, rb.velocity.z);
            }
        }
    }

    private bool isOnGround() {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer);
        return colliders.Length > 0;
    }

    private IEnumerator shoot() {
        BulletController.GetInstance().spawnBullet(bulletHole.transform.position, selectedGun, transform.rotation, gameObject);
        yield return new WaitForSeconds(Globals.shotCoolDownGuns[selectedGun] * 0.5f);  //Espera metade do tempo de cool down para liberar a mira novamente
        enableAiming = true;
        yield return new WaitForSeconds(Globals.shotCoolDownGuns[selectedGun] * 0.5f);
        canShoot = true;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!isDead) {
            if (collision.gameObject.CompareTag("bullet")) {
                Bullet scriptBullet = collision.gameObject.GetComponent<Bullet>();
                if (scriptBullet.enemyBullet)    //Se for um tiro de um inimigo
                    takeDamage(scriptBullet.damage);
            }
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (!isDead) {
            if (collider.gameObject.CompareTag("powerUpPrefab")) {
                PowerUp powerUpScript = collider.transform.GetChild(0).gameObject.GetComponent<PowerUp>();
                //Debug.Log("Pegou power-up do tipo: " + Enum.GetName(typeof(Globals.typesOfPowerUps), powerUpScript.type));
                SoundController.GetInstance().PlaySound("powerUp");
                collectPowerUp(powerUpScript.type);
                Destroy(collider.gameObject);
            }
        }
    }

    //Métodos para controlar power-ups:
    private void collectPowerUp(int powerUpType) {
        switch(powerUpType) {
            case (int)Globals.typesOfPowerUps.life:
                currentHealth += 20f;
                updateHealthBar();
                break;
            case (int)Globals.typesOfPowerUps.velocity:
                StartCoroutine(activatePowerUp(queuePowerUpVelocity));
                break;
            case (int)Globals.typesOfPowerUps.timesTwo:
                StartCoroutine(activatePowerUp(queuePowerUpTimesTwo));
                break;
        }
    }

    private IEnumerator activatePowerUp(Queue<bool> queuePowerUp) {
        //Debug.Log("Tamanho das filas: Velocity: " + queuePowerUpVelocity.Count + "    TimesTwo: " + queuePowerUpTimesTwo.Count);
        queuePowerUp.Enqueue(true);
        updatePowerUps();
        float timePassed = 0f, duration = GameController.GetInstance().powerUpTime;
        while (timePassed < duration) {
            float t = timePassed / duration;
            if (!GameController.GetInstance().gamePaused)
                timePassed += Time.deltaTime;
            yield return null;
        }
        //Debug.Log("Tempo de um power-up acabou!");
        queuePowerUp.Dequeue();
        updatePowerUps();
        //Debug.Log("Tamanho das filas: Velocity: " + queuePowerUpVelocity.Count + "    TimesTwo: " + queuePowerUpTimesTwo.Count);
    }

    private void updatePowerUps() {
        powerUpVelocity = queuePowerUpVelocity.Count > 0 ? true : false;
        powerUpTimesTwo = queuePowerUpTimesTwo.Count > 0 ? true : false;
        referenceOriginalSpeed = powerUpVelocity ? originalMovementSpeedBoost : originalMovementSpeed;
        GameController.GetInstance().coinsMultiplier = powerUpTimesTwo ? GameController.GetInstance().baseCoinsMultiplier * 2 : GameController.GetInstance().baseCoinsMultiplier;
        iconPowerUpVelocity.SetActive(powerUpVelocity);
        iconPowerUpTimesTwo.SetActive(powerUpTimesTwo);
    }

    //Métodos referentes à dano e barra de vida:
    private void takeDamage(float damage) {
        if (isLerpingDamage) {
            isLerpingDamage = false;
            meshRenderer.material.color = originalColor;
        }
        currentHealth -= damage;
        GameController.GetInstance().playerDead = currentHealth <= 0 ? true : false;
        updateHealthBar();
        StartCoroutine(blinkDamage());
    }
    private void updateHealthBar() {
        scriptHealthBar.updateHealth(currentHealth, originalHealth, GameController.GetInstance().playerDead);
    }

    private IEnumerator blinkDamage() {
        isLerpingDamage = true;
        float timePassed = 0f, lerpDuration = 0.2f;
        while (timePassed < lerpDuration) {
            meshRenderer.material.color = Color.Lerp(originalColor, Color.black, lerpDuration);
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

    private void IncreaseHealth() {
        if (!GameController.GetInstance().gamePaused) {
            if (currentHealth > 0) {
                if (currentHealth < originalHealth) {
                    currentHealth += healthIncrease;
                    scriptHealthBar.updateHealth(currentHealth, originalHealth, false);
                }
            }
        }
    }
}
