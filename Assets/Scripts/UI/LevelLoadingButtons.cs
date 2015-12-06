using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class LoadingButton
{
    public string text;
    public string levelName;
}

public class LevelLoadingButtons : MonoBehaviour {
    public Button buttonPrefab;
    public Vector2 anchoredPosition;
    public Vector2 buttonSize;
    public float gap;
    public LoadingButton[] buttons;

	void Start () {
        createButtons();
	}

    private void createButtons()
    {
        RectTransform parentRect = GetComponent<RectTransform>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = (Button)Instantiate(buttonPrefab);

            RectTransform trans = button.GetComponent<RectTransform>();
            trans.SetParent(parentRect, false);
            trans.anchoredPosition = anchoredPosition;
            trans.localPosition = new Vector2(0, (buttonSize.y + gap) * i - buttonSize.y/2.0f);
            trans.sizeDelta = buttonSize;

            Text buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = buttons[i].text;

            string levelName = buttons[i].levelName;
            button.onClick.AddListener(() => loadLevel(levelName));
        }
    }

    private void loadLevel(string levelName)
    {
        Application.LoadLevel(levelName);
    }
}
