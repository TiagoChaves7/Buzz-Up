using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    Rigidbody2D rgb2d;

    public float moveSpeed; // Speed of the pendulum
    public float leftAngle; // Left limit angle
    public float rightAngle; // Right limit angle

    // Variable to determine the direction of movement
    bool movingClockwise;

    [Header("Audio Settings")]
    public AudioClip pendulumSound; // Sound of the pendulum
    private AudioSource audioSource; // Reference to AudioSource

    [Header("Player Settings")]
    public GameObject player; // Reference to the player
    public float soundActivationDistance = 5f; // Distance to activate the sound

    void Start()
    {
        rgb2d = GetComponent<Rigidbody2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = pendulumSound;
        audioSource.loop = false; // Don't repeat the sound, it will only play when changing direction

        movingClockwise = true;
    }

    void Update()
    {
        Move();
    }

    public void ChangeMoveDir()
    {
        // Rotate in z-axis
        float zRotation = transform.eulerAngles.z;
        // Adjust the angle to be between -180 and 180 degrees
        zRotation = (zRotation > 180) ? zRotation - 360 : zRotation;

        // Change direction based on limit angles
        if (movingClockwise && zRotation >= rightAngle)
        {
            movingClockwise = false;
            PlayPendulumSoundIfPlayerIsNear(); // Play sound when changing direction
        }
        else if (!movingClockwise && zRotation <= leftAngle)
        {
            movingClockwise = true;
            PlayPendulumSoundIfPlayerIsNear(); // Play sound when changing direction
        }
    }

    public void Move()
    {
        ChangeMoveDir();
        if (movingClockwise)
        {
            rgb2d.angularVelocity = moveSpeed;
        }
        else
        {
            rgb2d.angularVelocity = -moveSpeed;
        }
    }

    // Check if the player is nearby and play the pendulum sound
    private void PlayPendulumSoundIfPlayerIsNear()
    {
        if (player != null)
        {
            // Calculate the distance between the player and the pendulum
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // If the player is within the specified distance, play the sound
            if (distanceToPlayer <= soundActivationDistance)
            {
                if (!audioSource.isPlaying) // Play only if not already playing
                {
                    audioSource.Play();
                }
            }
        }
    }
}
