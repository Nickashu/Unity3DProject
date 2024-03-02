using UnityEngine;

public class Aim : MonoBehaviour {
    private Ray ray;
    private RaycastHit hit;
    private Color originalColor;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform playerTransform;
    private Player playerScript;

    private void Start() {
        playerScript = playerTransform.gameObject.GetComponent<Player>();
        originalColor = lineRenderer.material.color;
    }

    private void Update() {
        if (playerScript.showAiming) {   //Se o jogador estiver mirando
            Vector3 aimFinalPosition;
            ray = new Ray(transform.position, (playerTransform.rotation * Vector3.forward).normalized);
            if (Physics.Raycast(ray, out hit, 100f)) {
                aimFinalPosition = hit.point;
                //Debug.Log("ta mirando em algo!");
                //Debug.DrawLine(transform.position, hit.point);
                if (hit.collider.CompareTag("enemy"))
                    lineRenderer.material.color = Color.red;
                else
                    lineRenderer.material.color = originalColor;
            }
            else {
                //Debug.Log("nao ta mirando em nada");
                lineRenderer.material.color = originalColor;
                aimFinalPosition = playerTransform.forward.normalized * 100f;
            }
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, aimFinalPosition);
            lineRenderer.enabled = true;
        }
        else
            lineRenderer.enabled = false;
    }
}