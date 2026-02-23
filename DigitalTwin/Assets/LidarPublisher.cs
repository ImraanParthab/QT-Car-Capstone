using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class LidarPublisher : MonoBehaviour
{
    public string topicName = "/scan";

    [Header("Scan Settings")]
    public float rangeMin = 0.01f;
    public float rangeMax = 10.0f;
    public float fovDeg = 270f;
    public int rays = 720;
    public float publishHz = 10f;

    [Header("Frame")]
    public string frameId = "laser";

    ROSConnection ros;
    float nextTime;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<LaserScanMsg>(topicName);
        nextTime = Time.time;
    }

    void Update()
    {
        if (Time.time < nextTime) return;
        nextTime = Time.time + 1.0f / Mathf.Max(0.1f, publishHz);

        PublishScan();
    }

    void PublishScan(){
    float angleMin = -0.5f * fovDeg * Mathf.Deg2Rad;
    float angleMax =  0.5f * fovDeg * Mathf.Deg2Rad;
    float angleInc = (rays > 1) ? (angleMax - angleMin) / (rays - 1) : 0f;

    float[] ranges = new float[rays];

    Vector3 flatForward = transform.forward;
    flatForward.y = 0f;
    flatForward = flatForward.normalized;
    if (flatForward.sqrMagnitude < 1e-6f) flatForward = Vector3.forward;

    Vector3 origin = transform.position + Vector3.up * 0.05f;

    for (int i = 0; i < rays; i++)
    {
        float a = angleMin + i * angleInc;
        Vector3 dir = Quaternion.AngleAxis(a * Mathf.Rad2Deg, Vector3.up) * flatForward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, rangeMax))
        {
            float d = hit.distance;
            ranges[i] = (d >= rangeMin) ? d : float.PositiveInfinity;
        }
        else
        {
            ranges[i] = float.PositiveInfinity;
        }
    }

    var header = new HeaderMsg
    {
        frame_id = frameId,
        stamp   = new TimeMsg(
            (int)Time.realtimeSinceStartup,
            (uint)((Time.realtimeSinceStartup % 1f) * 1_000_000_000)
        )
    };

    var msg = new LaserScanMsg
    {
        header         = header,
        angle_min      = angleMin,
        angle_max      = angleMax,
        angle_increment= angleInc,
        time_increment = 0.0f,
        scan_time      = 1.0f / Mathf.Max(0.1f, publishHz),
        range_min      = rangeMin,
        range_max      = rangeMax,
        ranges         = ranges,
        intensities    = Array.Empty<float>()
    };

    ros.Publish(topicName, msg);
}
}
