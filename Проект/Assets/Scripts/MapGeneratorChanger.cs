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
        //�������� ���������� ���������� ��������� ����� � �������������� ��������, ���������������� �������� � �������� ���������� �����.

        mapGenerator.PerlinNoiseScale = slider.value;
        
        mapGenerator.PerlinNoiseOctaves = (int)Octaves.value;

        mapGenerator.PerlinNoisePersistance = Persistance.value;

        mapGenerator.PerlinNoiseLacunarity = Lacunarity.value;

        //����������� �������� �� UI � MapGenerator

        if(Seed.text == "")
        {
            Debug.Log("field is empty");
            Seed.text = Convert.ToString(rnd.Next(1,10000));
        }
        //������� ��������� seed � ������������ ����������
        if (MHM.text == "")
        {
            Debug.Log("field2 is empty");
            MHM.text = Convert.ToString(rnd.Next(1, 50));
        }
        //������� ��������� ��������� ������ � ������������ ����������

        mapGenerator.PerlinNoiseSeed = int.Parse(Seed.text); 

        mapGenerator.GeneratedMeshHeightMultiplier = int.Parse(MHM.text);

        endlessTerrain.GeneratedMapViewer = Body.transform;
        //����������� endless terrain ������������ 
        Destroy(go);
        //����������� ���������� ���������
        Destroy(destoy1);
        //����������� ��������� ���������
        go = Instantiate(Resources.Load("Map Generator")) as GameObject;
        //�������� ����� ���������
        Body.transform.position = new Vector3(0,425,0);
        //������������� ������������ �� ������������ ������� 
    }
}
