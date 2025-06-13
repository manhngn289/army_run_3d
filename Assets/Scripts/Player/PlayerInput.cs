using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        // General settings for player input
        public static float MIN_HORIZONTAL_OFFSET_THRESHOLD = 0.03f;
        
        
        [Header("Input Data")]
        [SerializeField] private Vector2 lastTouchPos;
        [SerializeField] private float horizontalOffset;
        [SerializeField] private bool isTouch;
        
        public bool CanSlideHorizontal => Mathf.Abs(horizontalOffset) > MIN_HORIZONTAL_OFFSET_THRESHOLD && isTouch;


        public float HandleInput()
        {
            horizontalOffset = 0f;
            isTouch = false;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    lastTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.position - lastTouchPos;
                    horizontalOffset = delta.x / (Screen.width / 10f);
                    lastTouchPos = touch.position;
                    isTouch = Mathf.Abs(horizontalOffset) > MIN_HORIZONTAL_OFFSET_THRESHOLD;
                }
            }
#if UNITY_EDITOR            
            else
            {
                float axis = Input.GetAxis("Horizontal");
                horizontalOffset = Mathf.Abs(axis) > MIN_HORIZONTAL_OFFSET_THRESHOLD ? axis : 0f;
                isTouch = Mathf.Abs(horizontalOffset) > MIN_HORIZONTAL_OFFSET_THRESHOLD;
            }
#endif
            
            return horizontalOffset;
        }
    }
}