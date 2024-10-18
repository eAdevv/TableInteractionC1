using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableInteraction.Singleton;
using TableInteraction.Events;
using UnityEditor.Search;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace TableInteraction.CoreLoop
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]private List<Character> characters = new List<Character>();
        private Queue<Character> tableQueue = new Queue<Character>();
        private List<float> usedSpeeds = new List<float>();

        private bool isQueueBusy;
        public Transform targetPosition;
        public Slider processSlider;


        private void OnEnable()
        {
            EventManager.OnGetSpeed += GetCharacterSpeed;
            EventManager.OnCharacterEnterQueue += CharacterEnterQueue;
        }
        

        private void OnDisable()
        {
            EventManager.OnGetSpeed -= GetCharacterSpeed;
            EventManager.OnCharacterEnterQueue -= CharacterEnterQueue;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && characters.Count > 0)
            {
                MoveCharacter();
            }
        }

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
            targetPosition.position = targetPosition.position - new Vector3(0, 0, 1);
            StartCoroutine(ProcessQueue(character));
        }


        private IEnumerator ProcessQueue(Character character)
        {
            if (!isQueueBusy)
            {
                isQueueBusy = true;
                processSlider.gameObject.SetActive(true);

                float waitTime = 5f;
                processSlider.value = 0;
                float elapsedTime = 0f;

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

        private void RemoveFromQueue(Character character)
        {
            processSlider.gameObject.SetActive(false);
            isQueueBusy = false;
            tableQueue.Dequeue();
            Destroy(character.gameObject);
            ReOrganizeQueue();
        }
        private void ReOrganizeQueue()
        {
            targetPosition.position = targetPosition.position + new Vector3(0, 0, 1);

            if (tableQueue.Count > 0)
            {
                StartCoroutine(ProcessQueue(tableQueue.Peek()));
                foreach (Character character in tableQueue)
                {
                    character.CharacterMoveInQueue();
                }
            }
        }


        public Vector3 TargetPos()
        {
            return targetPosition.position;
        }
    }
}
