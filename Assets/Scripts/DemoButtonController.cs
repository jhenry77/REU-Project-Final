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

public class DemoButtonController : MonoBehaviour 
{
    public GameObject TFPText;

    public GameObject charadeText;

    public GameObject GuesserText;
    public GameObject CharadeText;
    public GameObject Questionaire;
    public GameObject CorrectText;
    public GameObject IncorrectText;

    
    [HideInInspector]
    public bool showAnimation = false;
    
    [HideInInspector]
    public bool hideAnimation = false;
    
    [HideInInspector]
    public bool showConfidenceButtons = false;
    
    [HideInInspector]
    public bool hideConfidenceButtons = false;
    
    [HideInInspector]
    public bool hideConfidenceButtonsP2 = false;
    
    [HideInInspector]
    public bool showConfidenceButtonsP2 = false;

    public GameObject p1ConfirmButton;
    public GameObject p2ConfirmButton;

    public GameObject[] myButtons;
    public GameObject[] confidenceButtons;
    public GameObject[] P2QuestionaireButtons;
    public GameObject P17Button;
    public GameObject P27Button;
    public GameObject P11Button;
    public GameObject P21Button;
    public List<string> myQuestions = new List<string>();

    public List<NumbersClass> myNumberQuestions = new List<NumbersClass>();

    public int randQuestion;
    
    public int randOrder = 0;
    public bool gotCorrect = false;
    public bool gotIncorrect = false;
    public bool timeToShow = false;
    public bool p1Waiting = false;
    public bool p2Waiting = false;
    
    public List<float[]> handSizeOrder = new List<float[]> { new float[] {1f,.75f,1.25f}, new float[]{1f, 1.25f, .75f}, new float[]{1.25f, 1f, .75f}, new float[] {1.25f, .75f, 1f}, new float[] {.75f, 1.25f, 1f}, new float[] {.75f, 1f, 1.25f} };


    public enum questionPhase{
        Easy,Medium,Hard,Questionaire
    }
    
    public questionPhase currentQuestionPhase = questionPhase.Easy;

    public int currentPhase = 0;
    public bool questionaireUp = false;
    public bool questionaireDown = false;
  
    public bool quesitonairePhase = false;
    public float max_height;
    public float min_height;
    public int questionaireNumberP1 = 0;
    public int questionaireNumberP2 = 0;
    public bool timeToSetScale = false;
    public DemoScript myDemoScript;
 
    public int numWaitingInQuestionaire = 0;
    public bool hideP1Confidence = false;
    public bool hideP2Confidence = false;
    
    public int onlyCallOnce = 0;
    public int questionaireNumSeen = 0;

    public int player1Pid;
    public int player2Pid;
    public bool timeForRandom = false;
    public int orderOfhands;
    public bool pressed = false;
    public bool confidencePressed = true;
    public bool P2confidencePressed = true;
    public bool TlXQuestions = false;
    public bool P2TlXQuestions = false;
    public GameObject[] p1TlxButtons;
    public GameObject[] p2TlxButtons;
    public GameObject p1ConfidenceParent;
    public GameObject p2ConfidenceParent;
    public GameObject confidenceText;

    public int numConfirmed = 0;
    [HideInInspector]
    public bool firstQuetsion = true;

    public int questionNum = 0;

    




    
    public void setTrippleButtonsOn(){
        foreach(var x in myButtons){
            x.SetActive(true);
        }
    }

   

    public void setConfrimButtonsActive(){
        p1ConfirmButton.SetActive(true);
        p2ConfirmButton.SetActive(true);
    }
   
    public void showWhatToGesture(){
        charadeText.SetActive(true);
    }
    public void hideWhatToGesture(){
        charadeText.SetActive(false);

    }
    
    
    
    


    public void RandQuestionChanged(int oldVal, int newVal){
        randQuestion = newVal;
    }

    public void randOrderChanged(int oldVal, int newVal){
        randOrder = newVal;
        
    }

    public void CMDgetRandomQuestion(int low, int high){
        randQuestion = Random.Range(low,high);

    }
    public void CMDgetRandomOrder(int low, int high){
        // 
        randOrder = Random.Range(low,high);
        // 

    }

    public void updateGuesserCorrectText(bool isCorrect){
        if(isCorrect){
            CorrectText.SetActive(true);
            IncorrectText.SetActive(false);
        }else{
            IncorrectText.SetActive(true);
            CorrectText.SetActive(false);
        }

    }
    public void turnOffCorrectText(){
        CorrectText.SetActive(false);
        IncorrectText.SetActive(false);
    }
 

    
    
    void FixedUpdate(){
        if(questionaireUp){
            animateQuestionareUp();
            
        }
        if(questionaireDown){
            animateQuestionareDown();
        }
        if(timeForRandom){
            // 
            StartCoroutine(getRandomthenchangeButtonName());
            timeForRandom = false;

        }
    }
    

    


