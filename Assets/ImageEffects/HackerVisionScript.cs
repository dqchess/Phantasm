﻿using UnityEngine;
using System.Collections;

public class HackerVisionScript : MonoBehaviour
{
    public Material nightVisionMaterial;
    public Material thermalMaterial;
    public Material sonarMaterial;
    public Material filmGrainMaterial;
    public Material waveMaterial;
    public Texture thermalRamp;

    public Camera CameraSettings;

    private Color ambientLightTemp;
    public Color ambientLight = new Color(0.25f, 0.25f, 0.25f);

    [Range(0.0f, 1.0f)]
    public float filmGrainNightVisionAmount = 0.3f;

    [Range(0.0f, 1.0f)]
    public float filmGrainNormalAmount = 0.05f;

    public Color SonarColor = new Color(0.5f, 0.5f, 1.0f);
    [Range(0.0f, 1.0f)]
    public float SonarDiffusePass = 1.0f;
    public float SonarTimeMult = 1.0f;
    public float SonarMult = 0.02f;


    enum HackerVisionMode { Normal, Night, Thermal, Sonar, Last };
    HackerVisionMode Vision = HackerVisionMode.Normal;

    

    public Material AgentMaterial;
    public Material PhantomMaterial;
    
    public Vector2 WaveCount = new Vector2(40.0f, 40.0f);
    public Vector2 WaveIntensity = new Vector2(0.01f, 0.01f);
    public Vector2 WaveTimeMult = new Vector2(1.0f, 1.0f);

    RenderTexture temp = new RenderTexture(Screen.width, Screen.height, 0);


    Matrix4x4 ProjBiasMatrix = new Matrix4x4();

    float timeSinceSwap = 0.0f;
    void Start()
    {
        ProjBiasMatrix.SetRow(0, new Vector4(2.0f, 0.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(1, new Vector4(0.0f, 2.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(2, new Vector4(0.0f, 0.0f, 2.0f, -1.0f));
        ProjBiasMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f,  1.0f));

        ambientLightTemp = RenderSettings.ambientLight;
    }

    public void Update()
    {
        // Film Grain transition between vision modes

        timeSinceSwap += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.N))
        {
            timeSinceSwap = 0.0f;

            Vision++;
            if (Vision == HackerVisionMode.Last)
                Vision = HackerVisionMode.Normal;
            // Loop back to beginning
        }


    }

    public void OnPreRender()
    {
        if (Vision == HackerVisionMode.Night)
        {
            // If night vision is on, turn the ambient light up and store actual ambient light
            ambientLightTemp = RenderSettings.ambientLight;
            RenderSettings.ambientLight = ambientLight; 
        }
    }

    // OnRenderImage is called after all rendering is complete to render image
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Reset the ambient light
        RenderSettings.ambientLight = ambientLightTemp;

        float filmGrainAmountTotal = filmGrainNormalAmount + Mathf.InverseLerp(0.3f, 0.1f, timeSinceSwap);

        float RandomNum = Random.Range(0.0f, 1.0f);
        filmGrainMaterial.SetFloat("RandomNumber", RandomNum);
        filmGrainMaterial.SetFloat("uAmount", filmGrainAmountTotal);

        waveMaterial.SetVector("uWaveCount", WaveCount);
        waveMaterial.SetVector("uWaveIntensity", WaveIntensity);
        waveMaterial.SetVector("uTime", WaveTimeMult * Time.time);

        AgentMaterial.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f));
        PhantomMaterial.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f));

        if (Vision == HackerVisionMode.Normal)
        {
            Graphics.Blit(source, temp, waveMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
        else if (Vision == HackerVisionMode.Thermal)
        {
            //thermalRamp.filterMode = FilterMode.Point;
            thermalMaterial.SetTexture("ThermalRamp", thermalRamp);
            
            AgentMaterial.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 1.0f));
            PhantomMaterial.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 1.0f));

            Graphics.Blit(source, temp, thermalMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
        else if (Vision == HackerVisionMode.Sonar)
        {
            sonarMaterial.SetVector("uColorAdd", new Vector4(SonarColor.r, SonarColor.g, SonarColor.b, SonarTimeMult * Time.time));
            sonarMaterial.SetVector("uParameter", new Vector4(SonarMult, SonarDiffusePass, 0.0f, 0.0f));

            Matrix4x4 inverseMatrix = Matrix4x4.Inverse(CameraSettings.projectionMatrix) * ProjBiasMatrix;
            sonarMaterial.SetMatrix("uProjBiasMatrixInverse", inverseMatrix);

            Graphics.Blit(source, temp, sonarMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
        else if (Vision == HackerVisionMode.Night)
        {
            //float RandomNum = Random.Range(0.0f, 1.0f);
            nightVisionMaterial.SetFloat("RandomNumber", RandomNum);
            nightVisionMaterial.SetFloat("uAmount", 0.0f);
            nightVisionMaterial.SetFloat("uLightMult", 1.2f);


            filmGrainMaterial.SetFloat("uAmount", Mathf.Max(filmGrainNightVisionAmount, filmGrainAmountTotal));

            Graphics.Blit(source, temp, nightVisionMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
    }
}
