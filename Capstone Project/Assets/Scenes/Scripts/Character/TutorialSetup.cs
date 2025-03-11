using System.Collections;
using UnityEngine;

public class TutorialSetup : MonoBehaviour
{
    [SerializeField] private GameObject enemyAIFire;
    [SerializeField] private GameObject moveInstructions;
    [SerializeField] private GameObject attackInstructions;
    [SerializeField] private GameObject preparationDialogue;

    public double tutorialcounter = 0;

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

    void Update()
    {
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
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && (tutorialcounter < 7)) { tutorialcounter++; }
        if (Input.GetKeyDown(KeyCode.Alpha2) && (tutorialcounter < 7)) { tutorialcounter++; }
        if (Input.GetKeyDown(KeyCode.Alpha3) && (tutorialcounter < 7)) { tutorialcounter++; }
        if (Input.GetKey(KeyCode.Space) && (tutorialcounter < 7)) { tutorialcounter++; }

        if (tutorialcounter >= 7)
        {
            attackInstructions.SetActive(false);
        }
    }

    private void TutorialPreparationDialogue()
    {
        if (tutorialcounter == 7 && !preparationDialogueShown)
        {
            preparationDialogue.SetActive(true);
            preparationDialogueShown = true;
            StartCoroutine(ActivateEnemyAIDelayed());
        }
    }

    private void TutorialBossActivation()
    {
        if (GameData.deathcounter == 1 && !tutorialBossActivated)
        {
            tutorialBossActivated = true;
            //if still not work enemyAifire goes right here
            StartCoroutine(ActivateEnemySwarmDelayed());
        }
    }

    private IEnumerator ActivateEnemyAIDelayed()
    {
        yield return new WaitForSeconds(1.5f);
        tutorialBoss.SetActive(true);
    }

    private IEnumerator ActivateEnemySwarmDelayed()
    {
        yield return new WaitForSeconds(1.5f);
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
}