    // Start is called before the first frame update
    void Start()
    {
        searchForSceneController();
       
       
    

        //Ryans question
        myQuestions.Add("It felt easy to perform the gestures I intended to.");
        //Lin
        myQuestions.Add("I felt as if the virtual\nhands were part of my body.");
        myQuestions.Add("It sometimes seemed like my own hands\n came into contact with the buttons.");
        myQuestions.Add("I thought the virtual hands on\n the screen looked realistic.");
        myQuestions.Add("I was so immersed in the virtual\n environment, it seemed real.");
        myQuestions.Add("I felt like using my virtual hands\n to communicate was fun.");
        myQuestions.Add("I felt like I could very efficiently use\n my virtual hands to complete the task.");
        myQuestions.Add("I felt as if I could cause\n movements of the virtual hands.");
        myQuestions.Add("It felt as if I could control\n movements of the virtual hands.");
        myQuestions.Add("I felt as if the virtual hands moved\n just like I wanted them to,\n as if they were obeying my own will");
        //Networked minds
        myQuestions.Add("My thoughts were clear to my partner.");
        myQuestions.Add("My partner’s thoughts were clear to me.");
        myQuestions.Add("It was easy to understand my partner.");
        myQuestions.Add("Understanding my partner was difficult.");
        myQuestions.Add("My partner had difficulty understanding me");
        //Hecht
        myQuestions.Add("The other person let me know that\n I was communicating effectively.");
        myQuestions.Add("Nothing was accomplished.");
        myQuestions.Add("I was very dissatisfied with the communication\n with my partner during the game.");
        myQuestions.Add("I felt that during the game I was able\n to present myself as I wanted\n the other person to view me.");
        myQuestions.Add("I did not enjoy communicating with\n my partner during the game.");
        //Nasa Tlx
        myQuestions.Add("How mentally demanding was the task?");
        myQuestions.Add("How physically demanding was the task?");
        myQuestions.Add("How hurried or rushed was\n the pace of the task?");
        myQuestions.Add("How successful were you in accomplishing\n what you were asked to do?");
        myQuestions.Add("How hard did you have to work to accomplish\n your level of performance?");
        myQuestions.Add("How insecure, discouraged, irritated,\n stressed, and annoyed were you?");

        
        
        max_height = Questionaire.transform.position.y + .8f;
        min_height = Questionaire.transform.position.y;
        // 
        
    }
    
    public void setInitialNumbers(){
        changeButtonName(currentQuestionPhase);
        
        
    }
    
    public IEnumerator getRandomthenchangeButtonName(){
        if(currentQuestionPhase == questionPhase.Easy){
            // 
            CMDgetRandomQuestion(0,myNumberQuestions.Count);
        }
        CMDgetRandomOrder(0,6);
        yield return new WaitForSeconds(1);
        // 
        changeButtonName(currentQuestionPhase);
        if(firstQuetsion){
            firstQuetsion = false;
        }
        
    }

    
    public void changeButtonName(questionPhase currentQuestionPhase){
        if(currentPhase ==12){
            currentPhase = 13;
            return;
        }

        if (currentQuestionPhase == questionPhase.Easy)
        {

            for (int i = 0; i < 3; i++)
            {

                DemoButtonScript myTestScript = myButtons[i].GetComponent<DemoButtonScript>();
                if (myTestScript == null)
                {
                    Debug.Log("myTestScript was null");
                    return;
                }
                else
                {
                    myTestScript.setTextName(myDemoScript.buttonList[questionNum][i]);
                }
            }




            changeCharadeText(myDemoScript.buttonList[questionNum][3]);
            currentPhase++;
            questionNum++;




        }

    }

   
    public void changePhase(int phase){
        if(phase == 13){
            currentQuestionPhase = questionPhase.Questionaire;
            startQuestionairephase();
        }
        
    }
    public void changeCharadeText(string newInput){
        TMP_Text myText = charadeText.GetComponent<TMP_Text>();
        myText.text = "Gesture:\n" + newInput;
    }

   
    public void hideConfidenceText(){
        confidenceText.SetActive(false);
    }

    public void showConfidenceText(){
        confidenceText.SetActive(true);
    }

    
    public void startQuestionairephase(){
        // 
        confidenceText.SetActive(false);
        hideConfidenceText();
        turnOffCorrectText();
        
        
        hideWhatToGesture();
        searchForSceneController();
        quesitonairePhase = true;
        showConfidenceButtons = true;
        showConfidenceButtonsP2 = true;
        foreach(var x in myButtons){
            x.SetActive(false);
        }
        foreach(var x in P2QuestionaireButtons){
            x.SetActive(true);
        }
        Questionaire.SetActive(true);
        questionaireUp = true;
        setP2ConfidenceButtonson();
        changeQuestionaireTextPlayer1(myQuestions[questionaireNumberP1+1]);
        changeQuestionaireTextPlayer2(myQuestions[questionaireNumberP2]);

    
    }
    
