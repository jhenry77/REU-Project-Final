using System.Collections;
using System.Collections.Generic;
// using Unity.Mathematics;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

using UnityEngine;
using Mirror;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using TMPro;
using System.IO;
using Unity.XR.CoreUtils;
using JetBrains.Annotations;
using Unity.XR.CoreUtils.GUI;
using UnityEngine.UIElements;
public enum handSize{
        Small, Fitted, Large
    }
public class PositionRotation
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public int FrameNumber { get; set; }
    public float deltaTime { get; set; }

    public Vector3 localPosition {get; set;}
    public Quaternion localRotation {get; set;} 

   

    public handSize thisFrameHandSize;

    public PositionRotation(Vector3 position, Quaternion rotation, int frameNumber, Vector3 localPos, Quaternion localRot, handSize hand, float dTime)
    {
        Position = position;
        Rotation = rotation;
        FrameNumber = frameNumber;
        deltaTime = dTime;
        localPosition = localPos;
        localRotation = localRot;
        thisFrameHandSize = hand;
    }
}


public class playerPositionData{
    public Transform Camera { get; set; }
    public Transform leftHand{ get; set; }
    public Transform rightHand{ get; set; }
   

    // Left Thumb Joints
    public Transform LeftThumbMetacarpal { get; set; }
    public Transform LeftThumbProximal { get; set; }
    public Transform LeftThumbDistal { get; set; }
    public Transform LeftThumbTip{ get; set; }
    // Left Ring Joints
     public Transform LeftRingMetacarpal { get; set; }
    public Transform LeftRingProximal { get; set; }
    public Transform LeftRingIntermediate { get; set; }
    public Transform LeftRingDistal { get; set; }
    public Transform LeftRingTip{ get; set; }
    
    // Left Middle Joints
     public Transform LeftMiddleMetacarpal { get; set; }
    public Transform LeftMiddleProximal { get; set; }
    public Transform LeftMiddleIntermediate { get; set; }
    public Transform LeftMiddleDistal { get; set; }
    public Transform LeftMiddleTip{ get; set; }
    // Left  Index Joints
    public Transform LeftIndexTip{ get; set; }
    public Transform LeftIndexMetacarpal { get; set; }
    public Transform LeftIndexProximal { get; set; }
    public Transform LeftIndexIntermediate { get; set; }
    public Transform LeftIndexDistal { get; set; }
    // Left Pinky Joints
     public Transform LeftPinkyMetacarpal { get; set; }
    public Transform LeftPinkyProximal { get; set; }
    public Transform LeftPinkyIntermediate { get; set; }
    public Transform LeftPinkyDistal { get; set; }
    public Transform LeftPinkyTip{ get; set; }
    // Right  Thumb Joints
    public Transform RightThumbMetacarpal { get; set; }
    public Transform RightThumbProximal { get; set; }
    public Transform RightThumbDistal { get; set; }
    public Transform RightThumbTip{ get; set; }
    // Right  Ring Joints
    public Transform RightRingMetacarpal { get; set; }
    public Transform RightRingProximal { get; set; }
    public Transform RightRingIntermediate { get; set; }
    public Transform RightRingDistal { get; set; }
    public Transform RightRingTip{ get; set; }
    // Right Middle Joints
    public Transform RightMiddleMetacarpal { get; set; }
    public Transform RightMiddleProximal { get; set; }
    public Transform RightMiddleIntermediate { get; set; }
    public Transform RightMiddleDistal { get; set; }
    public Transform RightMiddleTip{ get; set; }
    // Right Index Joints
     public Transform RightIndexMetacarpal { get; set; }
    public Transform RightIndexProximal { get; set; }
    public Transform RightIndexIntermediate { get; set; }
    public Transform RightIndexDistal { get; set; }
    public Transform RightIndexTip{ get; set; }
    
    // Right Pinky Joints
     public Transform RightPinkyMetacarpal { get; set; }
    public Transform RightPinkyProximal { get; set; }
    public Transform RightPinkyIntermediate { get; set; }
    public Transform RightPinkyDistal { get; set; }
    public Transform RightPinkyTip{ get; set; }

}
public class motionTracking : MonoBehaviour
{

