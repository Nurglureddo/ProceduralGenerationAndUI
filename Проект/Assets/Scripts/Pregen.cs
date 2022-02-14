using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pregen : MonoBehaviour
{
	public static int pregen = 4; // ������� ������� ����� ������� ��� ������ ���� (� ������ ������ 1)
	[SerializeField]
	Sprite open; // �������� ���������/����������� ������
	[SerializeField]
	Sprite closed; //�������� ���������/�� ����������� ������

	void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{ // ������, ����� ���� �������� ������� � all_level + ����� 
		  // ���������� ������ � ���� ��������� 
		  //(�.�. ���������� ��(transform.childCount) �� 0(i=0) � �� ������� ����������(i++)
			if (i < pregen)
			{ // ���� i ������ ������ levels (� ������ ������ 1) , �� �� ������ ������ ��������
				transform.GetChild(i).GetComponent<Image>().sprite = open; // �������� ������ = �������� open (���� �������� ��)
				transform.GetChild(i).GetComponent<Button>().interactable = true; // ������ ����������� �������� ( ��������� �� �������)
			}
			else
			{ // ����� (���� �� ����������� �������������� �������)
				transform.GetChild(i).GetComponent<Image>().sprite = closed; // �������� ������ = �������� closed (���� �������� ��)
				transform.GetChild(i).GetComponent<Button>().interactable = false; // ������ ����������� �� �������� 
																				   // (�� ��������� �� �������)
			}
		}
	}
}