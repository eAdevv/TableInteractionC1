using System.Collections;
using System.Collections.Generic;
using TableInteraction.Events;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

namespace TableInteraction.CoreLoop
{
    public class Character : MonoBehaviour
    {
        private float speed;
        private bool isMoving;
        private Vector3 targetPosition;

        [Header("AI Settings")]
        private NavMeshAgent characterAgent;
        private NavMeshObstacle characterObstacle;

        [Header("Animation Settings")]
        [SerializeField] private Animator animator;
        private int runningHash;
        public float Speed { get => speed; set => speed = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }


        private void Start()
        {
            characterAgent = GetComponent<NavMeshAgent>();
            characterObstacle = GetComponent<NavMeshObstacle>();
            characterAgent.speed = EventManager.OnGetSpeed.Invoke();
            characterAgent.enabled = false;

            animator = GetComponent<Animator>();
            runningHash = Animator.StringToHash("Running");
        }

        private void Update()
        {
            // The Space key triggers isMoving.
            // It is constantly refreshed and assigned to the AI in case the Target position changes.
            if (IsMoving)
            {
                targetPosition = EventManager.OnGetTargetPosition.Invoke();

                AIMove(targetPosition);

                // If the character is not looking for a path and
                // the character's distance to the destination is sufficient,
                // the character has reached the queue.
                if (!characterAgent.pathPending && characterAgent.remainingDistance <= characterAgent.stoppingDistance)
                {
                    CharacterReachActivity();
                }
            }
        }

        #region Character Reach & Queue Reorganize

        // When the character reaches the table, the EnterQueue event initiates the character's interaction with the table.
        private void CharacterReachActivity()
        {
            AIStop();
            EventManager.OnCharacterEnterQueue?.Invoke(this);

            // Queue correction.
            transform.DOMove(targetPosition, 0.25f).OnComplete(() =>
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                animator.SetBool(runningHash, false);
            });
        }
        

        // It allows characters to take a step forward when one person is missing from the queue.
        public void CharacterMoveInQueue()
        {
            animator.SetBool(runningHash, true);
            transform.DOMoveZ(transform.position.z + 1, 0.25f).OnComplete(()=> animator.SetBool(runningHash, false));
        }

        #endregion

        #region AI Movement Assigments
        // AI Movement assigments.
        private void AIMove(Vector3 target)
        {
            characterAgent.enabled = true;
            characterObstacle.enabled = false;
            characterAgent.SetDestination(target);
            animator.SetBool(runningHash, true);
        }

        private void AIStop()
        {
            characterAgent.enabled = false;
            characterObstacle.enabled = true;
            IsMoving = false;
            animator.SetBool(runningHash, false);
        }

        #endregion

        #region Run Away
        // When the character's table process ends, it moves 30 meters to the right to a random place.
        public IEnumerator CharacterRunAwayFromTable()
        {
            Vector3 ranmdomPosition = new Vector3(characterAgent.transform.position.x + 30f, characterAgent.transform.position.y, Random.Range(1f, 25f));
            AIMove(ranmdomPosition);
            characterAgent.stoppingDistance = 2f;
            yield return new WaitUntil(() => 
                Vector3.Distance(characterAgent.transform.position, ranmdomPosition) <= characterAgent.stoppingDistance);
            AIStop();
        }

        #endregion
    }
}
