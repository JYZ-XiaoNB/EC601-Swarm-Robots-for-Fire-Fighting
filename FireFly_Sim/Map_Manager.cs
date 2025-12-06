using UnityEngine;

public class Map_Manager : MonoBehaviour
{
    public int[,] Map;
    //public float half_grid_size = 8.57f;
    public int gridsizeX = 7;
    public int gridsizeZ = 7;
    public int Drone_target_x = 0;
    public int Drone_target_z = 0;
    public int Fire_x = -1;
    public int Fire_z = -1;
    public bool Fire_found = false;
    public bool last_reach = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Map = new int[gridsizeX, gridsizeZ];
        for (int i = 0; i < gridsizeX; i++)
        {
            for (int j = 0; j < gridsizeZ; j++)
            {
                Map[i,j] = 0;       // 初始化地图； 0 = 未知； 1 = 已探索； 2 = 墙壁； 3 = 兴趣点
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        if (!Fire_found){
            if(last_reach){
                if (Map[Drone_target_x, Drone_target_z] != 0)       // 更新下个目标点
                {
                    for (int i = 0; i < gridsizeX; i++)
                    {
                        for (int j = 0; j < gridsizeZ; j++)
                        {
                            if (Map[i,j] == 0)
                            {
                                Drone_target_x = i;
                                Drone_target_z = j;
                                last_reach = false;
                                break;
                            }
                        }
                    }
                }
            }
        }else
        {
            Drone_target_x = Fire_x;
            Drone_target_z = Fire_z;
        }
    }

}
