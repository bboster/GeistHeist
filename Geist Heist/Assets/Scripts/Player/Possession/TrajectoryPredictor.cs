using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Splines;

public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField] LineRenderer trajectoryLine;
    [SerializeField] int maxPoints;
    float rayOverlap = 1.1f;

    public void PredictTrajectory(float launchStrength, float mass, Vector3 Direction, Vector3 position, float drag, float increment)
    {
        Vector3 velocity = Direction * (launchStrength / mass);
        Vector3 p1 = position;
        Vector3 nextPosition;
        float overlap;

        UpdateLineRender(maxPoints, (0, position));

        for (int i = 1; i < maxPoints; i++)
        {
            velocity = CalculateNewVelocity(velocity, drag, increment);
            nextPosition = p1 + (velocity * increment);

            overlap = Vector3.Distance(p1, nextPosition) * rayOverlap;

            if (Physics.Raycast(p1, velocity.normalized, out RaycastHit hit, overlap))
            {
                UpdateLineRender(i, (i - 1, hit.point));
                break;
            }

            p1 = nextPosition;
            UpdateLineRender(maxPoints, (i, p1));
        }
        /*Vector3 velocity = launchStrength / mass * Direction;
        Vector3 p1 = position;

        for(int i = 0; i < 100; i++)
        {
            velocity = CalculateNewVelocity(velocity, drag, increment);
            Vector3 p2 = p1 + velocity * increment;
            float Overlap = Vector3.Distance(p1, p2) * (float)1.01;

            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, Overlap))
            {
                UpdateLineRender(i, (i - 1, hit.point));
                break;
            }

            p1 = p2;
            UpdateLineRender(maxpoints, (i, position));
        }*/
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f- drag * increment);
        return velocity;
    }

    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        trajectoryLine.positionCount = count;
        trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
    }
}
