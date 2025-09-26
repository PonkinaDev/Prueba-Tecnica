using UnityEngine;
using UnityEngine.Events;

// Ensures a Collider is present for mouse interaction
[RequireComponent(typeof(Collider))]
public class OptionController : MonoBehaviour
{
    // Option letter, set via Inspector
    [SerializeField] private string OptionLetter;

    // Event invoked when option is clicked, passes OptionLetter
    public UnityEvent<string> onClick;

    void OnMouseDown()
    {
        onClick.Invoke(OptionLetter); 
    }
}

