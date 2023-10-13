using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidThemeControl : MonoBehaviour
{
    [SerializeField] Color mStatusBarColor;

    public void Awake() 
    {
        StatusBarControl(true);

        var hexColor = ColorUtility.ToHtmlStringRGBA(mStatusBarColor);
        uint colorValue = Convert.ToUInt32(hexColor, 16);
        StatusBarColorControl(colorValue);

        NavigationBarControl(false);
    }

    public void StatusBarControl(bool _isVisible)
    {
        ApplicationChrome.statusBarState = _isVisible ? ApplicationChrome.States.Visible : ApplicationChrome.States.Hidden;
    }

    public void StatusBarColorControl(uint _colorValue)
    {
        ApplicationChrome.statusBarColor = _colorValue;
    }

    public void NavigationBarControl(bool _isVisible)
    {
        ApplicationChrome.navigationBarState = _isVisible ? ApplicationChrome.States.Visible : ApplicationChrome.States.Hidden;
    }

    public void NavigationBarColorControl(uint _colorValue)
    {
        ApplicationChrome.navigationBarColor = _colorValue;
    }
}