    public void setP2ConfidenceButtonson(){
        Questionaire.SetActive(true);
        foreach(var x in myButtons){
            x.SetActive(false);
        }
        foreach(var x in P2QuestionaireButtons){
            x.SetActive(true);
        }
        
    }
    public void animateQuestionareUp(){
            // 
            if(Questionaire.transform.position.y < max_height){
            Vector3 myPosition = Questionaire.transform.position;
            myPosition.y += .003f;
            Questionaire.transform.position = myPosition;
            }else{
                questionaireUp = false;
            }
        

    }

    public void animateQuestionareDown(){
        // 
        if(Questionaire.transform.position.y > min_height){
            Vector3 myPosition = Questionaire.transform.position;
            myPosition.y -= .003f;
            Questionaire.transform.position = myPosition;
        }else{
            questionaireDown = false;
            if(questionaireNumSeen == 2){
                resetToBeginning();
            }else{
            startConfirmSection();
            }
            
        }
    }
    public void changeQuestionaireTextPlayer1(string newInput){
       TMP_Text myText = GuesserText.GetComponent<TMP_Text>();
        myText.text = newInput;
        changeP1QuestionaireTextOnClient(newInput);

    }
     public void changeQuestionaireTextPlayer2(string newInput){
        // 
       TMP_Text myText = CharadeText.GetComponent<TMP_Text>();
        myText.text = newInput;
        changeP2QuestionaireTextOnClient(newInput);

    }

    public void changeP1QuestionaireTextOnClient(string newInput){
        TMP_Text myText = GuesserText.GetComponent<TMP_Text>();
        myText.text = newInput;
    }

    public void changeP2QuestionaireTextOnClient(string newInput){
        TMP_Text myText = CharadeText.GetComponent<TMP_Text>();
        myText.text = newInput;
    }
     
    public void setServeranimateP2Down(){
        hideP2Confidence = true;
    } 
    public void setServeranimateP1Down(){
        hideP1Confidence = true;
    }


    public void startConfirmSection(){
        p1ConfirmButton.SetActive(true);
        p2ConfirmButton.SetActive(true);
        startConfirmSectionClient();
    }

    public void startConfirmSectionClient(){
        p1ConfirmButton.SetActive(true);
        p2ConfirmButton.SetActive(true);
    }
    public void changeQuestionairePhasePlayer1(int phase){
        if(questionaireNumberP1 == 18){
            TlXQuestions = true;
        }
        if(questionaireNumberP1 == 24){
            p1Waiting = true;
            hideConfidenceButtons = true;
            
            numWaitingInQuestionaire++;
            changeQuestionaireTextPlayer1("Please wait for your partner\n to finish answering their questions");
            if(numWaitingInQuestionaire == 2){
                
                questionaireDown = true;
                
                
                


            }
            return;
        }
        // 
        questionaireNumberP1++;
        changeQuestionaireTextPlayer1(myQuestions[questionaireNumberP1+1]);
        
        

    }
     public void changeQuestionairePhasePlayer2(int phase){
         if(questionaireNumberP2 == 19){
            P2TlXQuestions = true;
        }
        if(questionaireNumberP2 == 25){
            p2Waiting = true;
            hideConfidenceButtonsP2 = true;

            // 
            numWaitingInQuestionaire++;
            // 
            changeQuestionaireTextPlayer2("Please wait for your partner\n to finish answering their questions");
            if(numWaitingInQuestionaire == 2){
                // 
                questionaireDown = true;
            
                
            }
            
            return;
            
        }
        // 
        questionaireNumberP2++;
        changeQuestionaireTextPlayer2(myQuestions[questionaireNumberP2]);
        

    } 
    public void incrementNumWaitingInQuestionaire(){
        // 
        numWaitingInQuestionaire++;
    } 
    public void setServerQuestionaireDown(){
        questionaireDown = true;
    } 
    public void hideBothConfidence(){
        hideP1Confidence = true;
        hideP2Confidence = true;
    }
 
