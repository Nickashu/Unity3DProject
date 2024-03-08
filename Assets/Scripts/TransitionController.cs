using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour {
    public GameObject bgTransitions;

    private static TransitionController instance;
    private Animator animTransitionScenes;
    private float transistionTimeScenes = 2f;

    public static TransitionController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        animTransitionScenes = bgTransitions.GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name.ToLower().Contains("menu")) {
            if (!Globals.firstScene)
                animTransitionScenes.SetBool("fadeOut", true);
        }
        else
            animTransitionScenes.SetTrigger("fadeOut");

        if (Globals.firstScene) {   //Se o jogo tiver acabado de abrir
            Globals.LoadData();
            Globals.firstScene = false;
        }
        else
            Globals.SaveData();
    }

    public void playSceneMusic() {
        if (SceneManager.GetActiveScene().name.ToLower().Contains("menu"))
            SoundController.GetInstance().PlaySound("OST_menu", null);
        else
            SoundController.GetInstance().PlaySound("OST_level", null);
    }

    public void LoadNextScene() {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1) {
            StartCoroutine(LoadScene(0));   //Carregando a primeira cena novamente (menu)
        }
        else
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadMainScene() {
        StartCoroutine(LoadScene(1));   //Carregando a cena principal
    }
    public void LoadMenu() {
        StartCoroutine(LoadScene(0));   //Carregando o menu
    }

    private IEnumerator LoadScene(int sceneIndex) {
        animTransitionScenes.SetTrigger("fadeIn");
        yield return new WaitForSeconds(transistionTimeScenes);
        SceneManager.LoadScene(sceneIndex);
    }
}
