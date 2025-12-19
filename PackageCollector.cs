using UnityEngine;

public class PackageCollector : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private string packageTag = "Package";
    [SerializeField] private Camera arCamera;

    private void Awake() {
        if (arCamera == null)
            arCamera = GetComponent<Camera>();
    }

    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                TryCollectPackage(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0)) {
            TryCollectPackage(Input.mousePosition);
        }
    }

    private void TryCollectPackage(Vector2 touchPosition) {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.CompareTag(packageTag)) {
                GameManager.Instance.AddPackage();
                Destroy(hit.collider.gameObject);
            }
        }
    }
}