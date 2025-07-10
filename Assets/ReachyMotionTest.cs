using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Reachy;
using Reachy.Sdk.Joint;
using TMPro;

public class ReachyMotionTest : MonoBehaviour
{
    public ReachyController reachy;
    public TMP_InputField UserInputField;

    void Start()
    {
        Debug.Log("✅ ReachyMotionTest started! Use keys to control Reachy:");
        Debug.Log("🔹 Press R = Raise Right Arm");
        Debug.Log("🔹 Press L = Raise Left Arm");
        Debug.Log("🔹 Press H = Rotate Head");
        Debug.Log("🔹 Press W = Wave Right Hand");
        Debug.Log("🔹 Press S = Shake Head");
    }

    void Update()
    {
        if (UserInputField != null && UserInputField.isFocused)
            return;

        if (Input.GetKeyDown(KeyCode.R)) RaiseRightArm();
        if (Input.GetKeyDown(KeyCode.L)) RaiseLeftArm();
        if (Input.GetKeyDown(KeyCode.H)) RotateHead(0f, 30f, 30f);
        if (Input.GetKeyDown(KeyCode.W)) StartCoroutine(WaveRightHand());
        if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(ShakeHead());
    }

    // 👉 Public Methods callable from LLM integration
    public void RaiseRightArm()
    {
        Debug.Log("🙌 Raising right arm...");
        var rightArmCommand = new Dictionary<JointId, float>
        {
            { new JointId { Name = "r_shoulder_pitch" }, -90f },
            { new JointId { Name = "r_shoulder_roll" }, 45f },
            { new JointId { Name = "r_arm_yaw" }, 30f },
            { new JointId { Name = "r_elbow_pitch" }, -60f }
        };
        reachy.HandleCommand(rightArmCommand);
    }

    public void RaiseLeftArm()
    {
        Debug.Log("🙌 Raising left arm...");
        var leftArmCommand = new Dictionary<JointId, float>
        {
            { new JointId { Name = "l_shoulder_pitch" }, -90f },
            { new JointId { Name = "l_shoulder_roll" }, -45f },
            { new JointId { Name = "l_arm_yaw" }, -30f },
            { new JointId { Name = "l_elbow_pitch" }, -60f }
        };
        reachy.HandleCommand(leftArmCommand);
    }

    public void RotateHead(float roll, float pitch, float yaw)
    {
        Debug.Log("🧠 Rotating head...");
        var headCommand = new Dictionary<JointId, float>
        {
            { new JointId { Name = "neck_roll" }, roll },
            { new JointId { Name = "neck_pitch" }, pitch },
            { new JointId { Name = "neck_yaw" }, yaw }
        };
        reachy.HandleCommand(headCommand);
    }

    public IEnumerator WaveRightHand()
    {
        Debug.Log("👋 Waving right hand...");

        // Set base pose
        reachy.HandleCommand(new Dictionary<JointId, float>
        {
            { new JointId { Name = "r_shoulder_pitch" }, -70f },
            { new JointId { Name = "r_shoulder_roll" }, 40f },
            { new JointId { Name = "r_elbow_pitch" }, -40f }
        });

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < 3; i++)
        {
            reachy.HandleCommand(new Dictionary<JointId, float>
            {
                { new JointId { Name = "r_wrist_pitch" }, 40f }
            });
            yield return new WaitForSeconds(0.3f);

            reachy.HandleCommand(new Dictionary<JointId, float>
            {
                { new JointId { Name = "r_wrist_pitch" }, -40f }
            });
            yield return new WaitForSeconds(0.3f);
        }

        reachy.HandleCommand(new Dictionary<JointId, float>
        {
            { new JointId { Name = "r_wrist_pitch" }, 0f }
        });
    }

    public IEnumerator ShakeHead()
    {
        Debug.Log("🙅 Shaking head...");
        for (int i = 0; i < 2; i++)
        {
            reachy.HandleCommand(new Dictionary<JointId, float>
            {
                { new JointId { Name = "neck_yaw" }, 30f }
            });
            yield return new WaitForSeconds(0.4f);

            reachy.HandleCommand(new Dictionary<JointId, float>
            {
                { new JointId { Name = "neck_yaw" }, -30f }
            });
            yield return new WaitForSeconds(0.4f);
        }

        reachy.HandleCommand(new Dictionary<JointId, float>
        {
            { new JointId { Name = "neck_yaw" }, 0f }
        });
    }

    // 🔁 Optional shortcut for LLM to call head left turn
    public void RotateHeadLeft()
    {
        RotateHead(0f, 0f, 30f); // yaw = +30
    }

    public void RotateHeadRight()
    {
        RotateHead(0f, 0f, -30f); // yaw = -30
    }
}