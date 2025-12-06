using UnityEngine;

public class Building_Spawn : MonoBehaviour
{
    public GameObject Building_5;
    public GameObject Fire;
    public static float building_grid = 8.57f;      // 这是block边长的一半
    public int building_num = 8;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float[] randomX = new float[building_num+1];
        float[] randomZ = new float[building_num+1];
        for (int i = 0; i <= building_num; i++)
        {
            randomX[i] = Random.Range(1,7);
            randomZ[i] = Random.Range(1,7);
        }
        for (int i = 0; i < building_num; i++)
        {
            if (randomX[i]==4 && randomZ[i]==1) continue;   //      保护飞机出生点没有建筑
            else if (randomX[i]==1 && randomZ[i]==1) continue;      // 不知道什么原因，0，0有建筑会导致卡死
            Vector3 pos = new Vector3((randomX[i]-1)*2*building_grid + building_grid, 0, (randomZ[i]-1)*2*building_grid + building_grid);
            Instantiate(Building_5, pos, Quaternion.identity);
        }
        if (randomX[building_num]==4 && randomZ[building_num] == 1)
        {
            
        }
        else
        {
            Vector3 pos = new Vector3((randomX[building_num]-1)*2*building_grid + building_grid, 0, (randomZ[building_num]-1)*2*building_grid + building_grid);
            Instantiate(Fire, pos, Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
