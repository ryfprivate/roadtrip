using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

namespace com.dotdothorse.roadtrip
{
    public enum Turn
    {
        Left,
        Straight,
        Right
    }
    public struct InputData
    {
        public bool Accelerate;
        public bool Boost;
        public float TurnInput;
    }

    public class ArcadeCar : MonoBehaviour
    {

        [System.Serializable]
        public struct Stats
        {
            [Header("Movement Settings")]
            [Min(0.001f), Tooltip("Top speed attainable when moving forward.")]
            public float TopSpeed;

            [Tooltip("How much the top speed increases when boosting")]
            public float BoostSpeedMultiplier;

            [Tooltip("How quickly the kart reaches top speed.")]
            public float Acceleration;

            [Tooltip("How much faster the car accelerates when boosting.")]
            public float BoostAccelerationMultiplier;

            [Tooltip("How quickly the kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
            [Range(0.2f, 1)]
            public float AccelerationCurve;

            [Tooltip("How quickly the kart will reach a full stop when no inputs are made.")]
            public float CoastingDrag;

            [Range(0.0f, 1.0f)]
            [Tooltip("The amount of side-to-side friction.")]
            public float Grip;

            [Tooltip("How tightly the kart can turn left or right.")]
            public float Steer;

            [Tooltip("Additional gravity for when the kart is in the air.")]
            public float AddedGravity;

            // allow for stat adding for powerups.
            public static Stats operator +(Stats a, Stats b)
            {
                return new Stats
                {
                    Acceleration = a.Acceleration + b.Acceleration,
                    BoostAccelerationMultiplier = a.BoostAccelerationMultiplier + b.BoostAccelerationMultiplier,
                    AccelerationCurve = a.AccelerationCurve + b.AccelerationCurve,
                    CoastingDrag = a.CoastingDrag + b.CoastingDrag,
                    AddedGravity = a.AddedGravity + b.AddedGravity,
                    Grip = a.Grip + b.Grip,
                    TopSpeed = a.TopSpeed + b.TopSpeed,
                    BoostSpeedMultiplier = a.BoostSpeedMultiplier + b.BoostSpeedMultiplier,
                    Steer = a.Steer + b.Steer,
                };
            }
        }
        public Stats currentStats = new Stats
        {
            TopSpeed = 50f,
            BoostSpeedMultiplier = 1.5f,
            Acceleration = 5f,
            BoostAccelerationMultiplier = 1.2f,
            AccelerationCurve = 4f,
            Steer = 5f,
            CoastingDrag = 4f,
            AddedGravity = 1f,
        };

        public Rigidbody Rigidbody { get; private set; }
        public InputData Input;
        public float AirPercent { get; private set; }
        public float GroundPercent { get; private set; }

        #region Vehicle Physics
        [Header("Vehicle Physics")]
        [Tooltip("The transform that determines the position of the kart's mass.")]
        public Transform CenterOfMass;
        [Range(0.0f, 20.0f), Tooltip("Coefficient used to reorient the kart in the air. The higher the number, the faster the kart will readjust itself along the horizontal plane.")]
        public float AirborneReorientationCoefficient = 3.0f;
        #endregion
        #region Suspensions
        [Header("Suspensions")]
        [Tooltip("The maximum extension possible between the kart's body and the wheels.")]
        [Range(0.0f, 1.0f)]
        public float SuspensionHeight = 0.2f;
        [Range(10.0f, 100000.0f), Tooltip("The higher the value, the stiffer the suspension will be.")]
        public float SuspensionSpring = 20000.0f;
        [Range(0.0f, 5000.0f), Tooltip("The higher the value, the faster the kart will stabilize itself.")]
        public float SuspensionDamp = 500.0f;
        [Tooltip("Vertical offset to adjust the position of the wheels relative to the kart's body.")]
        [Range(-1.0f, 1.0f)]
        public float WheelsPositionVerticalOffset = 0.0f;
        #endregion
        #region Set Wheels
        [Header("Physical Wheels")]
        [Tooltip("The physical representations of the Kart's wheels.")]
        public WheelCollider FrontLeftWheel;
        public WheelCollider FrontRightWheel;
        public WheelCollider RearLeftWheel;
        public WheelCollider RearRightWheel;
        public void AdjustWheels(Vector3 frontLeft, Vector3 frontRight, Vector3 rearLeft, Vector3 rearRight)
        {
            FrontLeftWheel.transform.position = frontLeft;
            FrontRightWheel.transform.position = frontRight;
            RearLeftWheel.transform.position = rearLeft;
            RearRightWheel.transform.position = rearRight;
        }
        #endregion

        [Tooltip("Which layers the wheels will detect.")]
        public LayerMask GroundLayers = Physics.DefaultRaycastLayers;

        Vector3 m_VerticalReference = Vector3.up;

        // can the kart move?
        bool m_CanMove = true;

        Vector3 m_LastCollisionNormal;
        bool m_HasCollision;
        bool m_InAir = false;

        private Turn turnState;
        private bool turningBoth;

        private float realSpeed;

        public void Initialize(float startSpeed, float gameSpeed, float steer)
        {
            currentStats.TopSpeed = startSpeed;
            realSpeed = gameSpeed;
            currentStats.Steer = steer;
        }
        public void StartRealSpeed()
        {
            currentStats.TopSpeed = realSpeed;
        }
        public void EnableBoost()
        {
            currentStats.BoostSpeedMultiplier = 2f;
        }
        public void UpgradeAcceleration()
        {
            currentStats.Acceleration = 8f;
        }
        public void UpgradeSpeed()
        {
            realSpeed += 5f;
        }

        #region Input Functions
        public void ResetInput()
        {
            Input = new InputData()
            {
                Accelerate = false,
                Boost = false,
                TurnInput = 0
            };
            turningBoth = false;
        }
        public void TurnLeft()
        {
            if (turnState == Turn.Right)
            {
                turningBoth = true;
            }

            turnState = Turn.Left;
            Input.TurnInput = -1;
        }
        public void TurnRight()
        {
            if (turnState == Turn.Left)
            {
                turningBoth = true;
            }

            turnState = Turn.Right;
            Input.TurnInput = 1;
        }
        public void StartBoost()
        {
            Input.Boost = true;
        }

        public void StopLeft()
        {
            turnState = Turn.Straight;
        }
        public void StopRight()
        {
            turnState = Turn.Straight;
        }
        public void StopBoost()
        {
            Input.Boost = false;
        }
        public void TurnRelease()
        {
            if (turningBoth)
            {
                turningBoth = false;
                return;
            }
            Input.TurnInput = 0;
        }
        #endregion
        #region Movement Functions
        public bool CanMove() => Input.Accelerate;
        private Vector3 pauseVel = Vector3.zero;
        public void Freeze()
        {
            pauseVel = Rigidbody.velocity;
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            m_CanMove = false;
        }
        public void UnFreeze()
        {
            Rigidbody.constraints = RigidbodyConstraints.None;
            if (pauseVel.magnitude > 0)
            {
                Rigidbody.velocity = pauseVel;
            }
            m_CanMove = true;
        }
        public void TurnOnEngine()
        {
            Input.Accelerate = true;
        }
        public void TurnOffEngine()
        {
            Input.Accelerate = false;
        }
        #endregion

        void Awake()
        {
            ResetInput();

            Rigidbody = GetComponent<Rigidbody>();
            Freeze();

            UpdateSuspensionParams(FrontLeftWheel);
            UpdateSuspensionParams(FrontRightWheel);
            UpdateSuspensionParams(RearLeftWheel);
            UpdateSuspensionParams(RearRightWheel);

            turnState = Turn.Straight;
        }

        void FixedUpdate()
        {
            UpdateSuspensionParams(FrontLeftWheel);
            UpdateSuspensionParams(FrontRightWheel);
            UpdateSuspensionParams(RearLeftWheel);
            UpdateSuspensionParams(RearRightWheel);

            // apply our physics properties
            Rigidbody.centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);

            int groundedCount = 0;
            if (FrontLeftWheel.isGrounded && FrontLeftWheel.GetGroundHit(out WheelHit hit))
                groundedCount++;
            if (FrontRightWheel.isGrounded && FrontRightWheel.GetGroundHit(out hit))
                groundedCount++;
            if (RearLeftWheel.isGrounded && RearLeftWheel.GetGroundHit(out hit))
                groundedCount++;
            if (RearRightWheel.isGrounded && RearRightWheel.GetGroundHit(out hit))
                groundedCount++;

            // calculate how grounded and airborne we are
            GroundPercent = (float)groundedCount / 4.0f;
            AirPercent = 1 - GroundPercent;

            // apply vehicle physics
            if (m_CanMove)
            {
                MoveVehicle(Input.Accelerate, Input.Boost, Input.TurnInput);
            }

            GroundAirbourne();
        }

