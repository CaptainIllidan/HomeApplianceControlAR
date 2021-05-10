using System;
using System.Text;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

/// <summary>
/// Adds images to the reference library at runtime.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class DynamicImageLibrary : MonoBehaviour
{
    [Serializable]
    public class ImageData
    {
        [SerializeField, Tooltip("The source texture for the image. Must be marked as readable.")]
        Texture2D m_Texture;

        public Texture2D texture
        {
            get => m_Texture;
            set => m_Texture = value;
        }

        [SerializeField, Tooltip("The name for this image.")]
        string m_Name;

        public string name
        {
            get => m_Name;
            set => m_Name = value;
        }

        [SerializeField, Tooltip("The width, in meters, of the image in the real world.")]
        float m_Width;

        public float width
        {
            get => m_Width;
            set => m_Width = value;
        }

        public AddReferenceImageJobState jobState { get; set; }
    }

    [SerializeField, Tooltip("The set of images to add to the image library at runtime")]
    ImageData[] m_Images;

    /// <summary>
    /// The set of images to add to the image library at runtime
    /// </summary>
    public ImageData[] images
    {
        get => m_Images;
        set => m_Images = value;
    }

    public enum State
    {
        NoImagesAdded,
        AddImagesRequested,
        AddingImages,
        Done,
        Error
    }

    public State m_State;

    string m_ErrorMessage = "";

    StringBuilder m_StringBuilder = new StringBuilder();

    private RuntimeReferenceImageLibrary newLibrary;

    public void Update()
    {
        switch (m_State)
        {
            case State.AddImagesRequested:
                {
                    if (m_Images == null)
                    {
                        Debug.Log("No images to add.");
                        break;
                    }

                    var manager = GameObject.FindObjectOfType<ARTrackedImageManager>();
                    if (manager == null)
                    {
                        Debug.Log($"No {nameof(ARTrackedImageManager)} available.");
                        break;
                    }

                    // You can either add raw image bytes or use the extension method (used below) which accepts
                    // a texture. To use a texture, however, its import settings must have enabled read/write
                    // access to the texture.
                    foreach (var image in m_Images)
                    {
                        if (!image.texture.isReadable)
                        {
                            Debug.Log($"Image {image.name} must be readable to be added to the image library.");
                            break;
                        }
                    }

                    Debug.Log(
                        $"XRManager info: \nreferenceLibrary count:{manager.referenceLibrary.count}\nXrGeneralSettings is null:{XRGeneralSettings.Instance == null})");

                    if (newLibrary == null) 
                        newLibrary = manager.CreateRuntimeLibrary(manager.referenceLibrary as XRReferenceImageLibrary);

                    if (newLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
                    {
                        try
                        {
                            foreach (var image in m_Images)
                            {
                                // Note: You do not need to do anything with the returned JobHandle, but it can be
                                // useful if you want to know when the image has been added to the library since it may
                                // take several frames.
                                image.jobState = mutableLibrary.ScheduleAddImageWithValidationJob(image.texture, image.name, image.width);
                            }

                            m_State = State.AddingImages;
                        }
                        catch (InvalidOperationException e)
                        {
                            Debug.Log($"ScheduleAddImageJob threw exception: {e.Message}");
                        }
                    }
                    else
                    {
                        Debug.Log($"The reference image library is not mutable.");
                    }

                    break;
                }
            case State.AddingImages:
                {
                    // Check for completion
                    var done = true;
                    foreach (var image in m_Images)
                    {
                        if (!image.jobState.jobHandle.IsCompleted)
                        {
                            done = false;
                            break;
                        }
                    }

                    if (done)
                    {
                        var manager = GetComponent<ARTrackedImageManager>();
                        manager.referenceLibrary = newLibrary;
                        m_State = State.Done;
                    }

                    break;
                }
        }
    }
}