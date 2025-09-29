using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Outline))]
public class OutlinePulseController : MonoBehaviour
{
    [Header("Target Outline")]
    [Tooltip("If left null, will try to get an Outline on this GameObject.")]
    public Outline targetOutline;

    [Header("Appearance")]
    public Color outlineColor = Color.white;
    public Outline.Mode outlineMode = Outline.Mode.OutlineAll;

    [Header("Width Range")]
    [Min(0f)] public float minWidth = 0f;
    [Min(0f)] public float maxWidth = 5f;

    [Header("Animation")]
    [Tooltip("Time (seconds) to go from min->max then max->min (if PingPong).")]
    [Min(0.01f)] public float duration = 1.0f;
    public bool pingPong = true;
    [Tooltip("If true, auto start the pulsing on Start().")]
    public bool playOnStart = true;

    [Header("Easing")]
    [Tooltip("Curve evaluating 0..1 input. Use for ease-in/out effects.")]
    public AnimationCurve ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // runtime
    Coroutine pulseCoroutine;

    void Reset()
    {
        // sensible defaults
        duration = 1f;
        minWidth = 0f;
        maxWidth = 2f;
        ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    void Awake()
    {
        if (targetOutline == null)
        {
            targetOutline = GetComponent<Outline>();
            targetOutline.OutlineWidth = 0f;
        }

        if (targetOutline == null)
            Debug.LogWarning($"OutlinePulseController on '{gameObject.name}' has no Outline assigned.");
    }

    void Start()
    {
        ApplyAppearance();

        if (playOnStart)
            Play();
    }

    // void OnValidate()
    // {
    //     // clamp widths consistent so inspector changes take effect immediately
    //     if (minWidth > maxWidth)
    //     {
    //         float tmp = minWidth;
    //         minWidth = maxWidth;
    //         maxWidth = tmp;
    //     }

    //     if (targetOutline == null)
    //         targetOutline = GetComponent<Outline>();

    //     // update applied appearance in editor
    //     if (Application.isPlaying)
    //     {
    //         ApplyAppearance();
    //     }
    //     else
    //     {
    //         // in editor mode when changing values, try to update Outline so you see preview while editing
    //         if (targetOutline != null)
    //         {
    //             targetOutline.OutlineColor = outlineColor;
    //             targetOutline.OutlineMode = outlineMode;
    //             targetOutline.OutlineWidth = minWidth; // show min by default
    //         }
    //     }
    // }

    /// <summary>
    /// Apply static appearance values (color, mode) immediately.
    /// </summary>
    public void ApplyAppearance()
    {
        if (targetOutline == null) return;

        targetOutline.OutlineColor = outlineColor;
        targetOutline.OutlineMode = outlineMode;
    }

    /// <summary>
    /// Start pulsing. If already running, it restarts with current params.
    /// </summary>
    public void Play()
    {
        if (targetOutline == null) return;

        Stop();
        pulseCoroutine = StartCoroutine(PulseCoroutine());
    }

    /// <summary>
    /// Stop pulsing and leaves the current width as-is (or optionally set to min).
    /// </summary>
    public void Stop(bool setToMin = false)
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        if (targetOutline != null && setToMin)
            targetOutline.OutlineWidth = minWidth;
    }

    IEnumerator PulseCoroutine()
    {
        if (targetOutline == null) yield break;

        // ensure duration is not zero
        float halfDuration = Mathf.Max(0.0001f, duration * 0.5f);

        // We'll animate t from 0->1 then (if pingPong) back 1->0 repeatedly.
        while (true)
        {
            // forward: min -> max
            yield return AnimateWidth(minWidth, maxWidth, halfDuration);

            if (!pingPong)
            {
                // if not pingpong, we can optionally go directly back (max->min) within the same duration,
                // but user asked explicitly min-max then max-min, so we'll always do both directions.
                // Here, when pingPong==false, just continue to reverse anyway to satisfy "min-max and max-min".
            }

            // backward: max -> min
            yield return AnimateWidth(maxWidth, minWidth, halfDuration);
        }
    }

    IEnumerator AnimateWidth(float from, float to, float time)
    {
        if (targetOutline == null) yield break;

        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            float e = ease.Evaluate(t);
            float width = Mathf.Lerp(from, to, e);
            targetOutline.OutlineWidth = width;
            yield return null;
        }

        // ensure exact final
        targetOutline.OutlineWidth = to;
    }

    // --- Convenience runtime setters ---

    public void SetOutlineColor(Color c)
    {
        outlineColor = c;
        if (targetOutline != null) targetOutline.OutlineColor = c;
    }

    public void SetOutlineMode(Outline.Mode mode)
    {
        outlineMode = mode;
        if (targetOutline != null) targetOutline.OutlineMode = mode;
    }

    public void SetWidthRange(float min, float max)
    {
        minWidth = Mathf.Max(0f, min);
        maxWidth = Mathf.Max(minWidth, max);
        if (targetOutline != null)
            targetOutline.OutlineWidth = minWidth;
    }

    public void SetDuration(float seconds)
    {
        duration = Mathf.Max(0.01f, seconds);
    }

    // Toggle play/pause
    public void Toggle()
    {
        if (pulseCoroutine == null) Play(); else Stop();
    }
}
