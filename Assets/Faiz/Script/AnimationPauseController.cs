using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class AnimationPauseController : MonoBehaviour
{
    [Header("Animator Setup")]
    public Animator animator;
    [Tooltip("Name of the animation state in Animator Controller (state name, not necessarily clip name).")]
    public string stateName = "MyAnimation";

    [Header("Pause Points (0 = start, 1 = end)")]
    [Range(0f, 1f)] public List<float> pausePoints = new List<float>() { 0.25f, 0.5f, 0.75f };

    private int currentPauseIndex = 0;
    private bool isPlaying = false;
    private Coroutine pauseChecker;

    void Reset()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        // ensure list sorted and clamped
        pausePoints.Sort();
        for (int i = 0; i < pausePoints.Count; i++) pausePoints[i] = Mathf.Clamp01(pausePoints[i]);
    }

    /// <summary>
    /// Start playing animation from the beginning and auto-pause at the first checkpoint.
    /// </summary>
    public void PlayFromStart()
    {
        if (animator == null)
        {
            Debug.LogWarning("AnimationPauseController: Animator not assigned.");
            return;
        }

        currentPauseIndex = 0;

        // jump to the state at normalized time 0
        int layer = 0;
        animator.Play(stateName, layer, 0f);
        animator.speed = .8f;
        isPlaying = true;

        // stop any previous coroutine and start a new checker
        if (pauseChecker != null) StopCoroutine(pauseChecker);
        pauseChecker = StartCoroutine(CheckForPausesCoroutine());
    }

    /// <summary>
    /// Call to continue to next pause point.
    /// </summary>
    public void ContinueAnimation()
    {
        if (animator == null) return;

        if (isPlaying)
        {
            // already playing; nothing to do
            return;
        }

        if (currentPauseIndex >= pausePoints.Count)
        {
            // no more pause points: just resume normally
            animator.speed = 1f;
            isPlaying = true;
            return;
        }

        // resume playback and restart checker for the next checkpoint
        animator.speed = 1f;
        isPlaying = true;
        if (pauseChecker != null) StopCoroutine(pauseChecker);
        pauseChecker = StartCoroutine(CheckForPausesCoroutine());
    }

    private IEnumerator CheckForPausesCoroutine()
    {
        // let animator update for one frame so the state actually becomes active
        yield return null;

        if (animator == null) yield break;

        int stateHash = Animator.StringToHash(stateName);
        int layer = 0;

        // Wait until animator reports we're in the correct state
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        int maxWaitFrames = 5; // small safeguard
        int waited = 0;
        while (stateInfo.shortNameHash != stateHash && waited < 60) // wait up to 60 frames to enter state
        {
            yield return null;
            waited++;
            stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        }

        // If we never entered the requested state, exit
        if (stateInfo.shortNameHash != stateHash)
        {
            Debug.LogWarning($"AnimationPauseController: animator did not enter state '{stateName}'.");
            yield break;
        }

        // If no pause points, do nothing (play normally)
        if (pausePoints == null || pausePoints.Count == 0)
        {
            yield break;
        }

        // Wait until we reach the current checkpoint (use fractional normalizedTime to handle wraps)
        float target = Mathf.Clamp01(pausePoints[Mathf.Clamp(currentPauseIndex, 0, pausePoints.Count - 1)]);
        while (true)
        {
            if (animator == null) yield break;

            stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

            // Only consider time when we are in the correct state
            if (stateInfo.shortNameHash == stateHash)
            {
                // fractional normalized time in [0,1)
                float tFrac = stateInfo.normalizedTime % 1f;
                // If animation is non-looping normalizedTime can be >1 when finished; clamp then
                if (stateInfo.normalizedTime >= 1f && !animator.IsInTransition(layer))
                {
                    // final fallback: if target is 1, pause at end
                    if (Mathf.Approximately(target, 1f))
                    {
                        animator.speed = 0f;
                        isPlaying = false;
                        currentPauseIndex++;
                        yield break;
                    }
                }

                if (tFrac >= target)
                {
                    // Pause the animator at this checkpoint
                    animator.speed = 0f;
                    isPlaying = false;
                    currentPauseIndex++;
                    yield break;
                }
            }

            yield return null;
        }
    }
}
