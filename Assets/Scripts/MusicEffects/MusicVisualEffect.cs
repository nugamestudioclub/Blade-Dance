using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class MusicVisualEffect : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public Image targetImage; // UI Image using your shader

    [Header("FFT Settings")]
    [Tooltip("FFT sample count. Must match the spectrum array size. Common: 256, 512, 1024.")]
    public int fftSize = 512;

    [Tooltip("FFT windowing function. BlackmanHarris is usually the cleanest for visuals.")]
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("Band Ranges (normalized bin fraction of FFT)")]
    [Tooltip("Low band range (bass). Interpreted as fractions of FFT bins [0..1].")]
    [Range(0f, 1f)] public float lowStart = 0.00f;
    [Range(0f, 1f)] public float lowEnd = 0.08f;

    [Tooltip("Mid band range.")]
    [Range(0f, 1f)] public float midStart = 0.08f;
    [Range(0f, 1f)] public float midEnd = 0.30f;

    [Tooltip("High band range (treble).")]
    [Range(0f, 1f)] public float highStart = 0.30f;
    [Range(0f, 1f)] public float highEnd = 1.00f;

    [Header("Response / Smoothing")]
    [Tooltip("Overall multiplier for visual strength.")]
    public float sensitivity = 2.0f;

    [Tooltip("How quickly values rise (bigger = snappier).")]
    public float attack = 30f;

    [Tooltip("How quickly values fall (bigger = faster decay).")]
    public float release = 8f;

    [Tooltip("How quickly the running average follows bass (lower = longer memory).")]
    public float avgFollow = 2f;

    [Tooltip("How strong the beat accent is (difference from average bass).")]
    public float accentGain = 6f;

    [Header("Mapping to Shader")]
    [Tooltip("Base bins when music is quiet.")]
    public float baseBins = 10f;

    [Tooltip("Extra bins added by mids (blockiness).")]
    public float binsFromMids = 30f;

    [Tooltip("Extra frequency added by highs (wiggliness).")]
    public float freqFromHighs = 20f;

    [Tooltip("Base gap size.")]
    public float baseGapSize = 0.20f;

    [Tooltip("How much the beat accent widens the gap.")]
    public float gapFromAccent = 0.25f;

    [Tooltip("Base gradient intensity.")]
    public float baseGradPow = 1.0f;

    [Tooltip("How much highs sharpen the gradient.")]
    public float gradPowFromHighs = 3.0f;
    [SerializeField] float baseFreq = 10f;

    // Internal
    private Material _material;
    private float[] _spectrumData;

    // Smoothed band values
    private float _low, _mid, _high;

    // Bass average + accent
    private float _avgLow;
    private float _accent;

    // Shader property IDs (faster than string lookups)
    private static readonly int AmpID = Shader.PropertyToID("_Amp");
    private static readonly int BinsID = Shader.PropertyToID("_Bins");
    private static readonly int FreqID = Shader.PropertyToID("_Freq");
    private static readonly int GapSizeID = Shader.PropertyToID("_GapSize");
    private static readonly int GradPowID = Shader.PropertyToID("_GradPow");
    // Optional extra property if you add it to the shader:
    private static readonly int AccentID = Shader.PropertyToID("_Accent");

    void Start()
    {
        if (targetImage != null)
        {
            _material = Instantiate(targetImage.material);
            targetImage.material = _material;
        }

        fftSize = Mathf.ClosestPowerOfTwo(Mathf.Clamp(fftSize, 64, 8192));
        _spectrumData = new float[fftSize];
    }

    void Update()
    {
        if (audioSource == null || _material == null) return;

        // 1) Spectrum analysis (cleaner window)
        audioSource.GetSpectrumData(_spectrumData, 0, fftWindow);

        // 2) Compute simple 3-band energies from spectrum
        float lowRaw = BandEnergy(lowStart, lowEnd);
        float midRaw = BandEnergy(midStart, midEnd);
        float highRaw = BandEnergy(highStart, highEnd);

        // Apply sensitivity
        lowRaw *= sensitivity;
        midRaw *= sensitivity;
        highRaw *= sensitivity;

        // 3) Attack/Release smoothing per band
        _low = SmoothAR(_low, lowRaw, attack, release);
        _mid = SmoothAR(_mid, midRaw, attack, release);
        _high = SmoothAR(_high, highRaw, attack, release);

        // 4) Bass running average + accent (cheap onset-ish)
        _avgLow = Mathf.Lerp(_avgLow, _low, Time.deltaTime * avgFollow);
        float accentRaw = Mathf.Max(0f, _low - _avgLow) * accentGain;
        _accent = SmoothAR(_accent, accentRaw, attack * 1.5f, release * 2f);

        // 5) Map bands to shader params
        // Amp: driven by bass (low) plus a little accent
        float amp = Mathf.Clamp01(_low * 0.5f + _accent * 0.2f);

        // Bins: driven by mids (more blocky on midrange activity)
        float bins = baseBins + _mid * binsFromMids;

        // Freq: add more rapid detail on highs
        float freq = baseFreq + _high * freqFromHighs;
        

        // Gap size: widen on accent (punch on kicks)
        float gapSize = Mathf.Clamp01(baseGapSize + _accent * gapFromAccent);

        // Gradient power: sharpen/brighten with highs
        float gradPow = Mathf.Clamp(baseGradPow + _high * gradPowFromHighs, 0.1f, 12f);

        // 6) Send to shader
        _material.SetFloat(AmpID, amp);
        _material.SetFloat(BinsID, Mathf.Max(1f, bins));
        _material.SetFloat(FreqID, freq);
        _material.SetFloat(GapSizeID, gapSize);
        _material.SetFloat(GradPowID, gradPow);

        // Optional: if you add a float property "_Accent" to the shader, you can use this too
        if (_material.HasProperty(AccentID))
            _material.SetFloat(AccentID, _accent);
    }

    // -------- Helpers --------

    float BandEnergy(float startFrac, float endFrac)
    {
        startFrac = Mathf.Clamp01(startFrac);
        endFrac = Mathf.Clamp01(endFrac);
        if (endFrac <= startFrac) return 0f;

        int start = Mathf.Clamp(Mathf.FloorToInt(startFrac * (fftSize - 1)), 0, fftSize - 1);
        int end = Mathf.Clamp(Mathf.CeilToInt(endFrac * (fftSize - 1)), 0, fftSize - 1);

        float sum = 0f;
        for (int i = start; i <= end; i++)
            sum += _spectrumData[i];

        float count = Mathf.Max(1, (end - start + 1));
        return sum / count; // average energy in the band
    }

    static float SmoothAR(float current, float target, float attack, float release)
    {
        float rate = (target > current) ? attack : release;
        return Mathf.Lerp(current, target, Time.deltaTime * rate);
    }
}
