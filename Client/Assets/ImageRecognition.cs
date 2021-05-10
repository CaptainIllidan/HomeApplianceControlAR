using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageRecognition : MonoBehaviour
{
    private ARTrackedImageManager aRTrackedImageManager;

    private void Awake()
    {
        aRTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    public void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    public void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        if (args.removed.Any())
        {
            args.removed.ForEach(r => Debug.Log($"Missing device {r.referenceImage.name}"));
            StateManager.Instance.UpdateDevice(string.Empty);
            ButtonManager.Instance.UpdateStateText("Поиск устройств");
        }

        foreach (var trackedImage in args.added)
        {
            Debug.Log(trackedImage.name);
            var deviceId = trackedImage.referenceImage.name.Replace(".jpg", string.Empty);
            // Несовместимые устройства начинаеются с символа '_'
            if (deviceId.StartsWith("_"))
            {
                deviceId = string.Empty;
                ButtonManager.Instance.UpdateStateText("Неизвестное устройство");
            }

            Debug.Log($"Start tracking device {deviceId}");
            StateManager.Instance.UpdateDevice(deviceId);
        }
    }
}
