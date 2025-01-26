using UnityEngine;

public class TimedFadingLooper : MonoBehaviour
{
    [System.Serializable]
    public class ObjectFadeSettings
    {
        public GameObject obj;         // The object to control
        public float startTime;        // When the object should start appearing
        public float fadeInSpeed = 1f; // Speed of fade-in
        public float visibleTime = 2f; // How long the object stays fully visible
        public float fadeOutSpeed = 1f;// Speed of fade-out
    }

    public ObjectFadeSettings[] objects; // Array to hold object settings

    private CanvasGroup[] canvasGroups;  // CanvasGroups for all objects
    private float timer = 0f;            // Tracks time since start

    void Start()
    {
        // Initialize and hide all objects
        canvasGroups = new CanvasGroup[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            var settings = objects[i];
            if (settings.obj != null)
            {
                // Ensure CanvasGroup exists
                CanvasGroup cg = settings.obj.GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = settings.obj.AddComponent<CanvasGroup>();
                }
                cg.alpha = 0; // Start fully hidden
                settings.obj.SetActive(false); // Disable the object
                canvasGroups[i] = cg;
            }
            else
            {
                Debug.LogError($"Object at index {i} is null. Please assign it in the Inspector.");
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        for (int i = 0; i < objects.Length; i++)
        {
            var settings = objects[i];
            if (settings.obj == null) continue;

            CanvasGroup cg = canvasGroups[i];

            float fadeInStart = settings.startTime;
            float fadeInEnd = fadeInStart + settings.fadeInSpeed;
            float visibleEnd = fadeInEnd + settings.visibleTime;
            float fadeOutEnd = visibleEnd + settings.fadeOutSpeed;

            if (timer >= fadeInStart && timer < fadeInEnd)
            {
                // Fade In
                settings.obj.SetActive(true);
                cg.alpha = Mathf.Clamp01((timer - fadeInStart) / settings.fadeInSpeed);
            }
            else if (timer >= fadeInEnd && timer < visibleEnd)
            {
                // Fully Visible
                cg.alpha = 1;
            }
            else if (timer >= visibleEnd && timer < fadeOutEnd)
            {
                // Fade Out
                cg.alpha = Mathf.Clamp01(1 - (timer - visibleEnd) / settings.fadeOutSpeed);
            }
            else if (timer >= fadeOutEnd)
            {
                // Completely Hidden
                cg.alpha = 0;
                settings.obj.SetActive(false);
            }
        }
    }
}
