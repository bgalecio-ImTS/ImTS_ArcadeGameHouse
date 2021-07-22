using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBodyPartSelection : MonoBehaviour
{
    public static UIBodyPartSelection Instance { private set; get; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] private CharacterCustomization character = null;
    [SerializeField] private CharacterCustomization characterMimic = null;
    [SerializeField] private UIBodyPart bodyPartPrefab = null;
    [SerializeField] private Transform bodyPartVertical1 = null;
    [SerializeField] private Transform bodyPartVertical2 = null;
    [SerializeField] private Button btnAdjustHeight = null;

    private void Start()
    {
        List<BodyPart> vertical1 = new List<BodyPart>();
        List<BodyPart> vertical2 = new List<BodyPart>();

        for (int i = 0; i < character.bodyParts.Count; i++)
        {
            if (i < 4)
            {
                vertical1.Add(character.bodyParts[i]);
            }
            else
            {
                vertical2.Add(character.bodyParts[i]);
            }
        }

        foreach (var bodyPart1 in vertical1)
        {
            GameObject clone = Instantiate(bodyPartPrefab.gameObject, bodyPartVertical1);
            UIBodyPart uIBodyPart = clone.GetComponent<UIBodyPart>();
            uIBodyPart.txtLabel.text = bodyPart1.id.ToString();
            uIBodyPart.btnBodyPart.onClick.AddListener(() => {
                BodyPartID id = bodyPart1.id;
                HandleOnSelectBodyPart(id);
            });

            clone.name = bodyPart1.id.ToString();
            clone.SetActive(true);
        }

        foreach (var bodyPart2 in vertical2)
        {
            GameObject clone = Instantiate(bodyPartPrefab.gameObject, bodyPartVertical2);
            UIBodyPart uIBodyPart = clone.GetComponent<UIBodyPart>();
            uIBodyPart.txtLabel.text = bodyPart2.id.ToString();
            uIBodyPart.btnBodyPart.onClick.AddListener(() => {
                BodyPartID id = bodyPart2.id;
                HandleOnSelectBodyPart(id);
            });

            clone.name = bodyPart2.id.ToString();
            clone.SetActive(true);
        }


        btnAdjustHeight.onClick.AddListener(HandleOnAdjustHeight);

        UIBodyPartCustomization.Instance.OpenBodyPartSelections(character, characterMimic, BodyPartID.HELMET);
    }

    private void HandleOnSelectBodyPart(BodyPartID id)
    {
        UIBodyPartCustomization.Instance.OpenBodyPartSelections(character, characterMimic, id);
    }

    private void HandleOnAdjustHeight()
    {
        character.SetHeight();

        if (!characterMimic.gameObject.activeSelf)
        {
            characterMimic.transform.localScale = character.transform.localScale;
            characterMimic.gameObject.SetActive(true);
        }
    }



}


