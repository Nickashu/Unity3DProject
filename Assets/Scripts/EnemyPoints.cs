using UnityEngine;

public class EnemyPoints : MonoBehaviour {
    public GameObject canvasPoints;
    private void finishPointsAnimation() {
        Destroy(canvasPoints);    //Destruindo o canvas de pontuação
    }
}
