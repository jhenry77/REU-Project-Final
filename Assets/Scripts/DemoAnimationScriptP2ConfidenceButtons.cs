using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class DemoAnimationScriptP2ConfidenceButtons : MonoBehaviour 
{   
    public float increment = 0.003f;
    public DemoButtonController myButtonController;
    float minHeight;
    float maxHeight;
    public GameObject button1;
    public GameObject button7;
    public GameObject[] tempListButtons;
    // Start is called before the first frame update
    void Start()
    {
        minHeight = gameObject.transform.position.y;
        maxHeight = minHeight + .4f;
        
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(myButtonController.showConfidenceButtonsP2){
            animateUp();
        }
        if(myButtonController.hideConfidenceButtonsP2){
            animateDown();
        }
        
    }

    
    public void animateUp(){
        if(gameObject.transform.position.y < maxHeight){
            Vector3 myVector = gameObject.transform.position;
            myVector.y = myVector.y + increment; 
            Quaternion myRotation = gameObject.transform.rotation;
            gameObject.transform.SetPositionAndRotation(myVector,myRotation);
        }else{
            myButtonController.showConfidenceButtonsP2 = false;
            // Debug.Log("setting saftey text off");
            myButtonController.P2confidencePressed = false;
        }
        

    }
    public void animateDown(){
            if(gameObject.transform.position.y > minHeight){
                Vector3 myVector = gameObject.transform.position;
                myVector.y = myVector.y - increment; 
                Quaternion myRotation = gameObject.transform.rotation;
                gameObject.transform.SetPositionAndRotation(myVector,myRotation);
            }else{
                if(myButtonController.quesitonairePhase == false){
                myButtonController.hideConfidenceButtonsP2 = false;
                 }else{
                    if(myButtonController.P2TlXQuestions){
                        Vector3 myVector = gameObject.transform.position;
                        myVector.x -=.25f;
                        Quaternion myRotation = gameObject.transform.rotation;
                        gameObject.transform.SetPositionAndRotation(myVector,myRotation);
                        setTlxButtonsActive();
                        button1.GetComponentInChildren<TMP_Text>().text = "Low:\n1";
                        button7.GetComponentInChildren<TMP_Text>().text = "\n7";
                        foreach(var x in tempListButtons){
                            x.SetActive(true);
                        }

                        myButtonController.P2TlXQuestions = false;
                    }
                    myButtonController.hideConfidenceButtonsP2 = false;
                    if(myButtonController.p2Waiting == false){
                    myButtonController.showConfidenceButtonsP2 = true;
                    }
                 }
            }

        }

    public void setTlxButtonsActive(){
        button1.GetComponentInChildren<TMP_Text>().text = "Low:\n1";
        button7.GetComponentInChildren<TMP_Text>().text = "\n7";
        foreach(var x in tempListButtons){
            x.SetActive(true);
        }

    }


}
