using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Editor script to automatically set up the ResultPanel in the RpsBotScene
/// Run this from the Unity Editor menu: GameObject > UI > Setup Result Panel
/// </summary>
public class ResultPanelSetup
{
    [MenuItem("GameObject/UI/Setup Result Panel")]
    public static void SetupResultPanel()
    {
        Debug.Log("[ResultPanelSetup] Starting ResultPanel setup...");

        // Find Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[ResultPanelSetup] No Canvas found in scene! Please create a Canvas first.");
            return;
        }

        // Check if ResultPanel already exists
        Transform existingPanel = canvas.transform.Find("ResultPanel");
        if (existingPanel != null)
        {
            Debug.LogWarning("[ResultPanelSetup] ResultPanel already exists! Deleting old version...");
            Object.DestroyImmediate(existingPanel.gameObject);
        }

        // 1. Create ResultPanel
        GameObject resultPanelObj = new GameObject("ResultPanel");
        resultPanelObj.transform.SetParent(canvas.transform, false);

        // Add Image component (for background)
        Image panelImage = resultPanelObj.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.85f); // Semi-transparent black

        // Add CanvasGroup for fade animations
        CanvasGroup canvasGroup = resultPanelObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Start hidden
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Configure RectTransform
        RectTransform panelRect = resultPanelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(400f, 300f);

        Debug.Log("[ResultPanelSetup] Created ResultPanel with CanvasGroup");

        // 2. Find or create ResultText
        TextMeshProUGUI resultText = null;
        Transform existingResultText = canvas.transform.Find("ResultText");

        if (existingResultText != null)
        {
            // Reparent existing ResultText
            existingResultText.SetParent(resultPanelObj.transform, false);
            resultText = existingResultText.GetComponent<TextMeshProUGUI>();
            Debug.Log("[ResultPanelSetup] Reparented existing ResultText");
        }
        else
        {
            // Create new ResultText
            GameObject resultTextObj = new GameObject("ResultText");
            resultTextObj.transform.SetParent(resultPanelObj.transform, false);

            resultText = resultTextObj.AddComponent<TextMeshProUGUI>();
            resultText.fontSize = 120f;
            resultText.alignment = TextAlignmentOptions.Center;
            resultText.text = "승"; // Default text

            RectTransform textRect = resultTextObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(0f, 50f);
            textRect.sizeDelta = new Vector2(300f, 150f);

            Debug.Log("[ResultPanelSetup] Created new ResultText");
        }

        // 3. Find or create RestartBtn
        Button restartBtn = null;
        Transform existingRestartBtn = canvas.transform.Find("RestartBtn");

        if (existingRestartBtn != null)
        {
            // Reparent existing RestartBtn
            existingRestartBtn.SetParent(resultPanelObj.transform, false);
            restartBtn = existingRestartBtn.GetComponent<Button>();
            Debug.Log("[ResultPanelSetup] Reparented existing RestartBtn");
        }
        else
        {
            // Create new RestartBtn
            GameObject restartBtnObj = new GameObject("RestartBtn");
            restartBtnObj.transform.SetParent(resultPanelObj.transform, false);

            // Add Image component for button background
            Image btnImage = restartBtnObj.AddComponent<Image>();
            btnImage.color = Color.white; // White background

            restartBtn = restartBtnObj.AddComponent<Button>();

            // Create button text child
            GameObject btnTextObj = new GameObject("Text");
            btnTextObj.transform.SetParent(restartBtnObj.transform, false);

            TextMeshProUGUI btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
            btnText.fontSize = 36f;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.text = "다시하기";
            btnText.color = Color.black;

            RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.sizeDelta = Vector2.zero;
            btnTextRect.offsetMin = Vector2.zero;
            btnTextRect.offsetMax = Vector2.zero;

            RectTransform btnRect = restartBtnObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = new Vector2(0f, -80f);
            btnRect.sizeDelta = new Vector2(200f, 60f);

            Debug.Log("[ResultPanelSetup] Created new RestartBtn");
        }

        // 4. Link RpsUIManager reference
        RpsUIManager uiManager = Object.FindFirstObjectByType<RpsUIManager>();
        if (uiManager != null)
        {
            // Find the resultPanel field in RpsUIManager and assign it
            SerializedObject serializedManager = new SerializedObject(uiManager);
            SerializedProperty resultPanelProp = serializedManager.FindProperty("resultPanel");

            if (resultPanelProp != null)
            {
                resultPanelProp.objectReferenceValue = resultPanelObj;
                serializedManager.ApplyModifiedProperties();
                Debug.Log("[ResultPanelSetup] Linked ResultPanel to RpsUIManager");
            }
            else
            {
                Debug.LogWarning("[ResultPanelSetup] Could not find 'resultPanel' field in RpsUIManager");
            }
        }
        else
        {
            Debug.LogWarning("[ResultPanelSetup] Could not find RpsUIManager in scene");
        }

        // Select the ResultPanel in hierarchy
        Selection.activeGameObject = resultPanelObj;

        Debug.Log("[ResultPanelSetup] ResultPanel setup complete! Please verify the configuration in Inspector.");
    }
}
