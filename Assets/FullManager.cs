using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullManager : MonoBehaviour
{
    private AudioSource sound;
    public Transform ball1;
    public Transform ball2;
    public Transform ball3;
    public Transform ball4;
    public Transform ball2v;
    public Transform ball3v;
    public Transform ball4v;
    private Transform rotate;
    private List<GameObject> boxes = new List<GameObject>();
    public GameObject box;
    private float diff = 0;
    private Vector3 goal;
    private Vector3 prev;
    private Vector3 dir;
    private Vector3 goalDir;
    public float rotationalConst = 6f;
    public float ballVel = 12f;
    private float scalar = 7f;
    private float[] spectrum = new float[2048];
    private float[] spectrumMax = new float[4];
    private float[] oldSpectrum = new float[4];
    float[] squeezedSpectrum = new float[4];
    float[] squeezedScalars = {1,1,1,1};
    private protected float threshold = 0.25f;
    public bool debug = false;
    // Start is called before the first frame update
    void Start()
    {
        rotate = ball1.parent;
        sound = GetComponent<AudioSource>();
        for (int i = 0; i < 4; i++) {
            if (debug) {
                boxes.Add(Instantiate(box, new Vector3(-3.75f + 10f / 4f * i, 5f, 4.53f), Quaternion.identity, null));
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int testI = 0;
        int testI1 = 1;
        int testI2 = 2;
        int testI3 = 3;
        squeezedSpectrum = spectrumMax;
        Debug.Log(scalar);
        
        if (Mathf.Abs(squeezedSpectrum[testI] - oldSpectrum[testI]) > threshold / scalar)
        {
            oldSpectrum[testI] = squeezedSpectrum[testI];
        }
        if (Mathf.Abs(squeezedSpectrum[testI1] - oldSpectrum[testI1]) > threshold / scalar)
        {
            oldSpectrum[testI1] = squeezedSpectrum[testI1];
        }
        if (Mathf.Abs(squeezedSpectrum[testI2] - oldSpectrum[testI2]) > threshold / scalar)
        {
            oldSpectrum[testI2] = squeezedSpectrum[testI2];
        }
        if (Mathf.Abs(squeezedSpectrum[testI3] - oldSpectrum[testI3]) > threshold / scalar)
        {
            oldSpectrum[testI3] = squeezedSpectrum[testI3];
        }
        float xG = -oldSpectrum[testI] + oldSpectrum[testI1] - oldSpectrum[testI2] + oldSpectrum[testI3];
        float yG = -oldSpectrum[testI] - oldSpectrum[testI1] + oldSpectrum[testI2] + oldSpectrum[testI3];
        float diffE = 0.25f;
        if (xG * scalar > 4.5f || xG * scalar < -4.5f)
        {
            scalar -= diffE;
        }
        if (yG * scalar > 4.5f || yG * scalar < -4.5f)
        {
            scalar -= diffE;
        }
        if (scalar <= 0)
        {
            scalar = diffE;
        }

        goal = new Vector3(xG, yG, 0) * scalar;

        for (int i = 0; i < spectrumMax.Length; i++)
        {
            spectrumMax[i] = 0;
        }
        prev = new Vector3(ball1.localPosition.x,ball1.localPosition.y,0);
        diff = 0;
        goalDir = goal - prev;
        
        //Debug.Log(dir.magnitude);
        //Debug.Log(transform.position.x);
        //Debug.Log(sound.volume);
    }

    private void Update()
    {
        //rotate.localRotation = Quaternion.Euler(new Vector3(0,0,Time.time * 90));
        if (Input.GetKeyDown(KeyCode.S))
        {
            scalar-=1f;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            scalar += 1f;
        }
        AudioListener.GetSpectrumData(spectrum, 0,FFTWindow.Rectangular);
        for (int i = 0; i < spectrumMax.Length; i++)
        {
            squeezedSpectrum[i] = 0;
        }
        int[] quarts = {23,46,92,184};
        for (int i = 0; i < spectrum.Length; i++)
        {
            int j = 1;
            if (i < quarts[0])
            {
                j = 1;
            }
            else if (i < quarts[1])
            {
                j = 2;
            }
            else if (i < quarts[2])
            {
                j = 3;
            }
            else if (i < quarts[3])
            {
                j = 4;
            }
            squeezedSpectrum[j - 1] += spectrum[i] * squeezedScalars[j - 1];
        }
        for (int i = 0; i < spectrumMax.Length; i++)
        {
            if (debug) {
                boxes[i].transform.localScale = new Vector3(2.5f, spectrumMax[i] * 2.5f * scalar, 1);
            }
            if (squeezedSpectrum[i] > spectrumMax[i])
            {
                spectrumMax[i] = squeezedSpectrum[i];
            }
            float squeezedScalarConst = 0.025f;
            if (squeezedSpectrum[i] * 2.5f * scalar > 10f)
            {
                squeezedScalars[i] -= 0.1f;
            }
            if (squeezedScalars[i] <= squeezedScalarConst)
            {
                squeezedScalars[i] = squeezedScalarConst * 30f;
            }
            //squeezedSpectrum[i] = 0;
        }
        //Debug.Log(transform.position.y);
        diff += Time.deltaTime * 1;
        //ball1.position = Vector3.Lerp(prev,goal,diff);
        //ball2.position = Vector3.Lerp(prev1, goal1, diff);
        ball1.localPosition += dir * Time.deltaTime * ballVel;
        ball1.localPosition = new Vector3(Mathf.Clamp(ball1.localPosition.x,-4.5f,4.5f),Mathf.Clamp(ball1.localPosition.y, -4.5f, 4.5f),0);
        //ball1.localPosition = new Vector3(Mathf.Clamp(ball1.localPosition.x, -4.5f, 4.5f),ball1.localPosition.y);
        ball2.localPosition = new Vector3(-ball1.localPosition.x,-ball1.localPosition.y,0);
        ball3.localPosition = new Vector3(ball1.localPosition.y,ball1.localPosition.x,0);
        ball4.localPosition = new Vector3(-ball1.localPosition.y, -ball1.localPosition.x, 0);
        ball2v.localPosition = ball1.localPosition;
        ball3v.localPosition = ball1.localPosition;
        ball4v.localPosition = ball1.localPosition;
        if (dir.x < goalDir.x)
        {
            dir.x += rotationalConst * Time.deltaTime;
            if (dir.x > goalDir.x)
            {
                dir.x = goalDir.x;
            }
        }
        else
        {
            dir.x -= rotationalConst * Time.deltaTime;
            if (dir.x < goalDir.x)
            {
                dir.x = goalDir.x;
            }
        }
        if (dir.y < goalDir.y)
        {
            dir.y += rotationalConst * Time.deltaTime;
            if (dir.y > goalDir.y)
            {
                dir.y = goalDir.y;
            }
        }
        else
        {
            dir.y -= rotationalConst * Time.deltaTime;
            if (dir.y < goalDir.y)
            {
                dir.y = goalDir.y;
            }
        }
    }
}
