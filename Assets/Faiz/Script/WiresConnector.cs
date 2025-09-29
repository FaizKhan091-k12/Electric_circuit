using UnityEngine;

public class WiresConnector : MonoBehaviour
{
    public int clickCount = 0;
    [SerializeField] GameObject[] outlines;
    public AnimationPauseController animationPauseController;
    void OnMouseUpAsButton()
    {


        if (clickCount == 0)
        {
            animationPauseController.PlayFromStart();
            clickCount++;
            outlines[0].SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (clickCount == 1)
        {
            animationPauseController.ContinueAnimation();
            clickCount++;
            outlines[1].SetActive(false);
            GetComponent<BoxCollider>().enabled = false;

        }
        else if (clickCount == 2)
        {
            animationPauseController.ContinueAnimation();
            clickCount++;
            outlines[2].SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (clickCount == 3)
        {
            animationPauseController.ContinueAnimation();
            clickCount++;
            outlines[3].SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
        }

    }
    void Update()
    {
             if (outlines[0].activeInHierarchy || outlines[1].activeInHierarchy || outlines[2].activeInHierarchy || outlines[3].activeInHierarchy)
            GetComponent<BoxCollider>().enabled = true;

    }
}
