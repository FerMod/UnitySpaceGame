using SpaceGame;
using SpaceGame.Network;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneReferenceSearcher : NetworkBehaviour
{

    public PlaneNet planeNet;
    public PlayerInputControl playerInputControl;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsLocalPlayer) return;
        var mouseFlightController = FindMouseFlightController();
        mouseFlightController.Plane = planeNet;
        playerInputControl.Controller = mouseFlightController;

        var playerHudGameObject = FindPlayerHudGameObject();
        playerHudGameObject.GetComponent<Hud>().MouseFlight = mouseFlightController;
        playerHudGameObject.GetComponent<PlaneUIHandler>().Plane = planeNet;

        GetComponent<PlayerInput>().enabled = true;
    }

    private MouseFlightController FindMouseFlightController()
    {
        var cameraRig = GameObject.FindWithTag("CameraRig");
        if (cameraRig == null)
        {
            Debug.LogError($"{name} {GetType().Name} - No camera rig found!");
            return default;
        }

        if (!cameraRig.TryGetComponent<MouseFlightController>(out var controller))
        {
            Debug.LogError($"{name} {GetType().Name} - No mouse flight controller found!");
            return default;
        }

        return controller;
    }

    private GameObject FindPlayerHudGameObject()
    {
        var gameObject = GameObject.FindWithTag("PlayerHud");
        if (gameObject == null)
        {
            Debug.LogError($"{name} {GetType().Name} - No HUD GameObject found!");
            return default;
        }
        return gameObject;
    }

}
