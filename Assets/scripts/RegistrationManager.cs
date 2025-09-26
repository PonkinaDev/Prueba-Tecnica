using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;

public class RegistrationManager : MonoBehaviour
{
    public static RegistrationManager Instance;

    [Header("First Scene Only")]
    public TMP_InputField NameText;
    public TMP_InputField EmailText;

    [Header("Error Panel")]
    public GameObject errorPanel;
    public TMP_Text errorText;

    [Header("Photo Panel")]
    public GameObject photoPanel; // Initially inactive
    public Button btnSubir;       // Upload photo button
    public Button btnTomar;       // Take photo button
    public Button btnCancelar;   // Cancel button
    public Button btnCapturar;   // Capture button when camera is active
    public RawImage previewImage; // Photo preview display

    public string UserName { get; private set; } = "Pau"; // Final saved name
    private string UserEmail = "Pau@ejemplo.com"; // Final saved email

    private Texture2D userPhoto; // Final saved photo
    private WebCamTexture webcamTex; // Camera for Android or PC
    private bool isCameraActive = false; // Camera state

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Auto-find UI elements if we're in 01Register scene
            if (SceneManager.GetActiveScene().name == "01Register")
            {
                FindUIElementsAutomatically();
            }

            // Initialize panel states
            if (photoPanel != null) photoPanel.SetActive(false);
            if (errorPanel != null) errorPanel.SetActive(false);

            // Setup button events
            SetupButtonEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Automatically find UI elements by name if not assigned in inspector
    /// </summary>
    private void FindUIElementsAutomatically()
    {
        // Find text input fields if not assigned
        if (NameText == null)
        {
            NameText = FindObjectByName<TMP_InputField>("NamePlaceHolder");
        }

        if (EmailText == null)
        {
            EmailText = FindObjectByName<TMP_InputField>("MailPlaceHolder");
        }

        // Find error panel elements
        if (errorPanel == null)
        {
            errorPanel = FindGameObjectByName("ErrorMessagePanel");
        }

        if (errorText == null)
        {
            errorText = FindObjectByName<TMP_Text>("ErrorMessage");
        }

        // Find photo panel elements
        if (photoPanel == null)
        {
            photoPanel = FindGameObjectByName("PhotoOptionsPanel");
        }

        if (btnSubir == null)
        {
            btnSubir = FindObjectByName<Button>("ButtonUpload");
        }

        if (btnTomar == null)
        {
            btnTomar = FindObjectByName<Button>("ButtonTake");
        }

        if (btnCancelar == null)
        {
            btnCancelar = FindObjectByName<Button>("ButtonCancel");
        }

        if (btnCapturar == null)
        {
            btnCapturar = FindObjectByName<Button>("ButtonCapture");
        }

        if (previewImage == null)
        {
            previewImage = FindObjectByName<RawImage>("RawImage");
        }
    }

    /// <summary>
    /// Helper method to find objects by name
    /// </summary>
    private T FindObjectByName<T>(string name) where T : Component
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            return obj.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// Helper method to find GameObjects by name
    /// </summary>
    private GameObject FindGameObjectByName(string name)
    {
        return GameObject.Find(name);
    }

    /// <summary>
    /// Configure button events if buttons exist
    /// </summary>
    private void SetupButtonEvents()
    {
        if (btnSubir != null) 
        {
            btnSubir.onClick.AddListener(OnClickSubir);
            
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            // Disable upload button on Windows standalone
            btnSubir.interactable = false;
            
            // Add tooltip or visual indication (optional)
            var buttonText = btnSubir.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "Subir (No disponible)";
                buttonText.color = Color.gray;
            }
#endif
        }
        
