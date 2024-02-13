
using UnityEngine;
enum State { on, off }
public class Foguete : MonoBehaviour
{
   [SerializeField] State state;
   [SerializeField] GameObject compartment;
   [SerializeField] float fuel;
   [SerializeField] float speed;
   [SerializeField] GameObject Parachute;
   [SerializeField] ParticleSystem fire;
   [SerializeField] LayerMask layerMask;
   [SerializeField] float windForce;
   [SerializeField] AudioSource audio;

   public float rotationSpeed = 1f; // Velocidade de rotação do foguete
   public float maxRotationAngle = 45f;
   Rigidbody compartmentRB;
   Rigidbody rocketRB;
   Rigidbody parachuteRB;
   RaycastHit hit;
   private void Start()
   {
      compartmentRB = compartment.GetComponent<Rigidbody>();
      rocketRB = GetComponent<Rigidbody>();
      parachuteRB = Parachute.GetComponent<Rigidbody>();
   }
   private void Update()
   {

      Checker();
      

   }
   public void Checker()
   {
      if(Input.GetKeyDown(KeyCode.Space)){
         state = State.on;
         rocketRB.useGravity=true;
         compartmentRB.useGravity= true;
          audio.Play();
          fire.Play();

      }

      if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 100f, layerMask))
      {
         Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
         OpenParachute();
      }

      if (fuel > 0 && state == State.on)
      {

         Move(new Vector3(windForce, 0, speed));


      }
      else
      {
         fire.Stop();
         audio.Stop();
         if (rocketRB.velocity.y < 0)
         {
            ReleaseCompartment();

            TorqueRotate();

         }
      }

   }


   void ReleaseCompartment()
   {
       
      compartmentRB.isKinematic = false;
      compartmentRB.AddForce(Vector3.right);

   }
   void Move(Vector3 dir)
   {
      Vector3 globalDirection = transform.TransformDirection(dir);

      rocketRB.AddForce(globalDirection);

      RemoveFuel(1f);
   }
   void RemoveFuel(float amount)
   {
      if (fuel > 0)
      {
         fuel -= amount * Time.deltaTime;
      }

   }
   void OpenParachute()
   {
      Parachute.SetActive(true);
     
      parachuteRB.drag = 20;


   }
   void TorqueRotate()
   {

      // Calcule a direção do vetor para baixo
      Vector3 downDirection = -Vector3.up;
      // Calcule a direção do vetor da frente do foguete
      Vector3 forwardDirection = transform.forward;

      // Calcule a direção da rotação como a cruz entre os vetores para baixo e para frente
      Vector3 rotationDirection = Vector3.Cross(forwardDirection, downDirection);

      // Calcule o ângulo de rotação entre o vetor para baixo e o vetor da frente
      float rotationAngle = Vector3.Angle(forwardDirection, downDirection);

      // Limitar o ângulo de rotação ao máximo definido
      rotationAngle = Mathf.Clamp(rotationAngle, 0f, maxRotationAngle);

      // Aplique torque ao foguete para girar em torno do eixo de rotação calculado
      rocketRB.AddTorque(rotationDirection * rotationAngle * rotationSpeed * Time.fixedDeltaTime);
   }



}
