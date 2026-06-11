using UnityEngine;
using System.Collections.Generic;

public class MultiballController : MonoBehaviour
{
    [Header("Multiball Settings")]
    public int ballsToSpawn = 2;
    public float spawnDelay = 0.3f;
    public float spawnForce = 10f;

    [Header("References")]
    public BallController originalBall;
    public GameObject ballPrefab;
    public Transform[] spawnPoints;

    [Header("Activation")]
    public int hitsRequired = 5;
    public float activationWindow = 10f;

    [Header("Visual")]
    public ParticleSystem activationEffect;
    public GameObject multiballLight;

    [Header("Audio")]
    public AudioClip activationSound;
    public AudioClip spawnSound;

    private int _hitCount;
    private float _hitTimer;
    private bool _isMultiballActive;
    private bool _isSpawning;
    private List<BallController> _extraBalls = new List<BallController>();
    private AudioSource _audioSource;

    public bool IsMultiballActive => _isMultiballActive;
    public int HitsRemaining => Mathf.Max(0, hitsRequired - _hitCount);

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_isMultiballActive && _hitTimer > 0)
        {
            _hitTimer -= Time.deltaTime;
            if (_hitTimer <= 0)
                _hitCount = 0;
        }

        if (_isMultiballActive)
            CheckExtraBallsAlive();
    }

    public void RegisterHit()
    {
        if (_isMultiballActive) return;

        _hitCount++;
        _hitTimer = activationWindow;

        if (_hitCount >= hitsRequired)
        {
            ActivateMultiball();
        }
    }

    public void ActivateMultiball()
    {
        if (_isMultiballActive || _isSpawning) return;

        _isMultiballActive = true;
        _isSpawning = true;
        _hitCount = 0;

        if (activationEffect != null) activationEffect.Play();
        if (multiballLight != null) multiballLight.SetActive(true);
        if (_audioSource != null && activationSound != null)
            _audioSource.PlayOneShot(activationSound);

        StartCoroutine(SpawnBallsSequentially());
    }

    private System.Collections.IEnumerator SpawnBallsSequentially()
    {
        for (int i = 0; i < ballsToSpawn; i++)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnExtraBall(i);
        }
        _isSpawning = false;
    }

    private void SpawnExtraBall(int index)
    {
        if (ballPrefab == null) return;

        Transform spawnPoint = spawnPoints != null && index < spawnPoints.Length
            ? spawnPoints[index]
            : originalBall.transform.parent;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : originalBall.transform.position;
        GameObject newBall = Instantiate(ballPrefab, pos, Quaternion.identity);
        BallController ballCtrl = newBall.GetComponent<BallController>();

        if (ballCtrl != null)
        {
            _extraBalls.Add(ballCtrl);
            Vector3 randomDir = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f)).normalized;
            ballCtrl.LaunchBall(randomDir, spawnForce);
        }

        if (_audioSource != null && spawnSound != null)
            _audioSource.PlayOneShot(spawnSound, 0.8f);
    }

    private void CheckExtraBallsAlive()
    {
        _extraBalls.RemoveAll(b => b == null);

        if (_extraBalls.Count == 0)
        {
            _isMultiballActive = false;
            if (multiballLight != null) multiballLight.SetActive(false);
        }
    }

    public void EndMultiball()
    {
        foreach (var ball in _extraBalls)
        {
            if (ball != null) Destroy(ball.gameObject);
        }
        _extraBalls.Clear();
        _isMultiballActive = false;
        if (multiballLight != null) multiballLight.SetActive(false);
    }
}
