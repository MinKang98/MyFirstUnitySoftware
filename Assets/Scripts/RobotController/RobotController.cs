using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static RobotController;
using System;
using UnityEngine.Rendering;
using System.Collections;

/// <summary>
/// 로봇 3D object를 RobotController의 버튼, InputField의 값으로 움직인다.
/// Teach버튼을 누르면 각 AXS의 값이 Step으로 저장된다.
/// SingleCycle, Cycle, Stop, E-Stop 버튼을 누르면 로봇이 동작한다.
/// 필요속성 : 로봇의 모터 회전 속도(0~100), Duration, Min Angle, Max Angle
///             step(speed, duration, suction, angles)
/// </summary>

public class RobotController : MonoBehaviour
{
    [Serializable]
    public class Step
    {
        public int stepNumber;
        public float speed = 100;
        public float duration;
        public bool isSuctionOn;

        public float angleAxis1;
        public float minAngleAxis1;
        public float maxAngleAxis1;

        public float angleAxis2;
        public float minAngleAxis2;
        public float maxAngleAxis2;

        public float angleAxis3;
        public float minAngleAxis3;
        public float maxAngleAxis3;

        public float angleAxis4;
        public float minAngleAxis4;
        public float maxAngleAxis4;

        public float angleAxis5;
        public float minAngleAxis5;
        public float maxAngleAxis5;

        public float angleAxis6;
        public float minAngleAxis6;
        public float maxAngleAxis6;

        public Step (int _stepNumber, float _speed, float _duration, bool _isSuctionOn)
        {
            stepNumber = _stepNumber;
            speed = _speed;
            duration = _duration;
            isSuctionOn = _isSuctionOn;
        }
    }
    public List<Step> steps = new List<Step>();

    [Header("Axis Pivots")]
    [SerializeField] Transform motorAxis1;
    [SerializeField] Transform motorAxis2;
    [SerializeField] Transform motorAxis3;
    [SerializeField] Transform motorAxis4;
    [SerializeField] Transform motorAxis5;

    [Header("UI정리")]
    [SerializeField] TMP_Text nowStepInfoTxt;
    [SerializeField] int totalSteps;
    [SerializeField] int currentStepNumber;
    [SerializeField] TMP_InputField stepInput;
    [SerializeField] TMP_InputField speedInput;
    [SerializeField] TMP_InputField durationInput;
    [SerializeField] Toggle suctionToggle;
    [SerializeField] TMP_InputField angleAxis1Input;
    [SerializeField] TMP_InputField angleAxis2Input;
    [SerializeField] TMP_InputField angleAxis3Input;
    [SerializeField] TMP_InputField angleAxis4Input;
    [SerializeField] TMP_InputField angleAxis5Input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nowStepInfoTxt.text = $"Total step count: {totalSteps} / Current step: {currentStepNumber}";

        stepInput.text = "0";
        speedInput.text = "100";
        durationInput.text = "0";
        suctionToggle.isOn = false;

