using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class BoxMesh : MonoBehaviour
{
    [HideInInspector] public Action _unclickCallback;
    [SerializeField] private float SwaySpeed;
    [SerializeField] private float SwayAmount;
    [SerializeField] private float Length;
    [SerializeField] private float Height;
    [SerializeField] private float Extrude;
    [SerializeField] private float ClickDuration;
    [SerializeField] private float UnclickDuration;
    [SerializeField] private MeshFilter MeshFilter;
    [SerializeField] private MeshFilter BottomMeshFilter;
    private Mesh _mesh;
    private Mesh _bottomMesh;
    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector3> _originalVertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Vector3> _bottomVertices = new List<Vector3>();
    private List<int> _bottomTriangles = new List<int>();
    private Vector3[] _auxVertices;
    private List<int> _topLayer = new List<int>();
    private List<int> _bevelLayer = new List<int>();
    private bool _meshChanged = false;
    private Coroutine _changeTopRoutine;
    void Awake()
    {
        _mesh = new Mesh();
        MeshFilter.mesh = _mesh;
        if(BottomMeshFilter != null)
        {

            _bottomMesh = new Mesh();
            BottomMeshFilter.mesh = _bottomMesh;
        }
        CreateVertices();
        CreateMesh();
    }
    void OnEnable()
    {
        StartCoroutine(SwayTop());
    }
    void Update()
    {
        if(_meshChanged)
        {
            _mesh.vertices = _vertices.ToArray();
            _mesh.RecalculateNormals();
            _meshChanged = false;
        }
    }
    private void CreateVertices()
    {
        float halfLength = (Length - Length*Extrude)/2f;
        float thirdHeight = Height/3f;
        float halfLengthExtruded = halfLength + (Length*Extrude)/2f;
      
        _auxVertices = new Vector3[]
        {
            //top
            new Vector3(-halfLength, Height, -halfLength),
            new Vector3(halfLength, Height, -halfLength),
            new Vector3(halfLength, Height, halfLength),
            new Vector3(-halfLength, Height, halfLength),

            //bevel
            new Vector3(-halfLengthExtruded, thirdHeight, -halfLengthExtruded),
            new Vector3(halfLengthExtruded, thirdHeight, -halfLengthExtruded),
            new Vector3(halfLengthExtruded, thirdHeight, halfLengthExtruded),
            new Vector3(-halfLengthExtruded, thirdHeight, halfLengthExtruded),

            //sides
            new Vector3(-halfLengthExtruded, 0, -halfLengthExtruded),
            new Vector3(halfLengthExtruded, 0, -halfLengthExtruded),
            new Vector3(halfLengthExtruded, 0, halfLengthExtruded),
            new Vector3(-halfLengthExtruded, 0, halfLengthExtruded),

        };
    }

    private void CreateMesh()
    {
        //top
        AddPolygon(2,1,0);
        AddPolygon(3,2,0);
        //bevel
        AddPolygon(1,5,4);
        AddPolygon(1,4,0);
        AddPolygon(0,4,7);
        AddPolygon(3,0,7);
        AddPolygon(6,3,7);
        AddPolygon(6,2,3);
        AddPolygon(6,1,2);
        AddPolygon(6,5,1); 
        AddPolygon(6,5,1);
        AddPolygon(6,5,1);
        //sides
        AddPolygon(5,9,8);
        AddPolygon(5,8,4);
        AddPolygon(4,8,7);
        AddPolygon(8,11,7);
        AddPolygon(7,11,10);
        AddPolygon(10,6,7);
        AddPolygon(10,5,6);
        AddPolygon(10,9,5);
        if(BottomMeshFilter != null)
        {
            //bottom
            AddPolygon(8,9,10,_bottomVertices,_bottomTriangles);
            AddPolygon(11,8,10,_bottomVertices,_bottomTriangles);

            _bottomMesh.vertices = _bottomVertices.ToArray();
            _bottomMesh.triangles = _bottomTriangles.ToArray();
            _bottomMesh.RecalculateNormals();
            
        }
        _originalVertices = _vertices;
        _mesh.vertices = _vertices.ToArray();
       _mesh.triangles = _triangles.ToArray();
       _mesh.RecalculateNormals();
    }
    /*
    Adds a Polygon with new vertexes,
    new vertexes are created even if there already exists a vertex at a specified position as to allow for different normals to coexist in a point.
*/
    private void AddPolygon(int a, int b, int c, List<Vector3> vertices,List<int> triangles)
    {
        int index = vertices.Count;

        checkLayer(a);
        vertices.Add(_auxVertices[a]);
        checkLayer(b);
        vertices.Add(_auxVertices[b]);
        checkLayer(c);
        vertices.Add(_auxVertices[c]);
        triangles.Add(index);
        triangles.Add(index+1);
        triangles.Add(index+2);
    }

    private void AddPolygon(int a, int b, int c)
    {
        AddPolygon(a,b,c,_vertices,_triangles);
    }
    private void checkLayer(int vertexIndex)
    {
        if(vertexIndex < 4)
        {
            _topLayer.Add(_vertices.Count);
        }
        else if(vertexIndex < 8)
        {
            _bevelLayer.Add(_vertices.Count);
        }
    }

    private IEnumerator ChangeTopLayerHeight(float duration, float goalHeight)
    {
        if(_topLayer.Count == 0)
            yield break;
        float percent = 0;
        float startTime = Time.time;
        float startHeight = _vertices[_topLayer[0]].y;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            float height = Mathf.Lerp(startHeight, goalHeight, percent);
            SetLayerHeight(height, _topLayer);
            yield return null;
        }
    }

    private void SetLayerHeight(float height, List<int> layer)
    {
        for(int i = 0; i < layer.Count; i++)
        {
            Vector3 thisVertex =  _vertices[layer[i]];
            _vertices[layer[i]] = new Vector3(thisVertex.x, height ,thisVertex.z);
        }
        _meshChanged = true;
    }
    private void SetLayerXoffset(float xOffset, List<int> layer)
    {
        for(int i = 0; i < layer.Count; i++)
        {
            Vector3 thisVertex =  _vertices[layer[i]];
            _vertices[layer[i]] = new Vector3(_originalVertices[layer[i]].x + xOffset, thisVertex.y ,thisVertex.z);
        }
        _meshChanged = true;
    }

    public void Click()
    {
        if(_changeTopRoutine != null)
            StopCoroutine(_changeTopRoutine);
        _changeTopRoutine = StartCoroutine(LerpTopLayer(0,ClickDuration));
        StartCoroutine(CheckUnclick());

    }
    public void LetGo()
    {
        if(_changeTopRoutine != null)
            StopCoroutine(_changeTopRoutine);
        _changeTopRoutine = StartCoroutine(LerpTopLayer(Height,UnclickDuration));
        Invoke("RaiseUnclickEvent",UnclickDuration);
    }

    private IEnumerator LerpTopLayer(float goalY, float duration)
    {
        if(_topLayer.Count < 1 || _vertices.Count < 1)
            yield break;
        float startY = _vertices[_topLayer[0]][1];
        float startTime = Time.time;
        float percent = 0;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/duration;
            float y = Mathf.Lerp(startY,goalY,percent);
            SetLayerHeight(y, _topLayer);
            yield return null;
        }
 
    }
    private IEnumerator SwayTop()
    {
        
        while(true)
        {
            float x = SwayAmount * Mathf.Cos(SwaySpeed*Time.realtimeSinceStartup);

            SetLayerXoffset(x,_topLayer);
            yield return null;
        }
 
    }

    private IEnumerator CheckUnclick()
    {
        
        while(true)
        {
            if(Input.GetKeyUp(KeyCode.Mouse0))
            {
                LetGo();
                yield break;
            }
            yield return null;
        }
    }
    private void RaiseUnclickEvent()
    {
        _unclickCallback?.Invoke();
    }
}
