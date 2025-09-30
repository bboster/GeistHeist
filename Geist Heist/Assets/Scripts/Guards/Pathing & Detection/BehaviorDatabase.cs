using UnityEngine;
using GuardUtilities;

public class BehaviorDatabase : MonoBehaviour
{
    [SerializeField] private Behavior[] guardBehaviors;

    public static BehaviorDatabase instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Retrieves a behavior from the database.
    /// </summary>
    /// <param name="state"></param>
    public Behavior GetBehavior(GuardData.GuardStates state)
    {
        return guardBehaviors[(int)state];
    }
}
