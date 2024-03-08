using UnityEngine;

public class BulletController : MonoBehaviour {

    public GameObject bulletObj;
    public float maxBulletVelocity;
    [SerializeField]
    private float bulletVelocity;
    private static BulletController instance;

    public static BulletController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void spawnBullet(Vector3 position, int typeOfGun, Quaternion rotation, GameObject origin, bool isEnemy = false, float enemyBulletDamage = 0f) {
        GameObject bullet = Instantiate(bulletObj, position, rotation);
        bullet.GetComponent<Bullet>().velocity = bulletVelocity;
        bullet.GetComponent<Bullet>().enemyBullet = isEnemy;
        if (isEnemy)
            bullet.GetComponent<Bullet>().damage = enemyBulletDamage;
        else {
            float damageTax = 1;
            switch (typeOfGun) {
                case (int)Globals.typesOfGuns.pistol:
                    damageTax = Globals.levelsDamageTax[Globals.levelPistol];
                    break;
                case (int)Globals.typesOfGuns.submachine:
                    damageTax = Globals.levelsDamageTax[Globals.levelSMG];
                    break;
            }
            bullet.GetComponent<Bullet>().damage = Globals.baseDamageGuns[typeOfGun] * damageTax;
        }
        SoundController.GetInstance().PlaySound("shot", origin);
        bullet.SetActive(true);
    }
}
