
# Proyecto AR Foundation: Recogida de Paquetes

**Asignatura:** Ingeniería Informática - Desarrollo Móvil / Realidad Aumentada  
**Dispositivo de Pruebas:** Xiaomi 14T (HyperOS/Android)

---

## 1. Descripción del Proyecto

Modificación del "Google ARCore Codelab" para implementar una mecánica de juego:

- Detección de planos
- Colocación de paquetes virtuales
- Sistema de Raycast para recoger paquetes tocando la pantalla
- UI con contador de puntuación (Patrón Singleton)

---

## 2. Bitácora de Problemas y Soluciones (Troubleshooting Log)

Durante el desarrollo e implementación en el dispositivo físico, se encontraron y resolvieron los siguientes bloqueos técnicos:

### A. Problemas de Despliegue (Deployment)

**Error:** `No Android devices connected`

- **Síntoma:** Unity no detecta el móvil aunque esté conectado por USB.
- **Causa:** Falta de permisos ADB o drivers incorrectos en Windows.
- **Solución:**
  - Activar Opciones de Desarrollador (Tocar 7 veces 'Versión del SO')
  - Seleccionar modo USB: Transferencia de archivos / Android Auto (si falla, usar PTP)
  - Aceptar la huella RSA en el pop-up del móvil

**Error:** `Installation Failed / User Rejected` (Xiaomi Específico)

- **Síntoma:** Unity compila pero falla al instalar el APK (`INSTALL_FAILED_USER_RESTRICTED`).
- **Causa:** Seguridad adicional de Xiaomi/HyperOS.
- **Solución:**
  - Insertar tarjeta SIM e iniciar sesión en Cuenta Mi
  - En Opciones de Desarrollador, activar "Instalar vía USB"
  - Desactivar optimización MIUI/HyperOS si persiste

### B. Problemas de Compilación (API Conflicts)

**Error:** `InvalidOperationException: Input System package is not enabled`

- **Síntoma:** Consola muestra error al intentar leer `Input.GetTouch`.
- **Causa:** El proyecto usaba el "New Input System" pero el código del tutorial usaba la clase legacy Input.
- **Solución:**
  - `Project Settings > Player > Other Settings > Active Input Handling` → Cambiar a **Both**

**Error:** `ARPlanesChangedEventArgs is obsolete`

- **Síntoma:** Error de compilación en `DrivingSurfaceManager.cs`.
- **Causa:** El tutorial es de 2021 (ARFoundation 4) y se está usando la versión 5.0/6.0.
- **Solución:** Actualizar la API:
  - Cambiar `planesChanged` por `trackablesChanged`
  - Cambiar el argumento del evento a `ARTrackablesChangedEventArgs<ARPlane>`

### C. Problemas de Ejecución (Runtime)

**Error:** `NullReferenceException` en bucle

- **Síntoma:** La consola se llena de errores rojos en tiempo de ejecución.
- **Causa:** Referencias no asignadas en el Inspector.
- **Solución:** Arrastrar el objeto XR Origin (que contiene el `DrivingSurfaceManager`) al campo público del script `ReticleBehaviour`.

### D. Problemas de Renderizado (Graphics)

**Error:** Pantalla amarilla / sin video de cámara

- **Síntoma:** La UI se ve bien, pero el fondo es un color sólido (amarillo) en lugar de la realidad.
- **Causa:** Conflicto entre ARCore y la API gráfica Vulkan en Unity URP.
- **Solución:**
  - `Project Settings > Player > Android > Auto Graphics API` (Desmarcar)
  - Eliminar Vulkan de la lista
  - Dejar solo OpenGLES3

No todas las soluciones funcionaron, realmente no pude hacer funcionar el renderizado aunque lo probé de multiples maneras pero listaré lo que pensaba hacer.

---

## 3. Scripts Finales Implementados

### GameManager.cs (Lógica y UI)

```csharp
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI scoreText;
    private int collectedPackages = 0;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddPackage() {
        collectedPackages++;
        if (scoreText != null) scoreText.text = $"Paquetes: {collectedPackages}";
    }
}
```

### PackageCollector.cs (Interacción AR)

```csharp
using UnityEngine;

public class PackageCollector : MonoBehaviour {
    [SerializeField] private string packageTag = "Package";
    [SerializeField] private Camera arCamera;

    private void Awake() {
        if (arCamera == null) arCamera = GetComponent<Camera>();
    }

    void Update() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.CompareTag(packageTag)) {
                    GameManager.Instance.AddPackage();
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}
```

---

## 4. Configuración del Editor Recomendada

Parece que los problemas se ven influenciados por la configuración del proyecto en Unity. Puede que usar la versión de Unity que especifica Google en el tutorial ayude a evitar conflictos **Unity Version:** 2022.3 LTS. En este caso usamos la **6000.2.6f2** pues es la más reciente y no tiene problemas de seguridad detectados.
