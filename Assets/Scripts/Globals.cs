using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Globals : MonoBehaviour {
    public static int idLanguage = 0, numCoins=0, levelPistol=0, levelSMG=0;
    public static float volumeOST = 1, volumeSFX = 1, pistolDamageTax=1, SMGDamageTax=1;
    public static bool hasMisteryGun = false;

    //Essas informações não serão salvas e só servirão para definir certas coisas no jogo:
    public enum typesOfGuns {
        pistol,
        submachine,
        misteryGun,
        enemyGun,
    }
    public static Dictionary<int, int> upgradePricesPistol = new Dictionary<int, int>() {
        {0, 10}, {1, 30}, {2, 50}
    };
    public static Dictionary<int, int> upgradePricesSMG = new Dictionary<int, int>() {
        {0, 10}, {1, 30}, {2, 50}
    };
    public static int priceMisteryGun = 500;

    public static Dictionary<int, float> levelsDamageTax = new Dictionary<int, float>() {    //Aqui estão as taxas de aumento de poder de cada upgrade
        {0, 1.2f}, {1, 1.5f}, {2, 2f}
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


    public static void SaveData() {
        string path = Application.persistentDataPath + "/globals.bin";
        ConfigsData data = new ConfigsData(idLanguage, volumeOST, volumeSFX);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }

    public static ConfigsData LoadData() {
        string path = Application.persistentDataPath + "/globals.bin";
        if (File.Exists(path)) {   //Se o arquivo existir
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            ConfigsData data = formatter.Deserialize(fileStream) as ConfigsData;
            fileStream.Close();

            return data;
        }

        return null;
    }

    public static void ResetData() {
        /*
        dificuldade = 1;
        indexPersonagemSelecionado = 1;
        numMoedas = 0;
        highScoreFacil = 0;
        highScorePadrao = 0;
        highScoreHardcore = 0;
        isCompradoPersonagens = new Dictionary<string, bool>() {
            {"frog", false}, {"pinkGuy", true}, {"virtualGuy", false}, {"maskDude", false}
        };
        */
        idLanguage = 0;

        Debug.Log("Dados zerados");
        SaveData();
        //GameController.LoadInicial();
    }
}

public class ConfigsData {
    public int idLanguage, numCoins;
    public float volumeOST, volumeSFX;
    //public Dictionary<string, bool> isCompradoPersonagens = new Dictionary<string, bool>() {
    //    {"frog", false}, {"pinkGuy", true}, {"virtualGuy", false}, {"maskDude", false}
    //};

    public ConfigsData(int idLanguage, float volumeOST, float volumeSFX) {
        this.idLanguage = idLanguage;
        this.volumeOST = volumeOST;
        this.volumeSFX = volumeSFX;
    }
}
