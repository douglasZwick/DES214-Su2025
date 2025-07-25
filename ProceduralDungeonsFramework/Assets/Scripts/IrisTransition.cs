using UnityEngine;
using UnityEngine.Events;


public class IrisTransition : MonoBehaviour
{
  static public IrisEvent s_OpenRequest = new();
  static public IrisEvent s_CloseRequest = new();

  public SpriteRenderer m_CardRenderer;
  public Transform m_IrisTx;
  public Camera m_Camera;
  public float m_Duration = 2;

  bool m_Running = false;
  bool m_Opening = false;
  bool m_IsOpen = true;
  float m_Timer = 0;
  Vector3 m_StartScale;
  Vector3 m_EndScale;

  // TODO: EXTREMELY UNLIKELY, BUT: If I ever change this game to use a
  //   perspective camera, these getters will probably be a little bit wrong,
  //   because I'm using the wrong Z value here. Of course, if I do do that,
  //   then I should probably just do this iris business in UI space anyway.
  Vector3 LBWorldPoint
  {
    get
    {
      return m_Camera.ViewportToWorldPoint(Vector3.zero);
    }
  }

  Vector3 RTWorldPoint
  {
    get
    {
      return m_Camera.ViewportToWorldPoint(Vector3.one);
    }
  }


  void Awake()
  {
    s_OpenRequest.AddListener(OnOpenRequest);
    s_CloseRequest.AddListener(OnCloseRequest);
  }


  void Update()
  {
    if (m_Running)
      Run();
  }


  void OnOpenRequest(IrisEventData irisED)
  {
    OpenFromWorldPoint(irisED.m_WorldPoint);
  }


  void OnCloseRequest(IrisEventData irisED)
  {
    CloseToWorldPoint(irisED.m_WorldPoint);
  }


  void OpenFromWorldPoint(Vector3 worldPoint)
  {
    m_Opening = true;

    m_IrisTx.position = worldPoint;
    var fromScale = new Vector3(0, 0, 1);
    var toScale = ComputeOpenScaleForWorldPoint(worldPoint);

    Begin(fromScale, toScale);
  }


  void CloseToWorldPoint(Vector3 worldPoint)
  {
    m_Opening = false;

    m_IrisTx.position = worldPoint;
    var fromScale = ComputeOpenScaleForWorldPoint(worldPoint);
    var toScale = new Vector3(0, 0, 1);

    Begin(fromScale, toScale);

    InputHold.Add(this);
  }


  void Begin(Vector3 fromScale, Vector3 toScale)
  {
    m_CardRenderer.enabled = true;
    m_IrisTx.gameObject.SetActive(true);

    m_IrisTx.localScale = fromScale;
    m_StartScale = fromScale;
    m_EndScale = toScale;
    m_Timer = 0;
    m_Running = true;
  }


  void Run()
  {
    m_Timer += Time.deltaTime;

    var frac = m_Timer / m_Duration;
    var irisLocalScale = Vector3.Lerp(m_StartScale, m_EndScale, frac);
    m_IrisTx.localScale = irisLocalScale;

    if (m_Timer >= m_Duration)
    {
      End();
    }
  }


  void End()
  {
    m_Running = false;

    if (m_Opening)
    {
      m_CardRenderer.enabled = false;
      m_IsOpen = true;

      InputHold.Release(this);
    }
    else
    {
      m_IrisTx.gameObject.SetActive(false);
      m_IsOpen = false;
    }
  }


  Vector3 ComputeOpenScaleForWorldPoint(Vector3 worldPoint)
  {
    var farthestCorner = FarthestCornerFromWorldPoint(worldPoint);
    var xyScale = 2 * (farthestCorner - worldPoint).magnitude;

    return new Vector3(xyScale, xyScale, 1);
  }


  Vector3 FarthestCornerFromWorldPoint(Vector3 worldPoint)
  {
    var lb = LBWorldPoint;
    var rt = RTWorldPoint;
    var r = Mathf.Abs(rt.x - worldPoint.x);
    var l = Mathf.Abs(lb.x - worldPoint.x);
    var t = Mathf.Abs(rt.y - worldPoint.y);
    var b = Mathf.Abs(lb.y - worldPoint.y);

    if (r >= l)
    {
      if (t >= b)
      {
        return rt;
      }

      return new Vector3(rt.x, lb.y);
    }

    if (t >= b)
    {
      return new Vector3(lb.x, rt.y);
    }

    return lb;
  }
}


[System.Serializable]
public class IrisEvent : UnityEvent<IrisEventData> { }

public class IrisEventData
{
  public Vector3 m_WorldPoint;
  // public Transform m_TargetObject;
}