        if (btnTomar != null) btnTomar.onClick.AddListener(OnClickTomar);
        if (btnCancelar != null) btnCancelar.onClick.AddListener(ClosePhotoPanel);
        if (btnCapturar != null)
        {
            btnCapturar.onClick.AddListener(OnClickCapturar);
            btnCapturar.gameObject.SetActive(false); // Initially hidden
        }
    }

    /// <summary>
    /// Load scene after validating user data
    /// </summary>
    public void LoadScene(string sceneName)
    {
        // Capture current form data
        if (NameText != null) UserName = NameText.text.Trim();
        if (EmailText != null) UserEmail = EmailText.text.Trim();

        if (IsValidData())
        {
            if (errorPanel != null) errorPanel.SetActive(false);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            ShowError(GetErrorMessage());
        }
    }

    /// <summary>
    /// Handle scene loading events
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-initialize UI elements if we load the registration scene again
        if (scene.name == "01Register" && Instance == this)
        {
            FindUIElementsAutomatically();
            SetupButtonEvents();
        }
    }

    #region Validation

    /// <summary>
    /// Validate all required user data
    /// </summary>
    private bool IsValidData()
    {
        if (string.IsNullOrEmpty(UserName)) return false;
        if (string.IsNullOrEmpty(UserEmail)) return false;

        // Basic email validation
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(UserEmail, emailPattern)) return false;

        // Photo is required
        if (userPhoto == null) return false;

        return true;
    }

    /// <summary>
    /// Get appropriate error message based on validation state
    /// </summary>
    private string GetErrorMessage()
    {
        if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(UserEmail))
            return "Por favor ingresa tu nombre y correo.";
        else if (string.IsNullOrEmpty(UserName))
            return "Por favor ingresa tu nombre.";
        else if (string.IsNullOrEmpty(UserEmail))
            return "Por favor ingresa tu correo.";
        else if (userPhoto == null)
            return "Por favor selecciona o toma una foto.";

        return "El correo ingresado no es válido.";
    }

    /// <summary>
    /// Display error message to user
    /// </summary>
    private void ShowError(string message)
    {
        if (errorPanel != null)
        {
            errorPanel.SetActive(true);
            if (errorText != null) errorText.text = message;
        }
    }

    #endregion

    #region Photo Management

    /// <summary>
    /// Show photo options panel
    /// </summary>
    public void OpenPhotoPanel()
    {
        if (photoPanel != null) photoPanel.SetActive(true);
    }

    /// <summary>
    /// Hide photo panel and stop camera if active
    /// </summary>
    private void ClosePhotoPanel()
    {
        if (photoPanel != null) photoPanel.SetActive(false);
        StopCamera();
    }

    /// <summary>
    /// Handle upload photo button click
    /// </summary>
    private void OnClickSubir()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Use NativeFilePicker for mobile platforms
        NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                LoadImageFromPath(path);
                HideCaptureButton();
            }
        }, new string[] { "image/*" });
#elif UNITY_EDITOR
        // Use Unity Editor file dialog in editor only
        string path = UnityEditor.EditorUtility.OpenFilePanel("Seleccionar imagen", "", "png,jpg,jpeg");
        if (!string.IsNullOrEmpty(path))
        {
            LoadImageFromPath(path);
            HideCaptureButton();
        }
#else
        // For Windows standalone, disable upload button and show message
        if (btnSubir != null) 
        {
            btnSubir.interactable = false;
        }
        ShowError("Subir foto no disponible en Windows. Use 'Tomar foto' para capturar una imagen.");
#endif
    }

    /// <summary>
    /// Load image from file path
    /// </summary>
    private void LoadImageFromPath(string path)
    {
        try
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            
            if (tex.LoadImage(fileData))
            {
                userPhoto = tex;
                
                if (previewImage != null)
                {
                    previewImage.texture = userPhoto;
                    previewImage.uvRect = new Rect(0, 0, 1, 1); // Reset UV rect
                }
                
                Debug.Log($"Image loaded successfully: {path}");
            }
            else
            {
                ShowError("No se pudo cargar la imagen seleccionada.");
                Destroy(tex);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading image: {e.Message}");
            ShowError("Error al cargar la imagen seleccionada.");
        }
    }

    /// <summary>
    /// Handle take photo button click
    /// </summary>
    private void OnClickTomar()
    {
        StartCoroutine(StartCameraCoroutine());
    }

    /// <summary>
    /// Initialize camera for photo capture
    /// </summary>
    private System.Collections.IEnumerator StartCameraCoroutine()
    {
        StopCamera(); // Stop any existing camera

        // Request camera permissions on mobile
#if UNITY_ANDROID || UNITY_IOS
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield break;
            }
        }