    public handSize currentHandSize = handSize.Fitted;

    public GameObject player1;
    public GameObject player2;

    public playerPositionData p1 = new playerPositionData();
    public playerPositionData p2 = new playerPositionData();


    private List<PositionRotation[]> player1Data;
    private List<PositionRotation[]> player2Data;

    public GameObject networkMan;
    string outputName = "";
    string header = "";
    private int frameNumber;

    public float deltaTime;
    string[] parts = new string[] {
                "Camera", "LeftHand", "RightHand",
                "LeftThumbMetacarpal", "LeftThumbProximal", "LeftThumbDistal", "LeftThumbTip",
                "LeftIndexMetacarpal", "LeftIndexProximal", "LeftIndexIntermediate", "LeftIndexDistal", "LeftIndexTip",
                "LeftMiddleMetacarpal", "LeftMiddleProximal", "LeftMiddleIntermediate", "LeftMiddleDistal", "LeftMiddleTip",
                "LeftRingMetacarpal", "LeftRingProximal", "LeftRingIntermediate", "LeftRingDistal", "LeftRingTip",
                "LeftPinkyMetacarpal", "LeftPinkyProximal", "LeftPinkyIntermediate", "LeftPinkyDistal", "LeftPinkyTip",
                "RightThumbMetacarpal", "RightThumbProximal", "RightThumbDistal", "RightThumbTip",
                "RightIndexMetacarpal", "RightIndexProximal", "RightIndexIntermediate", "RightIndexDistal", "RightIndexTip",
                "RightMiddleMetacarpal", "RightMiddleProximal", "RightMiddleIntermediate", "RightMiddleDistal", "RightMiddleTip",
                "RightRingMetacarpal", "RightRingProximal", "RightRingIntermediate", "RightRingDistal", "RightRingTip",
                "RightPinkyMetacarpal", "RightPinkyProximal", "RightPinkyIntermediate", "RightPinkyDistal", "RightPinkyTip"
            };

    

    // Start is called before the first frame update
    void Start()
    {
        frameNumber = 0;
        
        outputName = "MotionData_Participant1-PID" + networkMan.GetComponent<myNetworkManager>().player1PID.ToString() + "_Participant2-PID" + networkMan.GetComponent<myNetworkManager>().player2PID.ToString() + ".csv";

        // create header
        header = "FrameNumber, Player, deltaTime, Size,"; 

        foreach(string part in parts)
        {
            header += part + "_pX,"; //  to Jackson from Ryan: apparently modifying strings in this way is bad practice
            header += part + "_pY,";
            header += part + "_pZ,";
            header += part + "_rX,";
            header += part + "_rY,";
            header += part + "_rZ,";
            header += part + "_rW,";
            header += part + "_lpX,"; 
            header += part + "_lpY,";
            header += part + "_lpZ,";
            header += part + "_lrX,";
            header += part + "_lrY,";
            header += part + "_lrZ,";
            header += part + "_lrW,";
        }

        header = header.Remove(header.Length-1); // remove the last comma
    }

