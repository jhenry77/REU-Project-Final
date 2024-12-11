using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ParticipantData : NetworkBehaviour {
    [SyncVar]
    public int PID;
    void Start(){

    }

    void Update(){
        // Debug.Log("my pid is : " + PID);

    }

}