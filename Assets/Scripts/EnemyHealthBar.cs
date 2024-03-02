using System.Collections;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour {
    private float maxSize, newBarSize, currentBarSize;
    private bool barIsLerping = false;
    private RectTransform rectTransform, bgRectTransform;
    private Quaternion originalRotation;

    public GameObject enemy;
    
    private void Start() {
        maxSize = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        newBarSize = maxSize;
        currentBarSize = maxSize;
        bgRectTransform = transform.parent.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        originalRotation = bgRectTransform.rotation;
    }

    private void Update() {
        bgRectTransform.rotation = new Quaternion(originalRotation.x, Camera.main.transform.rotation.y, originalRotation.z, Camera.main.transform.rotation.w);
    }

    public void updateHealth(float currentHealth, float maxHealth, float damage, bool enemyDead=false) {
        if (barIsLerping) {   //Se a vida estiver diminuindo quando o próximo tiro acertar
            StopAllCoroutines();
            barIsLerping = false;
            rectTransform.sizeDelta = new Vector2(newBarSize, rectTransform.sizeDelta.y);
        }
        currentBarSize = rectTransform.sizeDelta.x;
        if (currentHealth >= 0) {
            newBarSize = (currentHealth * maxSize) / maxHealth;
        }
        StartCoroutine(updateBar(currentBarSize, newBarSize, enemyDead));
    }

    private IEnumerator updateBar(float currentSize, float newSize, bool enemyDead=false) {
        float timePassed = 0f, lerpDuration = 0.5f;
        newSize = enemyDead ? 0 : newSize;
        barIsLerping = true;
        while (timePassed < lerpDuration) {
            float t = timePassed / lerpDuration;
            float lerpValue = Mathf.Lerp(currentSize, newSize, t);
            rectTransform.sizeDelta = new Vector2(lerpValue, rectTransform.sizeDelta.y);
            timePassed += Time.deltaTime;
            yield return null;
        }
        barIsLerping = false;

        if (enemyDead)
            enemy.GetComponent<Animator>().Play("die");
    }
}
