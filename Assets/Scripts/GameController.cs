using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [HideInInspector] public bool gamePaused = false;
    [SerializeField] private int maxNumEnemies;
    [HideInInspector] public int numEnemies;
    private bool isInGameScene = false;

    private static GameController instance;
    public Transform[] enemySpawnPoints;
    public Transform playerTransform;
    public GameObject baseEnemy;

    public static GameController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        if (SceneManager.GetActiveScene().name.ToLower().Contains("main")) {
            isInGameScene = true;
            //Cursor.visible = false;   debug
            InvokeRepeating("spawnEnemy", 2f, 5f);
            numEnemies = 0;
        }
    }

    private void Update() {
        if (isInGameScene)
            if (Input.GetKeyDown(KeyCode.Escape)) {
                gamePaused = !gamePaused;
        }
    }

    private void spawnEnemy() {
        if(numEnemies < maxNumEnemies) {
            Vector3 spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length - 1)].position;
            GameObject enemy = Instantiate(baseEnemy, spawnPoint, Quaternion.identity);
            Enemy scriptEnemy = enemy.GetComponent<Enemy>();
            int enemyType = Random.Range(0, scriptEnemy.enemyConfigs.Length);
            scriptEnemy.enemyType = enemyType;
            scriptEnemy.playerTransform = playerTransform;
            enemy.SetActive(true);
            Debug.Log("spawnou inimigo " + enemyType);
            numEnemies++;
        }
    }


    //Métodos ativados com botões:
    public void StartGame() {
        TransitionController.GetInstance().LoadMainScene();
    }
    public void QuitGame() {
        Application.Quit();
    }
    public void Options() {
        Debug.Log("Menu de opções!");
    }

}
