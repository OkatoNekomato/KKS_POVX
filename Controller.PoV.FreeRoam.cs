﻿using ActionGame.Chara;
using Manager;
using UnityEngine;

namespace KK_PovX
{
    public static partial class Controller
    {
        public static void SetRotationFreeRoamPoV()
        {
            
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogWarning("Player object not found.");
                return;
            }

            Player player = playerObject.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogWarning("Player component not found on the player object.");
                return;
            }

            Vector3 nextPosition = player.transform.position;

            if (!KK_PovX.RotateHeadFirst.Value || nextPosition != prevPosition)
            {
                
                bodyAngle = cameraAngleY;
                bodyQuaternion = Quaternion.Euler(0f, bodyAngle, 0f);
            }
            else
            {
                // Rotate head first. If head rotation is at the limit, rotate body.
                float angle = Tools.GetClosestAngle(bodyAngle, cameraAngleY, out bool clockwise);
                float max = KK_PovX.HeadMax.Value;

                if (angle > max)
                {
                    if (clockwise)
                        bodyAngle = Tools.Mod2(bodyAngle + angle - max, 360f);
                    else
                        bodyAngle = Tools.Mod2(bodyAngle - angle + max, 360f);

                    bodyQuaternion = Quaternion.Euler(0f, bodyAngle, 0f);
                }
            }

            prevPosition = nextPosition;

            Transform neck = chaCtrl.neckLookCtrl.neckLookScript.aBones[0].neckBone;
            Vector3 neck_euler = neck.eulerAngles;

            SetRotation(Quaternion.Euler(cameraAngleOffsetX, cameraAngleY, 0f));

            player.transform.rotation = bodyQuaternion;
            neck.rotation = Quaternion.Euler(
                Tools.AngleClamp(
                    Tools.Mod2(neck_euler.x + cameraAngleOffsetX, 360f),
                    Tools.Mod2(neck_euler.x + KK_PovX.NeckMin.Value, 360f),
                    Tools.Mod2(neck_euler.x + KK_PovX.NeckMax.Value, 360f)
                ),
                cameraAngleY,
                neck_euler.z
            );
        }

        
        public static void CameraPoVFreeRoam()
        {
            if (inScenePoV)
            {
                inScenePoV = false;

                // Найти объект игрока через тег
                GameObject playerObject = GameObject.FindWithTag("Player");
                if (playerObject == null)
                {
                    Debug.LogWarning("Player object not found.");
                    return;
                }

                Player player = playerObject.GetComponent<Player>();
                if (player == null)
                {
                    Debug.LogWarning("Player component not found on the player object.");
                    return;
                }

                if (chaCtrl != player.chaCtrl)
                {
                    SetChaControl(GetChaControl());

                    if (!PoVToggled)
                        return;
                }
            }

            SetFoV();
            SetRotationFreeRoamPoV();
            SetPositionPoV();
        }
    }
}
