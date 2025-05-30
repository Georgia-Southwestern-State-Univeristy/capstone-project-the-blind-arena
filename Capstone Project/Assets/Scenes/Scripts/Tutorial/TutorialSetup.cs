using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TutorialSetup : MonoBehaviour
{
    [SerializeField] private GameObject enemyAIFire;
    [SerializeField] private GameObject moveInstructions;
    [SerializeField] private GameObject attackInstructions;
    [SerializeField] private GameObject attributePointExplainationDialogue;
    [SerializeField] private GameObject fieldPointIncreaseandDecreaseDialogue;
    [SerializeField] private GameObject buttonExplainationDialogue;
    [SerializeField] private GameObject preparationDialogue;
    [SerializeField] private GameObject runDialogue;
    [SerializeField] private GameObject attributeMenu;
    [SerializeField] private GameObject xbuttonupgrade;
    [SerializeField] private GameObject skipTutorialButton;
    [SerializeField] private GameObject attributResetButton;
    [SerializeField] private GameObject attributBuyButton;

    public int tutorialcounter = 0;

    [SerializeField] public GameObject tutorialBoss;
    [SerializeField] private GameObject[] tutorialBossSwarms; // <-- new array field
    [SerializeField] private float spawnDelayBetweenEnemies = 0.5f; // <-- time between spawns, adjustable in Inspector

    private bool tutorialBossActivated = false;
    private bool attackInstructionsShown = false;
    private bool preparationDialogueShown = false;
    private bool runDialogueShown = false;
    private bool attributeMenuShown = false;
    private bool hasResetTutorial = false; // Flag to track if reset has been triggered
    private bool didLeftClick = false;
    private bool didRightClick = false;
    private bool didPressE = false;
    private bool didPressSpace = false;
    private bool startedAttributeTutorial = false;
    private bool resumeWalkingSounds = false;



    public SinglePlayerAttack playerAttack;

    void Start()
    {
        GameData.deathcounter = 0;
        Debug.Log("Deathcounter on start: " + GameData.deathcounter);
        if (playerAttack == null)
            playerAttack = FindFirstObjectByType<SinglePlayerAttack>();
            playerAttack.attackChecker = false;



    }

    void Update()
    {

        Debug.Log("Tutorial Counter: " + tutorialcounter);
        TutorialMovement();
        TutorialAttackInstructions();
        
        TutorialPreparationDialogue();
        TutorialBossActivation();

        

    }

    private void TutorialMovement()
    {
        if (tutorialcounter == 0)
        {
            moveInstructions.SetActive(true);
            playerAttack.attackChecker = false;
        }

        if (Input.GetKeyDown(KeyCode.W) && (tutorialcounter < 4)) { tutorialcounter++; }
        if (Input.GetKeyDown(KeyCode.A) && (tutorialcounter < 4)) { tutorialcounter++; }
        if (Input.GetKeyDown(KeyCode.S) && (tutorialcounter < 4)) { tutorialcounter++; }
        if (Input.GetKeyDown(KeyCode.D) && (tutorialcounter < 4)) { tutorialcounter++; }

        if (tutorialcounter >= 4)
        {
            moveInstructions.SetActive(false);
        }
    }

    private void TutorialAttackInstructions()
    {
        if (tutorialcounter == 4 && !attackInstructionsShown)
        {
            attackInstructions.SetActive(true);
            attackInstructionsShown = true;
            playerAttack.attackChecker = true;
        }


        if (tutorialcounter >= 4 && tutorialcounter < 10)
            {
            // Check and track each input only once
            if (!didLeftClick && Input.GetMouseButtonDown(0))
            {
                didLeftClick = true;
                tutorialcounter++;
            }

            if (!didRightClick && Input.GetMouseButtonDown(1))
            {
                didRightClick = true;
                tutorialcounter++;
            }

            if (!didPressE && Input.GetKeyDown(KeyCode.E))
            {
                didPressE = true;
                tutorialcounter++;
            }

            if (!didPressSpace && Input.GetKeyUp(KeyCode.Space))
            {
                didPressSpace = true;
                tutorialcounter++;
            }
        }

        // Once all 4 are done (tutorialcounter should now be 8)
        if (didLeftClick && didRightClick && didPressE && didPressSpace && tutorialcounter >= 8 && !startedAttributeTutorial)
        {
            attackInstructions.SetActive(false);
            playerAttack.attackChecker = false;
            tutorialcounter = 10;
            startedAttributeTutorial = true; // make sure it doesn't run again
            StartCoroutine(TutorialAttributeExplainationDialogue());
        }
    }

    private IEnumerator TutorialAttributeExplainationDialogue()
    {

        // Disable another GameObject's AudioSource at the start
        GameObject anotherObject = GameObject.Find("WalkingOnLeaves");
        if (anotherObject != null)
        {
            AudioSource otherAudio = anotherObject.GetComponent<AudioSource>();
            if (otherAudio != null)
            {
                otherAudio.enabled = false;
            }
        }

        attributResetButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            attributBuyButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            attributeMenu.SetActive(true);
            playerAttack.attackChecker = false;
            xbuttonupgrade.SetActive(false);

        // Show first dialog
        attributePointExplainationDialogue.SetActive(true);
        yield return StartCoroutine(ShowDialogAndWait(attributePointExplainationDialogue));
        attributePointExplainationDialogue.SetActive(false);

        // Show second dialog
        fieldPointIncreaseandDecreaseDialogue.SetActive(true);
        yield return StartCoroutine(ShowDialogAndWait(fieldPointIncreaseandDecreaseDialogue));
        fieldPointIncreaseandDecreaseDialogue.SetActive(false);

        // Show third dialog
        buttonExplainationDialogue.SetActive(true);
        yield return StartCoroutine(ShowDialogAndWait(buttonExplainationDialogue));
        buttonExplainationDialogue.SetActive(false);
       
            attributeMenu.SetActive(false);
            xbuttonupgrade.SetActive(true);
            attributeMenuShown = true;
            attributResetButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            attributBuyButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            tutorialcounter++;
        resumeWalkingSounds = true;

        if (resumeWalkingSounds == true)
        {
            GameObject anotherObject2 = GameObject.Find("WalkingOnLeaves");
            if (anotherObject2 != null)
            {
                AudioSource otherAudio = anotherObject.GetComponent<AudioSource>();
                if (otherAudio != null)
                {
                    otherAudio.enabled = true;
                }
            }
        }
    }

    private void TutorialPreparationDialogue()
    {
        if (tutorialcounter == 11 && !preparationDialogueShown)
        {
            preparationDialogue.SetActive(true);
            preparationDialogueShown = true;
            playerAttack.attackChecker = true;



            StartCoroutine(ActivateEnemyAIDelayed());
        }
    }

    private void TutorialBossActivation()
    {
        if (GameData.deathcounter == 1 && !tutorialBossActivated)
        {
            tutorialBossActivated = true;
            preparationDialogue.SetActive(false);
            runDialogue.SetActive(true);

            StartCoroutine(ActivateEnemySwarmDelayed());
        }
    }

    private IEnumerator ActivateEnemyAIDelayed()
    {
        yield return new WaitForSeconds(1.5f);
        tutorialBoss.SetActive(true);
        ResetRestAreaTutorial();
    }

    private IEnumerator ActivateEnemySwarmDelayed()
    {
        yield return new WaitForSeconds(1.5f);

        if (Input.anyKeyDown && tutorialcounter < 8)
        {
            runDialogue.SetActive(false);
        }

        for (int i = 0; i < tutorialBossSwarms.Length; i++)
        {
            tutorialBossSwarms[i].SetActive(true);
            yield return new WaitForSeconds(spawnDelayBetweenEnemies); // wait between each spawn
        }
    }

    public void ResetRestAreaTutorial()
    {
        if (!hasResetTutorial)
        {
            PlayerPrefs.DeleteKey("RestAreaTutorial");
            PlayerPrefs.Save();
            Debug.Log("RestAreaTutorial progress has been reset.");
            hasResetTutorial = true; // Set flag to true so it only runs once
        }
    }

    private IEnumerator ShowDialogAndWait(GameObject dialogObject)
    {
        bool isFinished = false;

        DialogBox dialogBox = dialogObject.GetComponent<DialogBox>();
        dialogBox.OnDialogFinished = () => { isFinished = true; };

        dialogObject.SetActive(true);

        yield return new WaitUntil(() => isFinished);
    }

    public void LoadNextSceneAndIncrementDeathCounter()
    {
        GameData.deathcounter++; // Increment death counter
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Load next scene
    }
}
