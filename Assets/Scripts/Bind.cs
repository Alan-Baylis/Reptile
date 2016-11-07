using UnityEngine;
using System.Collections;

public class Bind : MonoBehaviour
{
    public static Binding current = new Binding();

    void LateUpdate()
    {
        current.shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        current.ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        current.alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        current.key = KeyCode.None;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (code != KeyCode.LeftShift &&
                    code != KeyCode.RightShift &&
                    code != KeyCode.LeftControl &&
                    code != KeyCode.RightControl &&
                    code != KeyCode.LeftAlt &&
                    code != KeyCode.RightAlt &&
                    code != KeyCode.LeftCommand &&
                    code != KeyCode.RightCommand &&
                    Input.GetKeyDown(code))
                {
                    current.key = code;
                }
            }
        }
    }
}
