using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSetter : MonoBehaviour
{
    // Start is called before the first frame update
    private float slidervalue = 1;
    private Image gammaPanel;

    void Start()
    {
        gammaPanel = GetComponent<Image>();
        slidervalue = PlayerPrefs.GetFloat("Brightness", 1);

        byte _a = (byte)Mathf.Clamp((byte)(255 - (255 * slidervalue)), 0, 240);
        gammaPanel.color = new Color32(0, 0, 0, _a);

    }

}
