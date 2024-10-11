//
// Copyright (c) Brian Hernandez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using Unity.Cinemachine;
using System;
using UnityEngine;

namespace SpaceGame
{
    public class Hud : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MouseFlightController mouseFlight = null;

        [Header("HUD Elements")]
        [SerializeField] private RectTransform boresight = null;
        [SerializeField] private RectTransform mousePos = null;

        //private Camera playerCam = null;
        private CinemachineCamera playerCam = null;

        private void Awake()
        {
            if (mouseFlight == null)
                Debug.LogError(name + ": Hud - Mouse Flight Controller not assigned!");

            playerCam = mouseFlight.GetComponentInChildren<CinemachineCamera>();
            CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);

            if (playerCam == null)
            {
                Debug.LogError(name + ": Hud - No camera found on assigned Mouse Flight Controller!");
            }
        }

        private void OnCameraUpdated(CinemachineBrain cinemachineBrain)
        {
            UpdateGraphics(mouseFlight, cinemachineBrain.OutputCamera);
        }

        private void Update()
        {
            //if (mouseFlight == null || playerCam == null)
            //    return;

            //UpdateGraphics(mouseFlight, playerCam);
        }

        private void UpdateGraphics(MouseFlightController controller, Camera camera)
        {
            if (mouseFlight == null || camera == null) return;

            if (boresight != null)
            {
                boresight.position = camera.WorldToScreenPoint(controller.BoresightPos);
                boresight.gameObject.SetActive(boresight.position.z > 1f);
            }

            if (mousePos != null)
            {
                mousePos.position = camera.WorldToScreenPoint(controller.MouseAimPos);
                mousePos.gameObject.SetActive(mousePos.position.z > 1f);
            }
        }

        public void SetReferenceMouseFlight(MouseFlightController controller)
        {
            mouseFlight = controller;
        }
    }
}
