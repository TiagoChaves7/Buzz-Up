using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }

    private float damage = 1f;
    private Player playerScript;

    [Header("Collect Health Sound")]
    [SerializeField] private AudioClip collectHealthClip;
    private AudioSource audioSource;

    [Header("Hurt Sound")]
    [SerializeField] private AudioClip hurtClip;

    // Animation variables
    Animator anim;
    public bool isHurt;

    private void Awake()
    {
        currentHealth = startingHealth;
        playerScript = GetComponent<Player>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            anim = GetComponent<Animator>();
        }
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;

        if (currentHealth <= 0)
        {
            playerScript.Die();
            FindObjectOfType<WinAndLoseMenu>().ShowLoseMenu();
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);

        // Play collect health sound
        if (audioSource != null && collectHealthClip != null)
        {
            audioSource.PlayOneShot(collectHealthClip);
        }
    }

    public void ResetHealth()
    {
        currentHealth = startingHealth;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage(damage);
            PlayHurtSound();
            anim.SetTrigger("IsHurt");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet") || collision.gameObject.CompareTag("BossBullet"))
        {
            TakeDamage(damage);
            PlayHurtSound();
            anim.SetTrigger("IsHurt");
            Destroy(collision.gameObject);
        }
    }

    private void PlayHurtSound()
    {
        if (audioSource != null && hurtClip != null)
        {
            audioSource.PlayOneShot(hurtClip);
        }
    }
}