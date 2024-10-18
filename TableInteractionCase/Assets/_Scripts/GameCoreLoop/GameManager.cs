using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableInteraction.Singleton;
using TableInteraction.Events;
using UnityEditor.Search;

namespace TableInteraction.CoreLoop
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]private List<Character> characters = new List<Character>();
        private Queue<Character> tableQueue = new Queue<Character>();
        private List<float> usedSpeeds = new List<float>();

        public Transform targetPosition;
       

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
        }

        public Vector3 TargetPos()
        {
            return targetPosition.position;
        }
    }
}
