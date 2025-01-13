using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerFinal : MonoBehaviour
{
    // Variables for turning
    private float horizontalTurn;
    private float verticalTurn;
    [SerializeField] private float horizontal;

    [Header("Movement Player")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 25f; // Defines the jump force
    [SerializeField] private bool isGrounded = false; // Variable to track if the player is on the ground
    [SerializeField] private float runningSpeed = 12f;
    [SerializeField] private float fallLimitY = -8f; // Defines the fall limit

    [SerializeField] private Rigidbody2D rb;

    // Reference to the health script
    private Health healthScript;

    [Header("Respawn Variables")]
    Vector2 checkpointPos;
    Rigidbody2D playerRb;
    Vector3 playerSize;

    // Animation variables
    Animator anim;

    [Header("Animations")]
    public bool isWalking;
    public bool isRunning;
    public bool isJumping;
    public bool isShooting;
    public bool isRunningAndShoot;
    public bool isHurt;
    public bool isDead;

    // Variables for sounds
    private AudioSource audioSource;

    // Variable for jump AudioSource
    private AudioSource jumpAudioSource;

    [Header("Audio Clips")]
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip dieClip;
    public AudioClip idleClip;
    public AudioClip hurtClip;

    [Header("Win menu reference")]
    public WinAndLoseMenu menuwin;
    [SerializeField] private GameObject loseMenu;   // Victory menu panel
    [SerializeField] private GameObject loseFirstButton;
    [SerializeField] private AudioSource backgroundMusic;  // Background music
    [SerializeField] private AudioSource soundEffects;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<Health>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Add a separate AudioSource for jump sound
        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.playOnAwake = false; // Ensure the sound doesn't play automatically
    }

    // Start is called before the first frame update
    void Start()
    {
        checkpointPos = transform.position;
        playerSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Horizontal Movement and Player Direction
        horizontal = Input.GetAxis("Horizontal");

        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-playerSize.x, playerSize.y, playerSize.z);
        }
        else { transform.localScale = playerSize; }

        // Running (Increases speed when pressing "Fire3")
        if (Input.GetButton("Fire3"))
        {
            if (anim.GetBool("IsWalking"))
            {
                anim.SetBool("IsRunning", true);
                anim.SetBool("IsWalking", false);
            }
            speed = runningSpeed; // Increase horizontal speed to running speed

            // Play "run" sound
            // Check if run sound is already playing to avoid restarting
            if (audioSource != null && runClip != null)
            {
                if (!audioSource.isPlaying || audioSource.clip != runClip)
                {
                    audioSource.clip = runClip;
                    audioSource.loop = true; // Enable loop for run sound
                    audioSource.Play(); // Start run sound
                }
            }
        }
        else
        {
            speed = 8f; // Restore normal speed if Shift key is not pressed

            // Walking sound (if player is walking)
            if (horizontal != 0 && isGrounded)
            {
                // Check if walk sound is already playing
                if (audioSource != null && walkClip != null)
                {
                    if (!audioSource.isPlaying || audioSource.clip != walkClip)
                    {
                        audioSource.clip = walkClip;
                        audioSource.loop = true; // Enable loop for walk sound
                        audioSource.Play(); // Start walk sound
                    }
                }
            }
        }

        // Update Movement Animations
        if (horizontal != 0 && isGrounded)
        {
            anim.SetBool("IsWalking", true);
            anim.SetBool("IsRunning", false);
        }
        else
        {
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsWalking", false);

            // Idle sound (if player is stationary)
            if (audioSource != null && idleClip != null)
            {
                if (!audioSource.isPlaying || audioSource.clip != idleClip)
                {
                    audioSource.clip = idleClip;
                    audioSource.loop = true; // Enable loop for idle sound
                    audioSource.Play(); // Start idle sound
                }
            }
        }

        // Stop running or walking sounds when player stops
        if (horizontal == 0 && (audioSource.clip == runClip || audioSource.clip == walkClip))
        {
            audioSource.Stop();
        }

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            isGrounded = false;

            anim.SetTrigger("IsJumping");
            // Ensure jump sound is restarted
            if (jumpAudioSource != null && jumpClip != null)
            {
                jumpAudioSource.clip = jumpClip; // Assign clip to AudioSource
                jumpAudioSource.Stop(); // Ensure any previous sound stops
                jumpAudioSource.Play(); // Play jump sound
            }

            Debug.Log("Jumping with Power: " + jumpingPower); // Debugging information
        }

        // Check if player fell below the fall limit
        if (transform.position.y < fallLimitY)
        {
            // Play death sound
            if (audioSource != null && dieClip != null)
            {
                audioSource.PlayOneShot(dieClip);
            }
            Die();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    // Detect collision when player is on the ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // Detect collision with moving platform
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            Debug.Log("Player landed on moving platform");
            isGrounded = true;
            transform.SetParent(collision.transform); // Set player as a child of the platform
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Play "hurt" sound
            if (audioSource != null && hurtClip != null)
            {
                audioSource.PlayOneShot(hurtClip);
            }
            anim.SetTrigger("IsHurt");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            Debug.Log("Player left the moving platform");
            isGrounded = false;
            transform.SetParent(null); // Detach the player from the platform
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Die();
        }
    }

    // Respawn
    public void UpdateCheckpoint(Vector2 pos)
    {
        checkpointPos = pos;
    }

    public void Die()
    {
        // Play death sound
        if (audioSource != null && dieClip != null)
        {
            audioSource.PlayOneShot(dieClip);
        }
        anim.SetTrigger("IsDead");
        ShowLoseMenu();

        StartCoroutine(Respawn(1f));
    }

    public IEnumerator Respawn(float duration)
    {
        anim.SetTrigger("Reset");
        playerRb.simulated = false;
        playerRb.linearVelocity = new Vector2(0, 0);
        transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(duration);
        transform.position = checkpointPos;
        transform.localScale = new Vector3(1, 1, 1);

        healthScript.ResetHealth(); // Call ResetHealth to reset player's health

        playerRb.simulated = true;

        // Debugging information after respawn
        Debug.Log("Respawned with Jumping Power: " + jumpingPower);
    }

    void ResetAllAnimatorBools()
    {
        foreach (var boolparameter in anim.parameters)
        {
            if (boolparameter.type == AnimatorControllerParameterType.Bool)
            {
                anim.SetBool(boolparameter.name, false);
            }
        }
    }

    // Enable or disable sound
    public void EnablePlayerAudio(bool enable)
    {
        if (audioSource != null)
        {
            audioSource.enabled = enable; // Enable or disable audio
        }
    }

    // Method to show the lose menu
    public void ShowLoseMenu()
    {
        loseMenu.SetActive(true);  // Activate the lose menu
        Time.timeScale = 0f;       // Pause the game

        // Pause music and sounds
        if (backgroundMusic != null)
        {
            backgroundMusic.Pause();
        }
        if (soundEffects != null)
        {
            soundEffects.Pause();
        }

        // Set the button as selected in the EventSystem
        EventSystem.current.SetSelectedGameObject(null); // Clear previous selection
        EventSystem.current.SetSelectedGameObject(loseFirstButton); // Select the first button of the Lose Menu
        Debug.Log("First button of winMenu selected: " + loseFirstButton.name);
    }
}
