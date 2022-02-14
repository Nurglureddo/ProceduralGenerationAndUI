using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class MapGeneratorChanger : MonoBehaviour
{

    public MapGenerator mapGenerator;
    public EndlessTerrain endlessTerrain;
    public Slider slider;
    public Slider Octaves;
    public Slider Persistance;
    public Slider Lacunarity;
    public InputField Seed;
    public InputField MHM;
    public GameObject destoy;
    public GameObject destoy1;
    public GameObject inst;
    public GameObject Body;
    GameObject go;

    public void Example()
    {
        Random rnd = new Random();
        //Создание экземпляра генератора случайных чисел с использованием значения, предоставленного системой в качестве начального числа.

        mapGenerator.PerlinNoiseScale = slider.value;
        
        mapGenerator.PerlinNoiseOctaves = (int)Octaves.value;

        mapGenerator.PerlinNoisePersistance = Persistance.value;

        mapGenerator.PerlinNoiseLacunarity = Lacunarity.value;

        //Присваиваем значения из UI в MapGenerator

        if(Seed.text == "")
        {
            Debug.Log("field is empty");
            Seed.text = Convert.ToString(rnd.Next(1,10000));
        }
        //Создаем рандомный seed с определенным диапазоном
        if (MHM.text == "")
        {
            Debug.Log("field2 is empty");
            MHM.text = Convert.ToString(rnd.Next(1, 50));
        }
        //Создаем рандомный множитель высоты с определенным диапазоном

        mapGenerator.PerlinNoiseSeed = int.Parse(Seed.text); 

        mapGenerator.GeneratedMeshHeightMultiplier = int.Parse(MHM.text);

        endlessTerrain.GeneratedMapViewer = Body.transform;
        //Присваиваем endless terrain пользователя 
        Destroy(go);
        //Уничтожение предыдущей генерации
        Destroy(destoy1);
        //Уничтожение начальной генерации
        go = Instantiate(Resources.Load("Map Generator")) as GameObject;
        //Создание новой генерации
        Body.transform.position = new Vector3(0,425,0);
        //Инициализация пользователя на определенной позиции 
    }
}
