using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class serverUpdateJoints : MonoBehaviour
{
    public List<GameObject> spheres;
    public List<Transform> joints;
    public GameObject handViz;

    public int index = 2;
    float scale;
    // Start is called before the first frame update
    void Start()
    {
        if (handViz != null)
            scale = handViz.GetComponent<UnityEngine.XR.Hands.NetworkedHandVIz>().scale;
    }

    // Update is called once per frame
    void LateUpdate()
    {
         if (handViz != null && handViz.GetComponent<UnityEngine.XR.Hands.NetworkedHandVIz>().scale != scale)
            scale = handViz.GetComponent<UnityEngine.XR.Hands.NetworkedHandVIz>().scale;
        updateJoints();
        ScaleHand();
    }
   

    void updateJoints(){
        for(int i = 0; i < joints.Count; i++){
            // Debug.Log("Updating the server joints");
            joints[i].position = spheres[i + 2].transform.position;
            if (i > 0)
            {
                joints[i-1].LookAt(joints[i].position);
                joints[i-1].RotateAround(joints[i-1].position, joints[i-1].right, 90);
            }
            // joints[i].up = spheres[i + 2].transform.forward;
        }

    }

    void ScaleHand()
    {
        transform.parent.localScale = new Vector3(scale, scale, scale);
    }
}
