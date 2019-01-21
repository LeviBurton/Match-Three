using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

[RequireComponent(typeof(RectXformMover))]
public class MessageWindow : MonoBehaviour
{
    [Required] public Image messageIcon;
    [Required] public TextMeshProUGUI messageText;
    [Required] public TextMeshProUGUI buttonText;

    public void ShowMessage(Sprite sprite = null, string message = "", string buttonMsg = "start")
    {
        if (messageIcon != null)
        {
            messageIcon.sprite = sprite;
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (buttonText != null)
        {
            buttonText.text = buttonMsg;
        }

    }
}
