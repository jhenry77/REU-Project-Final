using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using TMPro;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;


public class DemoScript : MonoBehaviour
{
    public string fileName = ""; // Make sure the CSV file is in the right directory
    private List<PositionRotation[]> player1Data;
    private List<PositionRotation[]> player2Data;
    private float timer;
    private int currentFrameP1, currentFrameP2;

    public Camera cameraObject;
    public GameObject text;



    int P2FrameDelay = 0;

    public GameObject slider;
    public GameObject playButton;

    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public GameObject player1GameObject;
    public GameObject player2GameObject;


    public List<string[]> buttonList = new List<string[]>();


    [HideInInspector]
    public bool play = false;

    public Transform player1Spawn;
    public Transform player2Spawn;
    
    public GameObject MoveOnButton;
    public GameObject MoveOnButtonP2;
    public DemoButtonController myButtonController;
   
    public GameObject SpawnPrefab;
    public Transform SpawnLocation;
    private int numWaiting = 0;

    public GameObject combinedSpawn1;
    public GameObject combinedSpawn2;



    private class PositionRotation
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Quaternion localRotation{get; set;}
        public int FrameNumber { get; set; }

        public PositionRotation(Vector3 position, Quaternion rotation, int frameNumber, Quaternion localRot)
        {
            Position = position;
            Rotation = rotation;
            FrameNumber = frameNumber;
            localRotation = localRot;
        }
    }

    void Start()
    {
        player1Data = new List<PositionRotation[]>();
        player2Data = new List<PositionRotation[]>();
        ReadCSV("D:" + "CharadeLogs/" + fileName);
        readQuestionOrder("D:" + "CharadeLogs/" + "Ryan-Varun-Gesture.csv");
        currentFrameP1 = 0;
        currentFrameP2 = -P2FrameDelay;
        slider.GetComponent<Slider>().minValue = 0;
        slider.GetComponent<Slider>().maxValue = Math.Max(player1Data.Count, player2Data.Count);

        
        // print(currentFrameP2);
        timer = 0;
    }

    bool player1Instantiated(){
        GameObject Prefab = GameObject.FindWithTag("DemoPlayer1");
        if(Prefab == null){
            return false;
        }else{
            return true;
        }
    }

    bool player2Instantiated(){
        GameObject Prefab = GameObject.FindWithTag("DemoPlayer2");
        if(Prefab == null){
            return false;
        }else{
            return true;
        }
    }

     void Update()
    {

        play = playButton.GetComponent<Toggle>().isOn;
        // if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     StepForward();
        // }
        // else if (Input.GetKey(KeyCode.LeftArrow))
        // {
        //     StepBackward();
        // }
        if(play){
        StepForward();
        }



    }

    void readQuestionOrder(string filePath){
         using (StreamReader reader = new StreamReader(filePath))
        {
            string[] header = reader.ReadLine().Split(','); // Skip header line
            string line = "";

            
            while ((line = reader.ReadLine()) != null)
            {
                if(line != "end of phase"){
                string[] values = line.Split(',');
                buttonList.Add(new string[]{values[2], values[3], values[4], values[6]});
                }
            }
        }
    }

    void ReadCSV(string filePath)
    {
        
        using (StreamReader reader = new StreamReader(filePath))
        {
            int prevFrameNumber = 0;
            int startFrame = 0;
            int frameNumberP1 = 0;
            int frameNumberP2 = 0;
            int frameNumber = 0;
            string[] header = reader.ReadLine().Split(','); // Skip header line
            string line = "";

            // for(int i = 0; i < header.Length; i++)
            //     print(header[i]);
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(',');
                //if (values.Length == 718)
                string readFrame = values[0];
                int subtractNum = int.Parse(readFrame);
                string player = values[1];
                // string part = values[2];
                for (int i = 4; i < values.Length; i+=14)
                {
                    string[] _info = header[i].Split('_');
                    string part = _info[0];
                    string component = _info[1];
                    
                    if (!component.Contains("l")) // ignore local transforms
                    {
                        if (values[i] != "")
                        {
                            // Vector3 position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                            // Quaternion rotation = new Quaternion(float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]));
                            Vector3 position = new Vector3(float.Parse(values[i]), float.Parse(values[i+1]), float.Parse(values[i+2]));
                            Quaternion rotation = new Quaternion(float.Parse(values[i+3]), float.Parse(values[i+4]), float.Parse(values[i+5]), float.Parse(values[i+6]));
                            Quaternion localRotation = new Quaternion(float.Parse(values[i+10]), float.Parse(values[i+11]), float.Parse(values[i+12]), float.Parse(values[i+13]));
                            PositionRotation pr = new PositionRotation(position, rotation, frameNumber, localRotation);
                            if (player == "1")
                            {
                                if (startFrame == 0)
                                    startFrame = subtractNum;

                                AddToPlayerData(player1Data, pr, part, frameNumberP1);
                            }
                            else if (player == "2")
                            {
                                if (P2FrameDelay == 0)
                                    P2FrameDelay = subtractNum - startFrame;
                                
                                AddToPlayerData(player2Data, pr, part, frameNumberP2);
                            }
                        }
                    }
                    
                }
                
                // frameNumber += prevFrameNumber == 0 ? 1 : (subtractNum-prevFrameNumber);
                // prevFrameNumber = subtractNum; 
                if (player == "1")
                    frameNumberP1++;
                if (player == "2")
                    frameNumberP2++;
            }
        }
    }

    void AddToPlayerData(List<PositionRotation[]> playerData, PositionRotation pr, string part, int frameNumber)
    {
        // Ensure the list has enough frames
        while (playerData.Count <= frameNumber)
        {
            playerData.Add(new PositionRotation[51]);
        }

        int partIndex = GetPartIndex(part);
        playerData[frameNumber][partIndex] = pr;
    }

     void AdjustCamera(PositionRotation[] frameData)
    {
        // Assuming head position is at index 0 and head rotation is at index 1
        PositionRotation headPositionData = frameData[0];


        if (headPositionData != null)
        {
            Vector3 headPosition = headPositionData.Position;
            Quaternion headRotation = headPositionData.Rotation;

            // Position the camera behind the head
            Vector3 offset = new Vector3(0, 0, -0.1f); // Adjust this offset as necessary
            cameraObject.transform.position = headPosition + headRotation * offset;

            // Make the camera look at the head
            cameraObject.transform.LookAt(headPosition);
        }
    }

    int GetPartIndex(string part)
{
    switch (part)
    {
        case "Camera": return 0;
        case "LeftHand": return 1;
        case "RightHand": return 2;
        case "LeftThumbMetacarpal": return 3;
        case "LeftThumbProximal": return 4;
        case "LeftThumbDistal": return 5;
        case "LeftThumbTip": return 6;
        case "LeftIndexMetacarpal": return 7;
        case "LeftIndexProximal": return 8;
        case "LeftIndexIntermediate": return 9;
        case "LeftIndexDistal": return 10;
        case "LeftIndexTip": return 11;
        case "LeftMiddleMetacarpal": return 12;
        case "LeftMiddleProximal": return 13;
        case "LeftMiddleIntermediate": return 14;
        case "LeftMiddleDistal": return 15;
        case "LeftMiddleTip": return 16;
        case "LeftRingMetacarpal": return 17;
        case "LeftRingProximal": return 18;
        case "LeftRingIntermediate": return 19;
        case "LeftRingDistal": return 20;
        case "LeftRingTip": return 21;
        case "LeftPinkyMetacarpal": return 22;
        case "LeftPinkyProximal": return 23;
        case "LeftPinkyIntermediate": return 24;
        case "LeftPinkyDistal": return 25;
        case "LeftPinkyTip": return 26;
        case "RightThumbMetacarpal": return 27;
        case "RightThumbProximal": return 28;
        case "RightThumbDistal": return 29;
        case "RightThumbTip": return 30;
        case "RightIndexMetacarpal": return 31;
        case "RightIndexProximal": return 32;
        case "RightIndexIntermediate": return 33;
        case "RightIndexDistal": return 34;
        case "RightIndexTip": return 35;
        case "RightMiddleMetacarpal": return 36;
        case "RightMiddleProximal": return 37;
        case "RightMiddleIntermediate": return 38;
        case "RightMiddleDistal": return 39;
        case "RightMiddleTip": return 40;
        case "RightRingMetacarpal": return 41;
        case "RightRingProximal": return 42;
        case "RightRingIntermediate": return 43;
        case "RightRingDistal": return 44;
        case "RightRingTip": return 45;
        case "RightPinkyMetacarpal": return 46;
        case "RightPinkyProximal": return 47;
        case "RightPinkyIntermediate": return 48;
        case "RightPinkyDistal": return 49;
        case "RightPinkyTip": return 50;
        default: return -1;
    }
}

 string GetPartName(int index)
{
   switch (index)
{
    case 0: return "HeadTarget";
    case 1: return "LeftArmTarget";
    case 2: return "RightArmTarget";
    case 3: return "L_ThumbMetacarpal";
    case 4: return "L_ThumbProximal";
    case 5: return "L_ThumbDistal";
    case 6: return "L_ThumbTip";
    case 7: return "L_IndexMetacarpal";
    case 8: return "L_IndexProximal";
    case 9: return "L_IndexIntermediate";
    case 10: return "L_IndexDistal";
    case 11: return "L_IndexTip";
    case 12: return "L_MiddleMetacarpal";
    case 13: return "L_MiddleProximal";
    case 14: return "L_MiddleIntermediate";
    case 15: return "L_MiddleDistal";
    case 16: return "L_MiddleTip";
    case 17: return "L_RingMetacarpal";
    case 18: return "L_RingProximal";
    case 19: return "L_RingIntermediate";
    case 20: return "L_RingDistal";
    case 21: return "L_RingTip";
    case 22: return "L_LittleMetacarpal";
    case 23: return "L_LittleProximal";
    case 24: return "L_LittleIntermediate";
    case 25: return "L_LittleDistal";
    case 26: return "L_LittleTip";
    case 27: return "R_ThumbMetacarpal";
    case 28: return "R_ThumbProximal";
    case 29: return "R_ThumbDistal";
    case 30: return "R_ThumbTip";
    case 31: return "R_IndexMetacarpal";
    case 32: return "R_IndexProximal";
    case 33: return "R_IndexIntermediate";
    case 34: return "R_IndexDistal";
    case 35: return "R_IndexTip";
    case 36: return "R_MiddleMetacarpal";
    case 37: return "R_MiddleProximal";
    case 38: return "R_MiddleIntermediate";
    case 39: return "R_MiddleDistal";
    case 40: return "R_MiddleTip";
    case 41: return "R_RingMetacarpal";
    case 42: return "R_RingProximal";
    case 43: return "R_RingIntermediate";
    case 44: return "R_RingDistal";
    case 45: return "R_RingTip";
    case 46: return "R_LittleMetacarpal";
    case 47: return "R_LittleProximal";
    case 48: return "R_LittleIntermediate";
    case 49: return "R_LittleDistal";
    case 50: return "R_LittleTip";
    default: return "Invalid Index";
}
}

