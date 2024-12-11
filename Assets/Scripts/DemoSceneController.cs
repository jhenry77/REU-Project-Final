using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DemoSceneController : MonoBehaviour
{
    
    public GameObject Player1Enviroment;
    public GameObject Player2Enviroment;
    public GameObject PlayEnviroment;
    public GameObject Enviroment;
    public GameObject EnviromentP2;
    public GameObject CombinedEnviorment;
    public GameObject MoveOnButton;
    public GameObject MoveOnButtonP2;
    public myNetworkManager myNetworkManager = new myNetworkManager();
    public DemoButtonController myButtonController;
   
    public GameObject SpawnPrefab;
    public Transform SpawnLocation;
    private int numWaiting = 0;

    public GameObject player1;
    public GameObject player2;
    public bool InstantiatedController = false;
    public bool clientFindController = false;

    

    // Start is called before the first frame update
    void Start()
    {
       
        
    }

   

  


        public void activatedCalibrationButtonP1(){
            numWaiting++;
            moveOnPlayer1();
        }

        public void moveOnPlayer1(){
            
            PlayEnviroment.SetActive(true);
            Player1Enviroment.SetActive(false);
            MoveOnButton.SetActive(false);
            Enviroment.SetActive(false);
            myNetworkManager.movePlayer1();
            ServersetPlayer1SecneOff();
        }

        public void activatedCalibrationButtonP2(){
            numWaiting++;
            moveOnPlayer2();
        }

        public void moveOnPlayer2(){
            
            PlayEnviroment.SetActive(true);
            Player2Enviroment.SetActive(false);
            CombinedEnviorment.SetActive(true);
            MoveOnButtonP2.SetActive(false);
            EnviromentP2.SetActive(false);
            myNetworkManager.movePlayer2();
            ServersetPlayer2SecneOff();
        }


    public void startGame(){
        myButtonController.setTrippleButtonsOn();
        myButtonController.setInitialNumbers();
        int player2Pid = myNetworkManager.player2PID;
        myButtonController.orderOfhands = ((player2Pid /2 ) % 6) - 1;
        myNetworkManager.setPlayerWristScales(myButtonController.handSizeOrder[myButtonController.orderOfhands][0]);
        

    }

    // [ClientRpc]
    public void changeClientFindClient(){
        clientFindController = true;
    }
    public void findTheController(){
        GameObject buttonObject = GameObject.FindGameObjectWithTag("ButtonController");
        myButtonController = buttonObject.GetComponent<DemoButtonController>();
        //myButtonController.setInitialNumbers();
        myButtonController.player1Pid = myNetworkManager.player1PID;
        myButtonController.player2Pid = myNetworkManager.player2PID;
        
    }

    

    
    


   

   
    // [Client]
    // public void startCalibartion(){
    //     
    //     rayInteractors = GameObject.FindGameObjectsWithTag("RayInteractor");
    //     Debug.Log("ray interactors length" + rayInteractors.GetLength(0));
    //     Debug.Log("Body parts length: " + bodyParts.GetLength(0));
    //     Debug.Log("before for loops");
    //     foreach(var x in bodyParts){x.SetActive(true);}
    //     foreach(var x in rayInteractors){x.SetActive(false); }
    //     Debug.Log("Setting things to active");
    //     UISample.SetActive(false);
    //     myMirror.SetActive(true);
    //     Enviroment.SetActive(true);
    //     theTable.SetActive(false);
    //     opposingChair.SetActive(false);
    //     MoveOnButton.SetActive(true);
    // }

    

    
    // [Client]
    public void startPlayPhase(){
        startServerPlayPhase();
        CombinedEnviorment.SetActive(true);
        //MovePlayer 1
        //MovePlayer 2
        
    

    }

    // [Command(requiresAuthority = false)]
    public void startServerPlayPhase(){
        CombinedEnviorment.SetActive(true);
        Enviroment.SetActive(false);
        EnviromentP2.SetActive(false);

    }

    //  [Command(requiresAuthority = false)]
        public void startPlayPhase_Client(){
            
            GameObject controller = Instantiate(SpawnPrefab, SpawnLocation.position, SpawnLocation.rotation);
            NetworkServer.Spawn(controller);

        }

        

        public void setAsplayer1(GameObject player){
            player1 = player;
            
        }

        public void setAsPlayer2(GameObject player){
            player2 = player;
            

        }


       
        // [Command(requiresAuthority = false)]
        public void ServersetPlayer1SecneOff(){
            Player1Enviroment.SetActive(false);

        }

        // [Command(requiresAuthority = false)]
        public void ServersetPlayer2SecneOff(){
            Player2Enviroment.SetActive(false);

        }
        //  [Command(requiresAuthority = false)]
        public void UpdateServerNumWaiting(){
            numWaiting++;
            


        }

        
        

}
