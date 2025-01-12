using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    Rigidbody2D rgb2d;

    public float moveSpeed; // velocidade do p�ndulo
    public float leftAngle; // �ngulo limite � esquerda
    public float rightAngle; // �ngulo limite � direita

    // vari�vel para determinar a dire��o do movimento
    bool movingClockwise;

    [Header("Audio Settings")]
    public AudioClip pendulumSound; // Som do p�ndulo
    private AudioSource audioSource; // Refer�ncia ao AudioSource

    [Header("Player Settings")]
    public GameObject player; // Refer�ncia ao jogador
    public float soundActivationDistance = 5f; // Dist�ncia para ativar o som

    void Start()
    {
        rgb2d = GetComponent<Rigidbody2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = pendulumSound;
        audioSource.loop = false; // N�o repetir o som, ele s� ser� reproduzido ao mudar de dire��o

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
        // ajusta o �ngulo para estar entre -180 e 180 graus
        zRotation = (zRotation > 180) ? zRotation - 360 : zRotation;

        // Muda a dire��o baseado nos �ngulos de limite
        if (movingClockwise && zRotation >= rightAngle)
        {
            movingClockwise = false;
            PlayPendulumSoundIfPlayerIsNear(); // Tocar som quando muda de dire��o
        }
        else if (!movingClockwise && zRotation <= leftAngle)
        {
            movingClockwise = true;
            PlayPendulumSoundIfPlayerIsNear(); // Tocar som quando muda de dire��o
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

    // Verificar se o jogador est� perto e tocar o som do p�ndulo
    private void PlayPendulumSoundIfPlayerIsNear()
    {
        if (player != null)
        {
            // Calcula a dist�ncia entre o jogador e o p�ndulo
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // Se o jogador estiver dentro da dist�ncia especificada, toca o som
            if (distanceToPlayer <= soundActivationDistance)
            {
                if (!audioSource.isPlaying) // Toca apenas se n�o estiver tocando
                {
                    audioSource.Play();
                }
            }
        }
    }
}