void StepForward()
    {
        TMP_Text myText = text.GetComponent<TMP_Text>();
        myText.text = slider.GetComponent<Slider>().value.ToString();
        if ((int)slider.GetComponent<Slider>().value < player1Data.Count - 1)
        {
            DisplayFrame((int)slider.GetComponent<Slider>().value, player1Data, "Player1");
        }


        if ((int)slider.GetComponent<Slider>().value + currentFrameP2 < player2Data.Count - 1)
        {
           
            
            if ((int)slider.GetComponent<Slider>().value + currentFrameP2 >= 0){
                DisplayFrame((int)slider.GetComponent<Slider>().value + currentFrameP2, player2Data, "Player2");
            }else{
           
            ClearSpheres("Player2");
            if(player2Instantiated()){
                Destroy(player2GameObject);
            }
        }
            
        }


        if(slider.GetComponent<Slider>().value <= Math.Max(player1Data.Count, player2Data.Count)){
            slider.GetComponent<Slider>().value = slider.GetComponent<Slider>().value + 1;
        }
        
    }


void DisplayFrame(int frameIndex, List<PositionRotation[]> playerData, string player)
{

    if(player == "Player1" && !player1Instantiated()){
        player1GameObject = Instantiate(player1Prefab, player1Spawn.transform.position, player1Spawn.transform.rotation);
    }

    if(player == "Player2" && !player2Instantiated()){
        player2GameObject = Instantiate(player2Prefab, player2Spawn.transform.position, player2Spawn.transform.rotation);
        player2GameObject.tag = "DemoPlayer2";
    }

    
    ClearSpheres(player);
    
    ReplayFrame(playerData[frameIndex], player);
    
    // leftWrist.transform.localScale *= 1.25f;
    // rightWrist.transform.localScale *= 1.25f;
    if (player == "Player1")
        AdjustCamera(playerData[frameIndex]);
    //ReplayFrame(player2Data[frameIndex], "Player2");
}

