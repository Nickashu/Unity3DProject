using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [HideInInspector] public bool gamePaused = false;
    [SerializeField] private int maxNumEnemies;
    [HideInInspector] public int numEnemies, coinsMultiplier = 1;
    private bool isInGameScene = false;

    private static GameController instance;
    public Transform[] enemySpawnPoints;
    public Transform playerTransform;
    public GameObject baseEnemy, canvasPause, canvasOptions, canvasUpgrades;
    public TextMeshProUGUI txtCoins;
    public CinemachineFreeLook camCinemachine;

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
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isInGameScene) {
                gamePaused = !gamePaused;
                if (gamePaused) {
                    canvasPause.SetActive(true);
                    camCinemachine.enabled = false;
                }
                else {
                    canvasPause.SetActive(false);
                    canvasOptions.SetActive(false);
                    camCinemachine.enabled = true;
                }
            }
            else {
                if (SceneManager.GetActiveScene().name.ToLower().Contains("menu")) {
                    if (canvasOptions.activeSelf)
                        canvasOptions.SetActive(false);
                    if (canvasUpgrades.activeSelf)
                        canvasUpgrades.SetActive(false);
                }
            }
        }
    }

    private void spawnEnemy() {
        if (!gamePaused) {
            if (numEnemies < maxNumEnemies) {
                Vector3 spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length - 1)].position;
                GameObject enemy = Instantiate(baseEnemy, spawnPoint, Quaternion.identity);
                Enemy scriptEnemy = enemy.GetComponent<Enemy>();
                int enemyType = Random.Range(0, scriptEnemy.enemyConfigs.Length);
                scriptEnemy.enemyType = enemyType;
                scriptEnemy.playerTransform = playerTransform;
                enemy.SetActive(true);
                numEnemies++;
            }
        }
    }

    public void updateCoins(int amount) {
        int currentAmount = int.Parse(txtCoins.text.Substring(1));
        int newAmount = currentAmount + amount * coinsMultiplier;
        txtCoins.text = "X" + newAmount.ToString();
    }


    //Métodos ativados com botões:
    public void StartGame() {
        TransitionController.GetInstance().LoadMainScene();
    }
    public void QuitGame() {
        Application.Quit();
    }
    public void Options() {
        canvasOptions.SetActive(true);
    }
    public void ExitOptions() {
        canvasOptions.SetActive(false);
    }
    public void Upgrades() {
        canvasUpgrades.SetActive(true);
    }
    public void ExitUpgrades() {
        canvasUpgrades.SetActive(false);
    }
    public void ResumeGame() {
        canvasPause.SetActive(false);
        canvasOptions.SetActive(false);
    }
    public void ReturnToMenu() {
        Debug.Log("Voltando para o menu!!");
        TransitionController.GetInstance().LoadMenu();
    }
}
