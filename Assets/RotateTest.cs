using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RotateTest : MonoBehaviour
{
    public GameObject[] cubes;
    
    public GameObject selectedCube;

    public Slider slider;
    public Text currentAngle;
    public Dropdown dropdownCubes;
    
    public float min = -180f;
    public float max = 180f;
    
    // Start is called before the first frame update
    void Start()
    {
        dropdownCubes.ClearOptions();
        
        var list = new List<string>();
        for (int i = 0; i < cubes.Length; ++i)
        {
            list.Add(cubes[i].name);
        }
        dropdownCubes.AddOptions(list);
        
        dropdownCubes.onValueChanged.AddListener(SelectCube);
        
        slider.minValue = min;
        slider.maxValue = max;
        
        slider.onValueChanged.AddListener(value =>
        {
            float round = Mathf.RoundToInt(value);
            
            currentAngle.text = round.ToString(); 
            OnRotate(round);
        });

        
        Vector3 normal = -selectedCube.transform.parent.forward;
        Quaternion rotWorld = Quaternion.LookRotation(selectedCube.transform.parent.right, normal);
        Quaternion rotLocal = Quaternion.Inverse(selectedCube.transform.parent.rotation) * rotWorld;
        
        selectedCube.transform.localRotation = rotLocal;
        
        //slider.value = GetCubeAngle();
        SetSliderValue(GetCubeAngle(), true);
    }

    private void SetSliderValue(float value, bool needNotify = true)
    {
        float round = Mathf.RoundToInt(value);
        
        if(needNotify)
            slider.value = round;
        else
            slider.SetValueWithoutNotify(round);
        
        currentAngle.text = round.ToString();
    }

    private void OnRotate(float rotationAngle)
    {
        Debug.Log("@@@@@@@@@@@@@@ OnRotate start, angle=" + rotationAngle + " @@@@@@@@@@@@@@ ");
        
        Transform parent = selectedCube.transform.parent;
        Vector3 normal = -parent.forward;
        Quaternion rotWorldCube = Quaternion.LookRotation(parent.right, normal);
        Quaternion rotLocalCube = Quaternion.Inverse(parent.rotation) * rotWorldCube;
        
        // Slot rotation edge case: 180 and -180 are actually same rotation for the slot in 3D space
        // If we set 180f or -180f here, we cannot decide if its 180f or -180f when we calculate it back to angle.
        // So the 0.01f difference is necessary.
        if (Mathf.Approximately(rotationAngle, 180f))
            rotationAngle = 179.99f;
        else if (Mathf.Approximately(rotationAngle, -180f))
            rotationAngle = -179.99f;
        
        selectedCube.transform.localRotation = rotLocalCube * Quaternion.AngleAxis(rotationAngle, Vector3.up);
        
        // verify
        Quaternion currentLocalRotation = selectedCube.transform.localRotation;
        Quaternion appliedRotation = Quaternion.Inverse(rotLocalCube) * currentLocalRotation;

        float angle = 0;
        Vector3 axis = Vector3.up;
        appliedRotation.ToAngleAxis(out angle, out axis);
        Debug.Log("angle: " + angle);
        Debug.Log("anxis: " + axis);
        Debug.Log("return angle: " + angle * axis.y);
        
        Debug.Log("@@@@@@@@@@@@@@ OnRotate end @@@@@@@@@@@@@@");
    }

    private void SelectCube(int cubeIndex)
    {
        selectedCube.GetComponent<MeshRenderer>().material.color = Color.blue;
        selectedCube = cubes[cubeIndex];
        selectedCube.GetComponent<MeshRenderer>().material.color = Color.red;

        float xAngle = GetCubeAngle();
        
        //slider.SetValueWithoutNotify(xAngle);
        //currentAngle.text = xAngle.ToString();
        SetSliderValue(xAngle, false);
    }

    private float GetCubeAngle()
    {
        Debug.Log("================= GetCubeAngle start ========================");
        //return selectedCube.transform.localRotation.eulerAngles.x;
        
        Transform parent = selectedCube.transform.parent;
        Vector3 normal = -parent.forward;
        Quaternion rotWorldCube = Quaternion.LookRotation(parent.right, normal);
        Quaternion rotLocalCube = Quaternion.Inverse(parent.rotation) * rotWorldCube;
        //selectedCube.transform.localRotation = rotLocalCube * Quaternion.AngleAxis(rotationAngle, Vector3.up);

        Quaternion currentLocalRotation = selectedCube.transform.localRotation;
        Quaternion appliedRotation = Quaternion.Inverse(rotLocalCube) * currentLocalRotation;

        float angle = 0;
        Vector3 axis = Vector3.up;
        appliedRotation.ToAngleAxis(out angle, out axis);
        
        Vector3 eulerAngles = appliedRotation.eulerAngles;
        
        Debug.Log("angle: " + angle);
        Debug.Log("axis: " + axis);
        Debug.Log("EulerAngles: " + eulerAngles);

        float middleAngle = angle * axis.y;
                    
        // check absolute value
        if (Mathf.Abs(middleAngle) > 180f)
        {
            middleAngle = (middleAngle + 360f) % 360f;
            middleAngle = Mathf.RoundToInt(middleAngle);
        }
        Debug.Log("middleAngle: " + middleAngle);

        Debug.Log("================= GetCubeAngle end ========================");
        
        return middleAngle;
    }
}
