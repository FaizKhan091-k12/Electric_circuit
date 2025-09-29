using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Plier_Controller : MonoBehaviour
{
    public GameObject dialogue1, dialogue2;

    public UnityEvent plierEvent;
    public Animator anim;
    public OutlinePulseController outlinePulseController;
    public AnimationClip wire_Anim;
    public GameObject[] Lables;
    void OnMouseUpAsButton()
    {
        foreach (var item in Lables)
        {
            item.SetActive(false);
        }
        outlinePulseController.Stop();
        outlinePulseController.SetWidthRange(0, 0);
        anim.SetTrigger("Plier");
        plierEvent.Invoke();

    }

    public void SecondDialogueON()
    {
        dialogue1.SetActive(false);
        dialogue2.SetActive(true);
    }
  
}
