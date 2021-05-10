using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class StateManager : Singleton<StateManager>
{
    private string DeviceId { get; set; }
    private string CurrentState { get; set; }
    private static DeviceImage[] DeviceImages { get; set; }
    public bool IsOn { get; set; }
    public string ApiUrl => @"https://192.168.1.2:45455/HomeAppliance/";
    public string ApiImagesUrl => @"https://192.168.1.2:45455/Device/GetAllImages";
    public int Temperature { get; set; }

    private DynamicImageLibrary dynamicImageLibrary;

    private List<string> availableCommands;

    private Coroutine UpdateStateCoroutine;

    public List<string> AvailableCommands
    {
        get
        {
            if (availableCommands != null)
                return availableCommands;
            availableCommands = new List<string>();
            return availableCommands;
        }
        set => availableCommands = value;
    }

    public string GetStateUrl => ApiUrl + "GetState?deviceId=" + DeviceId;

    void Start()
    {
        //for debug
        //UpdateDevice("194500");
        Debug.Log($"Start {nameof(StateManager)}");
        dynamicImageLibrary = GameObject.FindObjectOfType<ARSessionOrigin>().GetComponent<DynamicImageLibrary>();
        UpdateStateCoroutine = StartUpdateStateCoroutine();
        Debug.Log($"Started {nameof(UpdateState)}");
        DownloadImages();
        ButtonManager.Instance.UpdateButtons(AvailableCommands);
    }

    private Coroutine StartUpdateStateCoroutine()
    {
        return StartCoroutine(Instance.UpdateState(StateManager.Instance.GetStateUrl, 1, result =>
        {
            CurrentState = result;
            ButtonManager.Instance.UpdateStateText(CurrentState);
        }));
    }

    public void DownloadImages()
    {
        Debug.Log("Start DownloadImages");
        StartCoroutine(GetRequest(ApiImagesUrl, req =>
        {
            Debug.Log($"{nameof(DownloadImages)}: response {ApiImagesUrl}: error - {req.error}, result - {req.result} [{JsonConvert.DeserializeObject<DeviceImage[]>(req.downloadHandler.text)}]");
            if (!req.isDone || req.result != UnityWebRequest.Result.Success) return;
            DeviceImages = JsonConvert.DeserializeObject<DeviceImage[]>(req.downloadHandler.text);
            Debug.Log($"Images downloaded: {DeviceImages.Length}");
            Debug.Log($"DynamicLibrary not null : {dynamicImageLibrary != null}");
            dynamicImageLibrary.images = DeviceImages.Select(d =>
            {
                var tex = new Texture2D(2, 2);
                tex.LoadImage(d.ImageData);
                return new DynamicImageLibrary.ImageData
                {
                    name = d.DeviceId,
                    texture = tex,
                    width = d.WidthInMeters
                };
            }).ToArray();
            dynamicImageLibrary.m_State = DynamicImageLibrary.State.AddImagesRequested;
        }));
    }

    public void UpdateDevice(string deviceId)
    {
        DeviceId = deviceId;
        StartCoroutine(GetRequest($"{ApiUrl}GetAvailableCommands?deviceId={DeviceId}", req =>
        {
            if (!req.isDone || req.result != UnityWebRequest.Result.Success) return;
            AvailableCommands = JsonConvert.DeserializeObject<List<string>>(req.downloadHandler.text);
            ButtonManager.Instance.UpdateButtons(AvailableCommands);
        }));

        StopCoroutine(UpdateStateCoroutine);
        if (!string.IsNullOrEmpty(DeviceId))
            UpdateStateCoroutine = StartUpdateStateCoroutine();
    }

    private static Dictionary<string, string> ApiLocalizationDictionary = new Dictionary<string, string>
    {
        { "CurrentTemp", "Температура"},
        {"TempRefrigerator", "Температура холодильника" },
        {"TempGoal", "Целевая температура" },
        {"TempId", "Id" },
        {"State", "Состояние" },
        {"DoorOpenState", "Дверь" }
    };

    public enum State
    {
        [Description("Выключен")]
        Off = 0,
        [Description("Включен")]
        On = 3,
    }

    public void UpdateTemperature(bool isIncrease, int changeSize = 5)
    {
        var multiplier = isIncrease ? 1 : -1;
        StartCoroutine(UpdateState(GetExecuteCommandUrl("SetTemperature",
            (Temperature + multiplier * changeSize).ToString()), 0));
    }

    public void TurnOn()
    {
        StartCoroutine(UpdateState(GetExecuteCommandUrl("TurnOn"), 0));
    }

    public void TurnOff()
    {
        StartCoroutine(UpdateState(GetExecuteCommandUrl("TurnOff"), 0));
    }

    private string GetExecuteCommandUrl(string command, string args = "")
    {
        var argsParam = string.IsNullOrEmpty(args) ? string.Empty : $"&args={args}";
        return $"{ApiUrl}ExecuteCommand?deviceId={DeviceId}&command={command}{argsParam}";
    }

    public IEnumerator UpdateState(string url, int delaySeconds, Action<string> callback = null)
    {
        while (true)
        {
            if (string.IsNullOrEmpty(DeviceId)) yield return new WaitForSeconds(delaySeconds);
            if (!string.IsNullOrEmpty(DeviceId))
            {
                yield return GetRequest(url, req =>
                {
                    if (!req.isDone || req.result != UnityWebRequest.Result.Success) return;
                    var updatedState = FormatState(req.downloadHandler.text);
                    if (!string.IsNullOrEmpty(updatedState))
                        callback?.Invoke(updatedState);
                });
            }

            if (delaySeconds != 0)
                yield return new WaitForSeconds(delaySeconds);
            else
                break;
        }
    }

    private string FormatState(string apiState)
    {
        var stateDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(apiState);
        var localizedState = new StringBuilder();
        if (stateDictionary.Count == 1 && stateDictionary.First().Key == string.Empty)
            return string.Empty;

        foreach (var kvp in stateDictionary)
        {
            if (!ApiLocalizationDictionary.ContainsKey(kvp.Key))
                continue;
            var localizedKey = ApiLocalizationDictionary[kvp.Key];
            //TODO Вынести отдельно
            var key = kvp.Key;
            if (key == "TempGoal" || key == "TempRefrigerator")
                Temperature = int.Parse(kvp.Value);
            if (key == "State")
            {
                localizedState.AppendLine($"{localizedKey}: {((State)int.Parse(kvp.Value)).GetDescription()}");
                continue;
            }
            localizedState.AppendLine($"{localizedKey}: {kvp.Value}");
        }

        return localizedState.ToString();
    }

    static IEnumerator GetRequest(string uri, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.certificateHandler = new WebRequestCert();
            Debug.Log($"Requesting : {uri}");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }

            callback(webRequest);
        }
    }
}

[DataContract(Name = "deviceImage")]
public class DeviceImage
{
    [DataMember(Name = "deviceId")]
    public string DeviceId { get; set; }
    [DataMember(Name = "imageData")]
    public byte[] ImageData { get; set; }
    [DataMember(Name = "widthInMeters")]
    public float WidthInMeters { get; set; }

}
