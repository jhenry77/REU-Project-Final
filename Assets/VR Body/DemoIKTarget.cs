using UnityEngine;
using UnityEngine.XR;
using Mirror;



public class DemoIKTarget : MonoBehaviour
{
    [Range(0,1)]
    public float turnSmoothness = 1f;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;


    

    // Update is called once per frame
    void Update()
    {
       
        
        // transform.position = head.ikTarget.position + headBodyPositionOffset;
        // float yaw = head.vrTarget.eulerAngles.y;
        // transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z),turnSmoothness);
        

        // head.Map();
        // leftHand.Map();
        // rightHand.Map();
    }


    
}
