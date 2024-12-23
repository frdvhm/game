using System.Collections;

using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab; // Objek yang akan di-spawn
    public float spawnInterval = 3f; // Interval spawn default (Level 1)
    public AudioClip spawnSound; // Audio clip untuk efek suara
    private AudioSource audioSource; // Referensi ke Audio Source

    private Coroutine spawnCoroutine; // Referensi ke Coroutine spawn

    private void Awake()
    {
        // Pastikan AudioSource ada di GameObject ini
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        StartSpawning();
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    // Mulai proses spawning
    public void StartSpawning()
    {
        // Jika Coroutine sudah berjalan, hentikan terlebih dahulu
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    // Hentikan proses spawning
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    // Coroutine yang mengatur spawning berdasarkan spawnInterval
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Fungsi untuk memunculkan prefab
    private void Spawn()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);

        // Putar suara spawn menggunakan PlayOneShot
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }

    // Fungsi untuk mengatur interval spawn
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = Mathf.Max(0.1f, interval); // Pastikan minimal 0.1 detik untuk performa
        StartSpawning(); // Restart Coroutine dengan interval baru
    }
}
