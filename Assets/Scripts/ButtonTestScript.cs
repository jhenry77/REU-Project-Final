using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using TMPro;
using System;

public class ButtonTestScript : NetworkBehaviour
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

    private ButtonController myController;

    double min_height;
    double max_height; 
    public int currPhase = 0;
    public int confidence;
    public Vector3 maxHeight;
    public bool timeForNewRandom = false;


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
        myController = myControllerObject.GetComponent<ButtonController>();
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
    [Server]
    // Update is called once per frame
    void FixedUpdate()
    {

        

        
        
    }
    
    
    

   
  
    [Server]
    public void pressOnServer(){
        if(myController.pressed){
            return;
        }
        myController.pressed = true;
        // 
        myController.dataEndInterval.Add((Time.time.ToString()));
        // Debug.Log("Just added an end time");
        // Debug.Log("End time is " + Time.time.ToString());
        // Debug.Log("Size of data end interval is " + myController.dataEndInterval.Count);
        // // Debug.Log("Our end time is" + Time.time.ToString());
        string pressedButton = gameObject.GetComponentInChildren<TMP_Text>().text;
        pressedButton = myController.combineString(pressedButton);
        // 
        myController.dataButtonChosenName.Add(pressedButton);
           
        myController.hideWhatToGesture();
        
        if(thisButtonInfo == buttonInfo.Correct && thisButtonType == buttonInfoType.Answers){
            myController.gotCorrect = true;
            myController.hideAnimation = true;
            myController.dataChoseCorrect.Add("True");
        }else if(thisButtonType == buttonInfoType.Answers && thisButtonInfo == buttonInfo.Incorrect){
            myController.gotCorrect = false;
            myController.hideAnimation = true;
            myController.dataChoseCorrect.Add("False");
        }
        foreach(var x in myController.myButtons){
            x.GetComponent<ButtonTestScript>().enabled = false;
        }
        
        
    }


    [Server]
    public void pressConfirmOnServer(){
        pressConfirmOnClient();
        myController.numConfirmed++;
        gameObject.SetActive(false);
        // Debug.Log("Called pressed on server and numConfirmed is" + myController.numConfirmed);
        if(myController.numConfirmed == 2){
            myController.mySceneController.startGame();
        }else if (myController.numConfirmed == 4){
            myController.resetToBeginning();
        }else if (myController.numConfirmed == 6){
            myController.resetToBeginning();
        }
    }


    [ClientRpc]
    public void pressConfirmOnClient(){
        
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



    [Command(requiresAuthority = false)]
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

   
    // [Client]
    // public void pressConfidence7(){
    //      if(myController.quesitonairePhase == true){
    //         confidence = 7;
    //         myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
    //     }else{
    //     confidence = 7;
    //     myController.setConfidence(confidence.ToString());
    //     // myController.hideConfidenceButtons = true;
    //     serverSetAnimateConfidenceDownTrue();

    //     }
    // }
     [Server]
    public void pressConfidence1Server(){
        
        if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("1");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
       
        
        myController.dataconfidenceInt.Add("1");
        myController.hideConfidenceButtons = true;
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }

    }

    [Server]
    public void pressConfidence2Server(){
        
        if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("2");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            
            myController.hideConfidenceButtons = true;

        }else{
           
        
        myController.dataconfidenceInt.Add("2");
        myController.hideConfidenceButtons = true;
           
        myController.updateGuesserCorrectText(myController.gotCorrect);    
        }

    }
    [Server]
    public void pressConfidence3Server(){
        if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("3");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            
            myController.hideConfidenceButtons = true;

        }else{
             
        myController.dataconfidenceInt.Add("3");
        myController.hideConfidenceButtons = true;
           
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }
    [Server]
    public void pressConfidence4Server(){
        if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("4");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
             
        myController.dataconfidenceInt.Add("4");
        myController.hideConfidenceButtons = true;
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }
    [Server]
    public void pressConfidence5Server(){
        
        if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("5");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
            
        
        myController.dataconfidenceInt.Add("5");
        myController.hideConfidenceButtons = true;
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }
    [Server]
    public void pressConfidence6Server(){
        
        if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("6");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
             
        
        myController.dataconfidenceInt.Add("6");
        myController.hideConfidenceButtons = true;
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }
    [Server]
    public void pressConfidence7Server(){
        
         if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("7");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
        
        myController.dataconfidenceInt.Add("7");
        myController.hideConfidenceButtons = true;
           
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }
     [Server]
    public void pressConfidence8Server(){
         if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("8");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
        
        myController.dataconfidenceInt.Add("8");
        myController.hideConfidenceButtons = true;
           
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }
     [Server]
    public void pressConfidence9Server(){
         if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("9");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
        
        myController.dataconfidenceInt.Add("9");
        myController.hideConfidenceButtons = true;
           
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }
     [Server]
    public void pressConfidence10Server(){
         if(myController.confidencePressed){
            return;
        }
        myController.confidencePressed = true;
        if(myController.quesitonairePhase == true){
            myController.dataP1QuestionaireAnswer.Add("10");
            myController.changeQuestionairePhasePlayer1(myController.questionaireNumberP1);
            myController.hideConfidenceButtons = true;

        }else{
        
        myController.dataconfidenceInt.Add("10");
        myController.hideConfidenceButtons = true;
           
        myController.updateGuesserCorrectText(myController.gotCorrect);
        }
    }
   
    [Server]
    public void pressConfidenceP21Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
         if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("1");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
            
       

        }

    }
    [Server]
    public void pressConfidenceP22Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("2");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
       
        }

    }
    [Server]
    public void pressConfidenceP23Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("3");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
        
        }
    }
    [Server]
    public void pressConfidenceP24Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("4");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
        
        }
    }
    [Server]
    public void pressConfidenceP25Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("5");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
         
        }
    }
    [Server]
    public void pressConfidenceP26Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("6");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
         
        }
    }
    [Server]
    public void pressConfidenceP27Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("7");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
        
        }
    }
    [Server]
    public void pressConfidenceP28Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("8");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
        
        }
    }
    [Server]
    public void pressConfidenceP29Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("9");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
        
        }
    }
    [Server]
    public void pressConfidenceP210Server(){
        if(myController.P2confidencePressed){
            return;
        }
        myController.P2confidencePressed = true;
        if(myController.quesitonairePhase == true){
            // 
            myController.dataP2QuestionaireAnswer.Add("10");
            myController.changeQuestionairePhasePlayer2(myController.questionaireNumberP2);
            
            myController.hideConfidenceButtonsP2 = true;
        }else{
        
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
