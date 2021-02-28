using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;

public class AdvancedCamRecoil : Movement
{
    public float rotationSpeed = 6;
    public float returnSpeed = 2;
    public Camera mainCamera;
    public Image CrossHair;
    public CharacterController characterController;
    public float mouseSensitivity = 100f;
    public Text PickupableName;
    public Text PickupableAmount;
    public GameObject MagazinePrefab;

    public PlayerMovement PlayerMovement { get { return playerMovement; } }
    public GameObject InventoryCanvas { get { return inventoryCanvas; } }
    public EquipmentDisplay LootEquipment { get { return lootEquipment; } }
    public GameObject LootBoxInventory { get { return lootBoxInventory; } }
    public RectTransform LootBoxRect { get { return lootBoxRect; } }

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private EquipmentDisplay playerEquipment;
    [SerializeField] private EquipmentDisplay lootEquipment;
    [SerializeField] private GameObject lootBoxInventory;
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private RectTransform lootBoxRect;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private AudioListener AudioListener;

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

    void Start()
    {
        if (!IsLocalPlayer)
        {
            Destroy(Canvas);
            Destroy(playerMovement);
            Destroy(AudioListener);
            Destroy(mainCamera);
            Destroy(this);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        tex = new Texture2D(1, 1);
        SetColor(tex, crosshairColor); //Set color
        lineStyle = new GUIStyle();
        lineStyle.normal.background = tex;

        for (int i = 0; i < Inventory.EquipSlotInfo.Count; i++)
            playerEquipment.EquipSlots[i].SetInfo(Inventory.EquipSlotInfo[i]);

        for (int i = 0; i < Inventory.GunSlotInfo.Count; i++)
            playerEquipment.GunSlots[i].SetInfo(Inventory.GunSlotInfo[i]);

        playerEquipment.DisplayPockets(Inventory.SlotGridList[1]);
    }

    Vector2 Fire()
    {
        Vector2 newRotation = Vector2.zero;

        if (canShoot && Gun.UseAmmo())
        {
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            Vector3 targetPoint;
            Bullet bullet;

            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(1000); // You may need to change this value according to your needs
                                                  // Create the bullet and give it a velocity according to the target point computed before

            Gun.AudioSource.PlayOneShot(AudioClip);
            consecutiveShots++;
            bullet = Instantiate(BulletPrefab, Gun.BulletSpawn.position, Gun.transform.rotation).GetComponent<Bullet>();
            bullet.Shooter = CombatController;
            bullet.rb.velocity = (targetPoint - Gun.BulletSpawn.transform.position).normalized * 500;
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
        if (Input.GetButtonDown("Inventory"))
        {
            InventoryCanvas.SetActive(!InventoryCanvas.activeSelf);
            Cursor.visible = InventoryCanvas.activeSelf;

            if (Cursor.visible)
            {
                //FixCursor.lockState = CursorLockMode.Confined;
                Cursor.lockState = CursorLockMode.None;
                PlayerMovement.enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                LootBoxInventory.SetActive(false);
                LootEquipment.gameObject.SetActive(false);
                PlayerMovement.enabled = true;
            }
        }

        if (PlayerMovement.enabled)
        {
            if (Input.GetButtonDown("Interact"))
            {
                RaycastHit hit;

                if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 10.0f, LayerMask))
                {
                    Loot loot = hit.collider.GetComponent<Loot>();

                    if (loot != null)
                        loot.Action(this);
                }
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

                if (Input.GetButtonDown("Aiming"))
                {
                    aiming = true;
                    Animator.SetBool("Aiming", true);
                }
                else if (Input.GetButtonUp("Aiming"))
                {
                    aiming = false;
                    Animator.SetBool("Aiming", false);
                }

                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime + newRotation.x - removedRotation.x;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime + newRotation.y - removedRotation.y;

                xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);

                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                Body.Rotate(Vector3.up * mouseX);
            }
        }
    }

    void OnGUI()
    {
        var centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        if (!aiming)
        {
            float x = centerPoint.x - width / 2, y = centerPoint.y - width / 2;

            GUI.Box(new Rect(x, centerPoint.y - (height + spread), width, height), "", lineStyle);
            GUI.Box(new Rect(x, centerPoint.y + spread, width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x + spread, y, height, width), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - (height + spread), y, height, width), "", lineStyle);
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