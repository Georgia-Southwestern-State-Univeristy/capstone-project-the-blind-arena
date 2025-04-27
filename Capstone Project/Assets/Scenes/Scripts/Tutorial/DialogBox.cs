using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    public AudioClip typingSound; // Add this
    private AudioSource audioSource; // Add this

    private int index;

    public System.Action OnDialogFinished;

    private void Start()
    {
        textComponent.text = string.Empty;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = typingSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Set initial volume from saved settings
        float savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        audioSource.volume = savedSoundVolume;

        StartDialogue();
    }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                audioSource.Stop(); // Stop sound if line is instantly completed
            }
        }

        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textComponent.text = "";

        if (typingSound != null && audioSource != null)
            audioSource.Play();

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        if (audioSource.isPlaying)
            audioSource.Stop();
}

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine (TypeLine());
        }

        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            gameObject.SetActive(false);

            if (OnDialogFinished != null)
                OnDialogFinished.Invoke(); // Trigger the event
        }
    }
}
