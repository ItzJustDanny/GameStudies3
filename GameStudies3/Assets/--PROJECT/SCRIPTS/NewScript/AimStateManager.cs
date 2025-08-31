using UnityEngine;
using Unity.Cinemachine;

public class AimStateManager : MonoBehaviour
{
    //public UnityEngine.Cinemachine.AxisState xAxis, yAxis;
    [SerializeField] Transform camFollowPos;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       // xAxis.Update(Time.deltaTime);
        //yAxis.Update(Time.deltaTime);

    }

    private void LateUpdate()
    {
       // camFollowPos.localEulerAngles = new Vector3(yAxis.value, camFollowPos.localEulerAngles.y,
        //camFollowPos.localEulerAngles.z);
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis.value, transform.eulerAngles.z);
        
    }







}
