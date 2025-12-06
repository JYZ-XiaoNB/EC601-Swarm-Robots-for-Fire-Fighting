using UnityEngine;
using System.Collections.Generic;

public class Drone_AutoCtrl : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float turnSpeed = 30f;
    public float liftSpeed = 10f;
    //public float baseLift = 5f;

    private Rigidbody rb;
    private Transform tf;

    private int current_targetX;
    private int current_targetZ;


    private float Grid_size = 120f / 7f;
  

    private GameObject Control_Center;
    private Map_Manager Map;
    private int[,] _map;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
       // rb.freezeRotation = new Vector3(true, false, true); // 冻结侧翻
        rb.mass = 1f; // 质量固定，方便基础升力匹配
        rb.linearDamping = 3f; //阻力
        Control_Center = GameObject.Find("Control_Center");
        Map = Control_Center.GetComponent<Map_Manager>();
        _map = Map.Map;
        current_targetX = Map.Drone_target_x;
        current_targetZ = Map.Drone_target_z;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int startX = Mathf.FloorToInt(tf.position.x / Grid_size);
        int startZ = Mathf.FloorToInt(tf.position.z / Grid_size);
        if (tf.position.y < 18f)            // 先起飞获得高度
        {
            rb.AddForce(Vector3.up * liftSpeed, ForceMode.Force);
        }
        else
        {
            current_targetX = Map.Drone_target_x;          // 更新当前目标
            current_targetZ = Map.Drone_target_z;
            GridNode Next = A_star(current_targetX, current_targetZ);       // 现在每一帧都会尝试更新路线，或许可以优化
            float target_x_value = Next.X * Grid_size + 0.5f * Grid_size;
            float target_z_value = Next.Z * Grid_size + 0.5f * Grid_size;
            FlyTo(target_x_value, target_z_value);
            /*if (last_x != current_targetX || last_z != current_targetZ)
            {
                A_star(current_targetX, current_targetZ);
                last_x = current_targetX;
                last_z = current_targetZ;
            }
            */
        }
    }

    private void FlyTo(float x, float z)
    {
        Debug.Log("Next Target "+x+" "+z);
        //Debug.Log($"Current Forward {tf.forward.x}, {tf.forward.y}, {tf.forward.z}");
        if (Mathf.Abs(x - tf.position.x) > 1)      // x有显著距离
        {
            if (Mathf.Sign(x - tf.position.x) * tf.forward.x < 1f)
            {
                if (tf.forward.z > 0)
                {
                    tf.Rotate(0, Mathf.Sign(x - tf.position.x) * turnSpeed * Time.fixedDeltaTime, 0);
                }
                else
                {
                    tf.Rotate(0, -Mathf.Sign(x - tf.position.x) * turnSpeed * Time.fixedDeltaTime, 0);
                }
            }
            else            // 已经对齐
            {
                rb.AddForce(tf.forward * moveSpeed, ForceMode.Force);
            }
        }
        else if (Mathf.Abs(z - tf.position.z) > 1)
        {
            if (Mathf.Sign(z - tf.position.z) * tf.forward.z < 1f)       // 未对准
            {
                if (tf.forward.x > 0)
                {
                    tf.Rotate(0, -Mathf.Sign(z - tf.position.z) * turnSpeed * Time.fixedDeltaTime, 0);
                }
                else
                {
                    tf.Rotate(0, Mathf.Sign(z - tf.position.z) * turnSpeed * Time.fixedDeltaTime, 0);
                }
            }
            else            // 已经对齐
            {
                rb.AddForce(tf.forward * moveSpeed, ForceMode.Force);
            }
        }
        else
        {
            Map.last_reach = true;
        }
    }
    private GridNode A_star(int targetX, int targetZ)
    {
        int startX = Mathf.FloorToInt(tf.position.x / Grid_size);
        int startZ = Mathf.FloorToInt(tf.position.z / Grid_size);

        List<GridNode> _openList = new List<GridNode>();
        List<GridNode> _closedList = new List<GridNode>();

        GridNode startNode = new GridNode(startX, startZ, true);
        GridNode targetNode = new GridNode(targetX, targetZ, true);
        _openList.Add(startNode);

        while (_openList.Count > 0)
        {
            // 6.1 从开放列表中找到 F值最小的格点（优先级最高）
            GridNode currentNode = GetNodeWithMinFValue(_openList);

            // 6.2 将当前格点从开放列表移到关闭列表（标记为已考察）
            _openList.Remove(currentNode);
            _closedList.Add(currentNode);

            // 6.3 找到目标格点，回溯路径并返回
            if (currentNode.X == targetNode.X && currentNode.Z == targetNode.Z)
            {
                return RetracePath(startNode, currentNode)[0];
            }

            // 6.4 查找当前格点的所有相邻格点（上下左右4个方向，可扩展为8方向）
            List<GridNode> neighbors = GetNeighborNodes(currentNode, _map);

            // 6.5 遍历相邻格点，更新代价和父节点
            foreach (GridNode neighbor in neighbors)
            {
                // 相邻格点不可通行，或已在关闭列表中，跳过
                if (!neighbor.IsPassable || _closedList.Contains(neighbor))
                {
                    continue;
                }

                // 计算当前格点到相邻格点的G值（横向/纵向移动，代价+1）
                int newGValue = currentNode.G + 1;

                // 如果相邻格点不在开放列表，或新G值更小（找到更优路径）
                if (!_openList.Contains(neighbor) || newGValue < neighbor.G)
                {
                    // 更新相邻格点的代价和父节点
                    neighbor.G = newGValue;
                    neighbor.H = CalculateHValue(neighbor, targetNode); // 计算H值（曼哈顿距离）
                    neighbor.Parent = currentNode;

                    // 如果不在开放列表，加入开放列表
                    if (!_openList.Contains(neighbor))
                    {
                        _openList.Add(neighbor);
                    }
                }
            }
        }
        GridNode Origin = new GridNode(startX, startZ, true);
        return Origin;
    }
    private bool IsGridValid(int x, int z)
    {
        return x >= 0 && x < 7 && z >= 0 && z < 7;
    }

    
    private bool IsGridPassable(int x, int z, int[,] map)
    {
        // 你的 Map 组件中存储格点状态的数组（比如 GridState[,] globalGridState）
        if (map[x,z] == 0 || map[x,z] == 1 || map[x,z] == 3)return true;
        else return false;
    }

    
    private GridNode GetNodeWithMinFValue(List<GridNode> nodeList)
    {
        GridNode minNode = nodeList[0];
        foreach (GridNode node in nodeList)
        {
            if (node.F < minNode.F)
            {
                minNode = node;
            }
        }
        return minNode;
    }

    
    private int CalculateHValue(GridNode currentNode, GridNode targetNode)
    {
        int dx = Mathf.Abs(currentNode.X - targetNode.X);
        int dz = Mathf.Abs(currentNode.Z - targetNode.Z);
        return dx + dz; // 曼哈顿距离（每格代价为1，预估最短路程）
    }

    
    private List<GridNode> GetNeighborNodes(GridNode currentNode, int[,] map)
    {
        List<GridNode> neighbors = new List<GridNode>();

        // 4个相邻方向（x±1或z±1）
        int[] dx = { -1, 1, 0, 0 };
        int[] dz = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int neighborX = currentNode.X + dx[i];
            int neighborZ = currentNode.Z + dz[i];

            // 校验相邻格点是否合法（在地图范围内）
            if (IsGridValid(neighborX, neighborZ))
            {
                // 创建相邻格点节点，加入列表
                bool isPassable = IsGridPassable(neighborX, neighborZ, map);
                neighbors.Add(new GridNode(neighborX, neighborZ, isPassable));
            }
        }

        return neighbors;
    }

    
    private List<GridNode> RetracePath(GridNode startNode, GridNode targetNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = targetNode;

        // 从目标节点反向遍历父节点，直到回到起点
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;

            // 防止死循环（极端情况，比如父节点为null）
            if (currentNode == null)
            {
                //Debug.LogError("A* 路径回溯失败：父节点为空！");
                path.Add(startNode);
                return path;
            }
        }

        // 加入起点，反转列表（得到从起点到目标的路径）
        if (path.Count == 0)path.Add(startNode);
        path.Reverse();

        //Debug.Log($"A* 寻路成功：路径长度={path.Count}格点");
        return path;
    }
   
}

class GridNode
{
    // 格点在地图中的坐标（x=横向，z=纵向，对应你的 map 数组索引）
    public int X { get; private set; }
    public int Z { get; private set; }

    // A* 核心代价
    public int G { get; set; } // 起点到当前格点的实际代价
    public int H { get; set; } // 当前格点到目标的预估代价
    public int F => G + H; // 总代价（自动计算）

    // 父节点（用于回溯路径）
    public GridNode Parent { get; set; }

    // 格点性质（从你的 map 中读取：通路/墙壁/未知）
    public bool IsPassable { get; private set; } // true=通路，false=墙壁/未知（未知视为不可通行）
    public GridNode(int x, int z, bool isPassable)
    {
        X = x;
        Z = z;
        IsPassable = isPassable;
        G = 0;
        H = 0;
        Parent = null;
    }
}
