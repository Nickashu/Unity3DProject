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
        enemyGun,
    }

    public Dictionary<int, float> shotCoolDownGuns = new Dictionary<int, float>() {    //Este dicionário guardará o tempo de cool down entre os tiros de cada tipo de arma
        {(int)typesOfGuns.pistol, 1f },
        {(int)typesOfGuns.submachine, 0.2f },
    };

    public Dictionary<int, float> baseDamageGuns = new Dictionary<int, float>() {    //Este dicionário guardará o tempo de cool down entre os tiros de cada tipo de arma
        {(int)typesOfGuns.pistol, 1f },
        {(int)typesOfGuns.submachine, 0.3f }
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


    public void spawnBullet(Vector3 position, int typeOfGun, Quaternion rotation, bool isEnemy = false, float enemyBulletDamage = 0f) {
        GameObject bullet = Instantiate(bulletObj, position, rotation);
        bullet.GetComponent<Bullet>().velocity = bulletVelocity;
        bullet.GetComponent<Bullet>().enemyBullet = isEnemy;
        if (isEnemy)
            bullet.GetComponent<Bullet>().damage = enemyBulletDamage;
        else
            bullet.GetComponent<Bullet>().damage = baseDamageGuns[typeOfGun];
        bullet.SetActive(true);
    }
}
