using UnityEngine;
using UnityEngine.UI; // For Text UI
using TMPro;

public class DialogueManager : MonoBehaviour
{
 public GameObject[] dialogues; // Assign Dialogue1, Dialogue2, Dialogue3... in Inspector
    private int currentIndex = 0;

    void Start()
    {
        // HideAll();
        // if (dialogues.Length > 0) ShowDialogue(0); // Show first
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            NextDialogue();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
           // ShowDialogue(0);
         }
    }

//Call this function to show like this -> dialogue ShowDialogue(0);
    // public void ShowDialogue(int index)
    // {
    //     if (index >= 0 && index < dialogues.Length)
    //     {
    //         HideAll();
    //         dialogues[index].gameObject.SetActive(true);
    //         currentIndex = index;

    //         // Try to trigger WordByWordPopTMP typewriter effect
    //         var typewriter = dialogues[index].GetComponentInChildren<WordByWordPopTMP>();
    //         if (typewriter != null)
    //         {
    //             // Stop any running coroutine and restart
    //             typewriter.StopAllCoroutines();
    //             typewriter.StartCoroutine(typewriter.TypeSentence());
    //         }
    //     }
    // }

    public void NextDialogue()
    {  
         if (currentIndex == 2)
        {
            currentIndex = -1;
        }
    
        //ShowDialogue(currentIndex + 1);
    }

    private void HideAll()
    {
        foreach (var d in dialogues) d.gameObject.SetActive(false);
    }
}
