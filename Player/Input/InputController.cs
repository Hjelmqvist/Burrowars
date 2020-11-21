using UnityEngine;

[System.Serializable]
public class InputController
{
    [SerializeField] ControllerType controller;
    public ControllerType Controller
    {
        get { return controller; }
        protected set { controller = value; }
    }

    public InputController(ControllerType controller)
    {
        this.controller = controller;
    }

    public delegate void CurrentAxis(Axis axis, float value, InputController controller);
    public event CurrentAxis OnCurrentAxis;

    public delegate void ButtonDown(Action button, InputController controller);
    public event ButtonDown OnButtonDown;

    public delegate void ButtonUp(Action button, InputController controller);
    public event ButtonDown OnButtonUp;

    public void Update()
    {
        for (Axis i = Axis.MoveHorizontal; i <= Axis.Shoot; i++)
        {
            float value = Input.GetAxisRaw(string.Format("{0} {1}", Controller, i.ToString()));
            OnCurrentAxis?.Invoke(i, value, this);

            //Keyboard uses mouse for rotation
            if (controller == ControllerType.Keyboard && i == Axis.MoveVertical)
                break;
        }

        for (Action i = Action.Start; i <= Action.Heal; i++)
        {
            //Joysticks shoots with triggers (axis)
            if (controller != ControllerType.Keyboard && i == Action.Shoot)
                continue;

            string s = string.Format("{0} {1}", Controller, i.ToString());

            if (Input.GetButtonDown(s))
                OnButtonDown?.Invoke(i, this);

            if (Input.GetButtonUp(s))
                OnButtonUp?.Invoke(i, this);
        }
    }

    public void UnsubscribeAll()
    {
        OnCurrentAxis = null;
        OnButtonDown = null;
        OnButtonUp = null;
    }
}

public enum ControllerType
{
    Keyboard,
    Joystick1,
    Joystick2,
    Joystick3,
    Joystick4
}

public enum Axis
{
    MoveHorizontal,
    MoveVertical,
    RotateHorizontal,
    RotateVertical,
    Shoot
}

public enum Action
{
    Start,
    Cancel,
    Shoot,
    Reload,
    ChangeWeapon,
    Interact,
    Melee,
    Ability,
    Heal
}