     public void animateQuestionaiupServer(){
        animateQuestionareUp();
     
    } 
     public void animateQuestionaiupServerDown(){
        animateQuestionareDown();
     
    } 
    public void ServerP1QButtonsDown(){
        hideConfidenceButtons = true;
    } 
    public void ServerP2QButtonsDown(){
        hideP2Confidence = true;
    }

    
    public void searchForSceneController(){
        // 
        GameObject controllerObject = GameObject.FindGameObjectWithTag("SceneController");
        myDemoScript = controllerObject.GetComponent<DemoScript>();
        //searchForSceneControllerServer();
        
    }
    public void resetTlx(){
        P11Button.GetComponentInChildren<TMP_Text>().text = "Strongly Disagree:\n1";
        P21Button.GetComponentInChildren<TMP_Text>().text = "Strongly Disagree:\n1";
        P17Button.GetComponentInChildren<TMP_Text>().text = "Strongly Agree:\n7";
        P27Button.GetComponentInChildren<TMP_Text>().text = "Strongly Agree:\n7";

        
        foreach(var x in p1TlxButtons){
            x.SetActive(false);
        }
       
        foreach(var x in p2TlxButtons){
            x.SetActive(false);
        }
        
    }

    
    public void resetToBeginning(){
        // 
        confidenceText.SetActive(true);
        showConfidenceText();
        p1Waiting = false;
        p2Waiting = false;
        resetTlx();
        P11Button.GetComponentInChildren<TMP_Text>().text = "Strongly Disagree:\n1";
        P21Button.GetComponentInChildren<TMP_Text>().text = "Strongly Disagree:\n1";
        P17Button.GetComponentInChildren<TMP_Text>().text = "Strongly Agree\n7";
        P27Button.GetComponentInChildren<TMP_Text>().text = "Strongly Agree\n7";
        Vector3 myVector = p1ConfidenceParent.transform.position;
        myVector.x -= .25f;
        Quaternion myRotation = p1ConfidenceParent.transform.rotation;
        p1ConfidenceParent.transform.SetPositionAndRotation(myVector,myRotation);
        myVector = p2ConfidenceParent.transform.position;
        myVector.x +=.25f;
        myRotation = p2ConfidenceParent.transform.rotation;
        p2ConfidenceParent.transform.SetPositionAndRotation(myVector,myRotation);
       
        foreach(var x in p1TlxButtons){
            x.SetActive(false);
        }
        
        foreach(var x in p2TlxButtons){
            x.SetActive(false);
        }

        
        questionaireNumSeen++;
        currentPhase = 0;
        numWaitingInQuestionaire = 0;
        questionaireNumberP1 = 0;
        questionaireNumberP2 = 0;
        quesitonairePhase = false;
        currentQuestionPhase = questionPhase.Easy;
        if(questionaireNumSeen == 3){
            TFPText.SetActive(true);
            

        }else{
        foreach(var x in myButtons){
            x.SetActive(true);
        }
        foreach(var x in myButtons){
                x.GetComponent<DemoButtonScript>().enabled = true;
            }
        foreach(var x in confidenceButtons){
                x.GetComponent<DemoButtonScript>().enabled = true;
            }
        foreach(var x in P2QuestionaireButtons){
                x.GetComponent<DemoButtonScript>().enabled = true;
            }
        setInitialNumbers();
        showAnimation = true;
        
        }

    }

     public void endGameClient(){
        TFPText.SetActive(true);
        CorrectText.SetActive(false);
        IncorrectText.SetActive(false);
        foreach(var x in myButtons){
            x.SetActive(false);
        }
        foreach(var x in confidenceButtons){
            x.SetActive(false);
        }
        foreach(var x in P2QuestionaireButtons){
            x.SetActive(false);
        }

     }
    public void resetToBeginningClient(){
        quesitonairePhase = false;
        currentPhase = 0;
        numWaitingInQuestionaire = 0;
        questionaireNumberP1 = 0;
        questionaireNumberP2 = 0;
        currentQuestionPhase = questionPhase.Easy;
        foreach(var x in myButtons){
            x.SetActive(true);
        }

    }
    public IEnumerator waitForEndofanimation(){
        yield return new WaitForSecondsRealtime(2);

    }

    
    public void storeButtonName(string Button1, string Button2, string Button3){
        // 
        string input1 = combineString(Button1);
        string input2 = combineString(Button2);
        string input3 = combineString(Button3);
        

    }
    public string makeFileName(string fileName, int i = 0){
        if(File.Exists(fileName+"v_"+i.ToString() + ".csv")){
            return makeFileName(fileName, ++i);
        }
        return fileName +"v_"+i.ToString() +".csv";
    }
    

    
   
    public string combineString(string toSplit){
        string[] mySplits = toSplit.Split('\n');
        string myReturn = "";
        foreach(string x in mySplits){
            myReturn += " " + x;
        }
        return myReturn;

    }
    
    public string combineStringComma(string toSplit){
        string[] mySplits = toSplit.Split(',');
        string myReturn = "";
        foreach(string x in mySplits){
            myReturn += " " + x;
        }
        return myReturn;
    }
    
    public void startGame(){
        setTrippleButtonsOn();
        setInitialNumbers();

    }
   


    

   
  
    }

   
    
             

   
    
    



    