void ClearSpheres(string player)
    {
        GameObject[] spheres = GameObject.FindGameObjectsWithTag("ReplaySphere" + player);
        if (spheres == null){
            return;
        }
        foreach (GameObject sphere in spheres)
        {
            Destroy(sphere);
        }
    }

void ReplayFrame(PositionRotation[] frameData, string player)
    {
        string currentJoint = "";
        Transform jointObject;
        Animator myAnimator;

        // For loop to itterate through all the joints
        for (int i = 0; i < frameData.Length; i++)
        {
            // Make sure frame data isn't null
            if (frameData[i] != null)
            {
                // Case for player 1
                if(player == "Player1"){
                    // Get the animator for player 1 to disable and enable
                    myAnimator = player1GameObject.GetComponentInChildren<Animator>();
                    // Loops through just the first 3 as we dont need to alter any data
                    if(i < 3){
                        // Disable the animator and IK manager to prevent them from overriding your changes
                        if (myAnimator != null){
                            myAnimator.enabled = false;
                        }
                        currentJoint = GetPartName(i);
                        jointObject = FindChildByName(player1GameObject.transform, currentJoint);
                        if(jointObject == null){
                            Debug.Log("Couldnt find the object called" + currentJoint);
                        }else{
                            // Debug.Log("name, our pos and rot is" + jointObject.name + "," + jointObject.position + "," + jointObject.rotation);
                            jointObject.transform.position = frameData[i].Position;
                            jointObject.transform.rotation = frameData[i].Rotation;
                            // Debug.Log("name, our pos and rot after the position update is"+ jointObject.name + "," + jointObject.position + "," + jointObject.rotation);
                        }
                        if (myAnimator != null)
                        {
                            myAnimator.enabled = false;
                        }
                        // Case for all the other joints other than the first 3
                        }else{
                            if (myAnimator != null)
                            {
                                myAnimator.enabled = false;
                            }
                            currentJoint = GetPartName(i);
                            jointObject = FindChildByName(player1GameObject.transform, currentJoint);
                            if(jointObject == null){
                                Debug.Log("Couldnt find the object called" + currentJoint);
                            }else{
                                // Debug.Log("name, our pos and rot is" + jointObject.name + "," + jointObject.position + "," + jointObject.rotation);
                                // jointObject.transform.localRotation = frameData[i].localRotation;
                                // jointObject.transform.position = frameData[i].Position;
                                if(jointObject.name.Contains("Metacarpal")){
                                    // jointObject.transform.localRotation = Quaternion.Inverse(jointObject.transform.parent.rotation) * frameData[i].Rotation;
                                }else{
                                    // jointObject.LookAt(jointObject.parent);
                                    // jointObject.transform.localRotation = Quaternion.Inverse(jointObject.transform.parent.rotation) * frameData[i].Rotation;
                                    jointObject.position = frameData[i].Position;
                                    
                                    jointObject.parent.up = jointObject.position - jointObject.parent.position;
                                    // print(jointObject.name + " rotation:" + jointObject.rotation);
                                    // print(frameData[i].Rotation);
                                    // jointObject.parent.LookAt(jointObject.position, new Vector3(0, 0, -1));
                                    
                                }
                                
                                // jointObject.transform.rotation = Quaternion.Inverse(jointObject.transform.parent.rotation) * frameData[i].Rotation;
                                // jointObject.transform.localRotation = frameData[i].localRotation;
                                // Debug.Log("Joint name is" + jointObject.name + "and our local rotation is" + frameData[i].localRotation);
                                // jointObject.parent.LookAt(jointObject.transform.position, jointObject.parent.forward);
                            
                                // Debug.Log("name, our pos and rot after the position update is"+ jointObject.name + "," + jointObject.position + "," + jointObject.rotation);
                        }
                        if (myAnimator != null)
                        {
                            myAnimator.enabled = true;
                        }
                    
                

                        }
                }else{
                    myAnimator = player2GameObject.GetComponentInChildren<Animator>();
                    if(i < 3){
                    
                        // Disable the animator and IK manager to prevent them from overriding your changes
                            if (myAnimator != null)
                            {
                                myAnimator.enabled = false;
                            }
                            currentJoint = GetPartName(i);
                            jointObject = FindChildByName(player2GameObject.transform, currentJoint);
                            if(jointObject == null){
                                Debug.Log("Couldnt find the object called" + currentJoint);
                            }else{
                                // Debug.Log("name, our pos and rot is" + jointObject.name + "," + jointObject.position + "," + jointObject.rotation);
                                jointObject.transform.position = frameData[i].Position;
                                
                                
                                jointObject.transform.rotation = frameData[i].Rotation;
                                
                            
                                // Debug.Log("name, our pos and rot after the position update is"+ jointObject.name + "," + jointObject.position + "," + jointObject.rotation);
                        }
                         if (myAnimator != null)
                        {
                            myAnimator.enabled = true;
                        }
                    
                        }else{
                        if (myAnimator != null)
                            {
                                myAnimator.enabled = false;
                            }    
                        currentJoint = GetPartName(i);
                        jointObject = FindChildByName(player2GameObject.transform, currentJoint);
                        if(jointObject == null){
                            Debug.Log("Couldnt find the object called" + currentJoint);
                        }else{
                            // Debug.Log("name, our pos and rot is" + jointObject.name + "," + jointObject.position + "," + jointObject.rotation);
                            // jointObject.transform.localRotation = frameData[i].localRotation;
                            // jointObject.transform.position = frameData[i].Position;
                            if(jointObject.name.Contains("Metacarpal")){
                                // jointObject.transform.localRotation = Quaternion.Inverse(jointObject.transform.parent.rotation) * frameData[i].Rotation;
                            }else{
                                // jointObject.LookAt(jointObject.parent);
                                // jointObject.transform.localRotation = Quaternion.Inverse(jointObject.transform.parent.rotation) * frameData[i].Rotation;
                                jointObject.position = frameData[i].Position;
                                
                                jointObject.parent.up = jointObject.position - jointObject.parent.position;
                                // print(jointObject.name + " rotation:" + jointObject.rotation);
                                // print(frameData[i].Rotation);
                                // jointObject.parent.LookAt(jointObject.position, new Vector3(0, 0, -1));
                                
                            }
                            
                            // jointObject.transform.rotation = Quaternion.Inverse(jointObject.transform.parent.rotation) * frameData[i].Rotation;
                            // jointObject.transform.localRotation = frameData[i].localRotation;
                            // Debug.Log("Joint name is" + jointObject.name + "and our local rotation is" + frameData[i].localRotation);
                            // jointObject.parent.LookAt(jointObject.transform.position, jointObject.parent.forward);
                        
                            // Debug.Log("name, our pos and rot after the position update is"+ jointObject.name + "," + jointObject.position + "," + jointObject.rotation);
                        }
                        if (myAnimator != null)
                        {
                            myAnimator.enabled = true;
                        }
                

                        }
                   
                }
            
                
                // if(myAnimator != null){
                //     myAnimator.enabled = true;
                // }
            }
        }
    }

