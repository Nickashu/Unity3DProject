using UnityEngine;

public class Bullet : MonoBehaviour {
    private Rigidbody rb;
    private bool collided = false;
    [HideInInspector]
    public float velocity, damage;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        Invoke("DestroyBullet", 2f);
    }

    private void FixedUpdate() {
        if (!collided) {
            if (rb.velocity.magnitude < BulletController.GetInstance().maxBulletVelocity) {
                Vector3 moveDirection = (transform.rotation * Vector3.forward).normalized;
                rb.AddForce(moveDirection * velocity, ForceMode.VelocityChange);
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision != null) {   //Se colidir com qualquer coisa
            collided = true;
            //Debug.Log("colidiu!");
            DestroyBullet();
        }
    }

    private void DestroyBullet() {
        Destroy(gameObject);
    }
}
