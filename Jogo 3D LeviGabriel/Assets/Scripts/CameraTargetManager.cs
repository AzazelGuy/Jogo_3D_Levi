using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering; // Necessário para usar ShadowCastingMode


public class CameraTargetManager : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;

    [Header("Targets & Sensitivity")]
    [SerializeField] private Transform cameraTarget;       // Objeto na altura dos olhos
    [SerializeField] private Transform playerBody;         // Transform do Jogador (CharacterController)
    [SerializeField] private GameObject playerModelParent;  // Objeto Pai do Modelo
    [SerializeField] private SkinnedMeshRenderer[] Models;
    [SerializeField] private float sensitivity = 2f;

    [Header("Keybindings")]
    [SerializeField] private KeyCode switchCamKey = KeyCode.V;

    [Header("Settings")]
    [SerializeField] private float modelToggleDelay = 0.25f;

    private float pitch;
    private float yaw;
    private bool isFirstPerson = false;
    private Coroutine modelToggleCoroutine;

    public bool IsFirstPerson => isFirstPerson;
    public Transform CameraTarget => cameraTarget; // Referência pública limpa para o movimento usar


    private void UpdateModelShadows(bool isFirstPerson)
    {
        if (playerModelParent != null)
        {
            if (isFirstPerson)
            {
                // O modelo fica invisível para a câmera, mas CONTINUA projetando sombra no chão
                foreach(SkinnedMeshRenderer m in Models)
                {
                    m.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
                
            }
            else
            {
                // O modelo volta a ser visível normalmente na 3ª pessoa
                foreach (SkinnedMeshRenderer m in Models)
                {
                    m.shadowCastingMode = ShadowCastingMode.On;
                }
            }
        }
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Pega o ângulo inicial diretamente do corpo do jogador
        yaw = playerBody.eulerAngles.y;
        pitch = 0f;

        UpdateCameraMode();
    }

    private void Update()
    {
        if (Input.GetKeyDown(switchCamKey))
        {
            isFirstPerson = !isFirstPerson;
            UpdateCameraMode();
        }

        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        yaw += mouseX;

        // Aplica a rotação calculada no cameraTarget
        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);

        if (isFirstPerson)
        {
            // Na 1ª pessoa, o corpo também precisa estar rigidamente alinhado no eixo Y
            playerBody.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }

    private void UpdateCameraMode()
    {
        // Garante que o Yaw e Pitch fiquem perfeitamente sincronizados
        Vector3 currentAngles = cameraTarget.eulerAngles;
        pitch = NormalizeAngle(currentAngles.x);
        yaw = currentAngles.y;

        if (isFirstPerson)
        {
            firstPersonCam.Priority = 20;
            thirdPersonCam.Priority = 10;
        }
        else
        {
            firstPersonCam.Priority = 10;
            thirdPersonCam.Priority = 20;

            // Ao voltar para a 3ª Pessoa, ajustamos a rotação do corpo imediatamente para a frente da visão
            playerBody.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
        

        if (playerModelParent != null)
        {
            UpdateModelShadows(isFirstPerson);
        }
    }

    private IEnumerator ToggleModelVisibilityWithDelay(bool hideModel)
    {
        if (!hideModel)
        {
            playerModelParent.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(modelToggleDelay);
            playerModelParent.SetActive(false);
        }
    }

    // Auxiliar para corrigir a leitura de ângulos negativos da Unity (ex: -10° virando 350°)
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }
}