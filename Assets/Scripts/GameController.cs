using UnityEngine;

public class GameController : MonoBehaviour {

    private static GameController instance;
    [HideInInspector]
    public bool gamePaused = false;

    public static GameController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            gamePaused = !gamePaused;
        }
    }

}
