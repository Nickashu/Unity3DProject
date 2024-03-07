using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour {
    private bool lifeOver = false, canFade = false, fadeOut=false;
    [HideInInspector] public int type;
    [SerializeField] private float fadeTime, lifeTime;
    private Color originalColor;

    private void Start() {
        StartCoroutine(timeToFade());
        StartCoroutine(timeToDestroy());
        Color initialColor = new Color(GetComponent<MeshRenderer>().material.color.r, GetComponent<MeshRenderer>().material.color.g, GetComponent<MeshRenderer>().material.color.b, 1f);
        GetComponent<MeshRenderer>().material.color = initialColor;
        originalColor = GetComponent<MeshRenderer>().material.color;
    }

    private void Update() {
        if (canFade) {
            canFade = false;
            StartCoroutine(fade(fadeOut));
        }
    }

    private IEnumerator timeToFade() {
        yield return new WaitForSeconds(fadeTime);
        canFade = true;
        fadeOut = true;
    }
    private IEnumerator timeToDestroy() {
        yield return new WaitForSeconds(lifeTime);
        lifeOver = true;
    }

    private IEnumerator fade(bool fadeOutArg) {
        float timePassed = 0f, fadeDuration=1f;
        while (timePassed < fadeDuration) {
            float t = timePassed / fadeDuration, fadeValue=0;
            Color colorPowerUp = gameObject.GetComponent<MeshRenderer>().material.color;
            if(fadeOutArg)
                fadeValue = Mathf.Lerp(colorPowerUp.a, 0, t);
            else
                fadeValue = Mathf.Lerp(0, originalColor.a, t);

            Color newColor = new Color(colorPowerUp.r, colorPowerUp.g, colorPowerUp.b, fadeValue);
            gameObject.GetComponent<MeshRenderer>().material.color = newColor;
            timePassed += Time.deltaTime;
            yield return null;
        }
        if (lifeOver) {
            if(fadeOut)
                Destroy(transform.parent.gameObject);
        }
        fadeOut = !fadeOut;
        canFade = true;
    }
}
