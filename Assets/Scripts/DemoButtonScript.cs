using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using TMPro;
using System;

public class DemoButtonScript : MonoBehaviour
{
    
    float increment = 0.001F;
  
    public GameObject WinnerText;
    public GameObject LoserText;
    public enum buttonInfo{
        Correct,Incorrect
    }

    public int numConfirmed = 0;

    public enum buttonInfoType{
        Answers, Confidence, P2Confidence
    }
    bool correctSelection = false;
    

    buttonInfo thisButtonInfo =buttonInfo.Incorrect;
    buttonInfoType thisButtonType;

    public Vector3 initialLocation;
    public Quaternion initialRotation;

    public GameObject myControllerObject;

    private DemoButtonController myController;

    double min_height;
    double max_height; 
    public int currPhase = 0;
    public int confidence;
    public Vector3 maxHeight;
    public bool timeForNewRandom = false;

    public int numWaiting= 0;


    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.tag == "ConfidenceButtons"){
            thisButtonType = buttonInfoType.Confidence;
        }else if(gameObject.tag == "Button"){
            thisButtonType = buttonInfoType.Answers;
        }else if(gameObject.tag == "P2QuestionaireButtons"){
            thisButtonType = buttonInfoType.P2Confidence;
        }
        myController = myControllerObject.GetComponent<DemoButtonController>();
        initialLocation = gameObject.transform.position;
        initialRotation = gameObject.transform.rotation;
        maxHeight = initialLocation;
        if(gameObject.tag == "ConfidenceButtons" || gameObject.tag == "P2QuestionaireButtons"){
            max_height = initialLocation.y + .4f;
            min_height = initialLocation.y;
            maxHeight.y = (float)max_height;
        }
        if(gameObject.tag == "Button"){
            max_height = initialLocation.y;
            min_height = initialLocation.y - .4f;
        
        }
        WinnerText.gameObject.SetActive(false);
        LoserText.gameObject.SetActive(false);
        
    }
   
    
    

   
  
    public void pressOnServer(){
        if(myController.pressed){
            return;
        }
        myController.pressed = true;

           
        myController.hideWhatToGesture();
        myController.hideAnimation = true;
        // if(thisButtonInfo == buttonInfo.Correct && thisButtonType == buttonInfoType.Answers){
        //     myController.gotCorrect = true;
        //     myController.hideAnimation = true;
        // }else if(thisButtonType == buttonInfoType.Answers && thisButtonInfo == buttonInfo.Incorrect){
        //     myController.gotCorrect = false;
        //     myController.hideAnimation = true;
        // }
        foreach(var x in myController.myButtons){
            x.GetComponent<DemoButtonScript>().enabled = false;
        }
        
        
    }


    public void pressConfirmOnServer(){
        myController.numConfirmed++;
        if(myController.numConfirmed == 2){
            myController.startGame();
        }else if (myController.numConfirmed == 4){
            myController.resetToBeginning();
        }else if (myController.numConfirmed == 6){
            myController.resetToBeginning();
        }
        gameObject.SetActive(false);
    }

    public void showConfidenceP2(){
         if(gameObject.transform.position.y < max_height){
            Vector3 myVector = gameObject.transform.position;
            myVector.y = myVector.y + increment; 
            Quaternion myRotation = gameObject.transform.rotation;
            gameObject.transform.SetPositionAndRotation(myVector,myRotation);
        }else{
            gameObject.transform.SetPositionAndRotation(maxHeight,initialRotation);
            myController.showConfidenceButtonsP2 = false;
            // serverSetAnimateConfidenceUpFalse();
        }

    }
    public void hideConfidenceP2(){
         if(gameObject.transform.position.y > min_height){
            Vector3 myVector = gameObject.transform.position;
            myVector.y = myVector.y - increment; 
            Quaternion myRotation = gameObject.transform.rotation;
            gameObject.transform.SetPositionAndRotation(myVector,myRotation);
        }else{
            myController.hideConfidenceButtonsP2 = false;

         
            

        }

    }

    public void hideConfidenceP1QuestionairePhase(){
         if(gameObject.transform.position.y > min_height){
            Vector3 myVector = gameObject.transform.position;
            myVector.y = myVector.y - increment; 
            Quaternion myRotation = gameObject.transform.rotation;
            gameObject.transform.SetPositionAndRotation(myVector,myRotation);
        }else{

        }

    }



    public void serverSetAnimateConfidenceDownTrue(){
        myController.hideConfidenceButtons = true;

    }

  
    public void setCorrect(){
        thisButtonInfo = buttonInfo.Correct;
    }

    public void setIncorrect(){
        thisButtonInfo = buttonInfo.Incorrect;
    }

    public void setTextName(string changed){
        TMP_Text myText = gameObject.GetComponentInChildren<TMP_Text>();
        myText.text = changed;
    }

   
    public void pressConfidenceButton(){
         if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
        myController.hideConfidenceButtons = true;
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }

   
  
    public void pressConfidenceP2(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
         if(myController.quesitonairePhase == true){
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }

    }



    

    public void hideConfidenceQuestionaireP1(){
         if(gameObject.transform.position.y > min_height){
            Vector3 myVector = gameObject.transform.position;
            myVector.y = myVector.y - increment; 
            Quaternion myRotation = gameObject.transform.rotation;
            gameObject.transform.SetPositionAndRotation(myVector,myRotation);
        }else{
            myController.hideP1Confidence = false;
    
        }

    }
   

   
}
