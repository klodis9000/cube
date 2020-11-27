using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingCube : MonoBehaviour
{
    public static MovingCube CurrentCube { get; private set; }
    public static MovingCube LastCube { get; private set; } // крайний куб
    public MoveDirection MoveDirection { get;  set; }

    [SerializeField]
    private float moveSpeed = 1f;
    // Start is called before the first frame update
    //onenable() - функция вызывается когда объект становится активным
    private void OnEnable()
    {
        if (LastCube == null)
        {
            LastCube = GameObject.Find("Start").GetComponent<MovingCube>();
        }
        CurrentCube = this;
        GetComponent<Renderer>().material.color = GetRandomColor();  //задаю цвет кубам

        //задаю размер
        transform.localScale = new Vector3(LastCube.transform.localScale.x, transform.localScale.y, LastCube.transform.localScale.z);
    }

    void Update()
    {
        if (MoveDirection == MoveDirection.Z)
        {
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
        }
        else
        {
            transform.position += transform.right * Time.deltaTime * moveSpeed;
        }

        //mainCamera.transform.position += new Vector3(0f, 0.1f, 0f);
    }

    private Color GetRandomColor()
    {
        return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
    }

    private float GetHangover()
    {
        if (MoveDirection == MoveDirection.Z)
        {
            return transform.position.z - LastCube.transform.position.z;
        }
        else
        {
            return transform.position.x - LastCube.transform.position.x;
        }
        
    }
    
    
    
    //метод отвечает за остановку куба, а затем его измельчение
    internal void Stop()
    {
        moveSpeed = 0;
        //хранит значение того, как далеко я прошёл предыдущий куб, т.н пережиток
        float hangover = GetHangover();
        //если максимальное значение = Z, то использовать направление Z, иначе Х
        float max = MoveDirection == MoveDirection.Z ? LastCube.transform.localScale.z : LastCube.transform.localScale.x;
        //если пережиток вышел за пределы 
        if (Mathf.Abs(hangover) >= max)
        {
            LastCube = null;
            CurrentCube = null;
            SceneManager.LoadScene(0);  //загрузить базовую сцену
        }
        
        //если двигающееся значение больше нуля, то получаю 1, что значит я проталкиваю его мимо Z
        //если меньше, то возврашаю в обратном направлении двигающийся куб
        float direction = hangover > 0 ? 1f : -1f;

        if (MoveDirection == MoveDirection.Z)
        {
            SplitCubeOnZ(hangover, direction);
        }
        else
        {
            SplitCubeOnX(hangover, direction);
        }

        LastCube = this;

    }
    
    //разделение куба
    void SplitCubeOnX(float hangover, float direction)
    {
        //новый размер куба
        float newXSize = LastCube.transform.localScale.x - Mathf.Abs(hangover);
        //размер падающего блока
        float fallingBlockSize = transform.localScale.x - newXSize;
        
        //новая позиция двигающегося куба будучи обрезанным
        float newXPosition = LastCube.transform.position.x + (hangover / 2);
        transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
        
        //получаю край того что должно быть обрезано
        float cubeEdge = transform.position.x + (newXSize / 2f * direction);
        //получаю положение падающего блока по оси Z
        float fallingBlockXPosition = cubeEdge + fallingBlockSize / 2f * direction;

        SpawnDropCube(fallingBlockXPosition, fallingBlockSize);
    }
    
    //разделение куба
    void SplitCubeOnZ(float hangover, float direction)
    {
        //новый размер куба
        float newZSize = LastCube.transform.localScale.z - Mathf.Abs(hangover);
        //размер падающего блока
        float fallingBlockSize = transform.localScale.z - newZSize;
        
        //новая позиция двигающегося куба будучи обрезанным
        float newZPosition = LastCube.transform.position.z + (hangover / 2);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
        
        //получаю край того что должно быть обрезано
        float cubeEdge = transform.position.z + (newZSize / 2f * direction);
        //получаю положение падающего блока по оси Z
        float fallingBlockZPosition = cubeEdge + fallingBlockSize / 2f * direction;

        SpawnDropCube(fallingBlockZPosition, fallingBlockSize);
    }
    
    
    //создание падающего куба
    void SpawnDropCube(float fallingBlockZPosition, float fallingBlockSize)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube); //создаю куб
        if (MoveDirection == MoveDirection.Z)
        {
            cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingBlockSize); //задаю енр размер
            cube.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockZPosition); //задаю его позицию
        }
        else
        {
            cube.transform.localScale = new Vector3(fallingBlockSize, transform.localScale.y, transform.localScale.z); //задаю енр размер
            cube.transform.position = new Vector3(fallingBlockZPosition, transform.position.y, transform.position.z); //задаю его позицию
        }

        cube.AddComponent<Rigidbody>(); //падение
        cube.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color; //задание цвета падающему
        Destroy(cube.gameObject, 1f); //уничтожение объекта. t - интервал времени перед уничтожением
    }
}
