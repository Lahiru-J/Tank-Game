using UnityEngine;

/// <summary>
/// Handle hitpoints and damages
/// </summary>
public class Collision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Destroy(gameObject);
    }
}