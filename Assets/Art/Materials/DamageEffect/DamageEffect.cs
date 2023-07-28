using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageEffect : MonoBehaviour
{
    [SerializeField] public int DamageMaskResolutionX = 128;
    [SerializeField] public int DamageMaskResolutionY = 128;
    [SerializeField] public int DamageMaskResolutionZ = 128;
    [SerializeField] public float DamageFade = 0.99f;
    [SerializeField] public float DamageHoleThreashold = 0.99f;
    [SerializeField] private ComputeShader _drawDamageShader;


    private Renderer _renderer;
    private Collider _collider;
    private RenderTexture _damageMaskTex3D = null;
    private int _drawDamageKernelIndex;
    private Vector3 _dimenssionsScale;
    private Vector3 _invDimensions;
    private int _threadGroupX;
    private int _threadGroupY;
    private int _threadGroupZ;


    private void Awake()
    {
        var desc = new RenderTextureDescriptor(DamageMaskResolutionX, DamageMaskResolutionY, RenderTextureFormat.RGFloat)
        {
            volumeDepth = DamageMaskResolutionZ,
            dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
            enableRandomWrite = true
        };

        _damageMaskTex3D = new RenderTexture(desc);
        _damageMaskTex3D.Create();

        _drawDamageKernelIndex = _drawDamageShader.FindKernel("DrawDamage");

        uint threadX, threadY, threadZ;
        _drawDamageShader.GetKernelThreadGroupSizes(_drawDamageKernelIndex, out threadX, out threadY, out threadZ);
        _threadGroupX = (int)(DamageMaskResolutionX / threadX);
        _threadGroupY = (int)(DamageMaskResolutionY / threadY);
        _threadGroupZ = (int)(DamageMaskResolutionZ / threadZ);

        _collider = GetComponent<Collider>();

        _dimenssionsScale.x = 1.0f / _collider.bounds.extents.x;
        _dimenssionsScale.y = 1.0f / _collider.bounds.extents.y;
        _dimenssionsScale.z = 1.0f / _collider.bounds.extents.z;
        _invDimensions.x = 1.0f / DamageMaskResolutionX;
        _invDimensions.y = 1.0f / DamageMaskResolutionY;
        _invDimensions.z = 1.0f / DamageMaskResolutionZ;

        _renderer = GetComponentInChildren<Renderer>();
        _renderer.material.SetTexture("_DamageTex", _damageMaskTex3D);
        _renderer.material.SetVector("_DamageGridInvExtents", new Vector4(_dimenssionsScale.x, _dimenssionsScale.y, _dimenssionsScale.z, 0f));
        _renderer.material.SetFloat("_HoleThreashold", DamageHoleThreashold);
    }


    public void DrawDamage(Vector3 localPoint, float radius, float intensity)
    {
        _drawDamageShader.SetFloat("Fade", 1.0f);
        _drawDamageShader.SetFloat("Intensity", intensity);
        _drawDamageShader.SetFloat("Radius", radius);
        _drawDamageShader.SetVector("Position", Vector3.Scale(localPoint, _dimenssionsScale));
        _drawDamageShader.SetVector("InvDimensions", _invDimensions);
        _drawDamageShader.SetTexture(_drawDamageKernelIndex, "DamageMask", _damageMaskTex3D);

        _drawDamageShader.Dispatch(_drawDamageKernelIndex, _threadGroupX, _threadGroupY, _threadGroupZ);
    }

    private void Fade()
    {
        _drawDamageShader.SetFloat("Fade", DamageFade);
        _drawDamageShader.SetFloat("Intensity", 0.0f);
        _drawDamageShader.SetTexture(_drawDamageKernelIndex, "DamageMask", _damageMaskTex3D);

        _drawDamageShader.Dispatch(_drawDamageKernelIndex, _threadGroupX, _threadGroupY, _threadGroupZ);
    }


    private void Update()
    {
        Fade();
    }
}
