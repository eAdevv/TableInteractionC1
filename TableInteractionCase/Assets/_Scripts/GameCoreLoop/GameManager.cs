using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableInteraction.Events;
using UnityEngine.UI;

namespace TableInteraction.CoreLoop
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]private List<Character> characters = new List<Character>();
        private Queue<Character> tableQueue = new Queue<Character>();
        private List<float> usedSpeeds = new List<float>();

        private bool isQueueBusy;
        [SerializeField] private Transform targetPosition;
        [SerializeField] private Slider processSlider;


        private void OnEnable()
        {
            EventManager.OnGetSpeed += GetCharacterSpeed;
            EventManager.OnGetTargetPosition += GetTargetPos;
            EventManager.OnCharacterEnterQueue += CharacterEnterQueue;
        }
        

        private void OnDisable()
        {
            EventManager.OnGetSpeed -= GetCharacterSpeed;
            EventManager.OnGetTargetPosition -= GetTargetPos;
            EventManager.OnCharacterEnterQueue -= CharacterEnterQueue;
        }

        private void Start()
        {
            
        }
        private void Update()
        {
            // Space key triggers movement.
            if (Input.GetKeyDown(KeyCode.Space) && characters.Count > 0)
            {
                MoveCharacter();
            }
        }

        // Since the characters should not have the same speed,
        // it is checked whether there is a previously assigned speed in the given range.
        private float GetCharacterSpeed()
        {
            float speed;
            do
            {
                speed = Random.Range(2f, 10f); 
            }
            while (usedSpeeds.Contains(speed)); 

            usedSpeeds.Add(speed); 
            return speed;
        }

        private void MoveCharacter()
        {
            Character character = characters[Random.Range(0, characters.Count)];

            if (!character.IsMoving)
            {
                character.IsMoving = true;
            }

            characters.Remove(character);

        }

        private void CharacterEnterQueue(Character character)
        {
            tableQueue.Enqueue(character);
            targetPosition.position = targetPosition.position - Vector3.forward;
            StartCoroutine(ProcessQueue(character));
        }


        
        private IEnumerator ProcessQueue(Character character)
        {
            if (!isQueueBusy)
            {
                float elapsedTime = 0f;
                float waitTime = 5f;

                isQueueBusy = true;
                processSlider.gameObject.SetActive(true);
                processSlider.value = 0;

                while (elapsedTime < waitTime)
                {
                    elapsedTime += Time.deltaTime;
                    processSlider.value = Mathf.Clamp01(elapsedTime / waitTime);
                    yield return null;
                }

                RemoveFromQueue(character);
            }

            yield return null;
        }
        // The character at the head of the queue is removed from the queue and its replacement enters the table process.
        // If there is no character at the beginning of the queue, the process is terminated.
        private void RemoveFromQueue(Character character)
        {
            StartCoroutine(character.CharacterRunAwayFromTable());
            processSlider.gameObject.SetActive(false);
            tableQueue.Dequeue();           
            ReOrganizeQueue();
        }
        private void ReOrganizeQueue()
        {
            isQueueBusy = false;
            targetPosition.position = targetPosition.position + Vector3.forward;

            if (tableQueue.Count > 0)
            {
                foreach (Character character in tableQueue)
                {
                    character.CharacterMoveInQueue();
                }
                StartCoroutine(ProcessQueue(tableQueue.Peek()));
            }
        }


        public Vector3 GetTargetPos()
        {
            return targetPosition.position;
        }
    }
}
