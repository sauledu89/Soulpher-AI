using TMPro;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{

    // Posición
    // ya la tenemos a través del transform.position

    // velocidad, cuánto cambia la posición cada X tiempo. // Tiene dirección y magnitud
    protected Vector3 currentVelocity = Vector3.zero;

    // para limitar la velocidad de este agente en cada cuadro.
    [SerializeField]
    protected float maxVelocity = 10.0f;

    // Aceleración, cuánto cambia la velocidad cada X tiempo. // Tiene dirección y magnitud
    // protected Vector3 currentAcceleration = new Vector3();
    [SerializeField]
    protected float maxForce = 2.0f;


    // [SerializeField]
    // protected PredictableMovement ReferenciaEnemigo;
    protected GameObject ReferenciaObjetivo;
    protected Rigidbody targetRB;

    // si queda tiempo vemos cómo quedaría con esta forma de implementarlo.
    // protected PlayerControllerRef = null; 

    public void SetEnemyReference(GameObject enemyRef)
    { 
        ReferenciaObjetivo = enemyRef;
        // Debug.Log($"{name} tiene como objetivo a: {enemyRef.name}");

        // tenemos que checar si hay un rigidbody o no.
        if(ReferenciaObjetivo != null)
        { 
            targetRB = ReferenciaObjetivo.GetComponent<Rigidbody>();
            if(targetRB == null)
            {
                Debug.Log("El enemigo referenciado actualmente no tiene Rigidbody. ¿Así debería ser?");
            }
        }
    }

    //public void SetEnemyReference(PredictableMovement enemy)
    //{
    //    ReferenciaEnemigo = enemy;
    //}

    [SerializeField]
    protected Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    protected Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 steeringForce = Vector3.zero;

        // La flecha que me lleva hacia mi objetivo lo más rápido que yo podría ir.
        // el método punta menos cola nos da la flecha hacia el objetivo.
        Vector3 desiredDirection = targetPosition - transform.position;

        Vector3 desiredDirectionNormalized = desiredDirection.normalized;

        //                      // la pura dirección hacia objetivo * mi máxima velocidad posible
        Vector3 desiredVelocity = desiredDirectionNormalized        * maxVelocity;

        // Steering = velocidad deseada – velocidad actual
        steeringForce = desiredVelocity - rb.linearVelocity; // currentVelocity;

        return steeringForce;
    }

    protected Vector3 Flee(Vector3 targetPosition)
    {
        // Flee hace lo mismo que Seek pero en el sentido opuesto.
        // Lo hacemos del sentido opuesto usando el signo de menos '-'.
        return -Seek(targetPosition);
    }

    // Para pursuit necesitamos conocer la velocidad de nuestro objetivo.
    Vector3 Pursuit(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        // Cuánta distancia hay entre mi objetivo y yo, dividida entre mi máxima velocidad posible.
        float LookAheadTime = (transform.position - targetPosition).magnitude / maxVelocity;

        Vector3 predictedPosition = targetPosition + targetCurrentVelocity * LookAheadTime;

        return Seek(predictedPosition);
    }

    Vector3 Evade(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        // Cuánta distancia hay entre mi objetivo y yo, dividida entre mi máxima velocidad posible.
        float LookAheadTime = (transform.position - targetPosition).magnitude / maxVelocity;

        Vector3 predictedPosition = targetPosition + targetCurrentVelocity * LookAheadTime;

        return -Seek(predictedPosition);
    }

    private void FixedUpdate()
    {
        Vector3 steeringForce = Vector3.zero;

        // Vector3 steeringForce = Seek(ReferenciaEnemigo.transform.position);


        // Vector3 steeringForce = Pursuit(ReferenciaEnemigo.transform.position, ReferenciaEnemigo.rb.linearVelocity );

        // Solo aplicamos Pursuit si el objetivo que estamos persiguiendo tiene un Rigidbody.
        if (ReferenciaObjetivo != null)
        {
            if (targetRB != null)
            {
                steeringForce = Pursuit(ReferenciaObjetivo.transform.position, targetRB.linearVelocity);
            }
            else if (ReferenciaObjetivo != null)
            {
                steeringForce = Seek(ReferenciaObjetivo.transform.position);
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

        // Debería estar aquí pero ahorita no hace nada, según yo.
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

        rb.AddForce(steeringForce, ForceMode.Acceleration);

        if(rb.linearVelocity.magnitude > maxVelocity)
            Debug.LogWarning(rb.linearVelocity);
    }

    private void OnDrawGizmos()
    {
        Vector3 targetPosition = Vector3.zero;
        Vector3 targetCurrentVelocity = Vector3.zero;

        if (ReferenciaObjetivo != null)
        {
            targetPosition = ReferenciaObjetivo.transform.position;
            if (targetRB != null)
            { targetCurrentVelocity = targetRB.linearVelocity; }
        }
            
        float LookAheadTime = (transform.position - targetPosition).magnitude / maxVelocity;


        Vector3 predictedPosition = targetPosition + targetCurrentVelocity * LookAheadTime;

        Gizmos.DrawCube(predictedPosition, Vector3.one);

        Gizmos.DrawLine(transform.position, predictedPosition);

        Gizmos.color = Color.red;
        // Hacemos una línea de la velocidad que tiene este agente ahorita.
        Gizmos.DrawLine (transform.position, transform.position + rb.linearVelocity.normalized * 3);

        // Dibujamos las fuerzas.
        Gizmos.color = Color.green;

        Vector3 steeringForce = Vector3.zero;

        if (targetRB != null && ReferenciaObjetivo != null)
            steeringForce = Pursuit(ReferenciaObjetivo.transform.position, targetCurrentVelocity);

        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

        Gizmos.DrawLine(transform.position, transform.position + steeringForce);

    }

    // Update is called once per frame
    /* void Update()
    {
        // Para saber hacia dónde ir, aplicamos el método punta menos cola, 
        // ReferenciaEnemigo va a ser la punta.
        // La posición del dueño de este script va a ser la Cola del vector.
        // Vector3 Difference = ReferenciaEnemigo.transform.position - transform.position;

        // le aplicamos la fuerza del seek a nuestro agente.
        // con esto, no siempre vamos a acelerar la máxima cantidad.
        Vector3 accelerationVector = Seek(ReferenciaEnemigo.transform.position);

        // nuestra velocidad debe de incrementar de acuerdo a nuestra aceleración.
        currentVelocity += accelerationVector * Time.deltaTime;


        // Queremos obtener velocidad hacia el objetivo.
        // currentVelocity += Difference;
        Debug.Log($"currentVelocity antes de limitarla {currentVelocity}");

        // cómo limitan el valor de una variable?
        if (currentVelocity.magnitude < maxVelocity)
        {
            // entonces la velocidad se queda como está, porque no es mayor que max velocity.
        }
        else
        {
            // si no, haces que la velocidad actual sea igual que la velocidad máxima.
            currentVelocity = currentVelocity.normalized * maxVelocity;
            Debug.Log($"currentVelocity después de limitarla {currentVelocity}");
        }

        //if(Difference.magnitude < DetectionDistance)
        //{
        //    // ya lo detectaste.
        //}



        // Ahora hacemos que la velocidad cambie nuestra posición conforme al paso del tiempo.
        transform.position += currentVelocity * Time.deltaTime;
    }*/
}
