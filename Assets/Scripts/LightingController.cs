using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightingController : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    public int SegmentLength = 130;
    public int SegmentsCount = 10;
    public int Delay = 7;
	
	void Start ()
	{
	    _lineRenderer = GetComponent<LineRenderer>();
        _ambientLight = RenderSettings.ambientLight;
	    _timer = Random.Range(0, 5);
        MakeLighting();
	}

    private List<Vector3> MakeLightingPoints(int level, Vector3 pos)
    {
        var points = new List<Vector3>();

        var newPos = pos + new Vector3((Random.value - 0.5F)*SegmentLength, -Random.value*SegmentLength/2, (Random.value - 0.5F)*SegmentLength);
        points.Add(newPos);

        if (level < SegmentsCount)
            points.AddRange(MakeLightingPoints(level + 1, newPos));

        if (Random.Range(0, 4) == 0)
            points.AddRange(MakeLightingPoints(level + 1, newPos));

        points.Add(pos);

        return points;
    }
    

    private void MakeLighting()
    {
        var points = MakeLightingPoints(0, new Vector3(0, 0, 0));
        _lineRenderer.SetVertexCount(points.Count);
        for (var i = 0; i < points.Count; i++)
            _lineRenderer.SetPosition(i, points[i]);
    }


    private float _timer;
    private int _state = 0;
    private Color _ambientLight;

	// Update is called once per frame
	void Update ()
	{
	    _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            switch (_state)
            {
                case 0:
                    MakeLighting();
                    _timer = 0.2F;
                    _state = 1;
                    RenderSettings.ambientLight = Color.white;
                    break;
                case 1:
                    _lineRenderer.SetVertexCount(0);
                    _timer = Delay + Random.value * 2 - 1;
                    _state = 0;
                    RenderSettings.ambientLight = _ambientLight;
                    break;
            }
        }
	}
}
