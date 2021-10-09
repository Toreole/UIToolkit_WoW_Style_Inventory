using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class NumberInputField 
{
    TextField field;

    public int MaxValue { get; set;} = 50;

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
            if(cValue > MaxValue)
            {
                field.value = MaxValue.ToString();
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

    public void IncreaseValue()
    {
        int val = int.Parse(field.value);
        val++;
        val = Mathf.Min(val, MaxValue);
        field.SetValueWithoutNotify(val.ToString());
    }

    //lets just say that as a general rule, these fields never go below 0.
    public void DecreaseValue()
    {
        int val = int.Parse(field.value);
        val--;
        val = Mathf.Max(val, 0);
        field.SetValueWithoutNotify(val.ToString());
    }   

    public int GetIntValue() => int.Parse(field.value);

    public bool TryGetValue(out int amount) => int.TryParse(s: field.value, result: out amount);
    public void Reset() => field.SetValueWithoutNotify("0");

}
