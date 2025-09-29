using Unity.Mathematics;
using UnityEngine;

public class BillBoard : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.forward);
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
