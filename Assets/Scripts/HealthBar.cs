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
            newBarSize = (currentHealth * maxSize) / maxHealth;
            if (newBarSize >= maxHealth)
                newBarSize = maxHealth;
        }
        StartCoroutine(updateBar(currentBarSize, newBarSize, creatureDead));
    }

    private IEnumerator updateBar(float currentSize, float newSize, bool creatureDead = false) {
        float timePassed = 0f, lerpDuration = 0.5f;
        newSize = creatureDead ? 0 : newSize;
        barIsLerping = true;
        while (timePassed < lerpDuration) {
            float t = timePassed / lerpDuration;
            float lerpValue = Mathf.Lerp(currentSize, newSize, t);
            rectTransform.sizeDelta = new Vector2(lerpValue, rectTransform.sizeDelta.y);
            timePassed += Time.deltaTime;
            yield return null;
        }
        barIsLerping = false;

        if (creatureDead) {
            if (creature.CompareTag("Player")) {
                Debug.Log("jogador morreu!");
            }
            else
                creature.GetComponent<Animator>().Play("die");
        }
    }
}
