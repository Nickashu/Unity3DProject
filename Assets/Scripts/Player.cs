using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private float vertical, horizontal, turnSmoothVelocity;
    private Rigidbody rb;
    [SerializeField] 
    private float jumpPower, movementSpeed, jumpSmoothness, turnSmoothTime;

    public Transform groundCheck, cam;
    public LayerMask groundLayer;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Debug.Log(rb.velocity.y);
        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;   //Pegando o ângulo da movimentação em graus (levando em consideração a câmera)
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);    //Fazendo a rotação acontecer de forma suave
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = (Quaternion.Euler(0f, angle, 0f) * Vector3.forward).normalized;
            //characterController.Move(moveDirection * movementSpeed * Time.deltaTime);   //Usando o characterController para mover o personagem de acordo com a posição da câmera
            rb.AddForce(moveDirection * movementSpeed);    //Para usar o AddForce, lembrar de adicionar um drag no inspetor para limitar a velocidade!!!!
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

    public void SetMovement(InputAction.CallbackContext value) {
        //if (!GameController.GetInstance().gamePaused()) {
            horizontal = value.ReadValue<Vector3>().x;
            vertical = value.ReadValue<Vector3>().z;
            if (value.canceled) {    //Se o evento for cancelado
                horizontal = 0;
                vertical = 0;
            }
        //}
    }
    public void SetJump(InputAction.CallbackContext value) {
        if (value.performed) {
            if (isOnGround()) {
                //Debug.Log("pulou!");
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                //rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
            }
        }
        if (value.canceled) {    //Se o evento for cancelado
            if (rb.velocity.y > 0.1f) {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * jumpSmoothness, rb.velocity.z);
            }
        }
    }

    private bool isOnGround() {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer);
        return colliders.Length > 0;
    }
}
