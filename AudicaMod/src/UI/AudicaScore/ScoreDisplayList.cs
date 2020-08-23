using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

internal static class ScoreDisplayList
{
    static GameObject scoreListDisplay;
    static TextMeshPro scoreListText;
    static RectTransform rect;
    public static void Init()
    {
        if (scoreListDisplay != null)
        {
            scoreListDisplay = new GameObject();
            scoreListText = scoreListDisplay.AddComponent<TextMeshPro>();
            rect = scoreListDisplay.GetComponent<RectTransform>();
            rect.transform.position = new Vector3(-22.14f, 10.65f, 14.18f);
            rect.sizeDelta = new Vector2(20f, 4f);
            rect.rotation = Quaternion.Euler(new Vector3(0f, -51.033f, 0f));
            scoreListText.fontSize = 16;
            scoreListText.margin = new Vector4(0f, 0f, 0f, -18.82106f);
            scoreListText.text = "Hello"; 
        }
    }

    public static void UpdateTextFromList()
    {

    }

}
