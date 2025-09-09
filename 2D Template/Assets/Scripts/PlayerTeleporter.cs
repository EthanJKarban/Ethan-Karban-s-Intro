using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    private GameObject currentTeleporter;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // This code allows you to edit the input so Q could become E or T or K.
        {
            if (currentTeleporter != null) 
            {
                transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;

            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Teleporter")) // Enter
        {
            currentTeleporter = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))  // Exit
        {
            if (collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
        
    }

}
