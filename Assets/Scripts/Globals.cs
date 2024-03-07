using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Globals : MonoBehaviour {
    public static int idLanguage = 0, numCoins=1000, levelPistol=0, levelSMG=0, recordEnemiesDefeated=0, camSensitivity=100;
    public static float volumeOST = 1, volumeSFX = 1, pistolDamageTax=1, SMGDamageTax=1;
    public static bool hasMisteryGun = false;

    //Essas informa��es n�o ser�o salvas e s� servir�o para definir certas coisas no jogo:
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

    public static Dictionary<int, float> levelsDamageTax = new Dictionary<int, float>() {    //Aqui est�o as taxas de aumento de poder de cada upgrade
        {0, 1f}, {1, 1.2f}, {2, 1.5f}, {3, 2f}
    };

    public static Dictionary<int, float> shotCoolDownGuns = new Dictionary<int, float>() {    //Este dicion�rio guardar� o tempo de cool down entre os tiros de cada tipo de arma
        {(int)typesOfGuns.pistol, 1f },
        {(int)typesOfGuns.submachine, 0.2f },
        {(int)typesOfGuns.misteryGun, 0.5f },
    };

    public static Dictionary<int, float> baseDamageGuns = new Dictionary<int, float>() {    //Este dicion�rio guardar� o dano de cada tipo de arma
        {(int)typesOfGuns.pistol, 5f },
        {(int)typesOfGuns.submachine, 1f },
        {(int)typesOfGuns.misteryGun, 100f }
    };

    
    public static Dictionary<string, string[]> dictLanguage = new Dictionary<string, string[]> {
        {"txtStart", new string[] {"Start", "Come�ar"} },
        {"txtResume", new string[] {"Resume", "Continuar"} },
        {"txtQuit", new string[] {"Quit", "Sair"} },
        {"txtReset", new string[] {"Reset", "Recome�ar"} },
        {"txtOptions", new string[] {"Options", "Op��es"} },
        //{"txtControls", new string[] {"Controls", "Controles" } },
        {"txtLang", new string[] {"Language", "Idioma" } },
        {"txtOST", new string[] {"Music", "M�sica" } },
        {"txtSFX", new string[] {"Sound Effects", "Efeitos Sonoros" } },
        {"txtNewRecord", new string[] {"New Record!!", "Novo Recorde!!" } },
        {"txtEnemies", new string[] {"Enemies defeated:", "Inimigos derrotados:" } },
        {"txtPistol", new string[] {"Pistol", "Pistola" } },
        {"txtSMG", new string[] {"SMG", "Metralhadora" } },
        {"txtMisteryGun", new string[] {"Mistery Gun", "Arma Misteriosa" } },
        {"txtSensitivity", new string[] {"Cam Sensitivity", "Sensibilidade da C�mera" } },
        {"langEnglish", new string[] {"English", "Ingl�s" } },
        {"langPortuguese", new string[] {"Portuguese", "Portugu�s" } },
    };

    //M�todos para salvar/carregar dados
    public static void SaveData() {
        string path = Application.persistentDataPath + "/globals.bin";
        ConfigsData data = new ConfigsData(idLanguage, numCoins, levelPistol, levelSMG, recordEnemiesDefeated, pistolDamageTax, SMGDamageTax, volumeOST, volumeSFX, hasMisteryGun);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        formatter.Serialize(fileStream, data);
        fileStream.Close();
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
    }
}

public class ConfigsData {
    public int idLanguage, numCoins, levelPistol, levelSMG, recordEnemiesDefeated;
    public float volumeOST, volumeSFX, pistolDamageTax, SMGDamageTax;
    public bool hasMisteryGun;

    public ConfigsData(int idLanguage, int numCoins, int levelPistol, int levelSMG, int recordEnemiesDefeated, float pistolDamageTax, float SMGDamageTax, float volumeOST, float volumeSFX, bool hasMisteryGun) {
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
    }
}
