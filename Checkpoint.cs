using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    Player player;
    public Transform respawnPoint;
    Collider2D coll;

    Animator animator;

    private AudioSource audioSource;
    [Header("Audio Checkpoint")]
    public AudioClip checkpointClip;

    public bool PlayerTrigger;

    private void Awake()
    {
        // Find the Player object and get its Player component
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        // Get the Collider2D component attached to this GameObject
        coll = GetComponent<Collider2D>();
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();

        // Check if the AudioSource component is present, if not, add it
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the "Player" tag
        if (collision.CompareTag("Player"))
        {
            // Update the player's checkpoint to the current respawn point
            player.UpdateCheckpoint(respawnPoint.position);
            Debug.Log("Changed sprite");
            // Set the "PlayerTrigger" parameter in the animator to true
            animator.SetBool("PlayerTrigger", true);

            // Play the checkpoint sound if AudioSource and audio clip are available
            if (audioSource != null && checkpointClip != null)
            {
                audioSource.PlayOneShot(checkpointClip);
            }

            // Disable the collider to prevent multiple triggers
            coll.enabled = false;
        }
    }
}
