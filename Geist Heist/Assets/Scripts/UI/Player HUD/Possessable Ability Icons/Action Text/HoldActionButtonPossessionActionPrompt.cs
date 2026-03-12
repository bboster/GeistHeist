/*
 * Contributors: Toby
 * Creation:    3/5/2026
 * Last Edited: 3/5/2026
 * Summary: Action text for toy car and vending machine
 */

using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class HoldActionButtonPossessionActionPrompt : PossesableText
{
    [SerializeField] private TMP_Text textBox;
    public string Text = "Hold [button]";
    public string KeyboardIcon = "<sprite name=\"Action_Keyboard\">";
    public string ControllerIcon = "<sprite name=\"Action_Controller\">";

    protected override void ThisInitialize()
    {
        //throw new System.NotImplementedException();
    }

    public override void RefreshUI()
    {
        string button_icon = InputEvents.Instance.IsGamepadActive() ? ControllerIcon : KeyboardIcon;
        textBox.text = Text.Replace("[button]", button_icon);
    }

    [Button]
    private void DebugTestControllerButton()
    {
        textBox.text = Text.Replace("[button]", ControllerIcon);
    }

    [Button]
    private void DebugTestKeyboardButton()
    {
        textBox.text = Text.Replace("[button]", KeyboardIcon);
    }
}
