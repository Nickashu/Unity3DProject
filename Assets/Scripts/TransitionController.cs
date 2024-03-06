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

    private void Start() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        animTransitionScenes = bgTransitions.GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name.ToLower().Contains("menu")) {
            //if (SoundController.GetInstance().numTimesMenu == 0) {
            //    SoundController.GetInstance().ChangeVolumes();
            //    SoundController.GetInstance().numTimesMenu = 1;
            //}
            //else
            //    animTransitionScenes.SetBool("fadeOut", true);
            Debug.Log("Menu!");
        }
        else
            animTransitionScenes.SetTrigger("fadeOut");

        //playSceneMusic();
    }

    //private void playSceneMusic() {
    //    if (SceneManager.GetActiveScene().name.Contains("Menu"))
    //        SoundController.GetInstance().PlaySound("OST_menu", null);
    //    else
    //        SoundController.GetInstance().PlaySound("OST_level", null);
    //}

    public void LoadNextScene() {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1) {
            Debug.Log("Jogo terminado!");
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
