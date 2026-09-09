using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementCharController : MonoBehaviour
{
    private Vector2 _input;
    private CharacterController _characterController;

    [Tooltip("Referencia do Modelo")]
    [SerializeField] private Transform playerModel;
    [Tooltip("Velocidade da rotação do Jogador")]
    [SerializeField] private float rotationSpeed = 5f;

    private Vector3 _direction;

    [SerializeField] private float speed = 5;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }
    private void FixedUpdate()
    {
        if (_characterController != null)
        {
            _characterController.Move(_direction * speed * Time.deltaTime);
        }

        if (_direction.magnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_direction); //Angulo desejado
            targetRotation.z = 0f;
            targetRotation.x = 0f;
            playerModel.rotation = Quaternion.Slerp(
                playerModel.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            ); //Isso aqui tudo rotaciona o modelo na direção que queremos de forma suave (e linda)
        }
    }
    private void Move()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 camForward = Camera.main.transform.forward; //Posição "Para frente" da camera
        Vector3 camRight = Camera.main.transform.right; //Posição "De Lado" da camera

        //ZERA A PORRA DO Z para não termos problemas :3
        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();
        _direction = (camForward * _input.y + camRight * _input.x).normalized;
        Debug.Log($"MOVING in {_input}");
    }
}
