using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class ScoreGoal : Agent
{

    

    [SerializeField] private Transform targetTransform;
    [SerializeField] private int round = 0;
    [SerializeField] private int win = 0;
    [SerializeField] private int lose = 0;

    [SerializeField] private float reward = 0;


    private float prevX = -7;
    private float prevZ = -2;



    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-7, -6, -2);

        System.Console.WriteLine(targetTransform);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float moveSpeed = 7f;

        //Check if selected move is closer to the goal than the previous move
        Vector3 ballNewPosition = new Vector3(moveX, 0, moveZ);
        Vector3 ballPrevPosition = new Vector3(prevX, 0, prevZ);

        float selectedMoveDistance = Vector3.Distance(ballNewPosition, targetTransform.localPosition);
        float previousMoveDistance = Vector3.Distance(ballPrevPosition, targetTransform.localPosition);

        if (selectedMoveDistance < previousMoveDistance && ballNewPosition != ballPrevPosition)
        {
            float tempReward = 10;
            SetReward(tempReward);
            reward = GetCumulativeReward();
        } else if (selectedMoveDistance >= previousMoveDistance && ballNewPosition != ballPrevPosition)
        {
            float tempReward = -10;
            SetReward(tempReward);
            reward = GetCumulativeReward();
        }

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        prevX = moveX;
        prevZ = moveZ;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal> (out Goal goal)) {
            SetReward(+10000f);
            reward = GetCumulativeReward();
            round++;
            win++;
            EndEpisode();
        } else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-10000f);
            reward = GetCumulativeReward();
            round++;
            lose++;
            EndEpisode();
        }
    }
}
