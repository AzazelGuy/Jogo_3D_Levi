using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip ("Velocidade do Jogador")]
    [SerializeField] private float movementSpeed= 30f;
    [Tooltip("Velocidade do pulo do Jogador")]
    [SerializeField] private float jumpStrenght = 20f;
    [Tooltip("Velocidade da rotação do Jogador")]
    [SerializeField] private float rotationSpeed = 5f;
    [Tooltip("Camada referente do Chão")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Referencia do Modelo")]
    [SerializeField] private Transform playerModel;

    private bool isGrounded = false;
    private bool wantJump = false;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;

    private Vector2 InputDir;
    private Vector3 dir;
    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (coll == null) coll = GetComponent<Collider>();
    }

    private void Update()
    {
        CheckGround(); //Fica conferindo se o jogador está no chão
        //Debug.Log(isGrounded); //Debug de teste
        InputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //Pega os DIrecionais do "Analogicos" X e Y

        dir = Camera.main.transform.forward * InputDir.y + Camera.main.transform.right * InputDir.x; //Calcula a direção do movimento

        if (dir != Vector3.zero) {
            Quaternion targetRotation =  Quaternion.LookRotation(dir); //Angulo desejado
        playerModel.rotation = Quaternion.Slerp(
            playerModel.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        ); //Isso aqui tudo rotaciona o modelo na direção que queremos de forma suave (e linda)
        }

        if (Input.GetButtonDown("Jump"))
        {
            wantJump = true;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(
            dir.x * movementSpeed,
            rb.linearVelocity.y,
            dir.z * movementSpeed
        );

        if (wantJump)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            //Debug.Log("Pulou"); //Era debug de test
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); //Reseta a velocidade de queda
            rb.AddForce(Vector3.up * jumpStrenght, ForceMode.Impulse);
        }
        wantJump = false;
    }
    void CheckGround() //Lógica de conferir se estão no chão via CheckSphere (Que confer ENQUATO estiver colidindo)
    {
        Vector3 checkPosition = transform.position + Vector3.down * 0.1f;

        isGrounded = Physics.CheckSphere(
            checkPosition,
            0.25f,
            groundLayer,
            QueryTriggerInteraction.Ignore //Ignora o propio
        );
    }

}
