using System;
using UnityEngine;

public class PowerUpController : MonoBehaviour {
    [SerializeField] private PowerUpObject[] powerUpModels;    //Estes serão os scriptable objects
    [SerializeField] private GameObject powerUpPrefab;
    private static PowerUpController instance;

    public static PowerUpController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void createPowerUp(Vector3 position) {
        PowerUpObject chosenPowerUp = null;
        int rand = UnityEngine.Random.Range(1, 101);
        int minRarity = 101;
        foreach(PowerUpObject powerUpObject in powerUpModels) {    //Escolhendo o power-up mais raro possível
            if(rand <= powerUpObject.rarity) {
                if (minRarity > powerUpObject.rarity) {
                    minRarity = powerUpObject.rarity;
                    chosenPowerUp = powerUpObject;
                }
            }
        }
        if(chosenPowerUp != null) {
            GameObject powerUpSection = Instantiate(powerUpPrefab, position, Quaternion.identity);
            GameObject powerUp = powerUpSection.transform.GetChild(0).gameObject;
            powerUp.GetComponent<MeshRenderer>().material.color = chosenPowerUp.color;
            powerUp.GetComponent<PowerUp>().type = chosenPowerUp.type;
            powerUpSection.SetActive(true);
        }

    }
}
