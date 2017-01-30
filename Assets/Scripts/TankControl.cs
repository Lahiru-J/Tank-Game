using UnityEngine;

namespace Assets.Scripts
{
    public class TankControl : MonoBehaviour
    {
        private float _currentRotation;
        private Vector3 _destination;
        public float MotionSpeed;

        // Private properties
        private Quaternion _quaternion;
        public float RotationSpeed;
        // Public properties
        public float StepDistance;

        private void Start()
        {
            _quaternion = Quaternion.identity;
        }

        // Update is called once per frame
        private void Update()
        {
            // Forward Movement
            if (Input.GetKeyDown(KeyCode.UpArrow))
                Accelerate();

            // Backward Movement
            if (Input.GetKeyDown(KeyCode.DownArrow))
                Reverse();

            // Left Rotation
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                TurnLeft();

            // Right Rotation
            if (Input.GetKeyDown(KeyCode.RightArrow))
                TurnRight();

            // Apply transformation if present
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _quaternion, RotationSpeed*Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, _destination, MotionSpeed*Time.deltaTime);
        }

        // Tank Movement Logic
        private void Accelerate()
        {
            _destination += transform.up*StepDistance;
        }

        private void Reverse()
        {
            _destination -= transform.up*StepDistance;
        }

        private void TurnLeft()
        {
            _currentRotation += 90;
            _quaternion = Quaternion.Euler(new Vector3(_quaternion.x, _quaternion.y, _currentRotation));
        }

        private void TurnRight()
        {
            _currentRotation -= 90;
            _quaternion = Quaternion.Euler(new Vector3(_quaternion.x, _quaternion.y, _currentRotation));
        }
    }
}