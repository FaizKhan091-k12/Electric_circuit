using UnityEngine;
using DG.Tweening;

public class DialogueCanvas : MonoBehaviour
{

    [SerializeField] GameObject[] dialoguesContainer;
    void OnEnable()
    {
         gameObject.transform.localScale = Vector3.zero;

    }
    public void OnCanvasScale()
    {
        gameObject.transform.localScale = Vector3.zero;

        transform.DOScale(new Vector3(0.005f, 0.005f, 0.005f), .2f).SetEase(Ease.OutFlash);
        dialoguesContainer[0].SetActive(true);
    }
}
