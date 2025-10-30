using UnityEngine;

namespace Player.Scripts
{
    public class CameraPositioner : MonoBehaviour
    {
        [Range(0,10)]
        [SerializeField] private float _heightOffset;
        [Range(-10,0)]
        [SerializeField] private float _distanceOffset;
        [Range(0,10)]
        [SerializeField] private float _zoomMultiplier;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            SetCameraPosition();
            SetCameraRotation();
        }

        private void SetCameraRotation()
        {
            _camera.transform.LookAt(transform.position);
        }

        private void SetCameraPosition()
        {
            
            var resultPosition = transform.position + new Vector3(0, _heightOffset, _distanceOffset) * _zoomMultiplier;
            _camera.transform.position = resultPosition;
        }
    }
}
