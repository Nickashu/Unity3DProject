using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Globals : MonoBehaviour {
    public static int idLanguage = 0, numCoins=0, levelPistol=0, levelSMG=0, recordEnemiesDefeated=0, camSensitivity=100;
    public static float volumeOST = 1, volumeSFX = 1, pistolDamageTax=1, SMGDamageTax=1;
    public static bool hasMisteryGun = false;

    //Essas informações não serão salvas e só servirão para definir certas coisas no jogo:
    public static bool firstScene = true;
    public enum typesOfGuns {
        pistol,
        submachine,
        misteryGun,
        enemyGun,
    }
    public enum typesOfPowerUps {
        life,
        timesTwo,
        velocity,
    }
    public enum typesOfEnemies {
        weak,
        regular,
        strong,
    }
    public enum languages {
        english,
        portuguese,
    }

    public static Dictionary<int, int> upgradePricesPistol = new Dictionary<int, int>() {
        {0, 10}, {1, 30}, {2, 50}
    };
    public static Dictionary<int, int> upgradePricesSMG = new Dictionary<int, int>() {
        {0, 10}, {1, 30}, {2, 50}
    };
    public static int priceMisteryGun = 500;

    public static Dictionary<int, float> levelsDamageTax = new Dictionary<int, float>() {    //Aqui estão as taxas de aumento de poder de cada upgrade
        {0, 1f}, {1, 1.2f}, {2, 1.5f}, {3, 2f}
    };

    public static Dictionary<int, float> shotCoolDownGuns = new Dictionary<int, float>() {    //Este dicionário guardará o tempo de cool down entre os tiros de cada tipo de arma
        {(int)typesOfGuns.pistol, 1f },
        {(int)typesOfGuns.submachine, 0.2f },
        {(int)typesOfGuns.misteryGun, 0.5f },
    };

    public static Dictionary<int, float> baseDamageGuns = new Dictionary<int, float>() {    //Este dicionário guardará o dano de cada tipo de arma
        {(int)typesOfGuns.pistol, 5f },
        {(int)typesOfGuns.submachine, 1f },
        {(int)typesOfGuns.misteryGun, 100f }
    };

    
    public static Dictionary<string, string[]> dictLanguage = new Dictionary<string, string[]> {
        {"txtStart", new string[] {"Start", "Começar"} },
        {"txtResume", new string[] {"Resume", "Continuar"} },
        {"txtQuit", new string[] {"Quit", "Sair"} },
        {"txtReset", new string[] {"Reset", "Recomeçar"} },
        {"txtOptions", new string[] {"Options", "Opções"} },
        {"txtControls", new string[] {"Controls", "Controles" } },
        {"txtLang", new string[] {"Language", "Idioma" } },
        {"txtOST", new string[] {"Music", "Música" } },
        {"txtSFX", new string[] {"Sound Effects", "Efeitos Sonoros" } },
        {"txtNewRecord", new string[] {"New Record!!", "Novo Recorde!!" } },
        {"txtEnemies", new string[] {"Enemies defeated:", "Inimigos derrotados:" } },
        {"txtPistol", new string[] {"Pistol", "Pistola" } },
        {"txtSMG", new string[] {"SMG", "Metralhadora" } },
        {"txtMisteryGun", new string[] {"Mistery Gun", "Arma Misteriosa" } },
        {"txtSensitivity", new string[] {"Cam Sensitivity", "Sensibilidade da Câmera" } },
        {"txtResetSave", new string[] {"Reset Data", "Resetar Dados" } },
        {"langEnglish", new string[] {"English", "Inglês" } },
        {"langPortuguese", new string[] {"Portuguese", "Português" } },
        {"txtControlJump", new string[] {"Jump", "Pular" } },
        {"txtControlRun", new string[] {"Run", "Correr" } },
        {"txtControlWalk", new string[] {"Walk", "Andar" } },
        {"txtControlShoot", new string[] {"Shoot", "Atirar" } },
        {"txtControlAim", new string[] {"Aim", "Mirar" } },
        {"txtControlPause", new string[] {"Pause", "Pausar" } },
        {"txtControlChangeWeapon", new string[] {"Change weapon", "Trocar arma" } },
        {"txtUpgradeVelocity", new string[] {"Speed boost", "Aumento de velocidade" } },
        {"txtUpgradeTimesTwo", new string[] {"Double coins", "Moedas em dobro" } },
    };

    //Métodos para salvar/carregar dados
    public static void SaveData() {
        string path = Application.persistentDataPath + "/globals.bin";
        ConfigsData data = new ConfigsData(idLanguage, numCoins, levelPistol, levelSMG, recordEnemiesDefeated, camSensitivity, pistolDamageTax, SMGDamageTax, volumeOST, volumeSFX, hasMisteryGun);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        formatter.Serialize(fileStream, data);
        fileStream.Close();
        Debug.Log("Dados salvos com sucesso!");
    }

    public static void LoadData() {
        string path = Application.persistentDataPath + "/globals.bin";
        if (File.Exists(path)) {   //Se o arquivo existir
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            ConfigsData data = formatter.Deserialize(fileStream) as ConfigsData;
            fileStream.Close();

            idLanguage = data.idLanguage;
            numCoins = data.numCoins;
            levelPistol = data.levelPistol;
            levelSMG = data.levelSMG;
            recordEnemiesDefeated = data.recordEnemiesDefeated;
            pistolDamageTax = data.pistolDamageTax;
            SMGDamageTax = data.SMGDamageTax;
            volumeOST = data.volumeOST;
            volumeSFX = data.volumeSFX;
            hasMisteryGun = data.hasMisteryGun;

            Debug.Log("Dados carregados com sucesso!");
        }
    }

    public static void ResetData() {
        idLanguage = 0;
        numCoins = 0;
        levelPistol = 0;
        levelSMG = 0;
        recordEnemiesDefeated = 0;
        pistolDamageTax = 1;
        SMGDamageTax = 1;
        hasMisteryGun = false;

        SaveData();
        LoadData();
        Debug.Log("Dados zerados com sucesso!");
        TransitionController.GetInstance().LoadMenu();
    }
}

[Serializable]
public class ConfigsData {
    public int idLanguage, numCoins, levelPistol, levelSMG, recordEnemiesDefeated, camSensitivity;
    public float volumeOST, volumeSFX, pistolDamageTax, SMGDamageTax;
    public bool hasMisteryGun;

    public ConfigsData(int idLanguage, int numCoins, int levelPistol, int levelSMG, int recordEnemiesDefeated, int camSensitivity, float pistolDamageTax, float SMGDamageTax, float volumeOST, float volumeSFX, bool hasMisteryGun) {
        this.idLanguage = idLanguage;
        this.numCoins = numCoins;
        this.levelPistol = levelPistol;
        this.levelSMG = levelSMG;
        this.recordEnemiesDefeated = recordEnemiesDefeated;
        this.pistolDamageTax = pistolDamageTax;
        this.SMGDamageTax = SMGDamageTax;
        this.volumeOST = volumeOST;
        this.volumeSFX = volumeSFX;
        this.hasMisteryGun = hasMisteryGun;
        this.camSensitivity = camSensitivity;
    }
}
