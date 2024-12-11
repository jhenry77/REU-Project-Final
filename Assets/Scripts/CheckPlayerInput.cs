using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Hands;

public class CheckPlayerInput : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject HandVizualizerObject;
    public GameObject deviceSim;
    public GameObject deviceSimUi;
    public GameObject myRig;
    public GameObject[] players;
    public GameObject[] wristForScale;
 
    public GameObject leftHandSphere;
    public GameObject rightHandSphere;
    [SyncVar]
    public int PID;

    public bool canSee = false;

    // public SceneController sceneController;
    


    [Client]
    public override void OnStartClient()
    {
         base.OnStartClient();
        gameObject.tag = "NetworkPlayer";
       
        if(!isLocalPlayer){
            return;
        }
            leftHandSphere.GetComponent<serverUpdateJoints>().enabled = false;
            rightHandSphere.GetComponent<serverUpdateJoints>().enabled = false;
            
            myRig.SetActive(true);
            //Instantiate(deviceSim, gameObject.transform, false );
            //Instantiate(deviceSimUi, gameObject.transform, false);
            gameObject.tag = "NetworkPlayer";
            players = GameObject.FindGameObjectsWithTag("NetworkPlayer");
            // bodyParts = GameObject.FindGameObjectsWithTag("BodyParts");
            // foreach(var x in bodyParts){x.SetActive(false);}
            if(players.Length == 1){
                gameObject.transform.Rotate(new Vector3(0,180,0));
            }else if(players.Length == 2){
                //gameObject.transform.Rotate(new Vector3(0,-180,0));
            }
      
            
    }

    public void setScale(float input){
        // Debug.Log("calling setScale in checkPlayerInput");
        HandVizualizerObject.GetComponent<NetworkedHandVIz>().scale = input;
        setScaleLocal(input);
        
    }
    [ClientRpc]
    public void setScaleLocal(float input){
       
        HandVizualizerObject.GetComponent<NetworkedHandVIz>().scale = input;

    }





    
   
   

    [Client]
    public void movePlayer(Transform movePosition){
        if(!isLocalPlayer){return;}
        transform.position = movePosition.position;

    }

    

    // [Client]
    // public void setScenePlayer() {
    //     if (!isLocalPlayer) {
    //         return;
    //     }

    //     // sceneController = GameObject.Find("Scene Controller").GetComponent<SceneController>();
    //     if (PID == 1) {
    //         setButtonPlayer
    //     } else if (PID == 2) {
    //         sceneController.activedCalibartionButtonP2();
    //     }

    // }
    
}
