using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementCharController : MonoBehaviour
{
    private Vector2 _input;
    private CharacterController _characterController;

    [Header("Camera Manager Reference")]
    [SerializeField] private CameraTargetManager cameraManager;

    [Header("Model & Rotation")]
    [Tooltip("Referencia do Modelo 3D (Corpo/Malha)")]
    [SerializeField] private Transform playerModel;
    [Tooltip("Velocidade da rotação do Jogador na 3ª pessoa")]
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float runSpeed = 10f;
    private float curSpeed;

    [Header("Stats")]
    [SerializeField] private float Stamina = 100f;

    [Header("Jump & Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.2f;

    [Header("Slope Limits")]
    [SerializeField] private float rayDistance = 1.5f;

    private Vector3 _direction;
    private Vector3 _velocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInput();
        ApplyGravityAndJump();
        MoveAndRotate();
        UI_HUD.Instance.SetSprint(Stamina);
    }

    private void HandleInput()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Transform refTransform = (cameraManager != null && cameraManager.CameraTarget != null)
            ? cameraManager.CameraTarget
            : Camera.main.transform;

        Vector3 camForward = refTransform.forward;
        Vector3 camRight = refTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        _direction = (camForward * _input.y + camRight * _input.x).normalized;

        // Ajuste no Lerp para acelerar e desacelerar corretamente
        float targetSpeed = speed;
        if (_characterController.isGrounded){
        if (Input.GetKey(KeyCode.LeftShift) && _direction.magnitude > 0.0001f)
        {
            if (Stamina > 0f)
            {
                targetSpeed = runSpeed;
                Stamina -= Time.deltaTime * 15.5f;
            }
            else
            {
                targetSpeed = speed;
            }
        }else
        {
            targetSpeed = speed;
            Stamina += Time.deltaTime * 9.5f;
        }
            Stamina = Mathf.Clamp(Stamina, 0f, 100f);
        }
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 10f * Time.deltaTime);
    }

    private void ApplyGravityAndJump()
    {
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            // Força constante para manter o jogador colado no chão ao descer
            _velocity.y = -5f;
        }

        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplica a gravidade continuamente
        _velocity.y += gravity * Time.deltaTime;
    }

    private void MoveAndRotate()
    {
        // Projeta o vetor de movimento ao longo da inclinação da rampa
        Vector3 moveDirection = AdjustVelocityToSlope(_direction * curSpeed);

        // Junta movimento horizontal/rampa com a velocidade Y em uma única chamada
        Vector3 finalMove = moveDirection + new Vector3(0f, _velocity.y, 0f);
        _characterController.Move(finalMove * Time.deltaTime);

        // Rotação da câmera / modelo
        if (cameraManager != null && cameraManager.IsFirstPerson)
        {
            if (cameraManager.CameraTarget != null)
            {
                float targetYaw = cameraManager.CameraTarget.eulerAngles.y;
                playerModel.rotation = Quaternion.Euler(0f, targetYaw, 0f);
            }
        }
        else
        {
            if (_direction.magnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_direction);
                targetRotation.z = 0f;
                targetRotation.x = 0f;

                playerModel.rotation = Quaternion.Slerp(
                    playerModel.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    // Função para alinhar a direção com a normal do chão
    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Vector3 adjustedVelocity = slopeRotation * velocity;

            // Retorna o movimento inclinado apenas se o jogador estiver se movendo para baixo
            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }

    public bool Grounded => _characterController.isGrounded;
}