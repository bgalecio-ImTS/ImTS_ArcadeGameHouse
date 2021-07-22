using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBodyPartCustomization : MonoBehaviour
{    public static UIBodyPartCustomization Instance { private set; get; }

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

    [SerializeField] private TextMeshProUGUI txtLabel = null;
    [SerializeField] private List<UIBodyPartCustom> bodyPartsCustom = new List<UIBodyPartCustom>();

    public void OpenBodyPartSelections(CharacterCustomization character, CharacterCustomization characterMimic, BodyPartID id)
    {
        txtLabel.text = id.ToString();

        BodyPart bodyPart = character.bodyParts.Where(b => b.id == id).FirstOrDefault();
        BodyPart bodyPartMimic = characterMimic.bodyParts.Where(b => b.id == id).FirstOrDefault();

        foreach (var ui in bodyPartsCustom)
        {
            ui.gameObject.SetActive(false);
        }

        for (int i = 0; i < bodyPart.bodyPartMaterials.Count; i++)
        {
            bodyPartsCustom[i].imgBodyPartPreview.sprite = bodyPart.bodyPartSprites[i];
            bodyPartsCustom[i].btnBodyPartCustom.onClick.RemoveAllListeners();
            Material newMaterial = bodyPart.bodyPartMaterials[i];
            bodyPartsCustom[i].btnBodyPartCustom.onClick.AddListener(() => 
            { 
                bodyPart.ChangeMaterial(newMaterial);
                bodyPartMimic.ChangeMaterial(newMaterial);
            });
            bodyPartsCustom[i].gameObject.SetActive(true);
        }
    }
}
