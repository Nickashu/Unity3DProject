using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    [SerializeField] private int maxNumEnemies;
    [SerializeField] private GameObject baseEnemy, canvasPause, canvasOptions, canvasControls, canvasUpgradesScreen, canvasUpgradePistolGroup, canvasUpgradeSMGGroup, canvasMisteryGunGroup, canvasDeathPlayer, imgMisteryGunUpgrade;
    [SerializeField] private GameObject[] objsLang;
    [SerializeField] private Slider OSTVolumeSlider, SFXVolumeSlider, sensitivitySlider;
    [SerializeField] private TMP_Dropdown dropdownLanguage;
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
        updateCoins();   //Atualizando o texto do n�mero de moedas
        updateLanguage(Globals.idLanguage);
        SoundController.GetInstance().LoadSounds();
        SoundController.GetInstance().ChangeVolumes(true);

        if (canvasOptions != null) {   //Se 1 slider estiver ativo, os outros tamb�m estar�o
            updateConfigs();
            OSTVolumeSlider.value = Globals.volumeOST;
            SFXVolumeSlider.value = Globals.volumeSFX;
            sensitivitySlider.onValueChanged.AddListener((newValue) => {
                camCinemachine.m_XAxis.m_MaxSpeed = Globals.camSensitivity;
            });
            OSTVolumeSlider.onValueChanged.AddListener((newValue) => {
                Debug.Log(newValue);
                Globals.volumeOST = newValue;
                SoundController.GetInstance().ChangeVolumes(false);
            });
            SFXVolumeSlider.onValueChanged.AddListener((newValue) => {
                Globals.volumeSFX = newValue;
                SoundController.GetInstance().ChangeVolumes(false);
            });
        }
        TransitionController.GetInstance().playSceneMusic();

        if (SceneManager.GetActiveScene().name.ToLower().Contains("main")) {
            isInGameScene = true;
            InvokeRepeating("spawnEnemy", 2f, 5f);
            numEnemiesSpawned = 0;
            numEnemiesDefeated = 0;
        }
        else if (SceneManager.GetActiveScene().name.ToLower().Contains("menu")) {
            isInMenuScene = true;
            manageUpgrades();
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
                Cursor.visible = gamePaused;
            }
            else {
                if (isInMenuScene) {
                    if (canvasOptions.activeSelf)
                        ExitOptions();
                    if (canvasControls.activeSelf)
                        ExitControls();
                    if (canvasUpgradesScreen.activeSelf)
                        ExitUpgrades();
                }
            }
        }

        if (isInGameScene) {
            camCinemachine.enabled = !gamePaused;
            Cursor.visible = gamePaused;
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
                if (chosenEnemyType != null) {   //Sempre vai entrar aqui pois o inimigo mais fraco ter� uma chance de 100% de spawnar
                    Vector3 spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length - 1)].position;
                    GameObject enemy = Instantiate(baseEnemy, spawnPoint, Quaternion.identity);
                    Enemy scriptEnemy = enemy.GetComponent<Enemy>();
                    int enemyType = chosenEnemyType.type;
                    scriptEnemy.enemyConfigs = chosenEnemyType;
                    scriptEnemy.enemyType = enemyType;
                    scriptEnemy.playerTransform = playerTransform;
                    SoundController.GetInstance().objectsSounds.Add(enemy);    //Adicionando o inimigo criado como mais uma fonte de �udio
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
        SoundController.GetInstance().PlaySound("explosion", particles.gameObject);
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

    //M�todos ativados com bot�es:
    public void StartGame() {
        SoundController.GetInstance().PlaySound("btn");
        TransitionController.GetInstance().LoadMainScene();
    }
    public void QuitGame() {
        SoundController.GetInstance().PlaySound("btn");
        StartCoroutine(delayQuit());
    }
    private IEnumerator delayQuit() {
        yield return new WaitForSeconds(0.2f);
        Application.Quit();
    }
    public void Options() {
        SoundController.GetInstance().PlaySound("btn");
        canvasOptions.SetActive(true);
    }
    public void ExitOptionsBtn() {
        SoundController.GetInstance().PlaySound("btnExit");
        ExitOptions();
    }
    public void ExitOptions() {
        canvasOptions.SetActive(false);
        Globals.SaveData();
        updateConfigs();
    }
    public void Controls() {
        SoundController.GetInstance().PlaySound("btn");
        canvasControls.SetActive(true);
    }
    public void ExitControlsBtn() {
        SoundController.GetInstance().PlaySound("btnExit");
        ExitControls();
    }
    public void ExitControls() {
        canvasControls.SetActive(false);
    }
    public void Upgrades() {
        SoundController.GetInstance().PlaySound("btn");
        canvasUpgradesScreen.SetActive(true);
    }
    public void ExitUpgradesBtn() {
        SoundController.GetInstance().PlaySound("btnExit");
        ExitUpgrades();
    }
    public void ExitUpgrades() {
        canvasUpgradesScreen.SetActive(false);
    }
    public void ResumeGame() {
        SoundController.GetInstance().PlaySound("btnExit");
        canvasPause.SetActive(false);
        canvasOptions.SetActive(false);
        gamePaused = false;
    }
    public void ReturnToMenu() {
        SoundController.GetInstance().PlaySound("btn");
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
                GameObject newSection = sectionUpgrade.transform.parent.GetChild(sectionUpgrade.transform.GetSiblingIndex() + 1).gameObject;    //Pegando a pr�xima se��o de upgrade
                enableSectionUpgrade(newSection);
            }
            Globals.numCoins -= requiredCoins;    //Diminuindo o n�mero de moedas ap�s a compra
            SoundController.GetInstance().PlaySound("btnPurchase");
            updateCoins();
        }
        else {
            SoundController.GetInstance().PlaySound("btnFailPurchase");
            //Debug.Log("Sem moedas o suficiente!!!");
        }
    }
    public void ChangeLanguage(int newIdLang) {
        updateLanguage(newIdLang);
    }
    public void ResetSave() {
        SoundController.GetInstance().PlaySound("btn");
        Globals.ResetData();
    }

    //M�todos que controlam interfaces do jogo:
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
        imgMisteryGunUpgrade.SetActive(Globals.hasMisteryGun);
        if (!Globals.hasMisteryGun)
            enableSectionUpgrade(misteryGunSection);
        else
            disableSectionUpgrade(misteryGunSection, true);
    }
    private void createSectionUpgrade(GameObject gunGroup, int levelCreating, int price, int playerLevel) {
        GameObject newSection = Instantiate(gunGroup.transform.GetChild(1).gameObject);
        newSection.transform.SetParent(gunGroup.transform);
        newSection.gameObject.name = levelCreating.ToString();
        Button btnUpgrade = newSection.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>();    //Recuperando o bot�o de upgrade (se a hierarquia mudar, o c�digo tamb�m dever� mudar)
        btnUpgrade.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Level " + levelCreating;
        TextMeshProUGUI txtPrice = newSection.transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>();   //Recuperando o texto do pre�o
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
        sensitivitySlider.value = Globals.camSensitivity;
        dropdownLanguage.value = Globals.idLanguage;
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
