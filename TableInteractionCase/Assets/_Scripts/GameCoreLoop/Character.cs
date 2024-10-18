using System.Collections;
using System.Collections.Generic;
using TableInteraction.Events;
using UnityEngine;

namespace TableInteraction.CoreLoop
{
    public class Character : MonoBehaviour
    {
        private float speed;
        private bool isMoving;
        private Vector3 targetPosition;

        public float Speed { get => speed; set => speed = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }

        private void Start()
        {
            speed = EventManager.OnGetSpeed.Invoke();
        }

        private void Update()
        {
            if (IsMoving)
            {
                targetPosition = GameManager.Instance.TargetPos();
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    IsMoving = false;
                    EventManager.OnCharacterEnterQueue?.Invoke(this);
                }
            }
        }

       
    }
}
