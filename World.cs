using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    private int m_scansLeft = 6;
    private int m_extractLeft = 3;
    const int width = 16, height = 16;
    private float score;

    public Text scoreUI;
    public Text currentMode;

    public Text scanText, extractText;

    public enum gamestate{
        SCAN_MODE,
        EXTRACT_MODE,  
        GAME_OVER    
    };

    int[,] grid;
    Tile[,] physicalGrid;

    public GameObject cubePrefab;

    gamestate currentGS;

    void Awake()
    {
        currentGS = gamestate.SCAN_MODE;
    }

    void Start()
    {
        grid = new int[width, height];
        physicalGrid = new Tile[width, height];
        GenerateGrid(grid);
        PopulateGrid(grid);
        DrawGrid(grid);
    }


    void Update()
    {
        OnClick(currentGS);

        scoreUI.text = "Resources Collected: " + score;
        currentMode.text = currentGS.ToString();
        scanText.text = "Scans Left: " + m_scansLeft;
        extractText.text = "Extracts Left: " + m_extractLeft;
    }

    void GenerateGrid(int[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                map[x, y] = 1;
            }
        }
    }

    void PopulateGrid(int[,] map)
    {
        int rand = Random.Range(1, 5);
        for (int i = 0; i < rand; i++)
        {
            int mapRandCoordX = Random.Range(0, width);
            int mapRandCoordY = Random.Range(0, height);
            map[mapRandCoordX, mapRandCoordY] = 0;
            PopulateSurrounding(mapRandCoordX, mapRandCoordY, map, 1 , 2);
            PopulateSurrounding(mapRandCoordX, mapRandCoordY, map, 2, 3);
        }
    }

    void PopulateSurrounding(int x, int y, int[,] map, int s, int type)
    {
        for (int neighbourX = x - s; neighbourX <= x + s; ++neighbourX)
        {
            for (int neighbourY = y - s; neighbourY <= y + s; ++neighbourY)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    //really messy way of checking if its a type 2 tile 
                    if (map[neighbourX, neighbourY] != 0 && type == 2)
                    {
                        map[neighbourX, neighbourY] = type;
                    }
                    //once again really mesy but it works ;)
                    if (map[neighbourX, neighbourY] != 0 && map[neighbourX, neighbourY] != 2 && type == 3)
                    {
                        map[neighbourX, neighbourY] = type;
                    }
                }
            }
        }
    }

    void OnClick(gamestate gs)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (gs == gamestate.EXTRACT_MODE)
        {
            List<Tile> a = new List<Tile>();
            a.Clear();
            if (Input.GetMouseButtonDown(0) && m_extractLeft > 0)
            {
                m_extractLeft--;
                if (Physics.Raycast(ray, out hit, 10000))
                {
                    Debug.Log(hit.transform.name);
                    Debug.DrawLine(ray.origin, hit.point);

                    float x = hit.transform.position.x;
                    float z = hit.transform.position.z;
                    a = surroundingTiles((int)x, (int)z);

                    if (physicalGrid[(int)x, (int)z].GetType() != typeof(GrayResource))
                    {
                        for (int i = 0; i < a.Count; i++)
                        {
                            //Makes sure red resources arent degraded in the aoe
                            if (a[i].GetType() != typeof(RedResource))
                            {
                                if (!a[i].DegradeStatus())
                                    a[i].Degrade();
                                else if (a[i].DegradeStatus())
                                    a[i].SecondDegrade();
                            }
                        }
                    }
                    if (physicalGrid[(int)x, (int)z].GetType() == typeof(RedResource))
                    {
                        physicalGrid[(int)x, (int)z].Degrade();
                    }
                        score += ScoreToAdd(physicalGrid[(int)x, (int)z]);
                }
            }
        }

        if (gs == gamestate.SCAN_MODE)
        {
            if (Input.GetMouseButtonDown(0) && m_scansLeft > 0)
            {
                List<Tile> b = new List<Tile>();
                b.Clear();

                m_scansLeft--;
                if (Physics.Raycast(ray, out hit, 10000))
                {
                    Debug.Log(hit.transform.name);
                    Debug.DrawLine(ray.origin, hit.point);

                    float x = hit.transform.position.x;
                    float z = hit.transform.position.z;

                    b = surroundingTiles((int)x,(int) z);
                    for (int i = 0; i < b.Count; i ++)
                    {
                        b[i].SetColor(IsInMapRange(b[i].x, b[i].z));
                    }


                }
            }
        }
    }
 

    List<Tile> surroundingTiles(int x, int y)
    {
       List<Tile> surroundTiles = new List<Tile>();
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
              if (IsInMapRange(neighbourX, neighbourY))
                    surroundTiles.Add(physicalGrid[neighbourX, neighbourY]);
            }
        }

        return surroundTiles;
    }

    float ScoreToAdd(Tile tile)
    {
        return tile.points;
    }

    void DrawGrid(int[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 1)
                {
                    Tile tile = new GrayResource(GameObject.CreatePrimitive(PrimitiveType.Cube));
                    tile.SetPosition(new Vector3(x, 0, y));
                    physicalGrid[x, y] = tile;                
                }
                else if (map[x, y] == 2)
                {
                    Tile tile = new YellowResource(GameObject.CreatePrimitive(PrimitiveType.Cube));
                    tile.SetPosition(new Vector3(x, 0, y));
                    physicalGrid[x, y] = tile;
                }
                else if (map[x, y] == 0)
                {
                    Tile tile = new RedResource(GameObject.CreatePrimitive(PrimitiveType.Cube));
                    tile.SetPosition(new Vector3(x, 0, y));
                    physicalGrid[x, y] = tile;
                }
                else if (map[x, y] == 3)
                {
                    Tile tile = new OrangeResource(GameObject.CreatePrimitive(PrimitiveType.Cube));
                    tile.SetPosition(new Vector3(x, 0, y));
                    physicalGrid[x, y] = tile;
                }
            }
        }
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    //button
    public void ChangeMode()
    {
        switch(currentGS)
        {
            case gamestate.EXTRACT_MODE:
                currentGS = gamestate.SCAN_MODE;
                break;
            case gamestate.SCAN_MODE:
                currentGS = gamestate.EXTRACT_MODE;
                break;
            default:
                currentGS = gamestate.SCAN_MODE;
                break;

        }
    }

    //Debug Button
    public void DebugButton()
    {
        m_extractLeft += 100;
        m_scansLeft += 100;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    #region classes
    public class RedResource : Tile
    {
        public RedResource()
        {

        }

        public RedResource(GameObject cube) : base (cube)
        {
            points = 6000.0f;
            color = Color.red;
            //debug will remove     
            //this.cube.GetComponent<MeshRenderer>().material.color = color;
        }

        public Color getColor()
        {
            return color;
        }

    }

    public class OrangeResource : Tile
    {
        public OrangeResource()
        {

        }

        public OrangeResource(GameObject cube) : base (cube)
        {
            points = 4000.0f;
            color = Color.cyan;
            //debug will remove
            //this.cube.GetComponent<MeshRenderer>().material.color = color;
        }

        public Color getColor()
        {
            return color;
        }

    }

    public class YellowResource : Tile
    {      
        public YellowResource()
        {

        }

        public YellowResource(GameObject cube) : base(cube)
        {
            points = 3000.0f;
            color = Color.yellow;
            //debug will remove
            //this.cube.GetComponent<MeshRenderer>().material.color = color;
        }
        public Color getColor()
        {
            return color;
        }

    }

    public class GrayResource: Tile
    {
        public GrayResource()
        {
        }

        public GrayResource(GameObject cube) : base(cube)
        {
            points = 0.0f;
            color = Color.gray;
            //debug will remove
            //this.cube.GetComponent<MeshRenderer>().material.color = color;
        }
        public Color getColor()
        {
            return color;
        }
    }


    public class Tile 
    {
        public GameObject cube;

        public float points;

        public Color color;

        public int x;
        public int z;

        public Vector3 position;

        bool degradedOnce;

        public Tile() { }
        public Tile(GameObject cube)
        {
            this.cube = cube;
            cube.AddComponent<BoxCollider>();
            x = (int)cube.gameObject.transform.position.x;
            z = (int)cube.gameObject.transform.position.z;
            cube.GetComponent<MeshRenderer>().material.color = Color.white;
            degradedOnce = false;
        }

        public void SetColor(bool status)
        {
            if (status)
            {
                this.cube.GetComponent<MeshRenderer>().material.color = color;
            }
        }

        public void Degrade()
        {
            if (this is RedResource)
            {
                degradedOnce = true;
                cube.GetComponent<MeshRenderer>().material.color = Color.black;
                points = 0.0f;
            }
            if (this is YellowResource)
            {
                degradedOnce = true;
                cube.GetComponent<MeshRenderer>().material.color = Color.cyan;
            }
            if (this is OrangeResource)
            {
                degradedOnce = true;
                cube.GetComponent<MeshRenderer>().material.color = Color.gray;             
            }
        }

        public void SecondDegrade()
        {
            if (this is RedResource)
            {
                cube.GetComponent<MeshRenderer>().material.color = Color.black;
                points = 0.0f;
            }
            if (this is YellowResource)
            {
                cube.GetComponent<MeshRenderer>().material.color = Color.gray;
            }
            if (this is OrangeResource)
            {
                cube.GetComponent<MeshRenderer>().material.color = Color.gray;
            }
        }

        public void SetPosition(Vector3 pos)
        {
            cube.transform.position = pos;
            this.position = pos;
        }

        public bool DegradeStatus()
        {
            return degradedOnce;
        }
    }
    #endregion



}
