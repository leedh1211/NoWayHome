using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private Vector3 direction;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStat player = other.gameObject.GetComponent<PlayerStat>();
            if (player != null)
            {
                //player.TakeDamage(10f, other.transform.position);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Rocket"))
        {

        }
    }
}
