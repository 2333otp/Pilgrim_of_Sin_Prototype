using UnityEngine;

public class forReferenceSizeMove : MonoBehaviour
{
    #region 資料

    [field:SerializeField,Range(1,15)]
    public float moveSpeed { get; private set; } = 5f;
    [field: SerializeField, Range(0, 30)]
    public float turnSpeed { get; private set; } = 15f;

    public Rigidbody rig { get; private set; }
    private Transform mainCam;

    #endregion

    public void SetVelocity(Vector3 direction)
    {
        Vector3 velocity = rig.linearVelocity; 
        velocity.x = direction.x;
        velocity.z = direction.z;
        rig.linearVelocity = velocity;
    }

    public void Awake()
    {
        rig = GetComponent<Rigidbody>();

        mainCam = Camera.main.transform;
    }   

    public void Update()
    {
        //移動
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * v + transform.right * h;

        SetVelocity(move * moveSpeed);

        if (move != Vector3.zero)
        {
            LookAtCamera();
        }
    }

    public void LookAtCamera()
    {
        Quaternion camAngle = Quaternion.Euler(0, mainCam.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, camAngle, turnSpeed * Time.deltaTime);
    }
}
