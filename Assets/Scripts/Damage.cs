using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour {
    public void takeDamage(float damage, ref bool isLerpingDamage, float currentHealth, float originalHealth, HealthBar scriptHealthBar, MeshRenderer mesh, Color originalColor) {
        if (isLerpingDamage) {
            isLerpingDamage = false;
            mesh.material.color = originalColor;
        }
        currentHealth -= damage;
        bool dead = currentHealth <= 0 ? true : false;
        scriptHealthBar.updateHealth(currentHealth, originalHealth, dead);
        StartCoroutine(blinkDamage(isLerpingDamage, mesh, originalColor));
    }

    public IEnumerator blinkDamage(bool isLerpingDamage, MeshRenderer meshRenderer, Color originalColor) {
        isLerpingDamage = true;
        float timePassed = 0f, lerpDuration = 0.1f;
        while (timePassed < lerpDuration) {
            meshRenderer.material.color = Color.Lerp(originalColor, Color.white, lerpDuration);
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
}