        angleAxis1Input.text = "0";
        angleAxis2Input.text = "0";
        angleAxis3Input.text = "0";
        angleAxis4Input.text = "0";
        angleAxis5Input.text = "0";
    }

    // Teach버튼을 누르면 각 Axis의 값이 Step으로 저장된다.
    public void OnteachBtnClkEvent()
    {
/*        int stepNumber;
        bool isCorrect = int.TryParse(stepInput.text, out stepNumber);
        if (!isCorrect)
        {
            print("올바른 값을 입력해주세요.");
            return;
        }*/

        float speed;
        bool isCorrect = float.TryParse(speedInput.text, out speed);
        if (!isCorrect)
        {
            print("올바른 값을 입력해주세요.");
            return;
        }

        float duration;
        isCorrect = float.TryParse(durationInput.text, out duration);
        if (!isCorrect)
        {
            print("올바른 값을 입력해주세요.");
            return;
        }

        float angleAxis1;
        isCorrect = float.TryParse(angleAxis1Input.text, out angleAxis1);
        if (!isCorrect)
        {
            print("올바른 값을 입력해주세요.");
            return;
        }

        float angleAxis2;
        isCorrect = float.TryParse(angleAxis2Input.text, out angleAxis2);
        if (!isCorrect)
        {
            print("올바른 값을 입력해주세요.");
            return;
        }

        float angleAxis3;
        isCorrect = float.TryParse(angleAxis3Input.text, out angleAxis3);
        if (!isCorrect)
        {
            print("올바른 값을 입력해주세요.");
            return;
        }

        float angleAxis4;
        isCorrect = float.TryParse(angleAxis4Input.text, out angleAxis4);
        if (!isCorrect)
        {
            print("올바른 값을 입력해주세요.");
            return;
        }

        float angleAxis5;
        isCorrect = float.TryParse(angleAxis5Input.text, out angleAxis5);
        if (!isCorrect)
        {
            print("올바른 값을 입력해주세요.");
            return;
        }



        Step step = new Step(totalSteps, speed, duration, suctionToggle.isOn);
        step.angleAxis1 = angleAxis1;
        step.angleAxis2 = angleAxis2;
        step.angleAxis3 = angleAxis3;
        step.angleAxis4 = angleAxis4;
        step.angleAxis5 = angleAxis5;

        steps.Add(step);

        totalSteps++;
        stepInput.text = totalSteps.ToString();
        nowStepInfoTxt.text = $"Total step count: {totalSteps} / Current step: {currentStepNumber}";
        print("Step이 추가되었습니다.");
    }

    // Clear버튼 누르면 모든 Step들이 지워진다.
    public void OnClearBtnClkEvent()
        {
            steps.Clear();

        totalSteps = currentStepNumber = 0;

        nowStepInfoTxt.text = $"Total step count: {totalSteps} / Current step: {currentStepNumber}";

        stepInput.text = "0";
        speedInput.text = "100";
        durationInput.text = "0";
        suctionToggle.isOn = false;

        angleAxis1Input.text = "0";
        angleAxis2Input.text = "0";
        angleAxis3Input.text = "0";
        angleAxis4Input.text = "0";
        angleAxis5Input.text = "0";
    }

    // SingleCycle, Cycle, Stop, E-Stop 버튼을 누르면 로봇이 동작한다.
    public void OnSingleCycleBtnClkEvent()
    {
        // 각 Step에 따라 로봇의 모터가 움직여야 한다.
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        if (steps.Count > 0)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                if (i - 1 < 0)
                {
                    continue;
                }

                currentStepNumber = i;
                nowStepInfoTxt.text = $"Total step count: {totalSteps} / Current step: {currentStepNumber}";
                yield return RunStep(steps[i - 1], steps[i]);
            }
        }
    }

    IEnumerator RunStep(Step prevStep, Step nextStep)
    {
        Vector3 prevAxis1Euler = new Vector3(0, prevStep.angleAxis1, 0); // Axis1은 Y축 기준으로 회전
        Vector3 nextAxis1Euler = new Vector3(0, nextStep.angleAxis1, 0);

        Vector3 prevAxis2Euler = new Vector3(0, 0, prevStep.angleAxis2); // Axis2은 Z축 기준으로 회전
        Vector3 nextAxis2Euler = new Vector3(0, 0, nextStep.angleAxis2);

        Vector3 prevAxis3Euler = new Vector3(0, 0, prevStep.angleAxis3); // Axis3은 Z축 기준으로 회전
        Vector3 nextAxis3Euler = new Vector3(0, 0, nextStep.angleAxis3);

        Vector3 prevAxis4Euler = new Vector3(prevStep.angleAxis4, 0, 0); // Axis4은 X축 기준으로 회전
        Vector3 nextAxis4Euler = new Vector3(nextStep.angleAxis4, 0, 0);

        Vector3 prevAxis5Euler = new Vector3(0, 0, prevStep.angleAxis5); // Axis5은 Z축 기준으로 회전
        Vector3 nextAxis5Euler = new Vector3(0, 0, nextStep.angleAxis5);

        float currentTime = 0;
        while (true)
        {
            currentTime += Time.deltaTime;

            if ((currentTime / (prevStep.speed * 0.01f)) > 1)
            {
                break;
            }

            motorAxis1.localRotation = RotateAngle(prevAxis1Euler, nextAxis1Euler, currentTime / (prevStep.speed * 0.01f));
            motorAxis2.localRotation = RotateAngle(prevAxis1Euler, nextAxis2Euler, currentTime / (prevStep.speed * 0.01f));
            motorAxis3.localRotation = RotateAngle(prevAxis1Euler, nextAxis3Euler, currentTime / (prevStep.speed * 0.01f));
            motorAxis4.localRotation = RotateAngle(prevAxis1Euler, nextAxis4Euler, currentTime / (prevStep.speed * 0.01f));
            motorAxis5.localRotation = RotateAngle(prevAxis1Euler, nextAxis5Euler, currentTime / (prevStep.speed * 0.01f));

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(prevStep.duration);
    }

    private Quaternion RotateAngle(Vector3 from, Vector3 to, float t)
    {
        return Quaternion.Slerp(Quaternion.Euler(from), Quaternion.Euler(to), t);
    }
}
