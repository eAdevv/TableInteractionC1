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
        private NavMeshAgent characterAgent;
        private NavMeshObstacle characterObstacle;
        public float Speed { get => speed; set => speed = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }

        private void Start()
        {
            characterAgent = GetComponent<NavMeshAgent>();
            characterObstacle = GetComponent<NavMeshObstacle>();
            characterAgent.speed = EventManager.OnGetSpeed.Invoke();
        }

        private void Update()
        {
            if (IsMoving)
            {
                targetPosition = GameManager.Instance.TargetPos();

                characterAgent.enabled = true;
                characterObstacle.enabled = false;
                characterAgent.SetDestination(targetPosition);

                if (!characterAgent.pathPending && characterAgent.remainingDistance <= characterAgent.stoppingDistance)
                {
                    CharacterReachActivity();
                }
            }
        }

        private void CharacterReachActivity()
        {
            characterAgent.enabled = false;
            characterObstacle.enabled = true;
            IsMoving = false;
            EventManager.OnCharacterEnterQueue?.Invoke(this);

            transform.DOMove(targetPosition, 0.25f).OnComplete(() =>
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            });
        }

        public void CharacterMoveInQueue()
        {
            transform.DOMoveZ(transform.position.z + 1, 0.1f);
        }


    }
}
