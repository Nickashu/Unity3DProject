using System.Collections;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour {
    private float maxSize, newBarSize, currentBarSize;
    private bool barIsLerping = false;
    private RectTransform rectTransform, bgRectTransform;

    private void Start() {
        maxSize = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        newBarSize = maxSize;
        currentBarSize = maxSize;
        bgRectTransform = transform.parent.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        bgRectTransform.rotation = new Quaternion(bgRectTransform.rotation.x, Camera.main.transform.rotation.y, bgRectTransform.rotation.z, Camera.main.transform.rotation.w);
    }

    public void updateHealth(float currentHealth, float maxHealth, float damage) {
        if (barIsLerping) {   //Se a vida estiver diminuindo quando o próximo tiro acertar
            StopAllCoroutines();
            barIsLerping = false;
            rectTransform.sizeDelta = new Vector2(newBarSize, rectTransform.sizeDelta.y);
        }
        currentBarSize = rectTransform.sizeDelta.x;
        if (currentHealth >= 0) {
            newBarSize = (currentHealth * maxSize) / maxHealth;
        }
        StartCoroutine(updateBar(currentBarSize, newBarSize));
        Debug.Log("maxHealth: " + maxHealth + "    currentHealth:" + currentHealth + "   damage: " + damage + "    maxSize: " + maxSize);
    }


    private IEnumerator updateBar(float currentSize, float newSize) {
        float timePassed = 0f, lerpDuration = 0.5f;
        barIsLerping = true;
        while (timePassed < lerpDuration) {
            float t = timePassed / lerpDuration;
            float lerpValue = Mathf.Lerp(currentSize, newSize, t);
            rectTransform.sizeDelta = new Vector2(lerpValue, rectTransform.sizeDelta.y);
            timePassed += Time.deltaTime;
            yield return null;
        }
        barIsLerping = false;

        Debug.Log("Lerp concluído!");
    }
}
