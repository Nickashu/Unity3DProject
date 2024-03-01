using UnityEngine;

public class Aim : MonoBehaviour {
    private Ray ray;
    private Color originalColor;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Player playerScript;

    private void Start() {
        originalColor = lineRenderer.material.color;
    }

    private void Update() {
        if (playerScript.showAiming) {   //Se o jogador estiver mirando
            Vector3 aimFinalPosition;
            ray = new Ray(transform.position, transform.rotation * Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f)) {
                aimFinalPosition = hit.point;
                Debug.DrawLine(transform.position, hit.point);
                if (hit.collider.tag.Equals("enemy"))
                    lineRenderer.material.color = Color.red;
                else
                    lineRenderer.material.color = originalColor;
            }
            else {
                lineRenderer.material.color = originalColor;
                aimFinalPosition = transform.rotation * Vector3.forward * 100f;
            }
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, aimFinalPosition);
            lineRenderer.enabled = true;
        }
        else
            lineRenderer.enabled = false;
    }
}
