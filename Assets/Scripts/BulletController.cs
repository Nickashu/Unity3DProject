using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public GameObject bulletObj;
    public float maxBulletVelocity;
    public float[] bulletVelocity;

    private static BulletController instance;

    public enum typesOfGuns {
        pistol,
        submachine,
        shotgun,
        magnum
    }

    public Dictionary<int, float> shotCoolDownGuns = new Dictionary<int, float>() {    //Este dicionário guardará o tempo de cool down entre os tiros de cada tipo de arma
        {(int)typesOfGuns.pistol, 1f },
        {(int)typesOfGuns.shotgun, 2f },
        {(int)typesOfGuns.submachine, 0.3f },
        {(int)typesOfGuns.magnum, 1.5f }
    };

    public static BulletController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    public void spawnBullet(Vector3 position, int typeOfGun, Quaternion rotation) {
        GameObject bullet = Instantiate(bulletObj, position, rotation);
        bullet.GetComponent<Bullet>().velocity = bulletVelocity[typeOfGun];
        bullet.SetActive(true);
    }
}
