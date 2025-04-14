using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] private GameObject tutorialBoss;
    [SerializeField] private GameObject tutorialBossSwarm1;
    [SerializeField] private GameObject tutorialBossSwarm2;
    [SerializeField] private GameObject tutorialBossSwarm3;
    [SerializeField] private GameObject tutorialBossSwarm4;
    [SerializeField] private GameObject tutorialBossSwarm5;
    [SerializeField] private GameObject tutorialBossSwarm6;
    [SerializeField] private GameObject tutorialBossSwarm7;
    [SerializeField] private GameObject tutorialBossSwarm8;
    [SerializeField] private GameObject tutorialBossSwarm9;
    [SerializeField] private GameObject tutorialBossSwarm10;
    [SerializeField] private GameObject tutorialBossSwarm11;
    [SerializeField] private GameObject tutorialBossSwarm12;
    [SerializeField] private GameObject tutorialBossSwarm13;
    [SerializeField] private GameObject tutorialBossSwarm14;
    [SerializeField] private GameObject tutorialBossSwarm15;
    [SerializeField] private GameObject tutorialBossSwarm16;
    [SerializeField] private GameObject tutorialBossSwarm17;
    [SerializeField] private GameObject tutorialBossSwarm18;
    [SerializeField] private GameObject tutorialBossSwarm19;

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

    public SinglePlayerAttack playerAttack;

    void Start()
    {
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
            attributResetButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            attributBuyButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            attributeMenu.SetActive(true);
            playerAttack.attackChecker = false;
            xbuttonupgrade.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            attributePointExplainationDialogue.SetActive(true);
            yield return new WaitForSeconds(5f);
            attributePointExplainationDialogue.SetActive(false);
            fieldPointIncreaseandDecreaseDialogue.SetActive(true);
            yield return new WaitForSeconds(7f);
            buttonExplainationDialogue.SetActive(true);
            yield return new WaitForSeconds(5f);
            buttonExplainationDialogue.SetActive(false);
            attributeMenu.SetActive(false);
            xbuttonupgrade.SetActive(true);
            attributeMenuShown = true;
            attributResetButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            attributBuyButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            tutorialcounter++;
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
            runDialogue.SetActive(true);
            //if still not work enemyAifire goes right here
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

        tutorialBossSwarm1.SetActive(true);
        tutorialBossSwarm2.SetActive(true);
        tutorialBossSwarm3.SetActive(true);
        tutorialBossSwarm4.SetActive(true);
        tutorialBossSwarm5.SetActive(true);
        tutorialBossSwarm6.SetActive(true);
        tutorialBossSwarm7.SetActive(true);
        tutorialBossSwarm8.SetActive(true);
        tutorialBossSwarm9.SetActive(true);
        tutorialBossSwarm10.SetActive(true);
        tutorialBossSwarm11.SetActive(true);
        tutorialBossSwarm12.SetActive(true);
        tutorialBossSwarm13.SetActive(true);
        tutorialBossSwarm14.SetActive(true);
        tutorialBossSwarm15.SetActive(true);
        tutorialBossSwarm16.SetActive(true);
        tutorialBossSwarm17.SetActive(true);
        tutorialBossSwarm18.SetActive(true);
        tutorialBossSwarm19.SetActive(true);
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

    public void LoadNextSceneAndIncrementDeathCounter()
    {
        GameData.deathcounter++; // Increment death counter
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Load next scene
    }
}
