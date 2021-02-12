﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : Movement
{
    public LayerMask EnemyMask;
    public LayerMask ItemMask;
    public List<Vector3> targets = new List<Vector3>();
    public List<float> directions;

    private int currentTarget = 0;
    public List<Transform> IgnoredObjects = new List<Transform>();
    [SerializeField] private Collider CurrentAI;

    void Update()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, 100f, EnemyMask);
        float minimumDistance = float.MaxValue;
        Vector2 newRotation = Vector2.zero;
        RaycastHit hit;
        Transform rotationTarget = null;
        Vector3 moveTarget = Vector3.zero;

        foreach (Collider collider in enemyColliders)
        {
            if (collider != CurrentAI)
            {
                float distance = Vector3.Distance(Body.position, collider.transform.position);

                if (distance < minimumDistance && Vector3.Angle(collider.transform.position - transform.position, transform.forward) <= 60.0f)
                {
                    minimumDistance = distance;
                    rotationTarget = collider.transform;
                    moveTarget = collider.transform.position + (new Vector3(Body.position.x - collider.transform.position.x, 0.0f, Body.position.z - collider.transform.position.z)).normalized * 15f;
                }
            }
        }

        if (rotationTarget != null)
        {
            Collider[] itemColliders = Physics.OverlapSphere(transform.position, 100f, ItemMask);

            foreach (Collider collider in itemColliders)
            {
                if (!IgnoredObjects.Contains(collider.transform))
                {
                    float distance = Vector3.Distance(Body.position, collider.transform.position);

                    if (distance < minimumDistance && Vector3.Angle(collider.transform.position - transform.position, transform.forward) <= 60.0f)
                    {
                        minimumDistance = distance;
                        rotationTarget = collider.transform;
                        moveTarget = new Vector3(collider.transform.position.x, Body.position.y, collider.transform.position.z);
                    }
                }
            }
        }

        if (rotationTarget != null)
        {
            //Vector3 targetRotation = (Quaternion.LookRotation(nearestTarget.position - transform.position).eulerAngles - transform.eulerAngles).normalized;

            //Vector3 targetRotation = nearestTarget.position - transform.position;
            Vector3 rotation = Quaternion.LookRotation(rotationTarget.position - transform.position).eulerAngles - transform.eulerAngles;

            //Body.Rotate(new Vector3(0.0f, targetRotation.y, 0.0f));
            //transform.Rotate(new Vector3(targetRotation.x, 0.0f, 0.0f));

            if (rotation.x > 90.0f)
                rotation.x -= 360.0f;
            else if (rotation.x < -90.0f)
                rotation.x += 360.0f;

            if (rotation.y > 180.0f)
                rotation.y -= 360.0f;
            else if (rotation.y < -180.0f)
                rotation.y += 360.0f;

            if (rotation.magnitude > 1.0f)
                rotation = rotation.normalized;

            Body.Rotate(0.0f, rotation.y, 0.0f);
            transform.Rotate(rotation.x, 0.0f, 0.0f);
            Body.position += Vector3.ClampMagnitude(moveTarget - Body.position, Time.deltaTime * 10.0f);
            //transform.Rotate((Quaternion.LookRotation(new Vector3(0.0f, targetRotation.y, 0.0f)).eulerAngles - transform.eulerAngles).normalized);
            //transform.Rotate(new Vector3(Vector3.Angle((nearestTarget.position - transform.position).normalized, transform.forward), 0.0f, 0.0f));
        }
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

        /*if (minimumDistance >= 20f)
        {
            aiming = true;
            targetGunLocation = Gun.aimingLocation;
        }
        else
        {
            aiming = false;
            targetGunLocation = Gun.idleLocation;
        }

        Gun.transform.localPosition = Vector3.Lerp(Gun.transform.localPosition, targetGunLocation, Time.deltaTime * Gun.Maneuverability);*/

        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 10.0f))
        {
            //if (hit.collider.transform.CompareTag("Character"))
            //newRotation = Fire(nearestEnemy.position);

            if (hit.collider.gameObject.layer == 11)
                hit.collider.GetComponent<Loot>().Action(this);
        }
    }

    Vector2 Fire(Vector3 target)
    {
        Vector2 newRotation = new Vector2();

        if (canShoot)
        {
            Bullet bullet = Instantiate(BulletPrefab, Gun.BulletSpawn.position, Gun.transform.rotation).GetComponent<Bullet>();

            Gun.AudioSource.PlayOneShot(AudioClip);
            consecutiveShots++;
            bullet.rb.velocity = (target - Gun.BulletSpawn.transform.position).normalized * 100;
            bullet.Shooter = CombatController;
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