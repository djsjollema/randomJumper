using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RandomPlatformJumper : MonoBehaviour
{
    public GameObject platform;
    public GameObject randomJumper;

    int numberOfPlatforms = 5;

    Vector2 borderMax;
    Vector2 borderMin;
    List<GameObject> platforms = new List<GameObject>();

    int index = 0;

    string state = "grounded";
    float g = -10.0f;
    float startSpeed = 10.0f;

    float space;
    Vector3 velocity = new Vector3(0, 0, 0);
    Vector3 acceleration = new Vector3(0, 0, 0);

    float tmax;
    float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        borderMax = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        borderMin = Camera.main.ScreenToWorldPoint(Vector2.zero);

        space = (borderMax.x - borderMin.x) / (numberOfPlatforms);

        for (int i=0; i < numberOfPlatforms; i++)
        {
            GameObject platform_copy = Instantiate(platform);
            float x = borderMin.x + space/2 + i*space;
            float y = Random.Range(borderMin.y, 0);
            platform_copy.transform.position = new Vector3(x, y, 0);
            platforms.Add(platform_copy);
        }

        randomJumper.transform.position = platforms[0].transform.position + new Vector3(0, 0.6f, 0);

        
    }

    // Update is called once per frame
    void Update()
    {
        if(state == "grounded")
        {
            velocity = new Vector3(0, 0, 0);
            acceleration = new Vector3(0, 0, 0);
            state = "prepareJump";
        }

        if(state == "prepareJump")
        {
            GameObject current = platforms[index%numberOfPlatforms];
            GameObject target = platforms[(index + 1)%numberOfPlatforms];
            float dY = current.transform.position.y - target.transform.position.y;
            tmax = quadraticFormula(0.5f * g, startSpeed, dY).x;
            velocity = new Vector3(space/tmax, startSpeed, 0);
            acceleration = new Vector3(0, g, 0);
            t = 0;
            state = "airborne";
        }

        if(state == "airborne")
        {
            if(randomJumper.transform.position.x > borderMax.x)
            {
                randomJumper.transform.position = new Vector3(borderMin.x, randomJumper.transform.position.y, 0);
            }
            t += Time.deltaTime;

            if (t < tmax)
            {
                velocity += acceleration * Time.deltaTime;
                randomJumper.transform.position += velocity * Time.deltaTime;
            }
            else
            {
                velocity = new Vector3(0, 0, 0);
                acceleration = new Vector3(0, 0, 0);
                index++;
                randomJumper.transform.position = new Vector3(randomJumper.transform.position.x, platforms[index%numberOfPlatforms].transform.position.y + 0.6f,0);
                state = "grounded";
            }
        }
        
    }

    private Vector2 quadraticFormula(float a, float b, float c)
    {
        float discriminant = (b * b) - (4 * a * c);
        return new Vector2 ((-b - Mathf.Sqrt(discriminant)) / (2 * a), (-b + Mathf.Sqrt(discriminant)) / (2 * a)); 
    }
}
