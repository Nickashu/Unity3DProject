using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    [SerializeField] private int maxNumEnemies;
    [SerializeField] private GameObject baseEnemy, canvasPause, canvasOptions, canvasUpgradesScreen, canvasUpgradePistolGroup, canvasUpgradeSMGGroup, canvasMisteryGunGroup, canvasDeathPlayer;
    [SerializeField] private GameObject[] objsLang;
    [SerializeField] private ParticleSystem particlesDeathPlayer;
    private static GameController instance;
    private bool isInGameScene = false, isInMenuScene = false, playerDeadFlag=false;

    public Transform[] enemySpawnPoints;
    public Transform playerTransform;
    public TextMeshProUGUI txtCoins, txtNumEnemiesDefeated, txtNewRecord;
    public CinemachineFreeLook camCinemachine;
    [HideInInspector] public bool gamePaused = false, playerDead = false;
    [HideInInspector] public int numEnemiesSpawned, numEnemiesDefeated, coinsMultiplier = 1;

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
            numEnemiesSpawned = 0;
            numEnemiesDefeated = 0;
        }
        else if (SceneManager.GetActiveScene().name.ToLower().Contains("menu")) {
            isInMenuScene = true;
            manageUpgrades();
        }
        updateLanguage(Globals.idLanguage);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isInGameScene) {
                if (!playerDead) {
                    gamePaused = !gamePaused;
                    if (gamePaused)
                        canvasPause.SetActive(true);
                    else {
                        canvasPause.SetActive(false);
                        canvasOptions.SetActive(false);
                    }
                }
            }
            else {
                if (isInMenuScene) {
                    if (canvasOptions.activeSelf)
                        canvasOptions.SetActive(false);
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
                Vector3 spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length - 1)].position;
                GameObject enemy = Instantiate(baseEnemy, spawnPoint, Quaternion.identity);
                Enemy scriptEnemy = enemy.GetComponent<Enemy>();
                int enemyType = Random.Range(0, scriptEnemy.enemyConfigs.Length);
                scriptEnemy.enemyType = enemyType;
                scriptEnemy.playerTransform = playerTransform;
                enemy.SetActive(true);
                numEnemiesSpawned++;
            }
        }
    }

    public void updateCoins(int amount) {
        int currentAmount = int.Parse(txtCoins.text.Substring(1));
        int newAmount = currentAmount + amount;
        txtCoins.text = "X" + newAmount.ToString();
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
        canvasUpgradesScreen.SetActive(true);
    }
    public void ExitUpgrades() {
        canvasUpgradesScreen.SetActive(false);
    }
    public void ResumeGame() {
        canvasPause.SetActive(false);
        canvasOptions.SetActive(false);
    }
    public void ReturnToMenu() {
        Debug.Log("Voltando para o menu!!");
        TransitionController.GetInstance().LoadMenu();
    }
    public void BuyUpgrade(GameObject sectionUpgrade) {
        int level = 0, maxLevels=0;
        if (sectionUpgrade.CompareTag("pistolUpgrade")) {
            level = int.Parse(sectionUpgrade.name);
            maxLevels = Globals.upgradePricesPistol.Count;
            Globals.levelPistol++;
        }
        else if (sectionUpgrade.CompareTag("SMGUpgrade")) {
            level = int.Parse(sectionUpgrade.name);
            maxLevels = Globals.upgradePricesSMG.Count;
            Globals.levelSMG++;
        }
        else
            Globals.hasMisteryGun = true;

        disableSectionUpgrade(sectionUpgrade, true);
        if (level < maxLevels) {
            GameObject newSection = sectionUpgrade.transform.parent.GetChild(sectionUpgrade.transform.GetSiblingIndex() + 1).gameObject;    //Pegando a próxima seção de upgrade
            enableSectionUpgrade(newSection);
        }
    }
    public void ChangeLanguage(int newIdLang) {
        updateLanguage(newIdLang);
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
