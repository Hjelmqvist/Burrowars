using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; protected set; } 

    [SerializeField] List<InputController> controllers = new List<InputController>();
    public List<InputController> GetControllers() { return controllers; }

    public bool IsFull { get { return controllers.Count >= 4; } }

    public delegate void Change(List<InputController> controllers);
    public event Change OnChange;

    //For updating controllers information
    delegate void UpdateControllers();
    event UpdateControllers OnUpdate;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
    }

    void Update()
    {
        //Lets the controllers update information without deriving from MonoBehaviour
        OnUpdate?.Invoke();
    }

    /// <summary>
    /// Adds the controller to list of controllers if the list is not full already<para/>
    /// Returns true if the controller could be added, else false.
    /// </summary>
    public bool AddController(InputController controller)
    {
        if (!IsFull)
        {
            controllers.Add(controller);
            OnChange?.Invoke(controllers);
            OnUpdate += controller.Update;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true if controller was removed from the list, else false.
    /// </summary>
    public bool RemoveController(InputController controller)
    {
        if (controllers.Remove(controller))
        {
            OnChange?.Invoke(controllers);
            OnUpdate -= controller.Update;
            return true;
        }
        return false;
    }
}
