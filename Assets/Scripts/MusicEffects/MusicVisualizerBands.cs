using UnityEngine;
using UnityEngine.UI;

public class MusicVisualizerBands : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public Image targetImage;

    [Header("Bands")]
    [Range(8, 256)] public int bandCount = 64;   // number of equalizer columns
    public int fftSize = 1024;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("Response")]
    public float sensitivity = 12f;
    public float attack = 40f;
    public float release = 10f;

    [Header("Frequency Range (Hz)")]
    public float minHz = 40f;
    public float maxHz = 16000f;

    private Material _mat;
    private float[] _spectrum;
    private float[] _bands;          // smoothed band values
    private Texture2D _bandTex;
    private Color[] _pixels;

    static readonly int BandsTexID = Shader.PropertyToID("_BandsTex");
    static readonly int BandCountID = Shader.PropertyToID("_BandCount");

    void Start()
    {
        if (targetImage != null)
        {
            _mat = Instantiate(targetImage.material);
            targetImage.material = _mat;
        }

        fftSize = Mathf.ClosestPowerOfTwo(Mathf.Clamp(fftSize, 256, 8192));
        _spectrum = new float[fftSize];
        _bands = new float[bandCount];

        // 1D bands texture (RGBA32 is fine)
        _bandTex = new Texture2D(bandCount, 1, TextureFormat.RGBA32, false, true);
        _bandTex.wrapMode = TextureWrapMode.Clamp;
        _bandTex.filterMode = FilterMode.Point;

        _pixels = new Color[bandCount];

        if (_mat != null)
        {
            _mat.SetTexture(BandsTexID, _bandTex);
            _mat.SetFloat(BandCountID, bandCount);
        }
    }

    void Update()
    {
        if (audioSource == null || _mat == null) return;

        audioSource.GetSpectrumData(_spectrum, 0, fftWindow);

        float sampleRate = AudioSettings.outputSampleRate;
        int n = _spectrum.Length;

        // Build log-spaced bands
        for (int b = 0; b < bandCount; b++)
        {
            float t0 = (float)b / bandCount;
            float t1 = (float)(b + 1) / bandCount;

            float f0 = Mathf.Lerp(minHz, maxHz, Mathf.Pow(t0, 2.0f)); // bias toward lows
            float f1 = Mathf.Lerp(minHz, maxHz, Mathf.Pow(t1, 2.0f));

            int i0 = Mathf.Clamp(Mathf.FloorToInt(f0 / (sampleRate * 0.5f) * (n - 1)), 0, n - 1);
            int i1 = Mathf.Clamp(Mathf.CeilToInt(f1 / (sampleRate * 0.5f) * (n - 1)), 0, n - 1);

            float sum = 0f;
            int count = 0;
            for (int i = i0; i <= i1; i++)
            {
                sum += _spectrum[i];
                count++;
            }

            float raw = (count > 0) ? (sum / count) * sensitivity : 0f;

            // Attack/release smoothing per band
            float rate = (raw > _bands[b]) ? attack : release;
            _bands[b] = Mathf.Lerp(_bands[b], raw, Time.deltaTime * rate);

            // Optional clamp (keeps it sane)
            _bands[b] = Mathf.Clamp01(_bands[b]);

            // Store in texture (R channel is enough, but RGBA is fine)
            _pixels[b] = new Color(_bands[b], 0, 0, 1);
        }

        _bandTex.SetPixels(_pixels);
        _bandTex.Apply(false, false);
    }
}
