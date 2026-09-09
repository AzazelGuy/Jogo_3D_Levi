using UnityEngine;

//Código Usado para Estudo
public class MovePlayerVIni : MonoBehaviour
{
    [Header("Movimento")]

    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float forcaPulo = 6f;

    private Rigidbody rb;
    private bool noChao;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && noChao)
        {
            rb.AddForce(Vector3.up * forcaPulo, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Movement(h, v);
    }

    private void Movement(float h, float v)
    {
        Vector3 dir = new Vector3(h, 0f, v) * velocidade;
        dir.y = rb.linearVelocity.y;

        rb.linearVelocity = dir;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Chao"))
        {
            noChao = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        noChao = false;
    }
}
