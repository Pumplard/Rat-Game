using UnityEngine;
public class MapCamera : MonoBehaviour
{
    //Camera will move with cursor
    private Camera cameraComponent;
    public float mvSpeed = 20f;

    private float z = -10f;
    private float leftX = 10.16f;
    private float rightX = 12.84f;
    private float bottomY = 5.5f;
    private float topY = 17.5f; 

    private float rightXFloor;

    void Awake()
    {
        cameraComponent = GetComponent<Camera>();
    }


    //called in cursor manager
    //public void MoveCamera(Vector2 newDirection) {
     //   Vector3 direction = newDirection;
    //    transform.position += direction;
    //}

    //called in cursor manager
    public void SetPos(Vector2 newPos) {
        transform.position = new Vector3(newPos.x, newPos.y, z);
        //transform.position = new Vector3(newPos.x + .16f, newPos.y + .5f, z); //block offset
        //SetInBounds();
    }

    //keeps camera in bounds
    private void SetInBounds() {
        if (transform.position.x < leftX) {
            transform.position = new Vector3(leftX, transform.position.y, z);
        } 
        else if (transform.position.x > rightX) {
            transform.position = new Vector3(rightX, transform.position.y, z);
        }
        if (transform.position.y < bottomY) {
            transform.position = new Vector3(transform.position.x, bottomY, z);
        } 
        else if (transform.position.y > topY) {
            transform.position = new Vector3(transform.position.x, topY, z);
        } 
    }
}
