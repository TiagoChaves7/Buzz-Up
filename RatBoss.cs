using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatBoss : MonoBehaviour
{
    // Movement variables
    [Header("Movement")]
    public Transform player;
    [SerializeField] public float moveSpeed = 5f; // Boss movement speed
    [SerializeField] public float minDistance = 2f; // Minimum distance between boss and player
    private Rigidbody2D rb;
    private Animator animator;

    // Reference to bullet prefab
    [Header("Bullet")]
    public GameObject enemyBullet;
    private float shootTimer = 0;
    public float shootCooldown = 3f;

    // Sound variables
    [Header("Sounds")]
    public AudioSource audioSource; // Audio source for sounds
    public AudioClip shootSound; // Shooting sound
    public AudioClip deathSound; // Death sound

    // Jump attack variables
    [Header("Jump")]
    public float jumpForce = 10f; // Jump force
    public float jumpCooldown = 5f; // Cooldown between jumps
    private float jumpTimer = 0f; // Jump timer
    private int jumpDamage = 3; // Damage dealt to player on jump hit

    private Vector2 shootDirection;
    private bool isJumping = false;
    private Vector2 targetPosition; // Target position for jump

    // Reference to Final NPC
    [Header("NPC")]
    public NPCFinal npcFinal; // Reference to NPCFinal

    [Header("Post-Death Actions")]
    public GameObject npcfinal; // GameObject to be activated after boss death

    void Start()
    {
        // Initializations
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Get AudioSource if not assigned in inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (player != null)
        {
            HandleMovement();
            HandleShooting();
            HandleJumpAttack();
        }
    }

    private void HandleMovement()
    {
        // Calculate direction to player, only on X axis (horizontal)
        Vector2 direction = (player.position - transform.position).normalized;

        // Calculate current distance between boss and player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Move boss towards player until minimum distance is reached, unless jumping
        if (!isJumping)
        {
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            animator.SetBool("IsWalking", distanceToPlayer > minDistance);
        }

        // Update boss rotation to face player
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // Face left
            shootDirection = Vector2.left; // Set bullet direction to left
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Face right
            shootDirection = Vector2.right; // Set bullet direction to right
        }
    }

    private void HandleShooting()
    {
        // Shooting control
        if (shootTimer >= shootCooldown && !isJumping)
        {
            // Instantiate bullet and set direction
            GameObject bullet = Instantiate(enemyBullet, transform.position, Quaternion.identity);
            bullet.GetComponent<BossBullet>().SetDirection(shootDirection); // Set bullet direction

            // Play shooting sound
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            Debug.Log("Boss fired");
            shootTimer = 0;

            animator.SetTrigger("IsShoot");
        }
        else
        {
            shootTimer += Time.deltaTime;
        }
    }

    private void HandleJumpAttack()
    {
        if (!isJumping && jumpTimer >= jumpCooldown)
        {
            // Calculate jump direction towards player position
            Vector2 jumpDirection = (player.position - transform.position).normalized;

            // Set jump force combining horizontal and vertical direction
            float horizontalForce = jumpDirection.x * jumpForce; // Force in horizontal direction
            float verticalForce = jumpForce; // Force in vertical direction

            // Apply jump force combining horizontal and vertical direction
            rb.linearVelocity = new Vector2(horizontalForce, verticalForce);

            isJumping = true;
            jumpTimer = 0f;

            animator.SetBool("IsJumping", true); // Activate jump animation
        }
        else
        {
            jumpTimer += Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isJumping && collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to player when hit during jump
            collision.gameObject.GetComponent<Health>().TakeDamage(jumpDamage); // Apply 3 damage
            isJumping = false;

            animator.SetBool("IsJumping", false); // Deactivate jump animation
        }
        else if (isJumping && collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false; // Boss stopped jumping
            animator.SetBool("IsJumping", false); // Deactivate jump animation
        }
    }

    public void ReduceCooldown()
    {
        shootCooldown /= 2; // Reduce cooldown time by half
    }

    public void Die()
    {
        animator.SetTrigger("IsDied"); // Trigger "death" animation

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        Debug.Log("Boss Died");

        // Activate final NPC next to player
        if (npcfinal != null)
        {
            // Ensure NPC is active before calling ActivateNPC
            npcfinal.SetActive(true);  // Activate NPCFinal
            // npcFinal.ActivateNPC(player); // Activate NPC and position next to player
        }

        Destroy(gameObject); // Destroy Boss GameObject after death
    }
}
