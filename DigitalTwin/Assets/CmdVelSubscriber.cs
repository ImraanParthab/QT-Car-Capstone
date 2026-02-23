using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

[RequireComponent(typeof(Rigidbody))]
public class CmdVelSubscriber : MonoBehaviour
{
    public string topicName = "/cmd_vel";

    public float maxSpeed = 2.0f;     // m/s
    public float maxYawRate = 2.0f;   // rad/s
    public float accel = 5.0f;
    public float yawAccel = 5.0f;

    Rigidbody rb;
    float targetV = 0f;
    float targetW = 0f;
    float curV = 0f;
    float curW = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>(topicName, OnTwist);
    }

    void OnTwist(TwistMsg msg)
    {
        targetV = Mathf.Clamp((float)msg.linear.x, -maxSpeed, maxSpeed);
        targetW = Mathf.Clamp((float)msg.angular.z, -maxYawRate, maxYawRate);
    }

    void FixedUpdate()
    {
        curV = Mathf.MoveTowards(curV, targetV, accel * Time.fixedDeltaTime);
        curW = Mathf.MoveTowards(curW, targetW, yawAccel * Time.fixedDeltaTime);

        Vector3 v = transform.forward * curV;
        rb.linearVelocity = new Vector3(v.x, rb.linearVelocity.y, v.z);
        rb.angularVelocity = new Vector3(0f, curW, 0f);
    }
}
