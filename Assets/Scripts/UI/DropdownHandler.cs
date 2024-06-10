using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textBox;
    
    //private Dropdown dropdown;
    private TMP_Dropdown dropdown;
    

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown == null)
            Debug.LogError("Dropdown component not found");
        /*dropdown.options.Clear();

        List<string> items = new List<string>();

        // Fill dropdown with items
        foreach (var item in items)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = item });
        }*/
        
        DropdownItemSelected(dropdown);
        
        // Add listener to dropdown

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }
    
    
    private void DropdownItemSelected(TMP_Dropdown tmpDropdown)
    {
        int index = tmpDropdown.value;
        textBox.text = tmpDropdown.options[index].text; 

    }

    
    public void ClearTextBox()
    {
        textBox.text = "";
    }

}
