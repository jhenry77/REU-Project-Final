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

public class ButtonController : NetworkBehaviour
{
    public GameObject TFPText;

    public GameObject charadeText;

    public GameObject GuesserText;
    public GameObject CharadeText;
    public GameObject Questionaire;
    public GameObject CorrectText;
    public GameObject IncorrectText;

    [HideInInspector]
    [SyncVar]
    public bool showAnimation = false;
    [HideInInspector]
    [SyncVar]
    public bool hideAnimation = false;
    [HideInInspector]
    [SyncVar]
    public bool showConfidenceButtons = false;
    [HideInInspector]
    [SyncVar]
    public bool hideConfidenceButtons = false;
    [HideInInspector]
    [SyncVar]
    public bool hideConfidenceButtonsP2 = false;
    [HideInInspector]
    [SyncVar]
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

    public List<string> dataButtonName = new List<string>();
    public List<string> dataChoseCorrect = new List<string>();
    public List<string> dataButtonChosenName = new List<string>();
    public List<string> dataCorrectButtonName = new List<string>();
    public List<string> dataStartInterval = new List<string>();
    public List<string> dataEndInterval = new List<string>();
    public List<string> dataconfidenceInt = new List<string>();
    public List<string> dataHandSize = new List<string>();
    public List<string> dataP1QuestionaireAnswer = new List<string>();
    public List<string> dataP2QuestionaireAnswer = new List<string>();

    public List<NumbersClass> myNumberQuestions = new List<NumbersClass>();
    public List<MediumQuestions> myMediumQuestions = new List<MediumQuestions>();
    public List<HardQuestions> myHardQuestions = new List<HardQuestions>();
    [SyncVar(hook = "RandQuestionChanged")]
    public int randQuestion;
    [SyncVar(hook = "randOrderChanged")]
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
    [SyncVar]
    public questionPhase currentQuestionPhase = questionPhase.Easy;

    public int currentPhase = 0;
    public bool questionaireUp = false;
    public bool questionaireDown = false;
    [SyncVar]
    public bool quesitonairePhase = false;
    public float max_height;
    public float min_height;
    public int questionaireNumberP1 = 0;
    public int questionaireNumberP2 = 0;
    public bool timeToSetScale = false;
    public SceneController mySceneController;
    [SyncVar]
    public int numWaitingInQuestionaire = 0;
    public bool hideP1Confidence = false;
    public bool hideP2Confidence = false;
    [SyncVar(hook = "itterateServerCall")]
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

    
    public void itterateServerCall(int oldVal, int newVal){
        // 
        onlyCallOnce = newVal;
    }
    [Command(requiresAuthority = false)]
    public void itterateOnlyCallOnce(){
        // 
        onlyCallOnce++;
    }



    [Server]
    public void setTrippleButtonsOn(){
        foreach(var x in myButtons){
            x.SetActive(true);
        }
        setTrippleButtonsOnClient();
    }

    [ClientRpc]
    public void setTrippleButtonsOnClient(){
        foreach(var x in myButtons){
            x.SetActive(true);
        }
    }

    [ClientRpc]
    public void setConfrimButtonsActive(){
        p1ConfirmButton.SetActive(true);
        p2ConfirmButton.SetActive(true);
    }
   
    [ClientRpc]
    public void showWhatToGesture(){
        charadeText.SetActive(true);
    }
    [ClientRpc]
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

