using UnityEngine;

public class CircleAreaHighlighter : MonoBehaviour
{
    public SpriteRenderer circleRenderer;
    public Color player1Highlight = new Color(0, 0, 1, 0.1f);
    public Color player2Highlight = new Color(1, 0, 0, 0.1f);
    public Color defaultColor = new Color(1, 1, 1, 0.05f);

    void Update()
    {
        if (circleRenderer == null || GameManager.Instance == null) return;

        if (GameManager.Instance.IsGameOver())
        {
            circleRenderer.color = defaultColor;
            return;
        }

        // Highlight based on current turn
        if (GameManager.Instance.GetCurrentTurn() == PlayerTurn.Player1)
        {
            circleRenderer.color = player1Highlight;
        }
        else
        {
            circleRenderer.color = player2Highlight;
        }
    }
}