using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

//Control both weapon states (FPS & Top-down mode)
public class WeaponController : MonoBehaviour
{
    private Weapon weapon;
    //Weapon states
    private bool isReloading, isShooting, isCharging;
    private Coroutine shootingCoro, reloadCoro, chargeCoro;
    private Animator animator;
    private int reloadingBoolHash; //Animator state
    [SerializeField]
    private float chargeTime;
    //Player stats
    private int shotToggleHash, headshotCount = 0, bodyshotCount = 0, playerScore = 0;
    public float accuracy {get; set;}
    public float shotsFired {get; set;}
    public float shotsMissed {get; set;}
    [SerializeField]
    private bool infiniteAmmo; //Testing toggle
    //UI References
    [SerializeField]
    private GameObject reloadImage, throwablePrefab;
    [SerializeField]
    private Image chargeIndicator;
    [SerializeField]
    private TextMeshProUGUI ammoText, aimText, playerScoreText;
    public Transform muzzlePos; //Changed by each weapon
    private List<GameObject> throwables;
    public static Action headshot;
    public static Action<int> AddScore, UpdateScoreAction;
    public static Action<float> UpdateAimAction;
    public static Action<int, int> UpdateAmmoAction;
    public static Action<bool> AddAccuracy;
    private int difficulty;
    private int enemyLayerMask;

    void Start()
    {
        enemyLayerMask = LayerMask.GetMask("Enemy");
        difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        weapon = GetComponentInChildren<Weapon>();
        animator = GetComponentInChildren<Animator>();
        reloadingBoolHash = Animator.StringToHash("Reloading");
        shotToggleHash = Animator.StringToHash("Shot");
        UpdateAmmoText();
        UpdatePlayerScore(0);
        //Pool 10 throwable items
        throwables = new List<GameObject>();
        GameObject throwablesHolder = new GameObject("ThrowablesHolder");
        for (int i = 0; i < 10; i++)
        {
            throwables.Add(Instantiate(throwablePrefab, Vector3.zero, Quaternion.identity, throwablesHolder.transform));
            throwables[i].SetActive(false);
        }
        if (weapon.throwable)
        {
            chargeIndicator.fillAmount = 0;
        }
    }

    //View debugging to adjust crosshair
    // void Update()
    // {
    //     Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100f, Color.red);
    // }

    void StopShooting()
    {
        if (isShooting)
        {
            if (shootingCoro != null)
            {
                StopCoroutine(shootingCoro);
            }
            isShooting = false;
        } else if (isCharging) {
            StopCharge();
        }
    }

    void Shoot()
    {
        if (!isShooting && !weapon.throwable)
        {
            shootingCoro = StartCoroutine(Shooting());
        }
    }

    void Charge()
    {
        //Charge weapon
        if (!isCharging && weapon.throwable) {
            //Charge throw
            StartCharge();
        }
    }

    IEnumerator Shooting()
    {
        isShooting = true;
        while (weapon.currentAmmo > 0 && !isReloading)
        {
            //Shooting mechanics/anims
            animator.SetTrigger(shotToggleHash);
            if (!infiniteAmmo)
            {
                weapon.currentAmmo --;
                UpdateAmmoText();
            }
            AudioManager.audioManager.playClip(weapon.bulletAudio, 1);
            
            //Bullet mechs
            RaycastHit hit;
            Vector3 raycastPos;
            Vector3 raycastDir;
            if (!PlayerController.instance.topDownMode) //FPS mode
            {
                raycastPos = Camera.main.transform.position;
                raycastDir = Camera.main.transform.forward;
            } else { //Top Down mode
                raycastPos = transform.position;
                raycastDir = transform.forward;
            }
            if (Physics.Raycast(raycastPos, raycastDir, out hit, 100f, enemyLayerMask)) //Shoots ray from gun to center of screen
            {
                if (hit.collider.CompareTag("Head")) //Hit the "head" part of target
                {
                    headshot();
                    headshotCount++;
                    AddScore(500 * (difficulty + 1));
                    hit.transform.root.gameObject.SetActive(false);
                    // print("hit head");
                } else if (hit.collider.CompareTag("Body")) { //Hit the "body" part of target
                    bodyshotCount++;
                    AddScore(250 * (difficulty + 1));
                    hit.transform.root.gameObject.SetActive(false);
                    // print("hit bodu");
                }
                //Add bullet impact
            } else { //Didn't hit any target
                //Lower aim % and score
                shotsMissed++;
                AddScore(-50 * (difficulty + 1));
            }
            shotsFired++;
            UpdateAimText();
            UpdatePlayerScore(playerScore);
            yield return new WaitForSeconds(weapon.fireRate);
        }
        if (weapon.currentAmmo == 0 && !isReloading) //Reload
            {
                reloadCoro = StartCoroutine(Reload(weapon.reloadTime));
                StopShooting();
            }
    }

