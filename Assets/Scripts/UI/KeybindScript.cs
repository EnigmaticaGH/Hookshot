using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class Keybind
{
    public Text textBox;
    public Button button;
    public string name;
    public KeyCode key;
}

public class KeybindScript : MonoBehaviour
{
    public List<Keybind> keybinds = new List<Keybind>();
    string control;

    void Start()
    {
        control = "";
    }

	// Update is called once per frame
	void Update ()
    {
        foreach (Keybind keybind in keybinds)
        {
            keybind.textBox.text = keybind.name + ": " + keybind.key;
        }
	}

    void OnGUI()
    {
        Event e = Event.current;

        foreach (Keybind keybind in keybinds)
        {
            keybind.button.enabled = control == "";
        }

        if(e.isKey)
        {
            Keybind k = findKeyWithName(control);
            if (k != null && control != "" && e.keyCode != KeyCode.Escape)
            {
                k.key = e.keyCode;
                control = "";
            }
        }
    }

    public void changeKeybind(string c)
    {
        control = c;
    }

    public bool GetButtonDown(string name)
    {
        return Input.GetKeyDown(findKeyWithName(name).key);
    }

    public bool GetButton(string name)
    {
        return Input.GetKey(findKeyWithName(name).key);
    }

    public bool GetButtonUp(string name)
    {
        return Input.GetKeyUp(findKeyWithName(name).key);
    }

    public Keybind findKeyWithName(string name)
    {
        foreach (Keybind key in keybinds)
        {
            if (key.name == name)
                return key;
        }
        return null;
    }
}
