using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;

public class CameraPublisher : MonoBehaviour
{
    public string topicName = "/camera/image_raw";
    public string frameId = "camera";
    public float publishHz = 10f;

    [Header("Image Settings")]
    public RenderTexture sourceRT;
    public bool publishRGB = true; // true => rgb8, false => mono8

    ROSConnection ros;
    Texture2D tex;
    float nextTime;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImageMsg>(topicName);

        if (sourceRT == null)
        {
            Debug.LogError("CameraPublisher: sourceRT (RenderTexture) is not set.");
            enabled = false;
            return;
        }

        tex = new Texture2D(sourceRT.width, sourceRT.height, TextureFormat.RGB24, false);
        nextTime = Time.time;
    }

    void Update()
    {
        if (Time.time < nextTime) return;
        nextTime = Time.time + 1.0f / Mathf.Max(0.1f, publishHz);

        PublishImage();
    }

    void PublishImage()
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = sourceRT;

        tex.ReadPixels(new Rect(0, 0, sourceRT.width, sourceRT.height), 0, 0);
        tex.Apply(false);

        RenderTexture.active = prev;

        byte[] rgb = tex.GetRawTextureData();

        // Optional grayscale
        byte[] data;
        string encoding;
        uint step;

        if (publishRGB)
        {
            data = rgb;
            encoding = "rgb8";
            step = (uint)(sourceRT.width * 3);
        }
        else
        {
            // mono8 from rgb
            data = new byte[sourceRT.width * sourceRT.height];
            for (int i = 0, j = 0; i < data.Length; i++, j += 3)
            {
                // simple luma approx
                data[i] = (byte)((rgb[j] * 0.299f) + (rgb[j + 1] * 0.587f) + (rgb[j + 2] * 0.114f));
            }
            encoding = "mono8";
            step = (uint)sourceRT.width;
        }

        var header = new HeaderMsg { frame_id = frameId };

        var msg = new ImageMsg
        {
            header = header,
            height = (uint)sourceRT.height,
            width = (uint)sourceRT.width,
            encoding = encoding,
            is_bigendian = 0,
            step = step,
            data = data
        };

        ros.Publish(topicName, msg);
    }
}
