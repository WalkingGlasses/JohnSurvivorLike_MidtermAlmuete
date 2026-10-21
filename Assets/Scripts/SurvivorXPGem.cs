using UnityEngine;

public class SurvivorXPGem : MonoBehaviour
{
    [Header("Attraction")]
    public float attractionRadius = 3f;
    public float pickupRadius = 0.25f;
    public float moveSpeed = 7f;

    private Transform player;
    private SurvivorProgression progression;
    private int xpValue;
    private bool initialized;
    private bool beingCollected;

    public void Initialize(
        Transform target,
        SurvivorProgression playerProgression,
        int amount)
    {
        player = target;
        progression = playerProgression;
        xpValue = amount;
        initialized = player != null && progression != null;
        beingCollected = false;
    }

    private void Update()
    {
        if (!initialized || player == null)
            return;

        Vector2 difference =
            (Vector2)(player.position - transform.position);

        float distanceSquared = difference.sqrMagnitude;
        float pickupDistanceSquared = pickupRadius * pickupRadius;
        float attractionDistanceSquared =
            attractionRadius * attractionRadius;

        if (distanceSquared <= pickupDistanceSquared)
        {
            Collect();
            return;
        }

        if (!beingCollected &&
            distanceSquared <= attractionDistanceSquared)
        {
            beingCollected = true;
        }

        if (beingCollected)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void Collect()
    {
        if (progression != null)
            progression.AddXP(xpValue);

        Destroy(gameObject);
    }
}