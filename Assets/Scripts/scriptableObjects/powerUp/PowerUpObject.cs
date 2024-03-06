using UnityEngine;

[CreateAssetMenu(fileName = "powerUpConfigs", menuName = "PowerUp")]
public class PowerUpObject : ScriptableObject {
    public int rarity, type;   //rarity pode variar de 1 a 100
    public Color color;
}
