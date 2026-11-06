using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TheStack : MonoBehaviour
{
    [SerializeField]
    GameObject plateformPrefab;

    [SerializeField]
    List<Plateform> plateforms = new List<Plateform>();

    [SerializeField]
    Vector3 leftPos;

    [SerializeField]
    Vector3 rightPos;

    [SerializeField]
    float initialSpeed = 1f;
    [SerializeField]
    float currentSpeed = 5f;
    [SerializeField]
    float speedIncrement = 0.5f;
    [SerializeField]
    float heightIncrement = 0.1f;

    [SerializeField]
    bool lastPlateformLeft = true;

    [SerializeField]
    private InputActionAsset inputActionAsset; // Reference to your Input Action Asset

    [SerializeField]
    InputAction action;

    private int score = 0;
    private int highScore = 0;

    public static event System.Action<int> OnScoreChanged;
    public static event System.Action OnGameFinished;


    public static TheStack instance;

    private void Awake()
    {
        if (instance == null)
        {
            ResetGame();
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        action = inputActionAsset.FindAction("Interact");
        action.started += ctx => StopPlateform();
        InitFirstPlateform();
    }

    public void ResetGame()
    {
        plateforms.Clear();
        score = 0;
        currentSpeed = initialSpeed;
        lastPlateformLeft = true;

    }

    public void InitFirstPlateform()
    {
        GameObject firstPlateform = Instantiate(plateformPrefab, Vector3.zero, Quaternion.identity);
        Plateform plateform = firstPlateform.GetComponent<Plateform>();
        plateform.SetSpeed(0);
        plateforms.Add(plateform);
    }

    private void StopPlateform()
    {
        if (CutPlateform(plateforms[plateforms.Count - 1]))
        {
            score++;
            OnScoreChanged?.Invoke(score);
            if (score > highScore)
            {
                highScore = score;
                SetHighScore(highScore);
            }
            GenerateNewPlateform();
        }
        else
        {
            StopGame();
        }
    }

    private bool CutPlateform(Plateform plateform)
    {
        plateform.SetSpeed(0);

        GameObject currentPlateform = plateform.gameObject;
        GameObject lastPlateform = plateforms[plateforms.Count - 2].gameObject;

        if (lastPlateformLeft)
        {
            float plateformX = currentPlateform.transform.position.x;
            float lastPlateformX = lastPlateform.transform.position.x;
            if (plateformX > Mathf.Abs(lastPlateformX) + lastPlateform.transform.localScale.x)
                return false;

            float newScaleX = currentPlateform.transform.localScale.x - Mathf.Abs(plateformX - lastPlateformX);

            currentPlateform.transform.localScale = new Vector3(newScaleX, currentPlateform.transform.localScale.y, currentPlateform.transform.localScale.z);

            float newPosX = (plateformX + lastPlateformX) / 2;

            currentPlateform.transform.position = new Vector3(newPosX, currentPlateform.transform.position.y, currentPlateform.transform.position.z);

            Debug.Log("Cutting plateform: New scale X = " + newScaleX + ", New position X = " + newPosX);
        }
        else
        {
            float plateformZ = currentPlateform.transform.position.z;
            float lastPlateformZ = lastPlateform.transform.position.z;
            if (plateformZ < Mathf.Abs(lastPlateformZ) - lastPlateform.transform.localScale.z)
                return false;

            float newScaleZ = currentPlateform.transform.localScale.z - Mathf.Abs(plateformZ - lastPlateformZ);
            currentPlateform.transform.localScale = new Vector3(currentPlateform.transform.localScale.x, currentPlateform.transform.localScale.y, newScaleZ);

            float newPosZ = (plateformZ + lastPlateformZ) / 2;
            currentPlateform.transform.position = new Vector3(currentPlateform.transform.position.x, currentPlateform.transform.position.y, newPosZ);
            Debug.Log("Cutting plateform: New scale Z = " + newScaleZ + ", New position Z = " + newPosZ);
        }

        return true;
    }

    public void GenerateNewPlateform()
    {
        Vector3 spawnPos = plateforms[plateforms.Count - 1].transform.position;
        spawnPos += lastPlateformLeft ? Vector3.forward*2 : Vector3.right*2;
        spawnPos.y += heightIncrement;
        GameObject newPlateform = Instantiate(plateformPrefab, Vector3.zero, Quaternion.identity);
        newPlateform.transform.position = spawnPos;
        newPlateform.transform.localScale = plateforms[plateforms.Count - 1].transform.localScale; 

        Plateform plateform = newPlateform.GetComponent<Plateform>();


        plateform.SetSpeed(currentSpeed);
        if (!lastPlateformLeft)
        {
            plateform.InvertDirection();
        }

        plateforms.Add(plateform);


        //gestion de la hauteur et de la vitesse
        currentSpeed += speedIncrement;
        lastPlateformLeft = !lastPlateformLeft;

        //ajustement de la camera
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + heightIncrement, Camera.main.transform.position.z);
    }

    private void StopGame()
    {
        plateforms[plateforms.Count - 1].gameObject.SetActive(false);
        action.Disable();

        OnGameFinished?.Invoke();
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    private void SetHighScore(int score)
    {
        PlayerPrefs.SetInt("HighScore", score);
        PlayerPrefs.Save();
    }

}
