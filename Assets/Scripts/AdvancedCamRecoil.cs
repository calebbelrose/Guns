﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedCamRecoil : MonoBehaviour
{
    public float rotationSpeed = 6;
    public float returnSpeed = 2;
    public Vector3 recoilrotation = new Vector3(2f, 2f, 2f);
    public Vector3 recoilrotationaiming = new Vector3(0.5f, 0.5f, 1.5f);
    public bool aiming;
    public Transform PlayerBody;
    public GameObject BulletPrefab;
    public Camera mainCamera;
    public Image CrossHair;
    public CharacterController characterController;
    public float mouseSensitivity = 100f;
    public AudioClip AudioClip;
    public Inventory Inventory;
    public Text PickupableName;
    public Text PickupableAmount;
    public Gun Gun;
    public GameObject MagazinePrefab;

    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private GameObject InventoryCanvas;

    private float spread = 20f;          //Adjust this for a bigger or smaller crosshair
    private float maxSpread = 60f;
    private float minSpread = 20f;
    private float spreadPerSecond = 30f;
    private float decreasePerSecond = 25f;
    private float width = 3f;
    private float height = 35f;
    private float xRotation = 0f;
    private Color crosshairColor = Color.white;
    private Texture2D tex;
    private GUIStyle lineStyle;
    private bool canShoot = true;
    private int consecutiveShots = 0;
    private Vector2 totalRotation = Vector2.zero;
    private Vector3 targetGunLocation;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        tex = new Texture2D(1, 1);
        SetColor(tex, crosshairColor); //Set color
        lineStyle = new GUIStyle();
        lineStyle.normal.background = tex;
        targetGunLocation = Gun.idleLocation;
    }

    Vector2 Fire()
    {
        Vector2 newRotation = Vector2.zero;

        if (canShoot && Gun.UseAmmo())
        {
            Gun.AudioSource.PlayOneShot(AudioClip);
            consecutiveShots++;
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(1000); // You may need to change this value according to your needs
                                                  // Create the bullet and give it a velocity according to the target point computed before
            GameObject bullet = Instantiate(BulletPrefab, Gun.BulletSpawn.position, Gun.transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = (targetPoint - Gun.BulletSpawn.transform.position).normalized * 500;
            //Destroy(bullet, 2f);
            canShoot = false;

            if (aiming)
                newRotation = new Vector2(Random.Range(-recoilrotationaiming.x, recoilrotationaiming.x), Random.Range(0, recoilrotationaiming.y));
            else
                newRotation = new Vector2(Random.Range(-recoilrotation.x, recoilrotation.x), Random.Range(0, recoilrotation.y));

            StartCoroutine(ShootDelay());
        }

        return newRotation;
    }

    private void Update()
    {
        RaycastHit hit;

        if (Input.GetButtonDown("Inventory"))
        {
            InventoryCanvas.SetActive(!InventoryCanvas.activeSelf);
            Cursor.visible = InventoryCanvas.activeSelf;

            if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.Confined;
                PlayerMovement.enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                PlayerMovement.enabled = true;
            }
        }

        if (PlayerMovement.enabled)
        {
            if (Input.GetButtonDown("Interact") && Loot.CurrentLoot != null)
            {
                ItemScript newItem = ItemDatabase.Instance.ItemEquipPool.GetItemScript();

                newItem.SetItemObject(ItemDatabase.Instance.DBList(Loot.CurrentLoot.ItemID));

                if (InvenGridManager.Instance.StoreLoot(newItem))
                    Loot.Destroy();
            }

            if (Input.GetButtonDown("Reload"))
            {
                if (Gun.Magazine != null)
                    StartCoroutine(Gun.Reload(MagazinePrefab, Gun.transform));
            }
            else
            {
                Vector2 newRotation = Vector2.zero;
                Vector2 removedRotation;

                if (Input.GetButton("Fire1"))
                {
                    //spread += spreadPerSecond * Time.deltaTime;       //Incremente the spread
                    newRotation = Fire();
                }
                else
                {
                    consecutiveShots = 0;
                    //spread -= decreasePerSecond * Time.deltaTime;      //Decrement the spread        
                }

                totalRotation += newRotation;
                removedRotation = new Vector2(Mathf.Lerp(0, totalRotation.x, Time.deltaTime), Mathf.Lerp(0, totalRotation.y, Time.deltaTime));
                totalRotation -= removedRotation;

                //spread = Mathf.Clamp(spread, minSpread, maxSpread);

                if (Input.GetButtonDown("Fire2"))
                {
                    aiming = true;
                    targetGunLocation = Gun.aimingLocation;
                }
                else if (Input.GetButtonUp("Fire2"))
                {
                    aiming = false;
                    targetGunLocation = Gun.idleLocation;
                }

                Gun.transform.localPosition = Vector3.Lerp(Gun.transform.localPosition, targetGunLocation, Time.deltaTime * Gun.Maneuverability);

                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime + newRotation.x - removedRotation.x;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime + newRotation.y - removedRotation.y;

                xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);

                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                PlayerBody.Rotate(Vector3.up * mouseX);
            }

            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 100f) && hit.collider.transform.CompareTag("Pickupable"))
            {
                Pickup pickup = hit.collider.GetComponent<Pickup>();

                PickupableName.text = pickup.Pickupable.GetName();
                PickupableAmount.text = pickup.Amount.ToString();

                if (Input.GetButton("Interact"))
                    pickup.Pickupable.AddToInventory(Inventory, hit.collider.gameObject, pickup.Amount);
            }
        }
    }

    void OnGUI()
    {
        var centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        if (!aiming)
        {
            GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y - (height + spread), width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y + spread, width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x + spread, (centerPoint.y - width / 2), height, width), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - (height + spread), (centerPoint.y - width / 2), height, width), "", lineStyle);
        }
    }

    void SetColor(Texture2D myTexture, Color myColor)
    {
        for (int y = 0; y < myTexture.height; ++y)
        {
            for (int x = 0; x < myTexture.width; ++x)
            {
                myTexture.SetPixel(x, y, myColor);
            }
        }

        myTexture.Apply();
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(Gun.ShotDelay);
        canShoot = true;
    }
}