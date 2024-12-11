using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class DemoanimationScriptConfidenceButtons : MonoBehaviour 
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
        if(myButtonController.showConfidenceButtons){
            animateUp();
        }
        if(myButtonController.hideConfidenceButtons){
            
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
            myButtonController.confidencePressed = false;
            myButtonController.showConfidenceButtons = false;
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
                    myButtonController.changePhase(myButtonController.currentPhase);
                
                
                    
                    myButtonController.hideConfidenceButtons = false;
                    if(myButtonController.quesitonairePhase == false){
                    myButtonController.showAnimation = true;
                    }
                }else{
                    if(myButtonController.TlXQuestions){
                        Vector3 myVector = gameObject.transform.position;
                        myVector.x +=.25f;
                        Quaternion myRotation = gameObject.transform.rotation;
                        gameObject.transform.SetPositionAndRotation(myVector,myRotation);
                        setTlxButtonsActive();
                        button1.GetComponentInChildren<TMP_Text>().text = "Low:\n1";
                        button7.GetComponentInChildren<TMP_Text>().text = "\n7";
                        
                        foreach(var x in tempListButtons){
                            x.SetActive(true);
                        }

                        myButtonController.TlXQuestions = false;
                    }
                    myButtonController.hideConfidenceButtons = false;
                    if(myButtonController.p1Waiting == false){
                    myButtonController.showConfidenceButtons = true;
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
