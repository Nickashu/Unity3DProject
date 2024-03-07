using System.Collections;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    private float maxSize, newBarSize, currentBarSize;
    private bool barIsLerping = false;
    private RectTransform rectTransform, bgRectTransform;
    private Quaternion originalRotation;
    public GameObject creature;

    private void Start() {
        maxSize = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        newBarSize = maxSize;
        currentBarSize = maxSize;
        bgRectTransform = transform.parent.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        originalRotation = bgRectTransform.rotation;
    }

    private void Update() {
        if (!gameObject.CompareTag("playerHealth"))    //Se for a vida do inimigo
            bgRectTransform.rotation = new Quaternion(originalRotation.x, Camera.main.transform.rotation.y, originalRotation.z, Camera.main.transform.rotation.w);
    }

    public void updateHealth(float currentHealth, float maxHealth, bool creatureDead = false) {
        if (barIsLerping) {   //Se a vida estiver diminuindo quando o próximo tiro acertar
            StopAllCoroutines();
            barIsLerping = false;
            rectTransform.sizeDelta = new Vector2(newBarSize, rectTransform.sizeDelta.y);
        }
        currentBarSize = rectTransform.sizeDelta.x;
        if (currentHealth >= 0) {
            if (currentHealth >= maxHealth)
                currentHealth = maxHealth;
            newBarSize = (currentHealth * maxSize) / maxHealth;
        }
        StartCoroutine(updateBar(currentBarSize, newBarSize, creatureDead));
    }

    private IEnumerator updateBar(float currentSize, float newSize, bool creatureDead = false) {
        if (creatureDead) {
            Debug.Log("ataulizando vida na morte!");
        }
        float timePassed = 0f;
        float lerpDuration = creature.CompareTag("Player") ? 0.15f : 0.3f;
        newSize = creatureDead ? 0 : newSize;
        barIsLerping = true;
        if (newSize <= 0.1)
            newSize = 0;
        while (timePassed < lerpDuration) {
            float t = timePassed / lerpDuration;
            float lerpValue = Mathf.Lerp(currentSize, newSize, t);
            rectTransform.sizeDelta = new Vector2(lerpValue, rectTransform.sizeDelta.y);
            timePassed += Time.deltaTime;
            yield return null;
        }
        barIsLerping = false;
    }
}
