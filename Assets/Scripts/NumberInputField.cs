using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class NumberInputField 
{
    TextField field;

    public int maxValue = 50;

    static readonly Regex numberCheck = new Regex("^[0-9]+$");

    public void Init(TextField textField)
    {
        textField.value = "";
        field = textField;
        textField.RegisterCallback<ChangeEvent<string>>(OnTextChanged);
    }

    void OnTextChanged(ChangeEvent<string> e)
    {
        if(string.IsNullOrEmpty(e.newValue))
            return;
        if(numberCheck.IsMatch(e.newValue))
        {
            //check number against the maximum.
            int cValue = int.Parse(e.newValue);
            if(cValue > maxValue)
            {
                field.value = maxValue.ToString();
                //the parent of the textfield cant be focused, thus essentially removing focus entirely.
                field.parent.Focus();
            }
        }
        else 
        {
            //reset field value to previous.
            field.value = e.previousValue;
        }
    }

}
