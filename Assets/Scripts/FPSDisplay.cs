using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public int avgFrameRate;
    public string display_Text;

    public void Update()
    {
        float current = 0;
        current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        display_Text = avgFrameRate.ToString() + " FPS";
    }

    void OnGUI()
    {


        //Output the angle found above
        GUI.Label(new Rect(25, 25, 200, 40), "FPS: " + display_Text);
    }
}
