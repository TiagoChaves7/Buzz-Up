using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    Rigidbody2D rgb2d;

    public float moveSpeed; // velocidade do pêndulo
    public float leftAngle; // ângulo limite à esquerda
    public float rightAngle; // ângulo limite à direita

    // variável para determinar a direção do movimento
    bool movingClockwise;

    [Header("Audio Settings")]
    public AudioClip pendulumSound; // Som do pêndulo
    private AudioSource audioSource; // Referência ao AudioSource

    [Header("Player Settings")]
    public GameObject player; // Referência ao jogador
    public float soundActivationDistance = 5f; // Distância para ativar o som

    void Start()
    {
        rgb2d = GetComponent<Rigidbody2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = pendulumSound;
        audioSource.loop = false; // Não repetir o som, ele só será reproduzido ao mudar de direção

        movingClockwise = true;
    }

    void Update()
    {
        Move();
    }

    public void ChangeMoveDir()
    {
        // rotacionar em z 
        float zRotation = transform.eulerAngles.z;
        // ajusta o ângulo para estar entre -180 e 180 graus
        zRotation = (zRotation > 180) ? zRotation - 360 : zRotation;

        // Muda a direção baseado nos ângulos de limite
        if (movingClockwise && zRotation >= rightAngle)
        {
            movingClockwise = false;
            PlayPendulumSoundIfPlayerIsNear(); // Tocar som quando muda de direção
        }
        else if (!movingClockwise && zRotation <= leftAngle)
        {
            movingClockwise = true;
            PlayPendulumSoundIfPlayerIsNear(); // Tocar som quando muda de direção
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

    // Verificar se o jogador está perto e tocar o som do pêndulo
    private void PlayPendulumSoundIfPlayerIsNear()
    {
        if (player != null)
        {
            // Calcula a distância entre o jogador e o pêndulo
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // Se o jogador estiver dentro da distância especificada, toca o som
            if (distanceToPlayer <= soundActivationDistance)
            {
                if (!audioSource.isPlaying) // Toca apenas se não estiver tocando
                {
                    audioSource.Play();
                }
            }
        }
    }
}
