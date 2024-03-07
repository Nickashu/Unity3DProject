using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    [SerializeField] private int maxNumEnemies;
    [SerializeField] private GameObject baseEnemy, canvasPause, canvasOptions, canvasControls, canvasUpgradesScreen, canvasUpgradePistolGroup, canvasUpgradeSMGGroup, canvasMisteryGunGroup, canvasDeathPlayer;
    [SerializeField] private GameObject[] objsLang;
    [SerializeField] private Slider OSTVolumeSlider, SFXVolumeSlider, sensitivitySlider;
    [SerializeField] private ParticleSystem particlesDeathPlayer;
    [SerializeField] private EnemyObject[] enemiesConfig;
    private static GameController instance;
    private bool isInGameScene = false, isInMenuScene = false, playerDeadFlag=false;

    public Transform[] enemySpawnPoints;
    public Transform playerTransform;
    public TextMeshProUGUI txtCoins, txtNumEnemiesDefeated, txtNewRecord;
    public CinemachineFreeLook camCinemachine;
    public int baseCoinsMultiplier;
    public float powerUpTime;
    [HideInInspector] public bool gamePaused = false, playerDead = false;
    [HideInInspector] public int numEnemiesSpawned, numEnemiesDefeated, coinsMultiplier;



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
        coinsMultiplier = baseCoinsMultiplier;
        if (SceneManager.GetActiveScene().name.ToLower().Contains("main")) {
            isInGameScene = true;
            //Cursor.visible = false;   debug
            InvokeRepeating("spawnEnemy", 2f, 5f);
            numEnemiesSpawned = 0;
            numEnemiesDefeated = 0;
        }
        else if (SceneManager.GetActiveScene().name.ToLower().Contains("menu")) {
            isInMenuScene = true;
            manageUpgrades();
        }
        updateCoins();   //Atualizando o texto do número de moedas
        updateLanguage(Globals.idLanguage);
        updateConfigs();

        if (sensitivitySlider != null) {   //Se 1 slider estiver ativo, os outros também estarão
            sensitivitySlider.value = Globals.camSensitivity;
            OSTVolumeSlider.value = Globals.volumeOST;
            SFXVolumeSlider.value = Globals.volumeSFX;
            sensitivitySlider.onValueChanged.AddListener((newValue) => {
                Globals.camSensitivity = (int)newValue;
            });
            OSTVolumeSlider.onValueChanged.AddListener((newValue) => {
                Globals.volumeOST = newValue;
            });
            OSTVolumeSlider.onValueChanged.AddListener((newValue) => {
                Globals.volumeSFX = newValue;
            });
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isInGameScene) {
                if (!playerDead) {
                    gamePaused = !gamePaused;
                    if (gamePaused)
                        canvasPause.SetActive(true);
                    else {
                        ExitOptions();
                        ExitControls();
                        canvasPause.SetActive(false);
                    }
                }
            }
            else {
                if (isInMenuScene) {
                    if (canvasOptions.activeSelf)
                        canvasOptions.SetActive(false);
                    if (canvasControls.activeSelf)
                        canvasControls.SetActive(false);
                    if (canvasUpgradesScreen.activeSelf)
                        canvasUpgradesScreen.SetActive(false);
                }
            }
        }

        if (isInGameScene) {
            if (gamePaused)
                camCinemachine.enabled = false;
            else
                camCinemachine.enabled = true;
        }

        if (playerDead) {
            if (!playerDeadFlag) {
                playerDeadFlag = true;
                playerDeath();
            }
        }
    }

    private void spawnEnemy() {
        if (!gamePaused) {
            if (numEnemiesSpawned < maxNumEnemies) {
                EnemyObject chosenEnemyType = null;
                int rand = UnityEngine.Random.Range(1, 101);
                int minRarity = 101;
                foreach (EnemyObject enemyObject in enemiesConfig) {
                    if (rand <= enemyObject.rarity) {
                        if (minRarity > enemyObject.rarity) {
                            minRarity = enemyObject.rarity;
                            chosenEnemyType = enemyObject;
                        }
                    }
                }
                if (chosenEnemyType != null) {   //Sempre vai entrar aqui pois o inimigo mais fraco terá uma chance de 100% de spawnar
                    Vector3 spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length - 1)].position;
                    GameObject enemy = Instantiate(baseEnemy, spawnPoint, Quaternion.identity);
                    Enemy scriptEnemy = enemy.GetComponent<Enemy>();
                    int enemyType = chosenEnemyType.type;
                    scriptEnemy.enemyConfigs = chosenEnemyType;
                    scriptEnemy.enemyType = enemyType;
                    scriptEnemy.playerTransform = playerTransform;
                    enemy.SetActive(true);
                    numEnemiesSpawned++;
                }


            }
        }
    }

    public void updateCoins() {
        if(txtCoins != null)
            txtCoins.text = "X" + Globals.numCoins.ToString();
    }

    private void playerDeath() {
        playerTransform.gameObject.SetActive(false);
        gamePaused = true;
        ParticleSystem particles = Instantiate(particlesDeathPlayer, playerTransform.position, Quaternion.identity);
        ParticleSystem.MainModule particlesMain = particles.main;
        Color colorParticles = playerTransform.GetComponent<MeshRenderer>().material.color;
        colorParticles.a = 1f;
        particlesMain.startColor = colorParticles;
        particles.gameObject.SetActive(true);
        particles.Play();
        StartCoroutine(delayDeathPlayer());
    }

    public IEnumerator delayDeathPlayer() {
        yield return new WaitForSeconds(2);
        showPanelGameOver();
    }

    private void showPanelGameOver() {
        txtNumEnemiesDefeated.text = numEnemiesDefeated.ToString();
        if (numEnemiesDefeated > Globals.recordEnemiesDefeated) {
            Globals.recordEnemiesDefeated = numEnemiesDefeated;
            txtNewRecord.gameObject.SetActive(true);
        }
        canvasDeathPlayer.SetActive(true);
        Globals.SaveData();
    }

    private void OnApplicationQuit() {
        Globals.SaveData();
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
        Globals.SaveData();
        updateConfigs();
    }
    public void Controls() {
        canvasControls.SetActive(true);
    }
    public void ExitControls() {
        canvasControls.SetActive(false);
    }
    public void Upgrades() {
        canvasUpgradesScreen.SetActive(true);
    }
    public void ExitUpgrades() {
        canvasUpgradesScreen.SetActive(false);
    }
    public void ResumeGame() {
        canvasPause.SetActive(false);
        canvasOptions.SetActive(false);
        gamePaused = false;
    }
    public void ReturnToMenu() {
        TransitionController.GetInstance().LoadMenu();
    }
    public void BuyUpgrade(GameObject sectionUpgrade) {
        int level = 0, maxLevels=0;
        int requiredCoins = int.Parse(sectionUpgrade.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text.Substring(1));
        bool hasCoins = Globals.numCoins >= requiredCoins ? true : false;

        if (hasCoins) {    //Se tiver moedas o suficiente
            if (sectionUpgrade.CompareTag("pistolUpgrade")) {
                maxLevels = Globals.upgradePricesPistol.Count;
                level = int.Parse(sectionUpgrade.name);
                Globals.levelPistol++;
            }
            else if (sectionUpgrade.CompareTag("SMGUpgrade")) {
                maxLevels = Globals.upgradePricesSMG.Count;
                level = int.Parse(sectionUpgrade.name);
                Globals.levelSMG++;
            }
            else
                Globals.hasMisteryGun = true;

            disableSectionUpgrade(sectionUpgrade, true);
            if (level < maxLevels) {
                GameObject newSection = sectionUpgrade.transform.parent.GetChild(sectionUpgrade.transform.GetSiblingIndex() + 1).gameObject;    //Pegando a próxima seção de upgrade
                enableSectionUpgrade(newSection);
            }
            Globals.numCoins -= requiredCoins;    //Diminuindo o número de moedas após a compra
            updateCoins();
        }
        else
            Debug.Log("Sem moedas o suficiente!!!");
    }
    public void ChangeLanguage(int newIdLang) {
        updateLanguage(newIdLang);
    }
    public void ResetSave() {
        Globals.ResetData();
    }

    //Métodos que controlam interfaces do jogo:
    private void manageUpgrades() {
        for (int i = 0; i < Globals.upgradePricesPistol.Count; i++) {   //Upgrades da pistola
            createSectionUpgrade(canvasUpgradePistolGroup, i + 1, Globals.upgradePricesPistol[i], Globals.levelPistol);
        }
        for (int i = 0; i < Globals.upgradePricesSMG.Count; i++) {    //Upgrades da SMG
            createSectionUpgrade(canvasUpgradeSMGGroup, i + 1, Globals.upgradePricesSMG[i], Globals.levelSMG);
        }
        GameObject misteryGunSection = canvasMisteryGunGroup.transform.GetChild(1).gameObject;
        TextMeshProUGUI txtPriceMisteryGun = misteryGunSection.transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        txtPriceMisteryGun.text = "X" + Globals.priceMisteryGun;
        if (!Globals.hasMisteryGun)
            enableSectionUpgrade(misteryGunSection);
        else
            disableSectionUpgrade(misteryGunSection, true);
    }
    private void createSectionUpgrade(GameObject gunGroup, int levelCreating, int price, int playerLevel) {
        GameObject newSection = Instantiate(gunGroup.transform.GetChild(1).gameObject);
        newSection.transform.SetParent(gunGroup.transform);
        newSection.gameObject.name = levelCreating.ToString();
        Button btnUpgrade = newSection.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>();    //Recuperando o botão de upgrade (se a hierarquia mudar, o código também deverá mudar)
        btnUpgrade.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level " + levelCreating;
        TextMeshProUGUI txtPrice = newSection.transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>();   //Recuperando o texto do preço
        txtPrice.text = "X" + price;
        newSection.GetComponent<RectTransform>().localScale = canvasUpgradeSMGGroup.transform.GetChild(1).GetComponent<RectTransform>().localScale;
        if (playerLevel == levelCreating-1)
            enableSectionUpgrade(newSection);
        else {
            bool check = playerLevel > (levelCreating-1) ? true : false;
            disableSectionUpgrade(newSection, check);
        }
        newSection.SetActive(true);
    }

    private void enableSectionUpgrade(GameObject sectionUpgrade) {
        Button btnUpgrade = sectionUpgrade.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        btnUpgrade.interactable = true;
    }
    private void disableSectionUpgrade(GameObject sectionUpgrade, bool check) {
        Button btnUpgrade = sectionUpgrade.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        btnUpgrade.interactable = false;
        if (check) {
            Color newColorCheck = btnUpgrade.transform.parent.transform.GetChild(0).GetComponent<Image>().color;
            newColorCheck.a = 1;
            btnUpgrade.transform.parent.transform.GetChild(0).GetComponent<Image>().color = newColorCheck;
        }
    }

    private void updateConfigs() {
        if (camCinemachine != null)
            camCinemachine.m_XAxis.m_MaxSpeed = Globals.camSensitivity;
        SoundController.GetInstance().ChangeVolumes();
    }

    private void updateLanguage(int idLang) {
        Globals.idLanguage = idLang;
        foreach (GameObject objLang in objsLang) {
            if (objLang.GetComponent<TextMeshProUGUI>() != null) {
                TextMeshProUGUI txtObj = objLang.GetComponent<TextMeshProUGUI>();
                txtObj.text = Globals.dictLanguage[objLang.name][idLang];
            }
            else {
                if (objLang.GetComponent<TMP_Dropdown>() != null) {
                    TMP_Dropdown dropdown = objLang.GetComponent<TMP_Dropdown>();
                    if (objLang.name == "dropdownLanguage") {
                        for (int i = 0; i < dropdown.options.Count; i++) {
                            if (i == (int)Globals.languages.english)
                                dropdown.options[i].text = Globals.dictLanguage["langEnglish"][idLang];
                            else if (i == (int)Globals.languages.portuguese)
                                dropdown.options[i].text = Globals.dictLanguage["langPortuguese"][idLang];
                        }
                        dropdown.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = dropdown.options[dropdown.value].text;
                    }
                }
            }
        }
    }
}
