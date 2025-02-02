using UnityEngine;

public class PredictableMovement : SteeringBehaviors
{

    // Tener los waypoints que nuestro agente va a visitar en el orden en que estén.
    [SerializeField]
    GameObject[] waypoints;

    // Nos dice a  cuál waypoint se va a dirigir actualmente.
    private int currentTargetWaypoint = 0;
    
    [SerializeField]
    private float acceptanceRadius = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        // Checar si ya llegó Cerca de ese waypoint.
        // Decimos que algo está cerca cuando la distancia hacia ese algo no es tan grande.
        // ¿en este caso, qué es Grande?

        if( (transform.position - waypoints[currentTargetWaypoint].transform.position).magnitude < 
            acceptanceRadius)
        {
            // Entonces ya llegó y ahora se mueve hacia el siguiente waypoint.
            currentTargetWaypoint++;
            if (currentTargetWaypoint >= waypoints.Length)
            {
                currentTargetWaypoint = 0;
            }
            // currentTargetWaypoint = currentTargetWaypoint % waypoints.Length; // daría lo mismo que el if de arriba
            //

            // Gizmos
            // .
        }

        // Hacemos seek hacia el waypoint que actualmente queremos llegar.
        Vector3 steeringForce = Seek(waypoints[currentTargetWaypoint].transform.position);

        rb.AddForce(steeringForce, ForceMode.Acceleration);

    }

    void OnDrawGizmos()
    {
        // vamos a dibujar el radio de aceptación alrededor de nuestros waypoints.
        Gizmos.DrawWireSphere(waypoints[currentTargetWaypoint].transform.position, acceptanceRadius);

    }
}