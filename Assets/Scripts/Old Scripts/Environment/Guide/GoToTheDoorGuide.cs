using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Door Guide", menuName = "Guide/Door Guide", order = 0)]

public class GoToTheDoorGuide : Guide
{
    [SerializeField] private List<EnvironmentPoints> pointsToRender = new List<EnvironmentPoints>();
    [SerializeField] private EnvironmentPoints pointToGo = EnvironmentPoints.AvatarRoomMainCenter;

    [Space]
    [SerializeField] private bool showMessage = false;
    [SerializeField] private string message = "";
    [SerializeField] private Vector3 direction = Vector3.zero;

    [Space]
    [SerializeField] private AudioClip announcement = null;

    public override void ShowGuide(UnityAction OnEndGuide)
    {
        EnvironmentGuideManager.Instance.StartCoroutine(DelayCour(() =>
        {
            List<Vector3> points = new List<Vector3>();
            foreach (var point in pointsToRender)
            {
                points.Add(Environment.Instance.PointsDictionary[point].point.TransformPoint(Vector3.zero));
            }

            EnvironmentGuideManager.Instance.RenderLine(true, points.ToArray());
            EnvironmentGuideManager.Instance.StartCoroutine(GuideCour(OnEndGuide));
            Environment.Instance.DoorDictionary[pointsToRender.Last()].HightLightThisDoor(true);

            if (showMessage)
            {
                PlayerHUD.Instance.ShowMessage(direction, message, showMessage);
            }

            if (announcement != null)
            {
                UserInteraction.Instance.AnnounceToPlayer(announcement, true);
            }

            base.ShowGuide(OnEndGuide);
        }));
    }

    private IEnumerator GuideCour(UnityAction OnEnd)
    {
        while (UserInteraction.Instance.CurrentPoint != pointToGo)
        {
            yield return new WaitForEndOfFrame();
        }

        if(Environment.Instance.DoorDictionary[pointsToRender.Last()] != null)
        {
            Environment.Instance.DoorDictionary[pointsToRender.Last()].HightLightThisDoor(false);
        }

        if (showMessage)
        {
            PlayerHUD.Instance.ShowMessage(direction, message, false);
        }

        OnEnd.Invoke();
    }

    public override void UnShowGuide()
    {
        EnvironmentGuideManager.Instance.RenderLine(false);

        if (announcement != null)
        {
            UserInteraction.Instance.AnnounceToPlayer(announcement, false);
        }
    }

    public override bool isGuideAcomplish()
    {
        return UserInteraction.Instance.CurrentPoint == pointToGo;
    }
}
