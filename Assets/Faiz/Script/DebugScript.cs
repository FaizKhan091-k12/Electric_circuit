using UnityEngine;

public class DebugScript : MonoBehaviour
{
    [SerializeField] float time = 3f;

    public bool isActive;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isActive = !isActive;

            if (isActive)
            {
                Time.timeScale = time;

            }
            else if (!isActive)
            {
                Time.timeScale = 1f;
            }
        }
    }
}
