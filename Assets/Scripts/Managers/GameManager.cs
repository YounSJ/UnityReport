﻿using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public int m_NumRoundsToWin = 5;        
	public float m_StartDelay = 3f;         
	public float m_EndDelay = 3f;           
	public CameraControl m_CameraControl;   
	public Text m_MessageText;              
	public GameObject m_TankPrefab;         
	public TankManager[] m_Tanks;
	public float time; //add
	public bool isPause = false;


	private int m_RoundNumber;              
	private WaitForSeconds m_StartWait;     
	private WaitForSeconds m_EndWait;       
	private TankManager m_RoundWinner;
	private TankManager m_GameWinner;       


	private void Start()
	{
		m_StartWait = new WaitForSeconds(m_StartDelay);
		m_EndWait = new WaitForSeconds(m_EndDelay);

		SpawnAllTanks();
		SetCameraTargets();

		StartCoroutine(GameLoop());

		time = 0; //add
	}

	private void Update(){
		if (Input.GetKeyDown (KeyCode.Tab)) { //Tab키로 일시정지
			isPause = true;
			Time.timeScale = 0;
		} else if (Input.GetKeyDown (KeyCode.Escape)) { //ESC경기재개
			isPause = false;
			Time.timeScale = 1;
		}
	}

	private void SpawnAllTanks()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			m_Tanks[i].m_Instance =
				Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
			m_Tanks[i].m_PlayerNumber = i + 1;
			m_Tanks[i].Setup();
		}
	}


	private void SetCameraTargets()
	{
		Transform[] targets = new Transform[m_Tanks.Length];

		for (int i = 0; i < targets.Length; i++)
		{
			targets[i] = m_Tanks[i].m_Instance.transform;
		}

		m_CameraControl.m_Targets = targets;
	}


	private IEnumerator GameLoop()
	{
		yield return StartCoroutine(RoundStarting());
		yield return StartCoroutine(RoundPlaying());
		yield return StartCoroutine(RoundEnding());

		if (m_GameWinner != null)
		{
			//Application.LoadLevel (Application.loadedLevel);
			Application.Quit();
			//SceneManager.LoadScene(0);
		}
		else
		{
			StartCoroutine(GameLoop());
		}
	}


	private IEnumerator RoundStarting()
	{
		ResetAllTanks ();
		DisableTankControl ();

		// Snap the camera's zoom and position to something appropriate for the reset tanks.
		m_CameraControl.SetStartPositionAndSize ();

		// Increment the round number and display text showing the players what round it is.
		m_RoundNumber++;
		m_MessageText.text = "ROUND " + m_RoundNumber;


		yield return m_StartWait;
	}


	private IEnumerator RoundPlaying()
	{
		EnableTankControl ();
		//m_MessageText.text = string.Empty;

		while (!OneTankLeft ())
		{
			time += Time.deltaTime; //add
			int t = Mathf.FloorToInt (time); //add
			m_MessageText.text = "Time : " + t.ToString (); //add
			yield return null;
		}
	}

	private IEnumerator RoundEnding()
	{
		DisableTankControl ();

		m_RoundWinner = null;

		m_RoundWinner = GetRoundWinner ();

		if (m_RoundWinner != null)
			m_RoundWinner.m_Wins++;

		m_GameWinner = GetGameWinner ();

		string message = EndMessage ();
		m_MessageText.text = message;

		time = 0; //add
		yield return m_EndWait;
	}


	private bool OneTankLeft()
	{
		int numTanksLeft = 0;

		for (int i = 0; i < m_Tanks.Length; i++)
		{
			if (m_Tanks[i].m_Instance.activeSelf)
				numTanksLeft++;
		}

		return numTanksLeft <= 1;
	}


	private TankManager GetRoundWinner()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			if (m_Tanks[i].m_Instance.activeSelf)
				return m_Tanks[i];
		}

		return null;
	}


	private TankManager GetGameWinner()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
				return m_Tanks[i];
		}

		return null;
	}


	private string EndMessage()
	{
		string message = "DRAW!";

		if (m_RoundWinner != null)
			message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND! \n" + "Time is " + Mathf.FloorToInt (time) + " Second"; //add

		message += "\n\n"; //"\n\n\n\n" → "\n\n"

		for (int i = 0; i < m_Tanks.Length; i++)
		{
			message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
		}

		if (m_GameWinner != null)
			message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

		return message;
	}


	private void ResetAllTanks()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			m_Tanks[i].Reset();
		}
	}


	private void EnableTankControl()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			m_Tanks[i].EnableControl();
		}
	}


	private void DisableTankControl()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			m_Tanks[i].DisableControl();
		}
	}
}