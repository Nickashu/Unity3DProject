using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public GameObject bulletObj;
    public float maxBulletVelocity;

    [SerializeField]
    private float bulletVelocity;

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
        {(int)typesOfGuns.submachine, 0.2f },
        {(int)typesOfGuns.magnum, 1.5f }
    };

    public Dictionary<int, float> baseDamageGuns = new Dictionary<int, float>() {    //Este dicionário guardará o tempo de cool down entre os tiros de cada tipo de arma
        {(int)typesOfGuns.pistol, 1f },
        {(int)typesOfGuns.shotgun, 5f },
        {(int)typesOfGuns.submachine, 0.3f },
        {(int)typesOfGuns.magnum, 3f }
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
        bullet.GetComponent<Bullet>().velocity = bulletVelocity;
        bullet.GetComponent<Bullet>().damage = baseDamageGuns[typeOfGun];
        bullet.SetActive(true);
    }
}
