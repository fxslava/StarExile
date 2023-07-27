using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketScript : MonoBehaviour
{
    [Header("Propulsion system")]
    [SerializeField] public float Thrust = 10.0f;
    [SerializeField] public GameObject EngineParticleSystem = null;

    [Header("Guidance system")]
    [SerializeField][Range(0f, 1f)] public float AimingRateFactor = 0.5f;
    [SerializeField] public Targetable Target = null;

    [Header("Detonator")]
    [SerializeField] public float SelfDestructTimeout = 10.0f;
    [SerializeField] public GameObject ExplosionParticleSystem = null;

    [Header("General")]
    [SerializeField] public MeshRenderer render = null;
    [SerializeField] public float DamageAmount = 10;


    private Rigidbody _rigidbody;
    private float _selfDestrutionTime = 0f;
    private bool _isExplodes = false;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _selfDestrutionTime = 0f;
        ExplosionParticleSystem.SetActive(false);
        _isExplodes = false;
    }


    private void FixedUpdate()
    {
        if (_isExplodes)
            return;

        var currentDirection = transform.forward;

        if (Target && Target.IsExist())
        {
            var currentPosition = transform.position;
            var targetPosition = Target.GetPosition();
            var toTargetDirection = targetPosition - currentPosition;

            transform.forward = Vector3.Normalize(Vector3.Lerp(currentDirection, toTargetDirection, AimingRateFactor));
        }

        _selfDestrutionTime += Time.fixedDeltaTime;

        if (_selfDestrutionTime > SelfDestructTimeout)
        {
            Explode();
        }

        _rigidbody.AddForce(Thrust * currentDirection);
    }


    private void OnTriggerEnter(Collider other)
    {
        var destroyable = other.gameObject.GetComponent<Destroyable>();
        if (destroyable != null)
        {
            destroyable.ApplyDamage(DamageAmount);
        }

        Explode();
    }

    private void Explode()
    {
        _isExplodes = true;
        ExplosionParticleSystem.transform.position = transform.position;
        ExplosionParticleSystem.SetActive(true);
        var particleSystem = ExplosionParticleSystem.GetComponent<ParticleSystem>();
        particleSystem.Play();

        EngineParticleSystem.SetActive(false);

        Destroy(gameObject, particleSystem.main.duration);

        render.enabled = false;
    }
}
