using System.Collections;
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
    public Text PickupableName;
    public Text PickupableAmount;
    public Gun Gun;
    public GameObject MagazinePrefab;

    public PlayerMovement PlayerMovement;
    public GameObject InventoryCanvas;
    public GameObject LootInventory;
    public GameObject LootBoxInventory;
    public RectTransform LootBoxRect;
    public Inventory Inventory;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject lootInventory;
    [SerializeField] private GameObject lootBoxInventory;
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private RectTransform lootBoxRect;
    [SerializeField] private Inventory inventory;
    [SerializeField] private CombatController CombatController;

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
        //FixCursor.lockState = CursorLockMode.Locked;
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
                PlayerMovement.enabled = false;
            }
            else
            {
                //FixCursor.lockState = CursorLockMode.Locked;
                LootBoxInventory.SetActive(false);
                LootInventory.SetActive(false);
                PlayerMovement.enabled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
            Debug.Log(Inventory.SlotGridList[1].List[0].SlotInfo[0, 0].SlotScript.transform.position + " " + Inventory.SlotGridList[1].List[0].SlotInfo[0, 0].SlotScript.ItemScript.transform.position);

        if (PlayerMovement.enabled)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (Loot.CurrentLoot != null)
                    Loot.CurrentLoot.Action(this);
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