using UnityEngine;

public class WallController : MonoBehaviour, IPinballElement
{
    [Header("Wall Settings")]
    public float bounciness = 0.6f;
    public bool isGuideWall = false;

    [Header("Visual")]
    public MeshRenderer wallRenderer;
    public Color wallColor = new Color(0.3f, 0.3f, 0.5f);
    public Color glowColor = new Color(0.5f, 0.5f, 1f);
    public float glowIntensity = 0.5f;

    [Header("Audio")]
    public AudioClip wallHitSound;

    private AudioSource _audioSource;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        SetupCollider();
        UpdateVisuals();
    }

    private void SetupCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            PhysicMaterial mat = new PhysicMaterial("WallMaterial");
            mat.bounciness = bounciness;
            mat.bounceCombine = PhysicMaterialCombine.Average;
            mat.frictionCombine = PhysicMaterialCombine.Minimum;
            mat.dynamicFriction = 0.1f;
            mat.staticFriction = 0.1f;
            col.sharedMaterial = mat;
        }
    }

    public void OnBallHit(BallController ball, Collision collision)
    {
        if (!isGuideWall && wallHitSound != null && _audioSource != null)
        {
            float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10f);
            _audioSource.PlayOneShot(wallHitSound, volume * 0.5f);
        }
    }

    private void UpdateVisuals()
    {
        if (wallRenderer == null) return;

        wallRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor("_BaseColor", wallColor);
        _propertyBlock.SetColor("_EmissionColor", glowColor * glowIntensity);
        wallRenderer.SetPropertyBlock(_propertyBlock);
    }
}
