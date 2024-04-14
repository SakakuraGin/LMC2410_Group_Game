// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class End : MonoBehaviour
// {
//     // Set the start screen in the Inspector
//     [SerializeField] private GameObject startScreen;
//     // Set the player in the Inspector
//     [SerializeField] private GameObject player;

//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

//     // When a GameObject collides with another GameObject, Unity calls OnTriggerEnter
//     void OnTriggerEnter2D(Collider2D other)
//     {
//         startScreen.SetActive(true);
//         PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
//         playerMovement.Stop();
//     }
// }
