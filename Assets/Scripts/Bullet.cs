using UnityEngine;

public class Bullet : MonoBehaviour {
    private Rigidbody rb;
    [HideInInspector]
    public float velocity;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        Invoke("DestroyBullet", 2f);
    }
    private void Update() {
        Debug.Log(transform.rotation.x + " " + transform.rotation.y + " " + transform.rotation.z);
        if(rb.velocity.magnitude < BulletController.GetInstance().maxBulletVelocity) {
            rb.AddForce(Vector3.forward * velocity);
        }
    }

    private void DestroyBullet() {
        Destroy(gameObject);
    }
}