        void GroundAirbourne()
        {
            // while in the air, fall faster
            if (AirPercent >= 1)
            {
                Rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime * currentStats.AddedGravity;
            }
        }

        public void Reset()
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = euler.z = 0f;
            transform.rotation = Quaternion.Euler(euler);
        }

        void OnCollisionEnter(Collision collision) => m_HasCollision = true;
        void OnCollisionExit(Collision collision) => m_HasCollision = false;

        void OnCollisionStay(Collision collision)
        {
            m_HasCollision = true;
            m_LastCollisionNormal = Vector3.zero;
            float dot = -1.0f;

            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > dot)
                    m_LastCollisionNormal = contact.normal;
            }
        }

        void MoveVehicle(bool accelerate, bool boost, float turnInput)
        {
            float accelInput = (accelerate ? 1.0f : 0.0f);

            // manual acceleration curve coefficient scalar
            float accelerationCurveCoeff = 5;
            Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);

            bool accelDirectionIsFwd = accelInput >= 0;
            bool localVelDirectionIsFwd = localVel.z >= 0;

            // use the max speed for the direction we are going--forward or reverse.
            float maxSpeed = currentStats.TopSpeed;
            float accelPower = currentStats.Acceleration;
            if (boost)
            {
                maxSpeed *= currentStats.BoostSpeedMultiplier;
                accelPower *= currentStats.BoostAccelerationMultiplier;
            }

            float currentSpeed = Rigidbody.velocity.magnitude;
            float accelRampT = currentSpeed / maxSpeed;
            float multipliedAccelerationCurve = currentStats.AccelerationCurve * accelerationCurveCoeff;
            float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

            float finalAcceleration = accelPower * accelRamp;

            // apply inputs to forward/backward
            float turningPower = turnInput * currentStats.Steer;

            Quaternion turnAngle = Quaternion.AngleAxis(turningPower, transform.up);
            Vector3 fwd = turnAngle * transform.forward;
            Vector3 movement = fwd * accelInput * finalAcceleration * ((m_HasCollision || GroundPercent > 0.0f) ? 1.0f : 0.0f);

            // forward movement
            bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

            if (!boost)
            {
                // if over max speed, cannot accelerate faster.
                if (wasOverMaxSpeed)
                {
                    movement *= 0.0f;
                }
            }

            Vector3 newVelocity = Rigidbody.velocity + movement * Time.fixedDeltaTime;
            newVelocity.y = Rigidbody.velocity.y;

            //  clamp max speed if we are on ground
            if (GroundPercent > 0.0f && !wasOverMaxSpeed)
            {
                newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
            }

            if (wasOverMaxSpeed)
            {
                newVelocity = Vector3.MoveTowards(newVelocity, new Vector3(0, Rigidbody.velocity.y, 0), Time.fixedDeltaTime * currentStats.CoastingDrag);
            }

            Rigidbody.velocity = newVelocity;

            // Drift
            if (GroundPercent > 0.0f)
            {
                if (m_InAir)
                {
                    m_InAir = false;
                    // Instantiate(JumpVFX, transform.position, Quaternion.identity);
                }

                // manual angular velocity coefficient
                float angularVelocitySteering = 0.4f;
                float angularVelocitySmoothSpeed = 20f;

                // turning is reversed if we're going in reverse and pressing reverse
                if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
                    angularVelocitySteering *= -1.0f;

                var angularVel = Rigidbody.angularVelocity;

                // move the Y angular velocity towards our target
                angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.fixedDeltaTime * angularVelocitySmoothSpeed);

                // apply the angular velocity
                Rigidbody.angularVelocity = angularVel;

                // rotate rigidbody's velocity as well to generate immediate velocity redirection
                // manual velocity steering coefficient
                float velocitySteering = 25f;

                // rotate our velocity based on current steer value
                Rigidbody.velocity = Quaternion.AngleAxis(turningPower * Mathf.Sign(localVel.z) * velocitySteering * Time.fixedDeltaTime, transform.up) * Rigidbody.velocity;
            }
            else
            {
                m_InAir = true;
            }

            bool validPosition = false;
            if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out RaycastHit hit, 3.0f, 1 << 9 | 1 << 10 | 1 << 11)) // Layer: ground (9) / Environment(10) / Track (11)
            {
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > hit.normal.y) ? m_LastCollisionNormal : hit.normal;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime * (GroundPercent > 0.0f ? 10.0f : 1.0f)));    // Blend faster if on ground
            }
            else
            {
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > 0.0f) ? m_LastCollisionNormal : Vector3.up;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime));
            }

            validPosition = GroundPercent > 0.7f && !m_HasCollision && Vector3.Dot(m_VerticalReference, Vector3.up) > 0.9f;

            // Airborne / Half on ground management
            if (GroundPercent < 0.7f)
            {
                Rigidbody.angularVelocity = new Vector3(0.0f, Rigidbody.angularVelocity.y * 0.98f, 0.0f);
                Vector3 finalOrientationDirection = Vector3.ProjectOnPlane(transform.forward, m_VerticalReference);
                finalOrientationDirection.Normalize();
                if (finalOrientationDirection.sqrMagnitude > 0.0f)
                {
                    Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, Quaternion.LookRotation(finalOrientationDirection, m_VerticalReference), Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime)));
                }
            }
        }
        void UpdateSuspensionParams(WheelCollider wheel)
        {
            wheel.suspensionDistance = SuspensionHeight;
            wheel.center = new Vector3(0.0f, WheelsPositionVerticalOffset, 0.0f);
            JointSpring spring = wheel.suspensionSpring;
            spring.spring = SuspensionSpring;
            spring.damper = SuspensionDamp;
            wheel.suspensionSpring = spring;
        }
    }
}
