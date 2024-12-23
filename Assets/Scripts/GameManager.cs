using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const int NUM_LEVELS = 3; // Jumlah level dalam game

    public int level { get; private set; } = 0;
    public int lives { get; private set; } = 3;
    public int score { get; private set; } = 0;

    public Spawner spawner; // Referensi Spawner di Inspector

    private GameObject livesContainer; // LivesContainer di setiap scene

    public AudioClip endSceneSound;
    public AudioClip gameOverSound;
    private AudioSource audioSource; // AudioSource to play the sound

    // Event untuk memberi tahu ketika lives berubah
    public static event System.Action<int> OnLivesChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (spawner == null)
        {
            spawner = FindObjectOfType<Spawner>();
        }

        // Add AudioSource if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Tambahkan listener untuk sceneLoaded
        NewGame();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Hapus listener untuk mencegah error
            Instance = null;
        }
    }

    public void NewGame()
    {
        // Reset nilai game seperti nyawa dan skor
        lives = 3;
        score = 0;
        level = 1;
        // Panggil fungsi lainnya jika diperlukan
        LoadLevel(level); // Mulai level pertama
    }

    private void LoadLevel(int index)
    {
        level = index;

        if (spawner != null)
        {
            // Set interval spawn sesuai level
            switch (level)
            {
                case 1:
                    Debug.Log("Level 1 - Interval: 3f");
                    spawner.SetSpawnInterval(3f); // 3 detik untuk level 1
                    break;
                case 2:
                    Debug.Log("Level 2 - Interval: 2f");
                    spawner.SetSpawnInterval(2f); // 2 detik untuk level 2
                    break;
                case 3:
                    Debug.Log("Level 3 - Interval: 1f");
                    spawner.SetSpawnInterval(1f); // 1 detik untuk level 3
                    break;
                default:
                    Debug.LogWarning("Level tidak dikenali! Mengatur interval spawn ke 3 detik.");
                    spawner.SetSpawnInterval(3f);
                    break;
            }
        }

        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.cullingMask = 0; // Hapus rendering sementara untuk efek transisi
        }

        Invoke(nameof(LoadScene), 1f); // Tunggu 1 detik untuk transisi
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(level > 0 ? $"Level{level}" : "Preload");
    }

    public void LevelComplete()
    {
        score += 1000;

        if (level + 1 <= NUM_LEVELS)
        {
            LoadLevel(level + 1);
        }
        else
        {
            // Play the end scene sound and wait for it to finish before transitioning
            StartCoroutine(PlaySoundAndTransition(endSceneSound, "EndScene"));

            // // Play the Game Over sound
            // if (endSceneSound != null && audioSource != null)
            // {
            //     audioSource.PlayOneShot(endSceneSound);
            // }

            // SceneManager.LoadScene("EndScene");
        }
    }

    public void LevelFailed()
    {
        lives--; // Kurangi nyawa

        // Trigger event setelah lives berubah
        OnLivesChanged?.Invoke(lives);

        if (lives <= 0)
        {
            // Play the Game Over sound and wait for it to finish before transitioning
            StartCoroutine(PlaySoundAndTransition(gameOverSound, "GameOver"));

            // // Play the Game Over sound
            // if (gameOverSound != null && audioSource != null)
            // {
            //     audioSource.PlayOneShot(gameOverSound);
            // }

            // // Jika nyawa habis, kembalikan ke scene GameOver dan reset game
            // Debug.Log("Game Over!");
            // SceneManager.LoadScene("GameOver"); // Pindah ke scene GameOver
        }
        else
        {
            LoadLevel(level); // Jika masih ada nyawa, lanjutkan level yang sama
        }
    }

    private IEnumerator PlaySoundAndTransition(AudioClip clip, string sceneName)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
            DontDestroyOnLoad(audioSource.gameObject); // Keep AudioSource alive during scene change
            yield return new WaitForSeconds(clip.length); // Wait for sound to finish
        }

        SceneManager.LoadScene(sceneName);
    }

    // Callback saat scene baru dimuat
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindLivesContainer(); // Cari LivesContainer di scene baru
        SetLivesVisibility(scene.name != "Preload"); // Sembunyikan LivesContainer jika di preload
    }

    // Cari LivesContainer di Canvas pada scene aktif
    private void FindLivesContainer()
    {
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas != null)
        {
            livesContainer = canvas.transform.Find("LivesContainer")?.gameObject;
        }
        else
        {
            livesContainer = null;
        }
    }

    // Fungsi untuk mengatur visibilitas LivesContainer
    private void SetLivesVisibility(bool visible)
    {
        if (livesContainer != null)
        {
            livesContainer.SetActive(visible);
        }
    }
}