#endif

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            string selectedCamera = SelectBestCamera(devices);

            // Create WebCamTexture with reasonable settings
            webcamTex = new WebCamTexture(selectedCamera);
            webcamTex.requestedWidth = 640;
            webcamTex.requestedHeight = 480;
            webcamTex.requestedFPS = 30;

            webcamTex.Play();

            // Wait for camera initialization
            yield return WaitForCameraInitialization();

            if (webcamTex == null) yield break; // Camera failed to initialize

            ConfigureCameraPreview();
            ShowCaptureButton();
            isCameraActive = true;
        }
    }

    /// <summary>
    /// Select the best available camera (prefer back camera)
    /// </summary>
    private string SelectBestCamera(WebCamDevice[] devices)
    {
        string selectedCamera = "";
        
        foreach (WebCamDevice device in devices)
        {
            if (!device.isFrontFacing && string.IsNullOrEmpty(selectedCamera))
            {
                selectedCamera = device.name; // Prefer back camera
            }
            else if (string.IsNullOrEmpty(selectedCamera))
            {
                selectedCamera = device.name; // Use any available camera
            }
        }

        return string.IsNullOrEmpty(selectedCamera) ? devices[0].name : selectedCamera;
    }

    /// <summary>
    /// Wait for camera to initialize with timeout
    /// </summary>
    private System.Collections.IEnumerator WaitForCameraInitialization()
    {
        float timeout = 10f;
        float timer = 0f;

        while (!webcamTex.didUpdateThisFrame && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (timer >= timeout)
        {
            StopCamera();
        }
    }

    /// <summary>
    /// Configure camera preview display
    /// </summary>
    private void ConfigureCameraPreview()
    {
        if (previewImage != null && webcamTex != null)
        {
            previewImage.texture = webcamTex;

            RectTransform rectTransform = previewImage.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                ConfigureCameraAspectRatio(rectTransform);
            }

            // Remove AspectRatioFitter to avoid conflicts
            AspectRatioFitter aspectRatio = previewImage.GetComponent<AspectRatioFitter>();
            if (aspectRatio != null)
            {
                DestroyImmediate(aspectRatio);
            }
        }
    }

    /// <summary>
    /// Configure camera aspect ratio to fit container
    /// </summary>
    private void ConfigureCameraAspectRatio(RectTransform rectTransform)
    {
        Vector2 originalSize = rectTransform.sizeDelta;
        float cameraAspect = (float)webcamTex.width / webcamTex.height;
        float containerAspect = originalSize.x / originalSize.y;

        // Adjust UV Rect to maintain aspect ratio
        if (cameraAspect > containerAspect)
        {
            // Camera is wider - adjust height
            float scale = containerAspect / cameraAspect;
            previewImage.uvRect = new Rect(0, (1f - scale) * 0.5f, 1, scale);
        }
        else
        {
            // Camera is taller - adjust width
            float scale = cameraAspect / containerAspect;
            previewImage.uvRect = new Rect((1f - scale) * 0.5f, 0, scale, 1);
        }
    }

    /// <summary>
    /// Handle capture photo button click
    /// </summary>
    private void OnClickCapturar()
    {
        if (webcamTex != null && webcamTex.isPlaying && webcamTex.didUpdateThisFrame)
        {
            Texture2D capturedPhoto = CaptureWebcamFrame();
            userPhoto = capturedPhoto;
            
            DisplayCapturedPhoto();
            StopCamera();
            HideCaptureButton();
        }
    }

    /// <summary>
    /// Capture current webcam frame as Texture2D
    /// </summary>
    private Texture2D CaptureWebcamFrame()
    {
        Texture2D capturedPhoto = new Texture2D(webcamTex.width, webcamTex.height, TextureFormat.RGB24, false);
        Color[] pixels = webcamTex.GetPixels();

        // Apply rotation if needed
        if (webcamTex.videoRotationAngle != 0)
        {
            pixels = RotateImagePixels(pixels, webcamTex.width, webcamTex.height, webcamTex.videoRotationAngle);
        }

        capturedPhoto.SetPixels(pixels);
        capturedPhoto.Apply();
        
        return capturedPhoto;
    }

    /// <summary>
    /// Display captured photo in preview
    /// </summary>
    private void DisplayCapturedPhoto()
    {
        if (previewImage != null && userPhoto != null)
        {
            previewImage.texture = userPhoto;
            previewImage.uvRect = new Rect(0, 0, 1, 1); // Reset UV rect

            // Configure aspect ratio for captured photo
            RectTransform rectTransform = previewImage.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                ConfigureCapturedPhotoAspectRatio(rectTransform);
            }
        }
    }

    /// <summary>
    /// Configure aspect ratio for captured photo display
    /// </summary>
    private void ConfigureCapturedPhotoAspectRatio(RectTransform rectTransform)
    {
        Vector2 containerSize = rectTransform.sizeDelta;
        float photoAspect = (float)userPhoto.width / userPhoto.height;
        float containerAspect = containerSize.x / containerSize.y;

        // Center image without deformation
        if (photoAspect > containerAspect)
        {
            // Photo is wider
            float scale = containerAspect / photoAspect;
            previewImage.uvRect = new Rect(0, (1f - scale) * 0.5f, 1, scale);
        }
        else
        {
            // Photo is taller
            float scale = photoAspect / containerAspect;
            previewImage.uvRect = new Rect((1f - scale) * 0.5f, 0, scale, 1);
        }
    }

    /// <summary>
    /// Rotate image pixels by specified angle
    /// </summary>
    private Color[] RotateImagePixels(Color[] pixels, int width, int height, int angle)
    {
        Color[] rotatedPixels = new Color[pixels.Length];

        switch (angle)
        {
            case 90:
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        rotatedPixels[i * height + j] = pixels[(height - j - 1) * width + i];
                    }
                }
                break;
            case 180:
                for (int i = 0; i < pixels.Length; i++)
                {
                    rotatedPixels[i] = pixels[pixels.Length - 1 - i];
                }
                break;
            case 270:
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        rotatedPixels[i * height + j] = pixels[j * width + (width - i - 1)];
                    }
                }
                break;
            default:
                return pixels;
        }

        return rotatedPixels;
    }

    /// <summary>
    /// Stop camera and cleanup resources
    /// </summary>
    private void StopCamera()
    {
        if (webcamTex != null && webcamTex.isPlaying)
        {
            webcamTex.Stop();
            webcamTex = null;
        }
        isCameraActive = false;
    }

    /// <summary>
    /// Show capture button when camera is active
    /// </summary>
    private void ShowCaptureButton()
    {
        if (btnCapturar != null) btnCapturar.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide capture button
    /// </summary>
    private void HideCaptureButton()
    {
        if (btnCapturar != null) btnCapturar.gameObject.SetActive(false);
    }

    #endregion

    #region Public Accessors

    /// <summary>
    /// Get registered user name
    /// </summary>
    public string GetName() => UserName;

    /// <summary>
    /// Get registered user email
    /// </summary>
    public string GetMail() => UserEmail;

    /// <summary>
    /// Get user photo texture
    /// </summary>
    public Texture2D GetPhoto() => userPhoto;

    /// <summary>
    /// Check if camera is currently active
    /// </summary>
    public bool IsCameraActive() => isCameraActive;

    #endregion

    void OnDestroy()
    {
        StopCamera();
    }
}