using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;

[RequireComponent(typeof(Rigidbody))]
public class ImuPublisher : MonoBehaviour
{
    public string topicName = "/imu/data";
    public string frameId = "imu";
    public float publishHz = 50f;

    [Header("Acceleration")]
    public bool includeGravity = true; // set false if you want "proper acceleration" (gravity removed)

    ROSConnection ros;
    Rigidbody rb;

    Vector3 prevVel;
    float nextTime;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        rb = GetComponent<Rigidbody>();

        ros.RegisterPublisher<ImuMsg>(topicName);

        prevVel = rb.linearVelocity;
        nextTime = Time.time;
    }

    void Update()
    {
        if (Time.time < nextTime) return;
        float dt = 1.0f / Mathf.Max(1f, publishHz);
        nextTime = Time.time + dt;

        PublishImu(dt);
    }

    void PublishImu(float dt)
    {
        // Orientation (Unity quaternion -> ROS quaternion fields)
        Quaternion q = transform.rotation;
        var orientation = new QuaternionMsg(q.x, q.y, q.z, q.w);

        // Angular velocity (rad/s) from Rigidbody
        Vector3 w = rb.angularVelocity;
        var angVel = new Vector3Msg(w.x, w.y, w.z);

        // Linear acceleration (m/s^2) from velocity difference
        Vector3 v = rb.linearVelocity;
        Vector3 a = (v - prevVel) / Mathf.Max(1e-4f, dt);
        prevVel = v;

        if (!includeGravity)
        {
            // Remove gravity (approx) to get "proper acceleration"
            a -= Physics.gravity;
        }

        var linAcc = new Vector3Msg(a.x, a.y, a.z);

        var header = new HeaderMsg
        {
            frame_id = frameId
            // stamp left default (sec=0,nanosec=0) is OK for bringup/testing
        };

        var msg = new ImuMsg
        {
            header = header,
            orientation = orientation,
            angular_velocity = angVel,
            linear_acceleration = linAcc,

            // Covariances: -1 means "unknown"
            orientation_covariance = new double[9] { -1,0,0, 0,-1,0, 0,0,-1 },
            angular_velocity_covariance = new double[9] { -1,0,0, 0,-1,0, 0,0,-1 },
            linear_acceleration_covariance = new double[9] { -1,0,0, 0,-1,0, 0,0,-1 }
        };

        ros.Publish(topicName, msg);
    }
}
