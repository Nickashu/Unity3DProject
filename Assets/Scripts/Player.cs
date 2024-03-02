using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private float vertical, horizontal, turnSmoothVelocity;
    private bool canShoot = true, aiming = false, enableAiming = true, isDead=false;
    private int selectedGun=0;
    private Rigidbody rb;
    [SerializeField] 
    private float jumpPower, movementSpeed, jumpSmoothness, turnSmoothTime, originalMovementSpeed, maxMovementVelocity;

    public bool showAiming = false;
    public Transform groundCheck, cam, bulletHole;
    public LayerMask groundLayer;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        originalMovementSpeed = movementSpeed;
    }

    private void Update() {
        //Debug.Log("rotacao jogador: " + transform.rotation.eulerAngles.x + "   " + transform.rotation.eulerAngles.y + "   " + transform.rotation.eulerAngles.z);
        aiming = Input.GetKey(KeyCode.Mouse1);
        if (aiming)
            movementSpeed = originalMovementSpeed * 0.7f;
        else {   //Só poderá correr se não estiver mirando
            if (Input.GetKey(KeyCode.LeftShift))
                movementSpeed = originalMovementSpeed * 1.5f;
            else
                movementSpeed = originalMovementSpeed;
        }

        if (aiming && enableAiming)
            showAiming = true;
        else
            showAiming = false;


        //Detecções de botões:
        if (Input.GetKeyDown(KeyCode.R)) {    //Para trocar de arma
            if (selectedGun == BulletController.GetInstance().shotCoolDownGuns.Count - 1)
                selectedGun = 0;
            else
                selectedGun++;
            canShoot = true;
            updateSelectedGun();
        }

        if (Input.GetKey(KeyCode.Mouse0)) {
            if (canShoot) {
                canShoot = false;
                enableAiming = false;
                StartCoroutine(shoot());
            }
        }
    }

    private void FixedUpdate() {
        if (!GameController.GetInstance().gamePaused) {
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            if (direction.magnitude >= 0.1f) {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;   //Pegando o ângulo da movimentação em graus (levando em consideração a câmera)
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);    //Fazendo a rotação acontecer de forma suave
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                if(rb.velocity.magnitude < maxMovementVelocity) {
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

    public void SetMovement(InputAction.CallbackContext value) {
        if (!GameController.GetInstance().gamePaused) {
            horizontal = value.ReadValue<Vector3>().x;
            vertical = value.ReadValue<Vector3>().z;
            if (value.canceled) {    //Se o evento for cancelado
                horizontal = 0;
                vertical = 0;
            }
        }
    }
    public void SetJump(InputAction.CallbackContext value) {
        if (!GameController.GetInstance().gamePaused) {
            if (value.performed) {
                if (isOnGround()) {
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

    private void updateSelectedGun() {
        Debug.Log("Mudou para a arma: " + Enum.GetName(typeof(BulletController.typesOfGuns), selectedGun));
    }


    private IEnumerator shoot() {
        BulletController.GetInstance().spawnBullet(bulletHole.transform.position, selectedGun, transform.rotation);
        yield return new WaitForSeconds(BulletController.GetInstance().shotCoolDownGuns[selectedGun] * 0.5f);  //Espera metade do tempo de cool down para liberar a mira novamente
        enableAiming = true;
        yield return new WaitForSeconds(BulletController.GetInstance().shotCoolDownGuns[selectedGun] * 0.5f);
        canShoot = true;
    }



    private void takeDamage(float damage) {
        Debug.Log("player tomou dano!");
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
}
