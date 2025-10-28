using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Magnet : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision) //The OnTriggerStay function is exectued all the time the MagneticObject is inside the Magnet influence area.
    {
        MagneticObjects mag = collision.GetComponent<MagneticObjects>(); //This variable takes the MagneticObjects script to can use his public functions.

        /*if (collision.gameObject.CompareTag("MagneticObjects"))
            if (Input.GetKey(KeyCode.Q))//If I press the Q key and the object is inside the magnetic field, it will be attracted to the player.
            {
                mag.SetTarget(transform.parent.position); //Setting the target to the player position.
            }
        else
        {
            mag.NoTarget(); //Unsetting the target when it's not in the magnetic field or I dont press Q.
        }
        */
        if (collision.gameObject.CompareTag("MagneticObjects") && Input.GetKey(KeyCode.Q)) //If I press the Q key and the object is inside the magnetic field, it will be attracted to the player.
        {
            mag.SetTarget(transform.parent.position); //Setting the target to the player position.
            // Atrae
        }
        else if (Input.GetKey(KeyCode.E)) //If I press the E key and the object is inside the magnetic field, it will be repel to the player.
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if (box != null)
            {
                Vector2 center = (Vector2)transform.position;
                Vector2 forward = transform.parent.right;
                float halfWidth = box.size.x * 0.5f;

                Vector2 frontBorder = center + forward * halfWidth;
                mag.SetTarget(frontBorder);
            }
        }
        else
        {
            mag.NoTarget(); //Unsetting the target when it's not in the magnetic field or I dont press Q or E.
        }
    }

 //vector2.distanceSS




}