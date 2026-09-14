using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : MonoBehaviour
{
    public static UI_HUD Instance;

    [SerializeField] private Image SprintFill;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSprint(float amnt)
    {
        float targetFill = amnt / 100f;

        SprintFill.fillAmount = targetFill;
    }
}