    void StartCharge()
    {
        isCharging = true;
        chargeCoro = StartCoroutine(percentOverTime(chargeTime));
    }

    IEnumerator percentOverTime(float totalDuration)
    {
        float startTime = Time.time;
        float percentDone = 0;
        while (Time.time < startTime + totalDuration)
        {
            percentDone = (Time.time - startTime)/totalDuration;
            chargeIndicator.fillAmount = percentDone;
            yield return new WaitForEndOfFrame();
        }
    }

    void ShootCharge()
    {
        if (!isShooting && weapon.throwable)
        {
            StopCharge();
            isShooting = true;
            GameObject obj = GetFreeThrowable();
            obj.transform.position = transform.position + transform.forward;
            obj.transform.forward = transform.forward;
            obj.SetActive(true);
            StartCoroutine(ReloadCharge());
        }
    }

    IEnumerator ReloadCharge() //Charge shot cooldown
    {
        yield return new WaitForSeconds(0.25f);
        isShooting = false;
    }

    void StopCharge()
    {
        chargeIndicator.fillAmount = 0;
        if (chargeCoro != null)
        {
            StopCoroutine(chargeCoro);
        }
        isCharging = false;
    }

    GameObject GetFreeThrowable() //Return an inactive throwable from pool
    {
        foreach (GameObject throwableObj in throwables)
        {
            if (!throwableObj.activeSelf)
            {
                return throwableObj;
            }
        }
        return null; //If all are active (should be impossible)
    }

    IEnumerator Reload(float reloadTime) //Reload FPS weapon
    {
        isReloading = true;
        reloadImage.SetActive(true);
        animator.SetBool(reloadingBoolHash, true);
        AudioManager.audioManager.playClip(weapon.reloadAudio, 1);
        yield return new WaitForSeconds(reloadTime);
        reloadImage.SetActive(false);
        animator.SetBool(reloadingBoolHash, false);
        weapon.currentAmmo = weapon.maxAmmo;
        UpdateAmmoText();
        isReloading = false;
    }

    void AddPlayerScore(int score)
    {
        if (playerScore + score < 0)
        {
            playerScore = 0;
        } else {
            playerScore += score;
        }
        UpdatePlayerScore(playerScore);
    }

    void AddPlayerAccuracy(bool shotHit)
    {
        shotsFired += 1;
        if (shotHit)
        {
            bodyshotCount ++;
        } else {
            shotsMissed ++;
        }
        UpdateAimText();
    }

    void UpdateAmmoText()
    {
        if (UpdateAmmoAction != null)
            UpdateAmmoAction(weapon.currentAmmo, weapon.maxAmmo);
    }

    void UpdateAimText()
    {
        accuracy = ((shotsFired - shotsMissed)/shotsFired) * 100f;
        if (UpdateAimAction != null)
        {
            UpdateAimAction(accuracy);
        }
    }
    
    void UpdatePlayerScore(int score)
    {
        if (UpdateScoreAction != null)
            UpdateScoreAction(score);
    }


    void SendPlayerStats()
    {
        PostGameCanvas.instance.PlayerShotsFired = (int)shotsFired;
        PostGameCanvas.instance.PlayerHeadshotCount = headshotCount;
        PostGameCanvas.instance.PlayerBodyshotCount = bodyshotCount;
        PostGameCanvas.instance.PlayerShotsMissed = (int)shotsMissed;
        PostGameCanvas.instance.PlayerAccuracy = accuracy;
        PostGameCanvas.instance.PlayerScore = playerScore;
        PostGameCanvas.instance.setStats();
    }

    void OnEnable()
    {
        //Subscribers (guys let's get to 10k!!!1!!)
        PlayerController.StartShooting += Shoot;
        PlayerController.StartCharging += Charge;
        PlayerController.StopShooting += StopShooting;
        PlayerController.StopAim += ShootCharge;
        GameManager.GameEnd += SendPlayerStats;
        AddScore += AddPlayerScore;
        AddAccuracy += AddPlayerAccuracy;
    }

    void OnDisable()
    {
        //Unsubscribers >:(
        PlayerController.startShooting -= Shoot;
        PlayerController.startCharging -= Charge;
        PlayerController.StopShooting -= StopShooting;
        PlayerController.stopAim -= ShootCharge;
        GameManager.gameEnd -= SendPlayerStats;
        AddScore -= AddPlayerScore;
        AddAccuracy -= AddPlayerAccuracy;
    }
}