void CreateSphere(PositionRotation location, string name, string player)
    {
        // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sphere.transform.position = location.Position;
        // sphere.transform.rotation = location.Rotation;
        // sphere.name = name;
        // Vector3 scaleChange = new Vector3(-.99f,-.99f,-.99f);
        // sphere.transform.localScale += scaleChange;
        // sphere.tag = "ReplaySphere" + player; // Tag the sphere for easy cleanup
        // if(int.Parse(name) == 1 ){
        //     leftWrist = sphere;
        // }else if (int.Parse(name) == 2){
        //     rightWrist = sphere;
        // }

        // if(int.Parse(name) > 2 && int.Parse(name) < 27){
        //     sphere.transform.parent = leftWrist.transform;
        // }
        // if(int.Parse(name) > 26 && int.Parse(name) < 52){
        //     sphere.transform.parent = rightWrist.transform;
        // }
    }

Transform FindChildByName(Transform parent, string name)
    {
        // Check if the current parent has the name we're looking for
        if (parent.name == name)
        {
            return parent;
        }

        // Recursively search each child
        foreach (Transform child in parent)
        {
            Transform result = FindChildByName(child, name);
            if (result != null)
            {
                return result;
            }
        }

        // If no child with the given name is found, return null
        return null;
    }


// Functions for controlling the scene such as buttons and other actions.


public void activatedCalibrationButtonP1(){
    numWaiting++;
    moveOnPlayer1();
  
    MoveOnButton.SetActive(false);
    if(numWaiting == 2){
        GameObject controller = Instantiate(SpawnPrefab, SpawnLocation.position, SpawnLocation.rotation); 
    }
}

public void moveOnPlayer1(){
  
    player1GameObject.transform.position = combinedSpawn1.transform.position;
}
public void activatedCalibrationButtonP2(){
    numWaiting++;
    moveOnPlayer2();
    MoveOnButtonP2.SetActive(false);
    if(numWaiting == 2){
        GameObject controller = Instantiate(SpawnPrefab, SpawnLocation.position, SpawnLocation.rotation); 
    }
}
public void moveOnPlayer2(){

    player2GameObject.transform.position = combinedSpawn2.transform.position;
}












}





