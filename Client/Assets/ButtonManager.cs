using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ButtonManager : Singleton<ButtonManager>
{
    public Button TurnOnButton => transform.parent.Find("TurnOnButton").GetComponent<Button>();
    public Button TurnOffButton => transform.parent.Find("TurnOffButton").GetComponent<Button>();
    public Button TemperaturePlusButton => transform.parent.Find("TemperaturePlusButton").GetComponent<Button>();
    public Button TemperatureMinusButton => transform.parent.Find("TemperatureMinusButton").GetComponent<Button>();

    public void Start()
    {
        CheckStateManager();
    }

    void CheckStateManager()
    {
        Debug.Log($"State manager instance exists : {StateManager.Instance != null}");
    }

    public void UpdateStateText(string state)
    {
        var txt = transform.parent.Find("State").GetComponent<Text>();
        txt.text = state;
    }

    public void UpdateButtons(List<string> availableCommands)
    {
        //SetTemperature = 0,
        //SetFreezerTemperature = 1,
        //GetState = 2,
        //TurnOn = 3,
        //TurnOff = 4
        var isSetTemperatureAvailable = availableCommands.Contains("SetTemperature");
        TemperaturePlusButton.gameObject.SetActive(isSetTemperatureAvailable);
        TemperatureMinusButton.gameObject.SetActive(isSetTemperatureAvailable);
        TurnOnButton.gameObject.SetActive(availableCommands.Contains("TurnOn"));
        TurnOffButton.gameObject.SetActive(availableCommands.Contains("TurnOff"));
    }

    public void OnIncreaseTemperatureButtonPressed()
    {
        //var url = "https://192.168.1.6:45455/HomeAppliance/ExecuteCommand?deviceId=194500&command=TurnOn";
        //var url = "https://192.168.1.6:45455/HomeAppliance/GetState?deviceId=194500";
        StateManager.Instance.UpdateTemperature(true);
    }

    public void OnDecreaseTemperatureButtonPressed()
    {
        StateManager.Instance.UpdateTemperature(false);
    }

    public void OnTurnOnButtonPressed()
    {
        StateManager.Instance.TurnOn();
    }

    public void OnTurnOffButtonPressed()
    {
        StateManager.Instance.TurnOff();
    }
}
