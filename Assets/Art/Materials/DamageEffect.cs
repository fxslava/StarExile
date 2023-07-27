using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageEffect : MonoBehaviour
{
    [SerializeField] public int DamageMaskResolutionX = 128;
    [SerializeField] public int DamageMaskResolutionY = 128;
    [SerializeField] public int DamageMaskResolutionZ = 128;
    [SerializeField] public float damageHitRadius = 1.0f;
    [SerializeField] private ComputeShader _drawDamageShader;
    [SerializeField] private Material _damageMaterial;


    private Collider _collider;
    private RenderTexture _damageMaskTex3D = null;
    private int _kernelIndex;
    private uint _threadX;
    private uint _threadY;
    private uint _threadZ;

    private Renderer _renderer;


    private void Awake()
    {
        var desc = new RenderTextureDescriptor(DamageMaskResolutionX, DamageMaskResolutionY, RenderTextureFormat.RFloat)
        {
            volumeDepth = DamageMaskResolutionZ,
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            enableRandomWrite = true
        };

        _damageMaskTex3D = new RenderTexture(desc);
        _damageMaskTex3D.Create();

        _kernelIndex = _drawDamageShader.FindKernel("DrawDamage");

        _drawDamageShader.GetKernelThreadGroupSizes(_kernelIndex, out _threadX, out _threadY, out _threadZ);
        _drawDamageShader.SetTexture(_kernelIndex, "DamageMask", _damageMaskTex3D);

        _collider = GetComponent<Collider>();

        Vector3 invDimensions;
        invDimensions.x = 1.0f / DamageMaskResolutionX;
        invDimensions.y = 1.0f / DamageMaskResolutionY;
        invDimensions.z = 1.0f / DamageMaskResolutionZ;

        _drawDamageShader.SetVector("InvDimensions", invDimensions);

        _renderer = GetComponent<Renderer>();
        _renderer.material = _damageMaterial;
        _renderer.material.SetTexture("_MainTex", _damageMaskTex3D); // TODO: Rename to damage texture 3d
    }


    void DrawDamage(Vector3 localPoint, float intensity)
    {
        Vector3 dimenssionsScale;
        dimenssionsScale.x = 1.0f / _collider.bounds.extents.x;
        dimenssionsScale.y = 1.0f / _collider.bounds.extents.y;
        dimenssionsScale.z = 1.0f / _collider.bounds.extents.z;

        _drawDamageShader.SetFloat("Intensity", intensity);
        _drawDamageShader.SetFloat("Radius", damageHitRadius);
        _drawDamageShader.SetVector("Position", Vector3.Scale(localPoint, dimenssionsScale));

        var threadGroupX = (int)(DamageMaskResolutionX / _threadX);
        var threadGroupY = (int)(DamageMaskResolutionY / _threadY);
        var threadGroupZ = (int)(DamageMaskResolutionZ / _threadZ);

        _renderer.material.SetVector("_CubeOrigin", transform.position);

        _drawDamageShader.Dispatch(_kernelIndex, threadGroupX, threadGroupY, threadGroupZ);
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (_collider.Raycast(ray, out hit, 100.0f))
            {
                var localHitPoint = transform.InverseTransformPoint(hit.point);
                DrawDamage(localHitPoint, 0.1f);
            }
        }

    }
}
