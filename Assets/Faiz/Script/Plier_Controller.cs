using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

public class Plier_Controller : MonoBehaviour
{
    public Animator anim;
    public OutlinePulseController outlinePulseController;

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
        
    }
}
