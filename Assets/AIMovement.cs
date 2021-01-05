using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public GameObject BulletPrefab;
    public Gun Gun;
    public AudioClip AudioClip;
    public Vector3 recoilrotation = new Vector3(2f, 2f, 2f), recoilrotationaiming = new Vector3(0.5f, 0.5f, 1.5f), targetGunLocation;
    public Transform Body;
    public Inventory Inventory;
    public LayerMask LayerMask;
    public List<Vector3> targets = new List<Vector3>();
    public List<float> directions;

    private bool canShoot = true, aiming;
    private Vector2 totalRotation = Vector2.zero;
    private float xRotation = 0f;
    private int consecutiveShots = 0, currentTarget = 0;
    private Transform nearestEnemy = null;

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f, LayerMask);
        float minimumDistance = float.MaxValue;
        Vector2 newRotation = Vector2.zero, removedRotation;
        RaycastHit hit;

        foreach (Collider collider in hitColliders)
        {
            float distance = Vector3.Distance(Body.position, collider.transform.position);

            if (distance < minimumDistance && Vector3.Dot(Vector3.Normalize(collider.transform.position - transform.position), transform.forward) > 0.707)
            {
                minimumDistance = distance;
                nearestEnemy = collider.transform;
            }
        }

        if (nearestEnemy != null)
            Body.Rotate((Quaternion.LookRotation(new Vector3(nearestEnemy.position.x - transform.position.x, 0.0f, nearestEnemy.position.z - transform.position.z)).eulerAngles - Body.eulerAngles).normalized);
        else
        {
            float minDirection = Mathf.Repeat(Mathf.Round(Body.rotation.y - 22.5f), 360.0f), maxDirection = Mathf.Repeat(Mathf.Round(Body.rotation.y + 22.5f), 360.0f);
            int lowestIndex = -1;
            float lowestValue = float.MaxValue;

            if (Vector3.Distance(Body.position, targets[currentTarget]) <= 0.1f)
                currentTarget = ++currentTarget % targets.Count;

            for (int i = 0; i < directions.Count; i++)
            {
                if ((minDirection > maxDirection && (i >= minDirection || i <= maxDirection)) || (minDirection < maxDirection && i >= minDirection && i <= maxDirection))
                    directions[i] = 1.0f;
                else
                {
                    directions[i] = Mathf.Clamp(directions[i] - Time.deltaTime / 10, 0.0f, 1.0f);

                    if (directions[i] < lowestValue)
                    {
                        lowestValue = directions[i];
                        lowestIndex = i;
                    }
                }
            }

            Body.Rotate(0.0f, Mathf.Clamp((lowestIndex - Body.rotation.y) * Time.deltaTime, 0.0f, 0.1f), 0.0f);
            Body.position += Vector3.ClampMagnitude(targets[currentTarget] - Body.position, Time.deltaTime * 10.0f);
            consecutiveShots = 0;
            //spread -= decreasePerSecond * Time.deltaTime;      //Decrement the spread        
        }

        //spread = Mathf.Clamp(spread, minSpread, maxSpread);

        if (minimumDistance >= 20f)
        {
            aiming = true;
            targetGunLocation = Gun.aimingLocation;
        }
        else
        {
            aiming = false;
            targetGunLocation = Gun.idleLocation;
        }

        Gun.transform.localPosition = Vector3.Lerp(Gun.transform.localPosition, targetGunLocation, Time.deltaTime * Gun.Maneuverability);

        /**/

        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 100f))
        {
            //if (hit.collider.transform.CompareTag("Character"))
                //newRotation = Fire(nearestEnemy.position);

            if (hit.distance <= 10f && hit.collider.transform.CompareTag("Pickupable"))
            {
                Pickup pickup = hit.collider.GetComponent<Pickup>();
                pickup.Pickupable.AddToInventory(Inventory, hit.collider.gameObject, pickup.Amount);
            }
        }

        totalRotation += newRotation;
        removedRotation = new Vector2(Mathf.Lerp(0, totalRotation.x, Time.deltaTime), Mathf.Lerp(0, totalRotation.y, Time.deltaTime));
        totalRotation -= removedRotation;
        //float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime + newRotation.x - removedRotation.x;
        //float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime + newRotation.y - removedRotation.y;
        //xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //Body.Rotate(Vector3.up * mouseX);
    }

    Vector2 Fire(Vector3 target)
    {
        Vector2 newRotation = new Vector2();

        if (canShoot)
        {
            GameObject bullet = Instantiate(BulletPrefab, Gun.BulletSpawn.position, Gun.transform.rotation);

            Gun.AudioSource.PlayOneShot(AudioClip);
            consecutiveShots++;
            bullet.GetComponent<Rigidbody>().velocity = (target - Gun.BulletSpawn.transform.position).normalized * 100;
            //Destroy(bullet, 2f);
            canShoot = false;
            StartCoroutine(ShootDelay());

            if (aiming)
                newRotation = new Vector2(Random.Range(-recoilrotationaiming.x, recoilrotationaiming.x), Random.Range(0, recoilrotationaiming.y));
            else
                newRotation = new Vector2(Random.Range(-recoilrotation.x, recoilrotation.x), Random.Range(0, recoilrotation.y));

            StartCoroutine(ShootDelay());
        }

        return newRotation;
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(Gun.ShotDelay);
        canShoot = true;
    }
}
