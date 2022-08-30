using UnityEngine;

 public class PointToTarget : MonoBehaviour
 {
     [SerializeField] private Transform player;
     [SerializeField] private float turnSpeed;
     [SerializeField] private Transform anchor;

     private Quaternion rotGoal;
     private Vector3 dirn;
     private Vector3 targetPosition;
     private bool active;

     private void Start()
     {
         targetPosition = Vector3.zero;
         ToggleVisibility(false, transform.position, 0f);
     }

     private void Update()
     {
         if (!active) return;
         Vector3 playerPosition = player.position;
         dirn = (targetPosition - playerPosition).normalized;
         rotGoal = Quaternion.LookRotation(dirn);
         transform.position = anchor.position;
         transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed * Time.deltaTime);
     }

     public void ToggleVisibility(bool setActive,  Vector3 nextLandmark, float time=2f)
     {
         active = setActive;
         Vector3 arrowSize = setActive ? Vector3.one : Vector3.zero;
         LeanTweenType arrowAnim = setActive? LeanTweenType.easeInQuad: LeanTweenType.easeOutQuad;
         LeanTween.scale(gameObject, arrowSize, time).setEase(arrowAnim);
         targetPosition = setActive ? nextLandmark : transform.position;
     }

 }