    void OnApplicationQuit()
    {
        
        if (player1Data == null || player2Data == null)
        {
            print("No motion data to save.");
            return;
        }
        // string path = "D:" + "CharadeLogs/motionTrackingStart.csv ";
        string path = "D:" + "CharadeLogs/" + outputName;
        Debug.Log("Saving session data to: " + path);
        
        using (StreamWriter writer = new StreamWriter(path))
        {
            // Write headers
            writer.WriteLine(header);
            //writer.WriteLine("FrameNumber, Player, Part,PositionX,PositionY,PositionZ,RotationX,RotationY,RotationZ,RotationW,LocalPositionX,LocalPositionY,LocalPositionZ,LocalRotationX,LocalRotationY,LocalRotationZ,LocalRotationW,detlaTime");

            // Write Player 1 Data
            for (int i = 0; i < player1Data.Count; i++)
            {
                WritePlayerData(writer, player1Data[i], 1);
            }

            // Write Player 2 Data
            for (int i = 0; i < player2Data.Count; i++)
            {
                WritePlayerData(writer, player2Data[i], 2);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;
        frameNumber++;
        if (player1 != null){
            AppendCurrentFrameData(player1Data, p1);
        }

        if (player2 != null){
            AppendCurrentFrameData(player2Data, p2);
        }
    }

    public void setPlayer1(GameObject player)
    {
        player1 = player;
        player1Data = new List<PositionRotation[]>();
        p1.Camera = player1.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(0);
        p1.leftHand = player1.transform.GetChild(0).GetChild(3).GetChild(2).GetChild(0);
        p1.rightHand = player1.transform.GetChild(0).GetChild(3).GetChild(3).GetChild(0);

        // Left Thumb Joints
        p1.LeftThumbMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(2);// Replace with correct path
        p1.LeftThumbProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(3);// Replace with correct path
        p1.LeftThumbDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(4);
        p1.LeftThumbTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(5);
        // Left Index Joints
        p1.LeftIndexMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(6);// Replace with correct path
        p1.LeftIndexProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(7);
        p1.LeftIndexIntermediate = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(8); // Replace with correct path
        p1.LeftIndexDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(9);
        p1.LeftIndexTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(10);
        // Left Middle Joints
        p1.LeftMiddleMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(11); // Replace with correct path
        p1.LeftMiddleProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(12); // Replace with correct path
        p1.LeftMiddleIntermediate = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(13); // Replace with correct path
        p1.LeftMiddleDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(14);
        p1.LeftMiddleTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(15);
        // Left Ring Joints
         p1.LeftRingMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(16);// Replace with correct path
        p1.LeftRingProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(17); // Replace with correct path
        p1.LeftRingIntermediate = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(18); // Replace with correct path
        p1.LeftRingDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(19);
        p1.LeftRingTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(20);
        // Left Pinky Joints
        p1.LeftPinkyMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(21);// Replace with correct path
        p1.LeftPinkyProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(22);// Replace with correct path
        p1.LeftPinkyIntermediate = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(23); // Replace with correct path
        p1.LeftPinkyDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(24);
        p1.LeftPinkyTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(25);
        // Right Thumb Joints
        p1.RightThumbMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(2);// Replace with correct path
        p1.RightThumbProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(3); // Replace with correct path
        p1.RightThumbDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(4);
        p1.RightThumbTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(5);
        // Right Index Joints
        p1.RightIndexMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(6);
        p1.RightIndexProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(7); // Replace with correct path
        p1.RightIndexIntermediate = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(8); // Replace with correct path
        p1.RightIndexDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(9);
        p1.RightIndexTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(10);
        // Right Middle JOints
        p1.RightMiddleMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(11); // Replace with correct path
        p1.RightMiddleProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(12); // Replace with correct path
        p1.RightMiddleIntermediate = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(13); // Replace with correct path
        p1.RightMiddleDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(14);
        p1.RightMiddleTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(15);
        // Right Rings Joints
        p1.RightRingMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(16); // Replace with correct path
        p1.RightRingProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(17); // Replace with correct path
        p1.RightRingIntermediate = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(18);// Replace with correct path
        p1.RightRingDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(19); 
        p1.RightRingTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(20);
        // Right Pinky Joints
        p1.RightPinkyMetacarpal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(21); // Replace with correct path
        p1.RightPinkyProximal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(22); // Replace with correct path
        p1.RightPinkyIntermediate = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(23); // Replace with correct path
        p1.RightPinkyDistal = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(24);
        p1.RightPinkyTip = player1.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(25);
    }

    public void setPlayer2(GameObject player)
    {
        player2 = player;
        player2Data = new List<PositionRotation[]>();
        p2.Camera = player2.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(0);
        p2.leftHand = player2.transform.GetChild(0).GetChild(3).GetChild(2).GetChild(0);
        p2.rightHand = player2.transform.GetChild(0).GetChild(3).GetChild(3).GetChild(0);

        // Left Thumb Joints
        p2.LeftThumbMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(2);// Replace with correct path
        p2.LeftThumbProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(3);// Replace with correct path
        p2.LeftThumbDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(4);
        p2.LeftThumbTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(5);
        // Left Index Joints
        p2.LeftIndexMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(6);// Replace with correct path
        p2.LeftIndexProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(7);
        p2.LeftIndexIntermediate = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(8); // Replace with correct path
        p2.LeftIndexDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(9);
        p2.LeftIndexTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(10);
        // Left Middle Joints
        p2.LeftMiddleMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(11); // Replace with correct path
        p2.LeftMiddleProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(12); // Replace with correct path
        p2.LeftMiddleIntermediate = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(13); // Replace with correct path
        p2.LeftMiddleDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(14);
        p2.LeftMiddleTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(15);
        // Left Ring Joints
         p2.LeftRingMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(16);// Replace with correct path
        p2.LeftRingProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(17); // Replace with correct path
        p2.LeftRingIntermediate = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(18); // Replace with correct path
        p2.LeftRingDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(19);
        p2.LeftRingTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(20);
        // Left Pinky Joints
        p2.LeftPinkyMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(21);// Replace with correct path
        p2.LeftPinkyProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(22);// Replace with correct path
        p2.LeftPinkyIntermediate = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(23); // Replace with correct path
        p2.LeftPinkyDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(24);
        p2.LeftPinkyTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(25);
        // Right Thumb Joints
        p2.RightThumbMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(2);// Replace with correct path
        p2.RightThumbProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(3); // Replace with correct path
        p2.RightThumbDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(4);
        p2.RightThumbTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(5);
        // Right Index Joints
        p2.RightIndexMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(6);
        p2.RightIndexProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(7); // Replace with correct path
        p2.RightIndexIntermediate = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(8); // Replace with correct path
        p2.RightIndexDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(9);
        p2.RightIndexTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(10);
        // Right Middle JOints
        p2.RightMiddleMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(11); // Replace with correct path
        p2.RightMiddleProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(12); // Replace with correct path
        p2.RightMiddleIntermediate = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(13); // Replace with correct path
        p2.RightMiddleDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(14);
        p2.RightMiddleTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(15);
        // Right Rings Joints
        p2.RightRingMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(16); // Replace with correct path
        p2.RightRingProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(17); // Replace with correct path
        p2.RightRingIntermediate = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(18);// Replace with correct path
        p2.RightRingDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(19); 
        p2.RightRingTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(20);
        // Right Pinky Joints
        p2.RightPinkyMetacarpal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(21); // Replace with correct path
        p2.RightPinkyProximal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(22); // Replace with correct path
        p2.RightPinkyIntermediate = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(23); // Replace with correct path
        p2.RightPinkyDistal = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(24);
        p2.RightPinkyTip = player2.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(25);
    }

    private void AppendCurrentFrameData(List<PositionRotation[]> playerData, playerPositionData player)
    {
        PositionRotation[] currentFrameData = new PositionRotation[51]; // Updated to include finger tips

        currentFrameData[0] = GetPositionRotation(player.Camera);
        currentFrameData[1] = GetPositionRotation(player.leftHand);
        currentFrameData[2] = GetPositionRotation(player.rightHand);
        currentFrameData[3] = GetPositionRotation(player.LeftThumbMetacarpal);
        currentFrameData[4] = GetPositionRotation(player.LeftThumbProximal);
        currentFrameData[5] = GetPositionRotation(player.LeftThumbDistal);
        currentFrameData[6] = GetPositionRotation(player.LeftThumbTip);
        currentFrameData[7] = GetPositionRotation(player.LeftIndexMetacarpal);
        currentFrameData[8] = GetPositionRotation(player.LeftIndexProximal);
        currentFrameData[9] = GetPositionRotation(player.LeftIndexIntermediate);
        currentFrameData[10] = GetPositionRotation(player.LeftIndexDistal);
        currentFrameData[11] = GetPositionRotation(player.LeftIndexTip);
        currentFrameData[12] = GetPositionRotation(player.LeftMiddleMetacarpal);
        currentFrameData[13] = GetPositionRotation(player.LeftMiddleProximal);
        currentFrameData[14] = GetPositionRotation(player.LeftMiddleIntermediate);
        currentFrameData[15] = GetPositionRotation(player.LeftMiddleDistal);
        currentFrameData[16] = GetPositionRotation(player.LeftMiddleTip);
        currentFrameData[17] = GetPositionRotation(player.LeftRingMetacarpal);
        currentFrameData[18] = GetPositionRotation(player.LeftRingProximal);
        currentFrameData[19] = GetPositionRotation(player.LeftRingIntermediate);
        currentFrameData[20] = GetPositionRotation(player.LeftRingDistal);
        currentFrameData[21] = GetPositionRotation(player.LeftRingTip);
        currentFrameData[22] = GetPositionRotation(player.LeftPinkyMetacarpal);
        currentFrameData[23] = GetPositionRotation(player.LeftPinkyProximal);
        currentFrameData[24] = GetPositionRotation(player.LeftPinkyIntermediate);
        currentFrameData[25] = GetPositionRotation(player.LeftPinkyDistal);
        currentFrameData[26] = GetPositionRotation(player.LeftPinkyTip);
        currentFrameData[27] = GetPositionRotation(player.RightThumbMetacarpal);
        currentFrameData[28] = GetPositionRotation(player.RightThumbProximal);
        currentFrameData[29] = GetPositionRotation(player.RightThumbDistal);
        currentFrameData[30] = GetPositionRotation(player.RightThumbTip);
        currentFrameData[31] = GetPositionRotation(player.RightIndexMetacarpal);
        currentFrameData[32] = GetPositionRotation(player.RightIndexProximal);
        currentFrameData[33] = GetPositionRotation(player.RightIndexIntermediate);
        currentFrameData[34] = GetPositionRotation(player.RightIndexDistal);
        currentFrameData[35] = GetPositionRotation(player.RightIndexTip);
        currentFrameData[36] = GetPositionRotation(player.RightMiddleMetacarpal);
        currentFrameData[37] = GetPositionRotation(player.RightMiddleProximal);
        currentFrameData[38] = GetPositionRotation(player.RightMiddleIntermediate);
        currentFrameData[39] = GetPositionRotation(player.RightMiddleDistal);
        currentFrameData[40] = GetPositionRotation(player.RightMiddleTip);
        currentFrameData[41] = GetPositionRotation(player.RightRingMetacarpal);
        currentFrameData[42] = GetPositionRotation(player.RightRingProximal);
        currentFrameData[43] = GetPositionRotation(player.RightRingIntermediate);
        currentFrameData[44] = GetPositionRotation(player.RightRingDistal);
        currentFrameData[45] = GetPositionRotation(player.RightRingTip);
        currentFrameData[46] = GetPositionRotation(player.RightPinkyMetacarpal);
        currentFrameData[47] = GetPositionRotation(player.RightPinkyProximal);
        currentFrameData[48] = GetPositionRotation(player.RightPinkyIntermediate);
        currentFrameData[49] = GetPositionRotation(player.RightPinkyDistal);
        currentFrameData[50] = GetPositionRotation(player.RightPinkyTip);

        playerData.Add(currentFrameData);
    }

    private PositionRotation GetPositionRotation(Transform transform)
    {
        return new PositionRotation(transform.position, transform.rotation, frameNumber, transform.localPosition, transform.localRotation, currentHandSize, deltaTime);
    }

    private void WritePlayerData(StreamWriter writer, PositionRotation[] frameData, int playerNumber)
    {
        // string[] parts = new string[] {
        //     "Camera", "LeftHand", "RightHand",
        //     "LeftThumbMetacarpal", "LeftThumbProximal", "LeftThumbDistal", "LeftThumbTip",
        //     "LeftIndexMetacarpal", "LeftIndexProximal", "LeftIndexIntermediate", "LeftIndexDistal", "LeftIndexTip",
        //     "LeftMiddleMetacarpal", "LeftMiddleProximal", "LeftMiddleIntermediate", "LeftMiddleDistal", "LeftMiddleTip",
        //     "LeftRingMetacarpal", "LeftRingProximal", "LeftRingIntermediate", "LeftRingDistal", "LeftRingTip",
        //     "LeftPinkyMetacarpal", "LeftPinkyProximal", "LeftPinkyIntermediate", "LeftPinkyDistal", "LeftPinkyTip",
        //     "RightThumbMetacarpal", "RightThumbProximal", "RightThumbDistal", "RightThumbTip",
        //     "RightIndexMetacarpal", "RightIndexProximal", "RightIndexIntermediate", "RightIndexDistal", "RightIndexTip",
        //     "RightMiddleMetacarpal", "RightMiddleProximal", "RightMiddleIntermediate", "RightMiddleDistal", "RightMiddleTip",
        //     "RightRingMetacarpal", "RightRingProximal", "RightRingIntermediate", "RightRingDistal", "RightRingTip",
        //     "RightPinkyMetacarpal", "RightPinkyProximal", "RightPinkyIntermediate", "RightPinkyDistal", "RightPinkyTip"
        // };

        int prev_frame = -1;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int j = 0; j < frameData.Length; j++)
        {
            PositionRotation pr = frameData[j];
            //writer.WriteLine($"{pr.FrameNumber},{playerNumber},{parts[j]},{pr.Position.x},{pr.Position.y},{pr.Position.z},{pr.Rotation.x},{pr.Rotation.y},{pr.Rotation.z},{pr.Rotation.w},{pr.localPosition.x},{pr.localPosition.y},{pr.localPosition.z},{pr.localRotation.x},{pr.localRotation.y},{pr.localRotation.z},{pr.localRotation.w},{deltaTime},{pr.thisFrameHandSize}");
            
            // if (prev_frame == pr.FrameNumber)
            //     toWrite += $"{pr.Position.x},{pr.Position.y},{pr.Position.z},{pr.Rotation.x},{pr.Rotation.y},{pr.Rotation.z},{pr.Rotation.w},{pr.localPosition.x},{pr.localPosition.y},{pr.localPosition.z},{pr.localRotation.x},{pr.localRotation.y},{pr.localRotation.z},{pr.localRotation.w},";
            // else
            //     toWrite += $"{pr.FrameNumber},{playerNumber},{deltaTime},{pr.thisFrameHandSize},{pr.Position.x},{pr.Position.y},{pr.Position.z},{pr.Rotation.x},{pr.Rotation.y},{pr.Rotation.z},{pr.Rotation.w},{pr.localPosition.x},{pr.localPosition.y},{pr.localPosition.z},{pr.localRotation.x},{pr.localRotation.y},{pr.localRotation.z},{pr.localRotation.w},";
            
            if (prev_frame == pr.FrameNumber)
            {
                // if the current data is for the last body part in frameData, do not add comma at end
                bool lastPart = j == frameData.Length - 1;
                string jointData = lastPart ? $"{pr.Position.x},{pr.Position.y},{pr.Position.z},{pr.Rotation.x},{pr.Rotation.y},{pr.Rotation.z},{pr.Rotation.w},{pr.localPosition.x},{pr.localPosition.y},{pr.localPosition.z},{pr.localRotation.x},{pr.localRotation.y},{pr.localRotation.z},{pr.localRotation.w}"
                : $"{pr.Position.x},{pr.Position.y},{pr.Position.z},{pr.Rotation.x},{pr.Rotation.y},{pr.Rotation.z},{pr.Rotation.w},{pr.localPosition.x},{pr.localPosition.y},{pr.localPosition.z},{pr.localRotation.x},{pr.localRotation.y},{pr.localRotation.z},{pr.localRotation.w},";
                sb.Append(jointData);
            }

            else{ 
                string jointData = $"{pr.FrameNumber},{playerNumber},{pr.deltaTime},{pr.thisFrameHandSize},{pr.Position.x},{pr.Position.y},{pr.Position.z},{pr.Rotation.x},{pr.Rotation.y},{pr.Rotation.z},{pr.Rotation.w},{pr.localPosition.x},{pr.localPosition.y},{pr.localPosition.z},{pr.localRotation.x},{pr.localRotation.y},{pr.localRotation.z},{pr.localRotation.w},";
                sb.Append(jointData);
            }
            prev_frame = pr.FrameNumber;
        }
        writer.WriteLine(sb);
    }


    public void changeCurrentHandSize(float percentage)
    {
        if (percentage == .75f){
            currentHandSize = handSize.Small;
        }
        else if (percentage == 1f){
            currentHandSize = handSize.Fitted;
        }else if (percentage == 1.25f){
            currentHandSize = handSize.Large;
        }
    }
}
