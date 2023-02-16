using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraItem : Item
{

  public override void Drop()
  {
    Debug.Log("CameraItem dropped");
  }


  [SerializeField][Range(0, 180)] private float rayAngle = 15f;
  [SerializeField][Range(0, 10)] private float rayLength = 5f;

  [Header("Light")]
  [SerializeField][Range(0, 15)] private float flashIntencity = 0.8f;

  [SerializeField] private GameObject lightSource;

  protected override void Update()
  {
    base.Update();
    TrackMouse();
  }

  private void TrackMouse()
  {
    var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    var lightPos = lightSource.transform.position;

    var dif = mousePos - lightPos;
    var sign = (dif.x > 0) ? -1 : 1;
    var a = new Vector2(dif.x, dif.y);
    var angle = Vector2.Angle(Vector2.up, a) * sign;
    var rotationVector = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
    lightSource.transform.rotation = rotationVector;
  }

  private IEnumerator FlashCoroutine()
  {
    var light = lightSource.GetComponent<Light2D>();

    light.intensity = flashIntencity;

    yield return new WaitForSeconds(0.1f);

    light.intensity = 0;
  }


  protected override void Action(Player player)
  {
    // When used, emit a conecast from the player's camera


    var start = player.transform.position;
    var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    var direction = target - start;

    direction = direction.normalized * rayLength;


    StartCoroutine(FlashCoroutine());

    var topVector = Quaternion.AngleAxis(rayAngle, Vector3.forward) * direction;
    var bottomVector = Quaternion.AngleAxis(-rayAngle, Vector3.forward) * direction;

    var leftLimit = rayAngle;
    var rightLimit = -rayAngle;

    var splitWidth = 5f;
    var rayQantity = rayAngle * 2 / splitWidth;

    var vectors = new List<Vector3>();

    for (var i = 0; i <= rayQantity; i++)
    {
      var vector = Quaternion.AngleAxis(splitWidth * i - rayAngle, Vector3.forward) * direction;

      Debug.DrawRay(start, vector, Color.green, 1f);
      vectors.Add(vector);
    }


    foreach (var vec in vectors)
    {
      var hit = Physics2D.Raycast(start, vec, rayLength, LayerMask.GetMask("Walls"));


      if (hit.collider != null)
      {
        var hitpoint = hit.point;

        var mark = new GameObject("Mark");
        mark.transform.position = hitpoint;
        mark.AddComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
      }


    }

    // var topRay = Physics2D.Raycast(start, topVector, rayLength, LayerMask.GetMask("Walls"));
    // var bottomRay = Physics2D.Raycast(start, bottomVector, rayLength, LayerMask.GetMask("Walls"));
    // var centerRay = Physics2D.Raycast(start, direction, rayLength, LayerMask.GetMask("Walls"));

    // var rays = new List<RaycastHit2D> { topRay, bottomRay, centerRay };

    // foreach (var ray in rays)
    // {
    //   if (ray.collider != null)
    //   {
    //     var hitpoint = ray.point;

    //     var mark = new GameObject("Mark");
    //     mark.transform.position = hitpoint;
    //     mark.AddComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
    //   }
    // }




  }


}