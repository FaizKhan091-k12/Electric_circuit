using UnityEngine;
using TMPro;
using System.Collections;

public class WordByWordPopTMP : MonoBehaviour
{
    [Header("References")]
    public BoxCollider boxCollider;
    public OutlinePulseController outlinePulseController;
    public TMP_Text tmpText;
    public GameObject instruct; // parent or indicator to show
    public GameObject[] instructionPanel;

    [Header("Dialogue / Flow")]
    public bool[] whichDialogues; // decide which panel to show (index 0 -> instructionPanel[0], etc.)

    [Header("Typewriter Settings")]
    [TextArea]
    public string sentence = "This is a sample sentence for the typewriter effect.";
    [Tooltip("Delay between characters (seconds).")]
    public float charDelay = 0.05f;
    [Tooltip("If true, uses realtime wait (ignores Time.timeScale).")]
    public bool useRealtime = false;

    void Start()
    {
        // start the typewriter
        StartCoroutine(TypeSentenceCharByChar());
    }

    IEnumerator TypeSentenceCharByChar()
    {
        if (tmpText == null)
        {
            Debug.LogWarning("TMP_Text reference is null on " + gameObject.name);
            yield break;
        }

        tmpText.text = "";

        // iterate characters one by one
        for (int i = 0; i < sentence.Length; i++)
        {
            tmpText.text += sentence[i];

            if (useRealtime)
                yield return new WaitForSecondsRealtime(charDelay);
            else
                yield return new WaitForSeconds(charDelay);
        }

        // Ensure final text is exact
        tmpText.text = sentence;

        Invoke(nameof(WhatWEe), 1f);
    }

    private void WhatWEe()
    {
        // enable collider if assigned
        if (boxCollider != null)
            boxCollider.enabled = true;

        // play outline pulse if assigned
        if (outlinePulseController != null)
            outlinePulseController.Play();

        // handle instruction panels safely
        if (whichDialogues != null && whichDialogues.Length > 0 && whichDialogues[0])
        {
            // Show instruct root if provided
            if (instruct != null)
                instruct.SetActive(false);

            // if panels exist, disable all then enable the first (or corresponding) panel
            if (instructionPanel != null && instructionPanel.Length > 0)
            {
                for (int p = 0; p < instructionPanel.Length; p++)
                {
                    if (instructionPanel[p] != null)
                        instructionPanel[p].SetActive(false);
                }

                // Enable the first panel safely
                int idxToShow = 0;
                if (idxToShow >= 0 && idxToShow < instructionPanel.Length && instructionPanel[idxToShow] != null)
                    instructionPanel[idxToShow].SetActive(true);
            }
        }
    }

    // Optional public helper to set text and restart typing
    public void PlayWithText(string newSentence)
    {
        sentence = newSentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentenceCharByChar());
    }
}
