using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public GameObject MiraObject;

    float Horizontal = 0;
    float Vertical = 0;

    [Header("Shooting")]
    public GameObject bullet;
    public Transform bulletSpawnPoint; // Bullet origin point
    public bool canFire; // Initially allows firing
    public bool canShoot = true;

    // Time between shots
    private float timer;
    public float timeBetweenFiring = 1;

    // Reference to pause state
    private bool isPaused;

    // Reference to Player script
    private Player player;

    [Header("Shooting Sound")]
    private AudioSource audioSource;
    public AudioClip shootClip; // Place where the bullet is instantiated

    void Start()
    {
        player = GetComponent<Player>(); // Get reference to Player
        // Check if audio component is present
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Get axis values to rotate with right analog stick
        Horizontal = Input.GetAxis("HorizontalTurn");
        Vertical = Input.GetAxis("VerticalTurn");

        // Rotate player based on input direction
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(Vertical, -Horizontal) * -180 / Mathf.PI);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-Vertical, Horizontal) * -180 / Mathf.PI);
        }

        isPaused = PauseMenu.isPaused;

        if (!isPaused) // Check if game is not paused
        {
            // Countdown timer between shots
            if (!canFire)
            {
                timer += Time.deltaTime;
                if (timer > timeBetweenFiring)
                {
                    canFire = true;
                    timer = 0;
                }
            }

            // Check if player can shoot
            if ((Input.GetButton("Shoot")) && canFire && canShoot)
            {
                canFire = false;
                GameObject Bullet = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
                float angle = Mathf.Atan2(transform.position.y - transform.position.y, transform.position.x - transform.position.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                Bullet.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-Vertical, Horizontal) * -180 / Mathf.PI);

                // Play shooting sound
                if (audioSource != null && shootClip != null)
                {
                    audioSource.PlayOneShot(shootClip);
                }
            }
        }
    }
}
