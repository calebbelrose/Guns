using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public Round Round;
    public LayerMask LayerMask;
    public GameObject testObject;
    public CombatController Shooter;

    private Vector3 oldPreviousPosition;
    private Vector3 newPreviousPosition;
    private Hitbox previousHitbox;

    private void Start()
    {
        newPreviousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude <= 10f)
            Destroy(gameObject);
        else
        {
            Vector3 vector = newPreviousPosition - transform.position;
            RaycastHit[] hits = Physics.RaycastAll(new Ray(transform.position, vector), vector.magnitude, LayerMask);

            foreach(RaycastHit hit in hits)
            {
                Hitbox hitbox = hit.transform.GetComponent<Hitbox>();

                if (hitbox != previousHitbox)
                    Hit(hitbox, hit.point, false);
            }
        }

        oldPreviousPosition = newPreviousPosition;
        newPreviousPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        Hit(other.transform.GetComponent<Hitbox>(), other.ClosestPointOnBounds(transform.position), true);
    }

    private void Hit(Hitbox hitbox, Vector3 position, bool test)
    {
        if (hitbox != null)
        {
            RaycastHit otherHit;

            Physics.Linecast(oldPreviousPosition, position, out otherHit, LayerMask);
            hitbox.TakeDamage(rb.velocity.magnitude, Shooter);
            rb.velocity *= 0.9f;
            previousHitbox = hitbox;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.velocity *= 0.25f;
    }

    private void OnCollisionStay(Collision collision)
    {
        rb.velocity *= 0.5f;
    }
}