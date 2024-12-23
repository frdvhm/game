using UnityEngine;
using UnityEngine.SceneManagement; // Untuk memuat scene
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip mainMenuMusic; // Background music for the main menu
    public AudioClip startButtonSound; // Sound effect for the Start button
    private AudioSource audioSource;

    private void Start()
    {
        // Ensure AudioSource is available
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Play main menu background music
        if (mainMenuMusic != null)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.loop = true; // Loop the music
            audioSource.Play();
        }
    }


    // Metode ini akan dipanggil saat tombol "Start" ditekan
    public void StartGame()
    {
        StartCoroutine(PlayStartSoundAndTransition());

        // SceneManager.LoadScene("Preload"); // Gantilah dengan nama scene yang sesuai
    }

    private IEnumerator PlayStartSoundAndTransition()
    {
        // Play the Start button sound
        if (startButtonSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(startButtonSound);
            yield return new WaitForSeconds(startButtonSound.length); // Wait for the sound to finish
        }

        // Load the next scene (Preload in this case)
        SceneManager.LoadScene("Preload");
    }
}
