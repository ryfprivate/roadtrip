using System;
using UnityEngine;

namespace com.dotdothorse.roadtrip
{
    // Make this script run earlier than others
    [DefaultExecutionOrder(100)]
    public class CarAnimation : MonoBehaviour
    {
        [Serializable]
        public class Wheel
        {
            [Tooltip("A reference to the transform of the wheel.")]
            public Transform wheelTransform;
            [Tooltip("A reference to the WheelCollider of the wheel.")]
            public WheelCollider wheelCollider;

            public Wheel(Transform visual, WheelCollider collider)
            {
                wheelTransform = visual;
                wheelCollider = collider;
            }
        }

        [Tooltip("What car are we controlling?")]
        public ArcadeCar carController;

        [Space]
        [Tooltip("The damping for the appearance of steering compared to the input.  The higher the number the less damping.")]
        public float steeringAnimationDamping = 10f;

        [Space]
        [Tooltip("The maximum angle in degrees that the front wheels can be turned away from their default positions, when the Steering input is either 1 or -1.")]
        public float maxSteeringAngle;
        [Tooltip("Information referring to the front left wheel of the kart.")]
        public Wheel frontLeftWheel;
        [Tooltip("Information referring to the front right wheel of the kart.")]
        public Wheel frontRightWheel;
        [Tooltip("Information referring to the rear left wheel of the kart.")]
        public Wheel rearLeftWheel;
        [Tooltip("Information referring to the rear right wheel of the kart.")]
        public Wheel rearRightWheel;

        float m_SmoothedSteeringInput;
        bool ready;

        void Awake()
        {
            carController = GetComponent<ArcadeCar>();
            ready = false;
        }

        public void Setup(Wheel frontLeft, Wheel frontRight, Wheel rearLeft, Wheel rearRight)
        {
            frontLeftWheel = frontLeft;
            frontRightWheel = frontRight;
            rearLeftWheel = rearLeft;
            rearRightWheel = rearRight;

            ready = true;
        }

        void FixedUpdate()
        {
            if (!ready) return;
            m_SmoothedSteeringInput = Mathf.MoveTowards(m_SmoothedSteeringInput, carController.Input.TurnInput,
                steeringAnimationDamping * Time.deltaTime);

            // Steer front wheels
            float rotationAngle = m_SmoothedSteeringInput * maxSteeringAngle;

            frontLeftWheel.wheelCollider.steerAngle = rotationAngle;
            frontRightWheel.wheelCollider.steerAngle = rotationAngle;

            // Update position and rotation from WheelCollider
            UpdateWheelFromCollider(frontLeftWheel);
            UpdateWheelFromCollider(frontRightWheel);
            UpdateWheelFromCollider(rearLeftWheel);
            UpdateWheelFromCollider(rearRightWheel);
        }

        void LateUpdate()
        {
            if (!ready) return;
            // Update position and rotation from WheelCollider
            UpdateWheelFromCollider(frontLeftWheel);
            UpdateWheelFromCollider(frontRightWheel);
            UpdateWheelFromCollider(rearLeftWheel);
            UpdateWheelFromCollider(rearRightWheel);
        }

        void UpdateWheelFromCollider(Wheel wheel)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);

            wheel.wheelTransform.position = position;
            wheel.wheelTransform.rotation = rotation;
        }
    }
}
