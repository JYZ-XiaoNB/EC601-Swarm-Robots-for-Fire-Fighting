using UnityEngine;

public class Drone_ManualCtrl : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float turnSpeed = 30f;
    public float liftSpeed = 5f;
    //public float baseLift = 5f;
    private Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
       // rb.freezeRotation = new Vector3(true, false, true); // 冻结侧翻
        rb.mass = 1f; // 质量固定，方便基础升力匹配
        rb.linearDamping = 3f; //阻力
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rb.AddForce(Vector3.up * baseLift, ForceMode.Force);

        // WASD 移动
        float forward = Input.GetAxis("Vertical") * moveSpeed;
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
        Vector3 move = transform.forward * forward + transform.right * horizontal;
        rb.AddForce(move, ForceMode.Force);

        // QE 转向
        float turn = 0;
        if (Input.GetKey(KeyCode.Q)) turn = -turnSpeed;
        if (Input.GetKey(KeyCode.E)) turn = turnSpeed;
        transform.Rotate(0, turn * Time.fixedDeltaTime, 0);

        // 空格/左Shift 升降
        float extralift = 0;
        if (Input.GetKey(KeyCode.Space)) extralift = liftSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) extralift = -liftSpeed;
        rb.AddForce(Vector3.up * extralift, ForceMode.Force);

        float maxHorizontalSpeed = 8f; // 前进/后退/左右的最大水平速度
        float maxVerticalSpeed = 5f;   // 上升/下降的最大垂直速度

        // 限制水平速度（X/Z轴）
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxHorizontalSpeed)
        {
            rb.linearVelocity = horizontalVelocity.normalized * maxHorizontalSpeed + new Vector3(0, rb.linearVelocity.y, 0);
        }

        // 限制垂直速度（Y轴）
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed), rb.linearVelocity.z);
        }

    public void Move(float forward, float horizontal)
{
    // 复用手动移动逻辑
    Vector3 move = transform.forward * forward * moveSpeed;
    move += transform.right * horizontal * moveSpeed;
    rb.AddForce(move, ForceMode.Force);
}

    public void Turn(float direction)
    {
        // direction=1→右转（E），direction=-1→左转（Q）
        float turn = direction * turnSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, turn, 0);
    }

    public void Lift(float direction)
    {
        // direction=1→上升（空格），direction=-1→下降（左Shift）
        float lift = direction * liftSpeed;
        rb.AddForce(Vector3.up * lift, ForceMode.Force);
    }

}
