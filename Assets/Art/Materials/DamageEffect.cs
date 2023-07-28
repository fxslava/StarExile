using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageEffect : MonoBehaviour
{
    [SerializeField] public int DamageMaskResolutionX = 128;
    [SerializeField] public int DamageMaskResolutionY = 128;
    [SerializeField] public int DamageMaskResolutionZ = 128;
    [SerializeField] public float damageHitRadius = 1.0f;
    [SerializeField] private ComputeShader _drawDamageShader;


    private Collider _collider;
    private RenderTexture _damageMaskTex3D = null;
    private int _kernelIndex;
    private uint _threadX;
    private uint _threadY;
    private uint _threadZ;
    Vector3 _dimenssionsScale;

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

        _collider = GetComponent<Collider>();

        _dimenssionsScale.x = 1.0f / _collider.bounds.extents.x;
        _dimenssionsScale.y = 1.0f / _collider.bounds.extents.y;
        _dimenssionsScale.z = 1.0f / _collider.bounds.extents.z;

        Vector3 invDimensions;
        invDimensions.x = 1.0f / DamageMaskResolutionX;
        invDimensions.y = 1.0f / DamageMaskResolutionY;
        invDimensions.z = 1.0f / DamageMaskResolutionZ;

        _drawDamageShader.SetVector("InvDimensions", invDimensions);

        _renderer = GetComponentInChildren<Renderer>();
        _renderer.material.SetTexture("_DamageTex", _damageMaskTex3D);
        _renderer.material.SetVector("_DamageGridInvExtents", new Vector4(_dimenssionsScale.x, _dimenssionsScale.y, _dimenssionsScale.z, 0f));
    }


    void DrawDamage(Vector3 localPoint, float intensity)
    {
        _drawDamageShader.SetFloat("Intensity", intensity);
        _drawDamageShader.SetFloat("Radius", damageHitRadius);
        _drawDamageShader.SetVector("Position", Vector3.Scale(localPoint, _dimenssionsScale));

        var threadGroupX = (int)(DamageMaskResolutionX / _threadX);
        var threadGroupY = (int)(DamageMaskResolutionY / _threadY);
        var threadGroupZ = (int)(DamageMaskResolutionZ / _threadZ);

        _drawDamageShader.SetTexture(_kernelIndex, "DamageMask", _damageMaskTex3D);

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
                var localHitPoint = _collider.transform.InverseTransformPoint(hit.point);
                DrawDamage(localHitPoint, 0.1f);
            }
        }

    }
}