    [ClientRpc]
    public void updateGuesserCorrectText(bool isCorrect){
        if(isCorrect){
            CorrectText.SetActive(true);
            IncorrectText.SetActive(false);
        }else{
            IncorrectText.SetActive(true);
            CorrectText.SetActive(false);
        }

    }
    [ClientRpc]
    public void turnOffCorrectText(){
        CorrectText.SetActive(false);
        IncorrectText.SetActive(false);
    }
 

    
    [Server]
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
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "10",
            incorrectAnswer1 = "2",
            incorrectAnswer2 = "9"
        });

        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "7",
            incorrectAnswer1 = "1",
            incorrectAnswer2 = "4"
        });

        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "3",
            incorrectAnswer1 = "8",
            incorrectAnswer2 = "6"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "5",
            incorrectAnswer1 = "3",
            incorrectAnswer2 = "2"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "7",
            incorrectAnswer1 = "10",
            incorrectAnswer2 = "9"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "4",
            incorrectAnswer1 = "1",
            incorrectAnswer2 = "2"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "9",
            incorrectAnswer1 = "3",
            incorrectAnswer2 = "5"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "6",
            incorrectAnswer1 = "4",
            incorrectAnswer2 = "1"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "8",
            incorrectAnswer1 = "7",
            incorrectAnswer2 = "6"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "1",
            incorrectAnswer1 = "5",
            incorrectAnswer2 = "10"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "9",
            incorrectAnswer1 = "3",
            incorrectAnswer2 = "7"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "2",
            incorrectAnswer1 = "8",
            incorrectAnswer2 = "4"
        });

        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Motorcycle",
            incorrectAnswer1 = "Plane",
            incorrectAnswer2 = "Car"
        });

        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Football",
            incorrectAnswer1 = "Baseball",
            incorrectAnswer2 = "Volleyball"
        });

        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Swimming",
            incorrectAnswer1 = "Kayaking",
            incorrectAnswer2 = "Pitcher"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Golf",
            incorrectAnswer1 = "Baseball",
            incorrectAnswer2 = "Basketball"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Diving",
            incorrectAnswer1 = "Swimming",
            incorrectAnswer2 = "Kayaking"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Ice Skating",
            incorrectAnswer1 = "Bobsledding",
            incorrectAnswer2 = "Climbing"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Baseball",
            incorrectAnswer1 = "Tennis",
            incorrectAnswer2 = "Golf"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Boxing",
            incorrectAnswer1 = "Karate",
            incorrectAnswer2 = "Weight lifting"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Violin",
            incorrectAnswer1 = "Cello",
            incorrectAnswer2 = "Guitar"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Clarinet",
            incorrectAnswer1 = "Trumpet",
            incorrectAnswer2 = "Flute"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Ukelele",
            incorrectAnswer1 = "Guitar",
            incorrectAnswer2 = "Triangle"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Harmonica",
            incorrectAnswer1 = "Trombone",
            incorrectAnswer2 = "Kazoo"
        });


        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Segway",
            incorrectAnswer1 = "Boat",
            incorrectAnswer2 = "HorseBack\nRiding"
        });

        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Softball",
            incorrectAnswer1 = "Baseball",
            incorrectAnswer2 = "Football"
        });

        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Table Tennis",
            incorrectAnswer1 = "Tennis",
            incorrectAnswer2 = "Baseball"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Kayaking",
            incorrectAnswer1 = "Rowing",
            incorrectAnswer2 = "Swimming"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Snowboarding",
            incorrectAnswer1 = "Skiing",
            incorrectAnswer2 = "Gymnastics"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Sloth",
            incorrectAnswer1 = "Rhino",
            incorrectAnswer2 = "Swan"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Frog",
            incorrectAnswer1 = "Rabbit",
            incorrectAnswer2 = "Mouse"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Steak",
            incorrectAnswer1 = "Lasagna",
            incorrectAnswer2 = "Spaghetti"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Secretary",
            incorrectAnswer1 = "Truck Driver",
            incorrectAnswer2 = "Judge"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Nurse",
            incorrectAnswer1 = "Flight\nAttendant",
            incorrectAnswer2 = "Construction\nWorker"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Gambling",
            incorrectAnswer1 = "Winning a Race",
            incorrectAnswer2 = "Monopoly"
        });
        myNumberQuestions.Add(new NumbersClass{
            correctAnswer = "Indiana Jones",
            incorrectAnswer1 = "The Office",
            incorrectAnswer2 = "Black Mirror"
        });

       
    

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
        
        // 
        StartCoroutine(getRandomthenchangeButtonName());
        // 
        if(questionaireNumSeen >= 1){
            return;
        }
        
    }
    
    public IEnumerator getRandomthenchangeButtonName(){
        if(currentQuestionPhase == questionPhase.Easy){
            // 
            CMDgetRandomQuestion(0,myNumberQuestions.Count);
        }else if(currentQuestionPhase == questionPhase.Medium){
            //  
            CMDgetRandomQuestion(0,myMediumQuestions.Count);
        }else if(currentQuestionPhase == questionPhase.Hard){
            //  
            CMDgetRandomQuestion(0,myHardQuestions.Count);
        }
        CMDgetRandomOrder(0,6);
        yield return new WaitForSeconds(1);
        // 
        changeButtonName(currentQuestionPhase);
        if(firstQuetsion){
          
        
            dataStartInterval.Add(Time.time.ToString());
            // Debug.Log("added a start time ");
            // Debug.Log("Start time is " + Time.time.ToString());
            // Debug.Log("Length of start interval is " + dataStartInterval.Count);
            firstQuetsion = false;
        }
        
    }

    [Server]
    public void changeButtonName(questionPhase currentQuestionPhase){
        // Debug.Log("Just entered change button name and current phase is" + currentPhase);
        // 
        if(currentPhase ==12){
            currentPhase = 13;
            // Debug.Log("Inside of the current phase == 12 check and now setting currentPhase to " + currentPhase);
            return;
        }
    
        if(currentQuestionPhase == questionPhase.Easy){
            // 
            //CMDgetRandom(0,myNumberQuestions.Count);
            // 
            
            // 
            switch(randOrder){
                case 0:
                    ButtonTestScript thisButtonScript = myButtons[0].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].correctAnswer);
                    thisButtonScript.setCorrect();
                    changeButtonNameClient(0,myNumberQuestions[randQuestion].correctAnswer, true);
                    thisButtonScript = myButtons[1].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer1);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(1,myNumberQuestions[randQuestion].incorrectAnswer1, false);
                    thisButtonScript = myButtons[2].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer2);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(2,myNumberQuestions[randQuestion].incorrectAnswer2, false);
                    break;
                case 1:
                    thisButtonScript = myButtons[0].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].correctAnswer);
                    thisButtonScript.setCorrect();
                    changeButtonNameClient(0,myNumberQuestions[randQuestion].correctAnswer, true);
                    thisButtonScript = myButtons[1].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer2);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(1,myNumberQuestions[randQuestion].incorrectAnswer2, false);
                    thisButtonScript = myButtons[2].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer1);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(2,myNumberQuestions[randQuestion].incorrectAnswer1, false);
                    
                    break;
                case 2:
                    thisButtonScript = myButtons[0].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer1);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(0,myNumberQuestions[randQuestion].incorrectAnswer1, false);
                    thisButtonScript = myButtons[1].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].correctAnswer);
                    thisButtonScript.setCorrect();
                    changeButtonNameClient(1,myNumberQuestions[randQuestion].correctAnswer, true);
                    thisButtonScript = myButtons[2].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer2);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(2,myNumberQuestions[randQuestion].incorrectAnswer2, false);
                    break;
                case 3:
                    thisButtonScript = myButtons[0].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer1);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(0,myNumberQuestions[randQuestion].incorrectAnswer1, false);
                    thisButtonScript = myButtons[1].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer2);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(1,myNumberQuestions[randQuestion].incorrectAnswer2, false);
                    thisButtonScript = myButtons[2].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].correctAnswer);
                    thisButtonScript.setCorrect();
                    changeButtonNameClient(2,myNumberQuestions[randQuestion].correctAnswer, true);

                    break;
                case 4:
                    thisButtonScript = myButtons[0].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer2);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(0,myNumberQuestions[randQuestion].incorrectAnswer2, false);
                    thisButtonScript = myButtons[1].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].correctAnswer);
                    changeButtonNameClient(1,myNumberQuestions[randQuestion].correctAnswer, true);
                    thisButtonScript.setCorrect();
                    thisButtonScript = myButtons[2].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer1);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(2,myNumberQuestions[randQuestion].incorrectAnswer1, false);
                    break;
                case 5:
                    thisButtonScript = myButtons[0].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer2);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(0,myNumberQuestions[randQuestion].incorrectAnswer2, false);
                    thisButtonScript = myButtons[1].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].incorrectAnswer1);
                    thisButtonScript.setIncorrect();
                    changeButtonNameClient(1,myNumberQuestions[randQuestion].incorrectAnswer1, false);
                    thisButtonScript = myButtons[2].GetComponent<ButtonTestScript>();
                    thisButtonScript.setTextName(myNumberQuestions[randQuestion].correctAnswer);
                    thisButtonScript.setCorrect();
                    changeButtonNameClient(2,myNumberQuestions[randQuestion].correctAnswer, true);
                    break;

            }
            string answer = myNumberQuestions[randQuestion].correctAnswer;
            // 
            answer = combineString(answer);
            dataCorrectButtonName.Add(answer);
            storeButtonName(myButtons[0].GetComponentInChildren<TMP_Text>().text,myButtons[1].GetComponentInChildren<TMP_Text>().text,myButtons[2].GetComponentInChildren<TMP_Text>().text );
            changeCharadeText(myNumberQuestions[randQuestion].correctAnswer);
            myNumberQuestions.RemoveAt(randQuestion);
            // Debug.Log("my number questions currently has" + myNumberQuestions.Count);
            currentPhase++;
            



        }

    }

    [ClientRpc]
    public void changeButtonNameClient(int buttonNum, string buttonName, bool correct){
        // 
        ButtonTestScript myTestScript = myButtons[buttonNum].GetComponent<ButtonTestScript>();
        myTestScript.setTextName(buttonName);
        if(correct){
            myTestScript.setCorrect();
            changeCharadeText(buttonName);
        }else{
            myTestScript.setIncorrect();
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

   
    [ClientRpc]
    public void hideConfidenceText(){
        confidenceText.SetActive(false);
    }

    [ClientRpc]
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
    
    [ClientRpc]
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

    [ClientRpc]
    public void changeP1QuestionaireTextOnClient(string newInput){
        TMP_Text myText = GuesserText.GetComponent<TMP_Text>();
        myText.text = newInput;
    }
     [ClientRpc]
    public void changeP2QuestionaireTextOnClient(string newInput){
        TMP_Text myText = CharadeText.GetComponent<TMP_Text>();
        myText.text = newInput;
    }
    
    [Command(requiresAuthority = false)]
    public void setServeranimateP2Down(){
        hideP2Confidence = true;
    }
    [Command(requiresAuthority = false)]
    public void setServeranimateP1Down(){
        hideP1Confidence = true;
    }


    public void startConfirmSection(){
        p1ConfirmButton.SetActive(true);
        p2ConfirmButton.SetActive(true);
        startConfirmSectionClient();
    }

    [ClientRpc]
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
    [Command(requiresAuthority = false)]
    public void incrementNumWaitingInQuestionaire(){
        // 
        numWaitingInQuestionaire++;
    }
    [Command(requiresAuthority = false)]
    public void setServerQuestionaireDown(){
        questionaireDown = true;
    }
    [Command(requiresAuthority = false)]
    public void hideBothConfidence(){
        hideP1Confidence = true;
        hideP2Confidence = true;
    }

    [Command(requiresAuthority = false)]
     public void animateQuestionaiupServer(){
        animateQuestionareUp();
     
    }
    [Command(requiresAuthority = false)]
     public void animateQuestionaiupServerDown(){
        animateQuestionareDown();
     
    }
    [Command(requiresAuthority = false)]
    public void ServerP1QButtonsDown(){
        hideConfidenceButtons = true;
    }
    [Command(requiresAuthority = false)]
    public void ServerP2QButtonsDown(){
        hideP2Confidence = true;
    }

    
    public void searchForSceneController(){
        // 
        GameObject controllerObject = GameObject.FindGameObjectWithTag("SceneController");
        mySceneController = controllerObject.GetComponent<SceneController>();
        //searchForSceneControllerServer();
        
    }
    [ClientRpc]
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
        if(questionaireNumSeen == 1){
            mySceneController.myNetworkManager.setPlayerWristScales(handSizeOrder[orderOfhands][1]);
        }else if(questionaireNumSeen == 2){
            mySceneController.myNetworkManager.setPlayerWristScales(handSizeOrder[orderOfhands][2]);
        }
        currentPhase = 0;
        numWaitingInQuestionaire = 0;
        questionaireNumberP1 = 0;
        questionaireNumberP2 = 0;
        quesitonairePhase = false;
        currentQuestionPhase = questionPhase.Easy;
        if(questionaireNumSeen == 3){
            storeDataInFile();
            TFPText.SetActive(true);
            endGameClient();
            

        }else{
        foreach(var x in myButtons){
            x.SetActive(true);
        }
        foreach(var x in myButtons){
                x.GetComponent<ButtonTestScript>().enabled = true;
            }
        foreach(var x in confidenceButtons){
                x.GetComponent<ButtonTestScript>().enabled = true;
            }
        foreach(var x in P2QuestionaireButtons){
                x.GetComponent<ButtonTestScript>().enabled = true;
            }
        setInitialNumbers();
        showAnimation = true;
        resetToBeginningClient();
        }

    }
     [ClientRpc]
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
    [ClientRpc]
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

    [Server]
    public void storeButtonName(string Button1, string Button2, string Button3){
        // 
        string input1 = combineString(Button1);
        string input2 = combineString(Button2);
        string input3 = combineString(Button3);
        dataButtonName.Add(input1);
        dataButtonName.Add(input2);
        dataButtonName.Add(input3);
        

    }
    public string makeFileName(string fileName, int i = 0){
        if(File.Exists(fileName+"v_"+i.ToString() + ".csv")){
            return makeFileName(fileName, ++i);
        }
        return fileName +"v_"+i.ToString() +".csv";
    }
    

    [Server]
    public void storeDataInFile(){
        
        string path = "D:" + "CharadeLogs/Charade_" + player1Pid + "_" + player2Pid + "_Results";
        path = makeFileName(path);
        
        
        using(StreamWriter myWriter = new StreamWriter(path)){
            myWriter.WriteLine("Int Pid,Act Pid,Button1, Button2, Button3, Button Answered, Correct Button, got correct, Confidence of Guess,Time when Seen, Time when pressed, Time Difference, HandSize");
            for(int j = 0; j < 3; j++){
                for(int i = 0; i < 12; i++){
                    

                    

                    myWriter.WriteLine(player1Pid.ToString() + "," + player2Pid.ToString() + "," + dataButtonName[0] + "," + dataButtonName[1] + "," + dataButtonName[2] + "," + dataButtonChosenName[0] + "," + dataCorrectButtonName[0] + "," + dataChoseCorrect[0] +"," + dataconfidenceInt[0] + ","  + dataStartInterval[0].ToString() + "," +  dataEndInterval[0].ToString() + "," + (float.Parse(dataEndInterval[0]) - float.Parse(dataStartInterval[0])).ToString() + "," + dataHandSize[j]);
                

                    dataButtonChosenName.RemoveAt(0);
                    dataButtonName.RemoveAt(0);
                    dataButtonName.RemoveAt(0);
                    dataButtonName.RemoveAt(0);
                    dataCorrectButtonName.RemoveAt(0);
                    dataChoseCorrect.RemoveAt(0);
                    dataconfidenceInt.RemoveAt(0);
                    dataStartInterval.RemoveAt(0);
                    dataEndInterval.RemoveAt(0);
                    

                    
                }
                myWriter.WriteLine("end of phase");
            }
        path = "D:" + "CharadeLogs/Charade_" + player1Pid + "_" + player2Pid + "_QuestionaireResults";
        path = makeFileName(path);
        using(StreamWriter myQuestionaireWriter = new StreamWriter(path)){
            myQuestionaireWriter.WriteLine("Int ID, Act ID,Question Asked, Player1Answer,Player2Answer");
            for(int y = 0; y < 3; y++){
                string myQuestion = myQuestions[0];
                myQuestion = myQuestion.Replace("\n","");
                myQuestion = myQuestion.Replace(",","");
                myQuestionaireWriter.WriteLine(player1Pid.ToString() + "," + player2Pid.ToString() + "," + myQuestion + ","  + "N/A" + "," + dataP2QuestionaireAnswer[0] + "," + dataHandSize[y]);
                dataP2QuestionaireAnswer.RemoveAt(0);
                for(int x = 1; x < 26; x++){
                    myQuestion = myQuestions[x];
                    myQuestion = myQuestion.Replace("\n","");
                    myQuestion = myQuestion.Replace(",","");
                    myQuestionaireWriter.WriteLine(player1Pid.ToString() + "," + player2Pid.ToString() + "," + myQuestion + "," +  dataP1QuestionaireAnswer[0] + "," + dataP2QuestionaireAnswer[0] + "," + dataHandSize[y]);
                    dataP1QuestionaireAnswer.RemoveAt(0);
                    dataP2QuestionaireAnswer.RemoveAt(0);
                }
            }

        }

        }
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
   


    

   
  
    }

   
    
             

   
    
    



    
