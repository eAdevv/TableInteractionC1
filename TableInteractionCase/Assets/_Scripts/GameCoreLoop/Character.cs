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
            if (IsMoving)
            {
                targetPosition = EventManager.OnGetTargetPosition.Invoke();

                AIMove(targetPosition);

                if (!characterAgent.pathPending && characterAgent.remainingDistance <= characterAgent.stoppingDistance)
                {
                    CharacterReachActivity();
                }
            }
        }

        private void CharacterReachActivity()
        {
            AIStop();
            EventManager.OnCharacterEnterQueue?.Invoke(this);

            transform.DOMove(targetPosition, 0.25f).OnComplete(() =>
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                animator.SetBool(runningHash, false);
            });
        }

        public void CharacterMoveInQueue()
        {
            animator.SetBool(runningHash, true);
            transform.DOMoveZ(transform.position.z + 1, 0.25f).OnComplete(()=> animator.SetBool(runningHash, false));
        }
 
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

        public IEnumerator CharacterRunAwayFromTable()
        {
            Vector3 ranmdomPosition = new Vector3(characterAgent.transform.position.x + 30f, characterAgent.transform.position.y, Random.Range(1f, 25f));
            AIMove(ranmdomPosition);
            characterAgent.stoppingDistance = 2f;
            yield return new WaitUntil(() => 
                Vector3.Distance(characterAgent.transform.position, ranmdomPosition) <= characterAgent.stoppingDistance);
            AIStop();
        }
    }
}
