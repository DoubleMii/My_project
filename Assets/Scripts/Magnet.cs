using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Magnet : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision) //The OnTriggerStay function is exectued all the time the MagneticObject is inside the Magnet influence area.
    {
        MagneticObjects mag = collision.GetComponent<MagneticObjects>(); //This variable takes the MagneticObjects script to can use his public functions.
        if (collision.gameObject.CompareTag("MagneticObjects") && Input.GetKey(KeyCode.Q)) //If I press the M key and the object is inside the magnetic field, it will be attracted to the player.
        {
            mag.SetTarget(transform.parent.position); //Setting the target to the player position.
        }
        else
        {
            mag.NoTarget(); //Unsetting the target when it's not in the magnetic field or I dont press M.
        }
    }






}
