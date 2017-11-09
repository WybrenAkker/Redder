using UnityEngine;
using UnityEditor;

//Editor script that debugs fire range of an airship's turrets.
[CustomEditor(typeof(AirshipAI))]
public class AirshipEditor : Editor
{
    void OnSceneGUI()
    {
        AirshipAI myScript = (AirshipAI)target;
        Handles.color = Color.white;

        foreach (AirshipAI.Turret turret in myScript.turrets) //Cycle through the airship's turrets
        {
            //Caclulate the addition to the fov of the current turret. (Turrets aren't always facing up)
            float angleAddition = Vector2.Angle(new Vector2(0, 1), turret.up); 
            Vector3 cross = Vector3.Cross(new Vector2(0, 1), turret.up); 

            if (cross.z > 0)
            {
                angleAddition = 360 - angleAddition;
            }

            //Caclulate the 2 extends of the turrets Field of View.
            Vector3 startPos = (myScript.DirFromAngle(angleAddition + turret.fov / 2, false) * turret.range);
            Vector3 endPos = (myScript.DirFromAngle(angleAddition - turret.fov / 2, false) * turret.range);

            //Draw a Arc between the 2 extends of the turrets Field of View
            Handles.DrawWireArc(turret.gun.position, new Vector3(0,0,-1), startPos, -turret.fov, turret.range);
        }

    }
}
