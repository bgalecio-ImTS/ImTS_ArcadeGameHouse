using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMeasurement : MonoBehaviour
{
    public static BodyMeasurement Instance { private set; get; }

    [SerializeField] private Transform vrCameraPoint = null;
    [SerializeField] private Transform vrLeftHandPoint = null;
    [SerializeField] private Transform vrRightHandPoint = null;
    [SerializeField] private CharacterCustomization characterCustomization = null;
    [SerializeField] private VRRig vrRig = null;
    [SerializeField] private VRFootIK vrFootIK = null;

    public Transform VrCameraPoint { get => vrCameraPoint; }
    public Transform VrLeftHandPoint { get => vrLeftHandPoint; }
    public Transform VrRightHandPoint { get => vrRightHandPoint; }
    public CharacterCustomization CharacterCustomization { get => characterCustomization; }
    public VRRig VrRig { get => vrRig; }
    public VRFootIK VrFootIK { get => vrFootIK; }

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
}