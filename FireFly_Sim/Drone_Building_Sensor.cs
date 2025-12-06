using UnityEngine;

public class Drone_Building_Sensor : MonoBehaviour
{
    // Debug 物体
    //public GameObject test_cube;
    // 摄像机探测距离
    public float rayDistance = 30f;
    public LayerMask detectLayer;
    // 扫描阵列参数（格点数、占屏幕大小）
    public int scanGridX = 5; // X轴射线数量（5=密集扫描，可改3/7）
    public int scanGridZ = 5; // Y轴射线数量（和X轴一致，形成方阵）
    public float scanAreaWidth = 0.5f; // 扫描区域占屏幕宽度比例（0.5=中间50%）
    public float scanAreaHeight = 0.5f; // 扫描区域占屏幕高度比例（和宽度一致）

    // 内部参数
    private float Grid_size = 120f / 7f;
    private Map_Manager Map;
    private int totalScanPoints; // 总扫描点数量（scanGridX * scanGridZ）
    private int currentScanIndex = 0; // 当前扫描点索引（循环递增）
    private Vector2[] scanScreenPoints; // 预计算的所有扫描点屏幕坐标（优化性能）
    private Camera _selfCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _selfCamera = this.GetComponent<Camera>();
        totalScanPoints = scanGridX * scanGridZ;
        PrecomputeScanScreenPoints();
        GameObject Control_Center = GameObject.Find("Control_Center");
        Map = Control_Center.GetComponent<Map_Manager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 currentScreenPoint = scanScreenPoints[currentScanIndex];
        //Debug.Log("Current Scanning" + currentScreenPoint);

        Ray ray = _selfCamera.ScreenPointToRay(currentScreenPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, detectLayer))
        {   
            //Debug.Log("Hit at " + hit.point);
            int hitX = Mathf.FloorToInt(hit.point.x / Grid_size);
            int hitZ = Mathf.FloorToInt(hit.point.z / Grid_size);
            //Instantiate(test_cube, hit.point, Quaternion.identity);
            //Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            if (hit.collider.CompareTag("Building"))
            {
                Map.Map[hitX, hitZ] = 2;
                //Debug.Log(hit.point);
                //Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                Map.Map[hitX, hitZ] = 1;
            }
            else if (hit.collider.CompareTag("Fire"))
            {
                Map.Map[hitX, hitZ] = 3;
                Map.Fire_x = hitX;
                Map.Fire_z = hitZ;
                Map.Fire_found = true;
            }
        }

        currentScanIndex ++;
        if (currentScanIndex >= totalScanPoints) currentScanIndex = 0;
    }

    void PrecomputeScanScreenPoints()       // 用于预计算扫描点阵的投影坐标
    {
        scanScreenPoints = new Vector2[totalScanPoints];
        int index = 0;

        // 计算屏幕中间扫描区域的边界（屏幕坐标：左下角(0,0)，右上角(Screen.width, Screen.height)）
        float screenCenterX = Screen.width / 2f;
        float screenCenterY = Screen.height / 2f;
        float halfScanWidth = (Screen.width * scanAreaWidth) / 2f;
        float halfScanHeight = (Screen.height * scanAreaHeight) / 2f;

        // 计算射线在屏幕上的间隔（均匀分布）
        float xStep = (halfScanWidth * 2) / (scanGridX - 1); // X轴每个扫描点的间隔
        float yStep = (halfScanHeight * 2) / (scanGridZ - 1); // Y轴每个扫描点的间隔

        // 遍历所有扫描点，存储屏幕坐标
        for (int x = 0; x < scanGridX; x++)
        {
            for (int y = 0; y < scanGridZ; y++)
            {
                float screenX = screenCenterX - halfScanWidth + x * xStep;
                float screenY = screenCenterY - halfScanHeight + y * yStep;
                scanScreenPoints[index] = new Vector2(screenX, screenY);
                index++;
            }
        }
    } 
}
