using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnatomyCaptureManager : MonoBehaviour
{
    #region Singleton

    public static AnatomyCaptureManager Instance { private set; get; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion

    #region Paramaters

    [Header("Start")]
    [SerializeField] private GameObject pnlStart = null;
    [SerializeField] private Button btnStart = null;

    [Header("Leg Selection")]
    [SerializeField] private GameObject pnlLegSelection = null;
    [SerializeField] private AudioClip legSelectionClip = null;
    [SerializeField] private Button btnLeftLegSelect = null;
    [SerializeField] private Button btnRightLegSelect = null;
    private int legSelected = 0;

    [Header("Instruction")]
    [SerializeField] private GameObject pnlInstruction = null;
    [SerializeField] private Image imgMessageImage = null;
    [SerializeField] private TextMeshProUGUI txtMessage = null;
    [SerializeField] private string captureHeightMessage = "";
    [SerializeField] private AudioClip tPoseInstructionClip = null;
    private Sprite imgCaptureHeight = null;

    [Header("Body Measurement")]
    [SerializeField] private GameObject pnlBodyMeasurement = null;
    [SerializeField] private Slider progressBar = null;
    [SerializeField] private AudioClip tPoseScanningProgressClip = null;
    [SerializeField] private AudioClip tPoseScanningCompleteClip = null;
    [SerializeField] private float captureSensitivity = 0.01f;
    [SerializeField] private float captureProgressSentivity = 10f;
    private Transform vRCameraPoint = null;
    private Transform vRLeftHandPoint = null;
    private Transform vRRightHandPoint = null;
    private DictionaryEvent onUpdateAnatomy = new DictionaryEvent();

    private AnatomyCapturePanel currentPanel = AnatomyCapturePanel.Start;
    private Dictionary<AnatomyCapturePanel, GameObject> panels = new Dictionary<AnatomyCapturePanel, GameObject>();

    #endregion

    #region Encapsulations

    public GameObject PnlStart { get => pnlStart; }
    public int LegSelected { get => legSelected; set => legSelected = value; }

    #endregion

    #region Start

    private void Start()
    {
        vRCameraPoint = CharacterManager.Instance.VRCamera;
        vRLeftHandPoint = CharacterManager.Instance.VRLeftHand;
        vRRightHandPoint = CharacterManager.Instance.VRRightHand;

        panels.Add(AnatomyCapturePanel.Start, PnlStart);
        panels.Add(AnatomyCapturePanel.LegSelection, pnlLegSelection);
        panels.Add(AnatomyCapturePanel.Instruction, pnlInstruction);
        panels.Add(AnatomyCapturePanel.BodyMeasurement, pnlBodyMeasurement);

        btnStart.onClick.AddListener(() => 
        {
            Transition(AnatomyCapturePanel.LegSelection);
            AssistantBehavior.Instance.Speak(legSelectionClip);
            AssistantBehavior.Instance.transform.DOLookAt(vRCameraPoint.position, 1f, AxisConstraint.Y, Vector3.up);
        });

        btnLeftLegSelect.onClick.AddListener(() => 
        { 
            HandleOnLegSelect(0);
            CharacterManager.Instance.PointersVisibility(false);
        });

        btnRightLegSelect.onClick.AddListener(() => 
        { 
            HandleOnLegSelect(1);
            CharacterManager.Instance.PointersVisibility(false);
        });
    }

    #endregion

    #region Body Measurement

    private IEnumerator BodyMeasurement(UnityAction OnEndAction)
    {
        yield return new WaitForSeconds(1f);
        Vector3 headPos = vRCameraPoint.position;
        Vector3 leftHandPos = vRLeftHandPoint.position;
        Vector3 rightHandPos = vRRightHandPoint.position;
        float progress = 0f;
        VoiceOverManager.Instance.PlayClip(tPoseScanningProgressClip);

        while (progress < 100f)
        {
            Vector3 headMagnitude = vRCameraPoint.position - headPos;
            Vector3 leftHandMagnitude = vRLeftHandPoint.position - leftHandPos;
            Vector3 rightHandMagnitude = vRRightHandPoint.position - rightHandPos;
            headPos = vRCameraPoint.position;
            leftHandPos = vRLeftHandPoint.position;
            rightHandPos = vRRightHandPoint.position;

            if (headMagnitude.magnitude < captureSensitivity && leftHandMagnitude.magnitude < captureSensitivity && rightHandMagnitude.magnitude < captureSensitivity)
            {
                progress += Time.deltaTime * captureProgressSentivity;
            }
            else
            {
                progress = 0f;
            }

            progressBar.value = progress / 100;
            yield return new WaitForEndOfFrame();
        }

        OnEndAction.Invoke();
    }

    private void AdjustHeight()
    {
        CharacterManager.Instance.CharacterSuit.SetActive(true);
        CharacterManager.Instance.SuitCustomization.SetHeight();
    }

    #endregion

    #region Leg Selection

    private void HandleOnLegSelect(int leg)
    {
        LegSelected = leg;
        Transition(AnatomyCapturePanel.Instruction);

        VoiceOverManager.Instance.HandleGame1AudioClip(leg);

        ShowMessage(captureHeightMessage, imgCaptureHeight, 5f, () =>
        {
            Transition(AnatomyCapturePanel.BodyMeasurement);
            StartCoroutine(BodyMeasurement(() =>
            {
                VoiceOverManager.Instance.PlayClip(tPoseScanningCompleteClip);
                Dictionary<string, Vector3> currentAnatomy = new Dictionary<string, Vector3>();
                currentAnatomy.Add(AnatomyPart.Head.ToString(), vRCameraPoint.position);
                currentAnatomy.Add(AnatomyPart.LeftHand.ToString(), vRLeftHandPoint.localPosition);
                currentAnatomy.Add(AnatomyPart.RightHand.ToString(), vRRightHandPoint.localPosition);
                AdjustHeight();
                onUpdateAnatomy.Invoke(currentAnatomy);
                CharacterManager.Instance.CurrentAnatomy = currentAnatomy;
                VoiceOverManager.Instance.ButtonsInteraction(true);
                panels[currentPanel].SetActive(false);
                currentPanel = AnatomyCapturePanel.Start;

                UsabilityHelper.Instance.StopUsabilityTimer(UsabilityTimer.Lobby);
            }));
        });
    }

    #endregion

    #region Instruction

    private void ShowMessage(string message, Sprite image, float duration, UnityAction OnComplete)
    {
        if (image != null)
        {
            imgMessageImage.gameObject.SetActive(true);
            imgMessageImage.sprite = image;
            imgMessageImage.DOFade(1f, 0.5f);
            imgMessageImage.DOFade(0f, 0.5f).SetDelay(duration + 0.5f);
        }

        AssistantBehavior.Instance.Speak(tPoseInstructionClip);
        AssistantBehavior.Instance.Animator.SetBool("isBlinking", true);
        StartCoroutine(FunctionWithDelay(tPoseInstructionClip.length, () => AssistantBehavior.Instance.Animator.SetBool("isBlinking", false)));
        txtMessage.gameObject.SetActive(true);
        txtMessage.text = message;
        txtMessage.DOFade(1f, 0.5f).OnComplete(() =>
        {
            txtMessage.DOFade(0f, 0.5f).SetDelay(duration).OnComplete(() =>
            {
                imgMessageImage.gameObject.SetActive(false);
                txtMessage.gameObject.SetActive(false);
                OnComplete.Invoke();
            });
        });
    }

    #endregion

    #region Transition

    private void Transition(AnatomyCapturePanel to)
    {
        panels[currentPanel].SetActive(false);
        panels[to].SetActive(true);
        currentPanel = to;
    }

    #endregion

    #region Function with Delay

    private IEnumerator FunctionWithDelay(float waitTime, UnityAction function)
    {
        yield return new WaitForSeconds(waitTime);
        function.Invoke();
    }

    #endregion
}